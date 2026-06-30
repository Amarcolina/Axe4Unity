using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Axe4Unity {

  public class Compiler {

    public static Program Compile(string filePath) {
      var tokens = Parser.ParseFile(filePath);
      return Compile(tokens, Path.GetDirectoryName(filePath));
    }

    public static Program Compile(List<List<Token>> parsedLines, string workingDirectory = null) {
      var compiler = new Compiler();

      compiler.WorkingDir = workingDirectory;
      compiler.Program = new();
      compiler.Program.Lines = parsedLines.Select(p => {
        return new Program.Line() {
          Tokens = p
        };
      }).ToList();

      return compiler.Compile();
    }

    internal ushort LetterVarAddress = Machine.ADDR_LETTER_VARS;

    internal Program Program;

    internal string WorkingDir;
    internal List<Token> Tokens;
    internal List<OpAndMetaData> CurrentLine;

    internal int I;
    internal int LineIndex;
    internal int Pass;

    internal Stack<(int lineIndex, int opIndex, IOpControl op)> ControlStack = new();
    internal Stack<List<OpAndMetaData>> OpStack = new();

    private Program Compile() {
      var lines = Program.Lines;

      //Remove comment lines
      for (int i = lines.Count - 1; i >= 0; i--) {
        if (lines[i].Tokens.Count == 0) {
          lines.RemoveAt(i);
        }
      }

      //Auto-add return at the end of the file
      lines.Add(new Program.Line() {
        Tokens = new List<Token>(){
          Token.Parse("Return")
        }
      });

      for (Pass = 0; Pass < 2; Pass++) {
        Program.Data.Clear();
        LetterVarAddress = Machine.ADDR_LETTER_VARS;
        bool isInMultiLineComment = false;

        for (LineIndex = 0; LineIndex < lines.Count; LineIndex++) {
          try {
            Tokens = lines[LineIndex].Tokens;
            I = 0;
            CurrentLine = new();

            bool isCommentLine = isInMultiLineComment;
            if (Tokens.Count != 0 && Tokens[0] == ".") {
              if (Tokens.Count >= 3 && Tokens[1] == "." && Tokens[2] == ".") {
                isInMultiLineComment = !isInMultiLineComment;
              }
              isCommentLine = true;
            }

            var programLine = Program.Lines[LineIndex];

            if (!isCommentLine) {
              EmitExpression();
            }

            if (CurrentLine.All(o => o.IsDataAddr)) {
              CurrentLine.Clear();
              CurrentLine.Add(new OpAndMetaData() {
                Op = new Op.Nop()
              });
            }

            programLine.Ops = CurrentLine;
            programLine.Indent = CalcIndent();
            programLine.Text = Token.ToString(programLine.Tokens);
          } catch (Exception e) {
            throw new Exception($"Encountered error while parsing line {BuildOpText(Tokens, 0, Tokens.Count)}", e);
          }
        }

        if (ControlStack.Count != 0) {
          throw new Exception("control stack was non-zero!\n" + string.Join("\n", ControlStack.Select(t => t.op + " : " + t.lineIndex)));
        }
      }

      for (int lineI = 0; lineI < Program.Lines.Count; lineI++) {
        var line = Program.Lines[lineI];
        for (int opI = 0; opI < line.Ops.Count; opI++) {
          var item = line.Ops[opI];

          item.Type = item.Op.GetType().Name;
          item.Display = BuildOpText(line.Tokens, item.ColStart, item.ColEnd);
          item.LineIndex = lineI;
          item.OpIndex = opI;
        }
      }

      return Program;
    }

    #region EXPRESSIONS

    private (int start, int length) EmitExpression() {
      SkipWhitespace();

      int exprStart = CurrentLine.Count;

      while (I < Tokens.Count) {
        if (Tokens[I] == ")" ||
            Tokens[I] == "}" ||
            Tokens[I] == "," ||
            Tokens[I] == "^^r") {
          break;
        }

        int iBefore = I;

        EmitValue();

        if (I == iBefore) {
          throw new Exception(BuildOpText(Tokens, I, Tokens.Count));
        }
      }

      return (exprStart, CurrentLine.Count - exprStart);
    }

    //Must be pointing to the first token of the first argument when called
    private int EmitArguments() {
      int count = 0;

      if (I >= Tokens.Count || Tokens[I] == ")" || Tokens[I] == "^^r") {
        Skip(")");
        return 0;
      }

      while (true) {
        if (I >= Tokens.Count || Tokens[I] == "^^r" || Tokens[I] == ")") {
          Emit(new Op.PushArg());
          count++;
          break;
        } else if (Tokens[I] == ",") {
          I++;
          Emit(new Op.PushArg());
          count++;
        } else {
          EmitExpression();
        }
      }

      if (I < Tokens.Count) {
        Skip(")");
      }

      return count;
    }

    private (int start, int length) EmitValue() {
      if (I >= Tokens.Count) {
        throw new Exception($"Read index of {I} was out of range for token count of {Tokens.Count}");
      }

      OpStack.Push(new List<OpAndMetaData>());

      int tokStart = I;
      int opStart = CurrentLine.Count;

      bool didEmit = TryEmitMultiTokenValue() ||
                     TryEmit3WideValue() ||
                     TryEmit2WideValue() ||
                     TryEmitExprValue() ||
                     TryEmitDataValue() ||
                     TryEmitSystemValue() ||
                     TryEmitMathValue() ||
                     TryEmitTextValue() ||
                     TryEmitFlowValue() ||
                     TryEmitGraphicsValue() ||
                     TryEmitSpriteValue() ||
                     TryEmitInterruptValue() ||
                     TryEmitAxiomValue();

      if (!didEmit) {
        throw new Exception($"Could not compile token [{Tokens[I]}] : {Tokens[I].Value:X4}");
      }

      int minTok = I;
      for (int i = opStart; i < CurrentLine.Count; i++) {
        if (!OpStack.Peek().Contains(CurrentLine[i])) {
          minTok = Mathf.Min(minTok, CurrentLine[i].ColStart);
        }
      }

      foreach (var item in OpStack.Peek()) {
        item.ColStart = tokStart;
        item.ColEnd = minTok;
        item.Row = LineIndex;
        item.Display = BuildOpText(Tokens, item.ColStart, item.ColEnd);
      }

      OpStack.Pop();

      return (opStart, CurrentLine.Count - opStart);
    }

    #endregion

    #region VALUES

    private bool TryEmitMultiTokenValue() {
      var t = Tokens[I];

      if (t.IsFileHandle) {
        Emit(new Op.FileHandle() {
          VarAddress = GetVarAddress(ReadName())
        });
        return true;
      } else if (t.IsLetter || t.IsCallingArg) {
        Emit(new Op.ReadAddress() {
          VarAddress = GetVarAddress(ReadName())
        });
        return true;
      } else if (t.IsLMemory || t.IsStaticVar) {
        Emit(new Op.Const() {
          Value = GetVarAddress(ReadName())
        });
        return true;
      } else if (t.IsDigit) {
        int value = ReadNumber();
        if (I < Tokens.Count && Tokens[I] == ".") {
          I++;
          int digStart = I;
          int frac = ReadNumber();
          int fracDigCount = I - digStart;
          float decValue = value + frac / Mathf.Pow(10, fracDigCount);
          Emit(new Op.Const() {
            Value = (ushort)(decValue * 256)
          });
        } else {
          Emit(new Op.Const() {
            Value = (ushort)value
          });
        }

        return true;
      }

      return false;
    }

    private bool TryEmit3WideValue() {
      if (Tokens.Count - I < 3) {
        return false;
      }

      switch (Tokens[I].ToString() +
              Tokens[I + 1].ToString() +
              Tokens[I + 2].ToString()) {
        default: return false;
        case "Return!If ": {
          I += 3;
          EmitExpression();
          Emit(new Op.If() {
            JumpLine = LineIndex + 1,
            Negated = true
          });
          Emit(new Op.Return());
          break;
        }
        case "WVLine(": I += 2; EmitFunction(new Op.StraightLine<PlotterErase>() { Vertical = true }); break;
        case "IVLine(": I += 2; EmitFunction(new Op.StraightLine<PlotterInvert>() { Vertical = true }); break;
        case "WHLine(": I += 2; EmitFunction(new Op.StraightLine<PlotterErase>() { Vertical = false }); break;
        case "IHLine(": I += 2; EmitFunction(new Op.StraightLine<PlotterInvert>() { Vertical = false }); break;
      }

      return true;
    }

    private bool TryEmit2WideValue() {
      if (I >= Tokens.Count - 1) {
        return false;
      }

      switch (Tokens[I].ToString() + Tokens[I + 1].ToString()) {
        default: return false;
        case "++": EmitIncOrDec<Op.Inc>(); break;
        case "--": EmitIncOrDec<Op.Dec>(); break;
        case "**": I++; EmitOpBinary_U16<Op.MulFixed>(); break;
        case "*^": I++; EmitOpBinary_U16<Op.MulHigh>(); break;
        case "//": I++; EmitOpBinary_S16<Op.DivS>(); break;
        case "/*": I++; EmitOpBinary_U16<Op.DivFixed>(); break;

        case "==": I++; EmitOpBinary_U16<Op.Eq>(); break;
        case "!=!=": I++; EmitOpBinary_U16<Op.NEq>(); break;
        case ">>": I++; EmitOpBinary_S16<Op.GreaterS>(); break;
        case ">=>=": I++; EmitOpBinary_S16<Op.GreaterEqS>(); break;
        case "<<": I++; EmitOpBinary_S16<Op.LessS>(); break;
        case "<=<=": I++; EmitOpBinary_S16<Op.LessEqS>(); break;

        case "Vertical +": I++; EmitFunction(new Op.Vertical() { Positive = true }); break;
        case "Vertical -": I++; EmitFunction(new Op.Vertical() { Positive = false }); break;
        case "Horizontal +": I++; EmitFunction(new Op.Horizontal() { Positive = true }); break;
        case "Horizontal -": I++; EmitFunction(new Op.Horizontal() { Positive = false }); break;

        case "Wref(": I++; EmitFunction<Op.Rect<PlotterErase>>(); break;
        case "Iref(": I++; EmitFunction<Op.Rect<PlotterInvert>>(); break;
        case "WLine(": I++; EmitFunction<Op.Line<PlotterErase>>(); break;
        case "ILine(": I++; EmitFunction<Op.Line<PlotterInvert>>(); break;

        case "VLine(": I++; EmitFunction(new Op.StraightLine<PlotterOr>() { Vertical = true }); break;
        case "HLine(": I++; EmitFunction(new Op.StraightLine<PlotterOr>() { Vertical = false }); break;

        case "WCircle(": I++; EmitFunction<Op.Circle<PlotterErase>>(); break;
        case "ICircle(": I++; EmitFunction<Op.Circle<PlotterInvert>>(); break;

        case "ReturnIf ": {
          I += 2;
          EmitExpression();
          Emit(new Op.If() {
            JumpLine = LineIndex + 1
          });
          Emit(new Op.Return());
          break;
        }
      }

      return true;
    }

    private bool TryEmitExprValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "(": {
          if (I != 0 && Tokens[I - 1] == ")") {
            I++;
            Emit(new Op.CallAddr() {
              ArgCount = EmitArguments()
            }); ;
            break;
          } else if (CurrentLine.Count != 0 &&
                     CurrentLine[CurrentLine.Count - 1].Op is Op.ReadAddress) {
            string labelName = CurrentLine[CurrentLine.Count - 1].Display;
            CurrentLine.RemoveAt(CurrentLine.Count - 1);

            I++;

            Emit(new Op.Call() {
              ArgCount = EmitArguments(),
              LabelAddress = GetLabelLine(labelName)
            });
            break;
          } else {
            I++;
            EmitExpression();
            Skip(")");
            break;
          }
        }
        case "{": {
          I++;
          EmitExpression();
          Skip("}");
          var fileHandle = CalcFileHandle();
          if (fileHandle != null) {
            Emit(ParseRMod(new Op.ReadFile() {
              VarAddress = ((Op.FileHandle)fileHandle.Op).VarAddress
            }));
          } else {
            Emit(ParseRMod(new Op.ReadMemory()));
          }
          break;
        }
        case "int(": {
          I++;
          EmitExpression();
          if (I < Tokens.Count && (Tokens[I] == ")" || Tokens[I] == "}")) {
            I++;
          }
          Emit(new Op.ReadMemorySignedByte());
          break;
        }
        case "->": {
          I++;

          if (Tokens[I] == "^^o") {
            I++;

            Assert.AreEqual(1, CurrentLine.Count);
            var varName = ReadName();
            var varAddr = ((Op.Const)CurrentLine[0].Op).Value;

            Program.CreateCustomVariable(varName, varAddr);

            CurrentLine.Clear();
            Emit(new Op.Nop());
          } else if (Tokens[I].IsLetter || Tokens[I].IsCallingArg) {
            Emit(new Op.StoreAddress() {
              Address = GetVarAddress(ReadName()),
              RMode = 1
            });
          } else if (Tokens[I] == "{") {
            I++;
            Emit(new Op.PushArg());
            (var expr, var count) = EmitExpression();
            Skip("}");

            if (count == 1 && CurrentLine[expr].Op is Op.Const constAddr) {
              //Remove the push, and the const op itself
              CurrentLine.RemoveAt(expr - 1);
              CurrentLine.RemoveAt(expr - 1);

              Emit(ParseRMod(new Op.StoreAddress() {
                Address = constAddr.Value
              }));
            } else {
              Emit(ParseRMod(new Op.StoreMemory()));
            }
          } else if (Tokens[I].IsStaticVar) {
            var varName = ReadName();
            var varAddr = ((Op.Const)CurrentLine[0].Op).Value;

            //Add terminator to string only if the string is directly
            //stored to a variable without any other ops in between
            if (CurrentLine.Count == 1 && CurrentLine[0].IsStringData) {
              Program.Data.Add(0);
            }

            Program.CreateStaticVariable(varName, varAddr);
          }
          break;
        }
      }

      return true;
    }

    private bool TryEmitDataValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "|E": {
          I++;

          int value = 0;
          while (I < Tokens.Count) {
            var tok = Tokens[I];
            if (!tok.IsHex) {
              break;
            }

            value = value * 16 + tok.HexValue;

            I++;
          }
          Emit(new Op.Const() {
            Value = (ushort)value
          });
          break;
        }
        case "|L": {
          I++;
          Emit(new Op.Const() {
            Value = (ushort)GetLabelLine(ReadName())
          });
          break;
        }
        case "[": {
          I++;
          int addr = Program.Data.Count + Machine.ADDR_PROGRAM_MEMORY;
          if (Tokens[I].IsHex) {
            while (true) {
              if (I >= Tokens.Count || Tokens[I] == "->") {
                break;
              }

              if (Tokens[I] == "]") {
                I++;
                break;
              }

              var h = Tokens[I++].HexValue;
              var l = Tokens[I++].HexValue;

              Program.Data.Add((byte)(h << 4 | l));
            }
          } else if (Tokens[I].IsStaticVar) {
            string filePath = Path.Combine(WorkingDir, $"{Tokens[I]}.8xi");
            using (var reader = File.OpenRead(filePath)) {
              //Header
              for (int i = 0; i < 55; i++) {
                reader.ReadByte();
              }

              //Metadata
              for (int i = 0; i < 19; i++) {
                reader.ReadByte();
              }

              for (int i = 0; i < 756; i++) {
                Program.Data.Add((byte)reader.ReadByte());
              }
            }

            I++;
            Skip("]");
          } else {
            Skip("]");
          }

          Emit(new Op.Const() {
            Value = (ushort)addr
          }).IsDataAddr = true;
          break;
        }
        case "\"": {
          I++;
          int addr = Program.Data.Count + Machine.ADDR_PROGRAM_MEMORY;
          while (true) {
            if (I >= Tokens.Count || Tokens[I] == "->") {
              break;
            }

            if (Tokens[I] == "\"") {
              I++;
              break;
            }

            foreach (var c in Tokens[I++].ToString()) {
              Program.Data.Add((byte)c);
            }
          }

          //If the string is in the middle of an expression
          if (OpStack.Count > 1) {
            Program.Data.Add(0);
          }

          var op = Emit(new Op.Const() {
            Value = (ushort)addr
          });
          op.IsDataAddr = true;
          op.IsStringData = true;
          break;
        }
        case "^^o": {
          I++;
          Emit(new Op.Const() {
            Value = GetVarAddress(ReadName())
          });
          break;
        }
        case "^^T": {
          I++;
          Emit(new Op.Const() {
            Value = (ushort)Tokens[I++].Value
          });
          break;
        }
        case "'": {
          I++;
          Emit(new Op.Const() {
            Value = (ushort)Tokens[I++].ToString()[0]
          });
          Skip("'");
          break;
        }
        case "greek_pi": {
          I++;

          int value = 0;
          while (I < Tokens.Count) {
            var tok = Tokens[I];
            if (tok == "0") {
              value = value * 2 + 0;
            } else if (tok == "1") {
              value = value * 2 + 1;
            } else {
              break;
            }

            I++;
          }

          Emit(new Op.Const() {
            Value = (ushort)value
          });
          break;
        }
        case "det(": {
          I++;
          int size = ReadNumber();
          int num = 0;
          if (I < Tokens.Count && Tokens[I] == ",") {
            I++;
            num = ReadNumber();
          }
          Skip(")");

          int addr = Program.Data.Count + Machine.ADDR_PROGRAM_MEMORY;
          for (int i = 0; i < size; i++) {
            Program.Data.Add((byte)num);
          }

          Emit(new Op.Const() {
            Value = (ushort)addr
          });
          break;
        }
        case "DeltaList(": {
          I++;
          int addr = Program.Data.Count + Machine.ADDR_PROGRAM_MEMORY;
          while (true) {
            (var expr, var count) = EmitExpression();

            Assert.AreEqual(1, count);
            Assert.AreEqual(typeof(Op.Const), CurrentLine[expr].Op.GetType());
            var value = ((Op.Const)CurrentLine[expr].Op).Value;

            CurrentLine.RemoveAt(expr);

            Program.Data.Add((byte)(value & 0xFF));
            if (I < Tokens.Count && Tokens[I] == "^^r") {
              I++;
              Program.Data.Add((byte)(value >> 8));
            }

            Skip(",");

            if (I >= Tokens.Count) {
              break;
            }

            if (Tokens[I] == ")") {
              I++;
              break;
            }
          }
          Emit(new Op.Const() {
            Value = (ushort)addr
          }).IsDataAddr = true;
          break;
        }
        case "conj(": EmitFunction<Op.Copy>(); break;
        case "expr(": EmitFunction<Op.Exch>(); break;
        case "inString(": EmitFunction<Op.InData>(); break;
        case "Fill(": EmitFunction<Op.Fill>(); break;
        case "length(": EmitFunction<Op.Length>(); break;
        case "stdDev(": EmitFunction<Op.StrGet>(); break;
        case "Equ>String(": EmitFunction<Op.StrEq>(); break;
        case "SortD(": EmitFunction<Op.Sort>(); break;
        case "cumSum(": EmitFunction<Op.CheckSum>(); break;
      }

      return true;
    }

    private bool TryEmitSystemValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "DiagnosticOff": EmitStandalone<Op.Nop>(); break;
        case "Normal": EmitStandalone<Op.Normal>(); break;
        case "FullScreen": EmitStandalone<Op.Full>(); break;
        case "Pause ": EmitFunction<Op.Pause>(); break;
        case "getKey": EmitFunctionOptionalArgs<Op.GetKey>(); break;
        case "Fix ": EmitFunction<Op.Fix>(); break;
        case "UnArchive ": EmitFunction<Op.UnArchive>(); break;
        case "Archive ": EmitFunction<Op.Archive>(); break;
        case "GetCalc(": {
          I++;
          EmitExpression();

          SkipToNextArg();
          if (I >= Tokens.Count || Tokens[I] == ")") {
            Emit(new Op.GetCalcFromRam());
          } else {
            if (Tokens[I].IsFileHandle) {
              Emit(new Op.GetCalcFromFileSystem() {
                VarAddress = GetVarAddress(ReadName())
              });
            } else {
              Emit(new Op.PushArg());
              EmitExpression();
              Emit(new Op.GetCalcCreate());
            }
          }

          Skip(")");
          break;
        }
        case "DelVar ": EmitFunction<Op.DelVar>(); break;
        case "real(": {
          I++;
          if (I >= Tokens.Count || Tokens[I] == ")") {
            LetterVarAddress = Machine.ADDR_LETTER_VARS;
          } else {
            (var expr, var count) = EmitExpression();
            Assert.AreEqual(1, count);
            LetterVarAddress = ((Op.Const)CurrentLine[expr].Op).Value;
            CurrentLine.RemoveAt(expr);
          }
          Skip(")");
          Emit(new Op.Nop());
          break;
        }
        case "ExprOn": EmitStandalone<Op.Nop>(); break;
        case "ExprOff": EmitStandalone<Op.Nop>(); break;
        case "identity(": {
          while (I < Tokens.Count && Tokens[I] != ")") {
            I++;
          }
          Skip(")");
          EmitStandalone<Op.Nop>();
          break;
        }
      }

      return true;
    }

    private bool TryEmitMathValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "~": {
          I++;
          (var expr, var count) = EmitValue();

          if (count == 1 && CurrentLine[expr].Op is Op.Const constVal) {
            CurrentLine.RemoveAt(expr);
            Emit(new Op.Const() {
              Value = (ushort)(-(short)constVal.Value)
            });
          } else {
            Emit(new Op.Negate());
          }
          break;
        }
        case "+": EmitOpBinary_U16<Op.Add>(); break;
        case "-": EmitOpBinary_U16<Op.Sub>(); break;
        case "*": EmitOpBinary_U16<Op.Mul>(); break;
        case "/": EmitOpBinary_U16<Op.Div>(); break;
        case "^": EmitOpBinary_U16<Op.Mod>(); break;
        case "^^2": I++; Emit(ParseRMod(new Op.Square())); break;
        case "^^-1": EmitStandalone<Op.Recip>(); break;

        case "=": EmitOpBinary_U16<Op.Eq>(); break;
        case "!=": EmitOpBinary_U16<Op.NEq>(); break;
        case ">": EmitOpBinary_U16<Op.Greater>(); break;
        case ">=": EmitOpBinary_U16<Op.GreaterEq>(); break;
        case "<": EmitOpBinary_U16<Op.Less>(); break;
        case "<=": EmitOpBinary_U16<Op.LessEq>(); break;

        case " or ": EmitOpBinary_U8<Op.Or_U8>(); break;
        case " and ": EmitOpBinary_U8<Op.And_U8>(); break;
        case " xor ": EmitOpBinary_U8<Op.Xor_U8>(); break;
        case "crossplot": EmitOpBinary_U16<Op.Or_U16>(); break;
        case "dotplot": EmitOpBinary_U16<Op.And_U16>(); break;
        case "squareplot": EmitOpBinary_U16<Op.Xor_U16>(); break;
        case "not(": EmitFunction<Op.Not>(); break;

        case "min(": EmitFunction<Op.Min>(); break;
        case "max(": EmitFunction<Op.Max>(); break;
        case "abs(": EmitFunction<Op.Abs>(); break;

        case "sin(": EmitFunction<Op.Sin>(); break;
        case "cos(": EmitFunction<Op.Cos>(); break;

        case "sqrt(": EmitFunction<Op.SquareRoot>(); break;

        case "rand": EmitStandalone<Op.Rand>(); break;
      }

      return true;
    }

    private bool TryEmitTextValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case ">Frac": EmitStandalone<Op.ToStringChar>(); break;
        case ">Dec": EmitStandalone<Op.ToStringNumber>(); break;
        case ">DMS": EmitStandalone<Op.ToStringToken>(); break;
        case ">Rect": EmitStandalone<Op.ToStringHex>(); break;
        case "[i]": EmitStandalone<Op.ToStringNewline>(); break;
        case "ClrHome": EmitStandalone<Op.ClrDraw>(); break;
        case "Disp ": {
          I++;
          EmitExpression();
          Emit(new Op.Disp());
          break;
        }
        case "DrawF ": EmitFunction(new Op.Text() { IsTextOnly = true }); break;
        case "Text(": EmitFunction<Op.Text>(); break;
        case "Output(": EmitFunction<Op.Output>(); break;
      }

      return true;
    }

    private bool TryEmitFlowValue() {
      bool negateFlag = false;

      if (Tokens[I] == "!") {
        negateFlag = true;
        I++;
      }

      switch (Tokens[I].ToString()) {
        default: return false;
        case "?": {
          I++;
          var ifOp = Emit(new Op.If());

          bool negated = false;
          if (I < Tokens.Count && Tokens[I] == "?") {
            I++;
            negated = true;
          }

          EmitExpression();

          int jumpIndex = CurrentLine.Count;
          if (I < Tokens.Count && Tokens[I] == ",") {
            I++;

            var elseOp = Emit(new Op.Else());

            jumpIndex = CurrentLine.Count;

            EmitExpression();

            elseOp.Op = new Op.Else() {
              JumpLine = LineIndex,
              JumpOp = CurrentLine.Count
            };
          }

          ifOp.Op = new Op.If() {
            JumpLine = LineIndex,
            JumpOp = jumpIndex,
            Negated = negated
          };

          Emit(new Op.End());

          break;
        }
        case "Lbl ": {
          I++;
          Program.CreateLabel(ReadName(), LineIndex);
          Emit(new Op.Label());
          break;
        }
        case "Goto ": {
          I++;
          if (I < Tokens.Count && Tokens[I] == "(") {
            Skip("(");
            EmitExpression();
            Skip(")");
            Emit(new Op.GotoExpr());
          } else {
            Emit(new Op.Goto() {
              LabelAddress = GetLabelLine(ReadName())
            });
          }
          break;
        }
        case "Z-Test(": {
          I++;
          EmitExpression();

          for (int i = 0; ; i++) {
            Skip(",");

            if (I >= Tokens.Count) {
              break;
            }

            if (Tokens[I] == ")") {
              I++;
              break;
            }

            Emit(new Op.GotoIfEq() {
              Value = (ushort)i,
              LabelAddress = GetLabelLine(ReadName())
            });
          }

          break;
        }
        case "Return": {
          I++;
          Emit(ParseRMod(new Op.Return()));
          break;
        }
        case "sub(": {
          I++;

          var labelName = ReadName();

          int argCount = 0;
          if (I < Tokens.Count && Tokens[I] == ",") {
            I++;
            if (I < Tokens.Count && Tokens[I] == ")") {
              argCount = 1;
              Emit(new Op.PushArg());
            } else {
              argCount = EmitArguments();
            }
          } else {
            Skip(")");
          }

          Emit(new Op.Call() {
            ArgCount = argCount,
            LabelAddress = GetLabelLine(labelName)
          });
          break;
        }
        case "For(": {
          I++;

          var firstOpIndex = CurrentLine.Count;

          EmitExpression();

          if (I >= Tokens.Count || Tokens[I] == ")") {
            Emit(new Op.PushArg());

            int loopOp = CurrentLine.Count;

            Skip(")");

            Emit(ParseRMod(PushControlOp(new Op.ForStack(), loopOp)));
          } else {
            Assert.AreEqual(1, CurrentLine.Count - firstOpIndex);

            var varName = CurrentLine[firstOpIndex].Display;

            //Remove the expression, since the variable address gets put
            //directly into the For op
            CurrentLine.RemoveAt(firstOpIndex);

            SkipToNextArg();
            EmitExpression();

            Emit(new Op.StoreAddress() {
              Address = GetVarAddress(varName),
              RMode = 1
            });

            int loopOp = CurrentLine.Count;

            SkipToNextArg();
            EmitExpression();

            Emit(PushControlOp(new Op.For() {
              VarAddress = GetVarAddress(varName)
            }, loopOp));

            Skip(")");
          }

          break;
        }
        case "If ": {
          I++;
          EmitExpression();
          Emit(PushControlOp(new Op.If() {
            Negated = negateFlag
          }));
          break;
        }
        case "Else": {
          I++;
          (_, _, var parent) = ControlStack.Pop();
          parent.JumpLine = LineIndex + 1;

          if (I < Tokens.Count && Tokens[I] == "!") {
            I++;
            negateFlag = true;
          }
          if (I < Tokens.Count && Tokens[I] == "If ") {
            Emit(PushControlOp(new Op.Else() {
              IsElseIf = true
            }));

            I++;
            parent.JumpLine = LineIndex;
            parent.JumpOp = CurrentLine.Count;
            EmitExpression();
            Emit(PushControlOp(new Op.If() {
              Negated = negateFlag
            }));
          } else if (parent is Op.If) {
            Emit(PushControlOp(new Op.Else() {
              IsElseIf = false
            }));
          }
          break;
        }
        case "DS<(": {
          I++;
          var varName = ReadName();
          SkipToNextArg();
          int jumpOp = CurrentLine.Count;
          EmitExpression();
          Emit(PushControlOp(new Op.DS() {
            VarAddress = GetVarAddress(varName)
          }, jumpOp));
          break;
        }
        case "While ": {
          I++;
          (var expr, var count) = EmitExpression();

          if (count == 1 && CurrentLine[expr].Op is Op.Const constVal && constVal.Value != 0) {
            CurrentLine.RemoveAt(expr);
            Emit(PushControlOp(new Op.WhileTrue()));
          } else {
            Emit(PushControlOp(new Op.While()));
          }
          break;
        }
        case "Repeat ": {
          I++;
          (var expr, var count) = EmitExpression();

          if (count == 1 && CurrentLine[expr].Op is Op.Const constVal && constVal.Value == 0) {
            CurrentLine.RemoveAt(expr);
            Emit(PushControlOp(new Op.WhileTrue()));
          } else {
            Emit(PushControlOp(new Op.Repeat()));
          }
          break;
        }
        case "End": {
          I++;
          (var parentLine, var parentOpIndex, var parent) = ControlStack.Pop();
          parent.JumpLine = LineIndex + 1;

          if (I < Tokens.Count && Tokens[I] == "!") {
            I++;
            negateFlag = true;
          }

          if (I < Tokens.Count && Tokens[I] == "If ") {
            I++;
            EmitExpression();
            Emit(new Op.If() {
              Negated = !negateFlag,
              JumpLine = LineIndex + 1
            });
          }

          if (parent is Op.If or Op.Else) {
            Emit(new Op.End());

            while (ControlStack.Count != 0 &&
                   ControlStack.Peek().op is Op.Else elseOp &&
                   elseOp.IsElseIf) {
              ControlStack.Peek().op.JumpLine = LineIndex + 1;
              ControlStack.Pop();
            }
          } else if (parent is Op.DS) {
            Emit(new Op.End());
          } else if (parent is Op.For opFor) {
            Emit(new Op.EndFor() {
              VarAddress = opFor.VarAddress,
              JumpLine = parentLine,
              JumpOp = parentOpIndex
            });
          } else {
            Emit(new Op.EndLoop() {
              JumpLine = parentLine,
              JumpOp = parentOpIndex
            });
          }
          break;
        }
      }

      return true;
    }

    private bool TryEmitGraphicsValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "ClrDraw": EmitFunction<Op.ClrDraw>(); break;
        case "DispGraph": {
          I++;

          var dispGraph = new Op.DispGraph();

          if (I < Tokens.Count && Tokens[I] == "ClrDraw") {
            dispGraph.DoClrDraw = true;
            I++;
          }

          if (I < Tokens.Count && Tokens[I] == "RecallPic ") {
            dispGraph.DoRecalPic = true;
            I++;
          }

          if (I < Tokens.Count && Tokens[I] == "(") {
            I++;
            dispGraph.ArgCount = EmitArguments();
          }

          Emit(ParseRMod(dispGraph));
          break;
        }
        case "RecallPic ": EmitStandalone<Op.RecallPic>(); break;
        case "StorePic ": EmitStandalone<Op.StorePic>(); break;
        case "StoreGDB ": EmitStandalone<Op.StoreGDB>(); break;
        case "DrawInv ": EmitFunctionOptionalArgs<Op.DrawInv>(); break;
        case "Pxl-On(": EmitFunction<Op.PxlPlot<PlotterOr>>(); break;
        case "Pxl-Off(": EmitFunction<Op.PxlPlot<PlotterErase>>(); break;
        case "Pxl-Change(": EmitFunction<Op.PxlPlot<PlotterInvert>>(); break;
        case "pxl-Test(": EmitFunction<Op.PxlTest>(); break;
        case "Line(": EmitFunction<Op.Line<PlotterOr>>(); break;
        case "Circle(": EmitFunction<Op.Circle<PlotterOr>>(); break;
        case "Tangent(": EmitFunction<Op.Bitmap>(); break;

        case "ref(": EmitFunction<Op.Rect<PlotterOr>>(); break;
        case "rref(": EmitFunction<Op.Rect<PlotterInvert>>(); break;
      }

      return true;
    }

    private bool TryEmitSpriteValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "Pt-On(": EmitFunction<Op.PtSprite<PlotterOr>>(); break;
        case "Pt-Off(": EmitFunction<Op.PtSprite<PlotterOverwrite>>(); break;
        case "Pt-Change(": EmitFunction<Op.PtSprite<PlotterInvert>>(); break;
        case "Plot3(": EmitFunction<Op.PtSprite<PlotterAnd>>(); break;

        case "ShadeNorm(": EmitFunction<Op.SpriteTransform<Op.RotC>>(); break;
        case "Shade_t(": EmitFunction<Op.SpriteTransform<Op.RotCC>>(); break;
        case "Shadechi^2(": EmitFunction<Op.SpriteTransform<Op.FlipV>>(); break;
        case "ShadeF(": EmitFunction<Op.SpriteTransform<Op.FlipH>>(); break;

        case "Plot1(": EmitFunction<Op.PtMask>(); break;
        case "Plot2(": EmitFunction<Op.PtGet>(); break;
      }

      return true;
    }

    private bool TryEmitInterruptValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        case "FnOff ": EmitStandalone<Op.Nop>(); break;
        case "FnOn ": EmitStandalone<Op.Nop>(); break;
      }

      return true;
    }

    private bool TryEmitAxiomValue() {
      switch (Tokens[I].ToString()) {
        default: return false;
        //All axiom commands are ignored
        case "AsmComp(": {
          while (I < Tokens.Count) {
            I++;
          }
          Emit(new Op.Nop());
          break;
        }

        //CRABCAKE
        case "AxesOn": EmitStandalone<Op.Nop>(); break;
        case "AxesOff": EmitStandalone<Op.Nop>(); break;

        //MEMKIT
        case "ZXmin": EmitStandalone<Op.MemKit.Load>(); Skip(")"); break;
        case "ZXmax": EmitStandalone<Op.MemKit.Next>(); Skip(")"); break;
        case "ZXres": EmitFunction<Op.MemKit.Print>(); break;
        case "Zthetamax": EmitFunction<Op.MemKit.New>(); break;
        case "Zthetastep": EmitFunction<Op.MemKit.Delete>(); break;
        case "dim(": I++; Emit(ParseRMod(new Op.MemKit.Dim())); break;

      }

      return true;
    }

    #endregion

    #region EMIT HELPERS

    private void EmitStandalone<OpT>() where OpT : IOp, new() {
      I++;
      Emit(new OpT());
    }

    private void EmitOpBinary_U16<OpT>() where OpT : IOp_Binary_U16, new() {
      EmitOpBinary<Op.Binary_U16<OpT>>();
    }

    private void EmitOpBinary_S16<OpT>() where OpT : IOp_Binary_S16, new() {
      EmitOpBinary<Op.Binary_S16<OpT>>();
    }

    private void EmitOpBinary_U8<OpT>() where OpT : IOp_Binary_U8, new() {
      EmitOpBinary<Op.Binary_U8<OpT>>();
    }

    private void EmitOpBinary<T>() where T : IOp, new() {
      I++;

      int constExprStart = CurrentLine.Count - 1;

      Emit(new Op.PushArg());
      EmitValue();
      Emit(new T());

      int constExprSize = CurrentLine.Count - constExprStart;

      if (constExprSize == 4 &&
          constExprStart >= 0 &&
          CurrentLine[constExprStart].Op is Op.Const lhs &&
          CurrentLine[constExprStart + 2].Op is Op.Const rhs) {
        CurrentLine.RemoveAt(constExprStart);
        CurrentLine.RemoveAt(constExprStart);
        CurrentLine.RemoveAt(constExprStart);
        CurrentLine.RemoveAt(constExprStart);

        ushort value;
        T binaryOp = default;
        MachineStateNative dummy = default;
        if (binaryOp is IOp_Binary_U8 binaryU8) {
          value = binaryU8.Execute(ref dummy, (byte)lhs.Value, (byte)rhs.Value);
        } else if (binaryOp is IOp_Binary_U16 binaryU16) {
          value = binaryU16.Execute(ref dummy, lhs.Value, rhs.Value);
        } else if (binaryOp is IOp_Binary_S16 binaryS16) {
          value = (ushort)binaryS16.Execute(ref dummy, (short)lhs.Value, (short)rhs.Value);
        } else {
          throw new();
        }

        Emit(new Op.Const() {
          Value = value
        });
      }
    }

    private void EmitIncOrDec<OpT>() where OpT : IOp, new() {
      I += 2;

      var prevOp = CurrentLine[CurrentLine.Count - 1].Op;
      if (prevOp is Op.ReadAddress varOp) {
        Emit(new OpT());
        Emit(new Op.StoreAddress() {
          Address = varOp.VarAddress,
          RMode = 1
        });
      } else if (prevOp is Op.ReadMemory memOp) {
        EmitBefore(new Op.PushArg(), memOp);
        Emit(new OpT());
        Emit(new Op.SwapStack());
        Emit(new Op.StoreMemory() {
          RMode = memOp.RMode,
        });
      }
    }

    private void EmitFunction<OpT>() where OpT : IOp_Function, new() {
      EmitFunction(new OpT());
    }

    private void EmitFunction(IOp_Function op) {
      I++;
      op.ArgCount = EmitArguments();
      Emit(ParseRMod(op));
    }

    private void EmitFunctionOptionalArgs<OpT>() where OpT : IOp_Function, new() {
      I++;
      IOp_Function op = new OpT();

      if (I < Tokens.Count && Tokens[I] == "(") {
        I++;
        op.ArgCount = EmitArguments();
      }

      Emit(ParseRMod(op));
    }

    private void EmitBefore(IOp op, IOp before) {
      int index = CurrentLine.FindIndex(m => m.Op.Equals(before));
      var opAndMetaData = new OpAndMetaData() {
        Op = op,
      };

      if (index < 0) {
        Debug.Log($"Couldn't find item {before.GetType().FullName} in list {string.Join(", ", CurrentLine.Select(t => t.Op.GetType().FullName))}");
      }

      CurrentLine.Insert(index, opAndMetaData);
      OpStack.Peek().Add(opAndMetaData);
    }

    private OpAndMetaData Emit(IOp op) {
      var opAndMetaData = new OpAndMetaData() {
        Op = op,
      };

      CurrentLine.Add(opAndMetaData);
      OpStack.Peek().Add(opAndMetaData);

      return opAndMetaData;
    }

    #endregion

    #region UTILS

    private ushort GetVarAddress(string name) {
      int addr;
      if (Program.TryGetVarAddress(name, out addr)) {
        return (ushort)addr;
      } else if (Machine.TryGetAddressOfBuiltInVariable(name, out addr)) {
        if ((name.Length == 1 && char.IsUpper(name[0])) ||
            name == "theta") {
          return (ushort)(addr - Machine.ADDR_LETTER_VARS + LetterVarAddress);
        } else {
          return (ushort)addr;
        }
      } else if (Machine.TryGetAddressOfBuiltInStaticVariable(name, out addr)) {
        return (ushort)addr;
      } else {
        return 12345;
      }
    }

    private int GetLabelLine(string name) {
      int line;
      if (Program.TryGetLabelLine(name, out line)) {
        return line;
      } else {
        return 45678;
      }
    }

    private IOp PushControlOp(IOpControl op, int opIndex = 0) {
      ControlStack.Push((LineIndex, opIndex, op));
      return op;
    }

    private IOp ParseRMod(IOp op) {
      if (op is IOpRModifier rMod) {
        while (I < Tokens.Count && Tokens[I] == "^^r") {
          rMod.RMode++;
          I++;
        }
      }
      return op;
    }

    private void SkipWhitespace() {
      while (I < Tokens.Count && Tokens[I] == " ") {
        I++;
      }
    }

    private void Skip(string tok) {
      if (I < Tokens.Count && Tokens[I] == tok) {
        I++;
      }
    }

    private void SkipToNextArg() {
      SkipWhitespace();
      Skip(",");
      SkipWhitespace();
    }

    private int CalcIndent() {
      int indent = 0;
      foreach (var op in ControlStack) {
        if (op.op is Op.Else elseOp && elseOp.IsElseIf) {
          indent--;
        }
        indent++;
      }

      if (CurrentLine.Any(l => l.Op is IOpControl c && ControlStack.Any(s => s.op == c))) {
        indent--;
      }

      return indent;
    }

    private OpAndMetaData CalcFileHandle() {
      Stack<OpAndMetaData> stack = new();
      OpAndMetaData hl = null;

      foreach (var op in CurrentLine) {
        if (op.Op is Op.FileHandle) {
          hl = op;
        } else if (op.Op is Op.PushArg) {
          stack.Push(hl);
        } else if (op.Op is IOp_Binary_S16 or IOp_Binary_U16 or IOp_Binary_U8) {
          var result = hl != null ? hl : stack.Peek();
          stack.Pop();
          hl = result;
        } else if (op.Op is IOp_Function func) {
          for (int i = 0; i < func.ArgCount; i++) {
            stack.Pop();
          }
          hl = null;
        } else if (op.Op is Op.StoreMemory) {
          stack.Pop();
          hl = null;
        } else {
          hl = null;
        }
      }

      return hl;
    }

    private string ReadName() {
      List<Token> tokens = new();

      if (Tokens[I].IsCallingArg ||
          Tokens[I].IsLMemory ||
          Tokens[I].IsFileHandle) {
        tokens.Add(Tokens[I++]);
      } else {
        while (I < Tokens.Count && (Tokens[I].IsLetterOrDigit || Tokens[I].IsStaticVar)) {
          tokens.Add(Tokens[I]);
          I++;
        }
      }

      return Token.ToString(tokens);
    }

    private int ReadNumber() {
      bool negative = false;
      if (Tokens[I] == "~") {
        negative = true;
        I++;
      }

      int value = 0;
      while (I < Tokens.Count && Tokens[I].IsDigit) {
        value = value * 10 + (Tokens[I++].Value - 0x30);
      }

      if (negative) {
        short s = (short)(ushort)value;
        value = (ushort)(-s);
      }

      return value;
    }

    private string BuildOpText(List<Token> tokens, int start, int end) {
      string str = "";
      for (int i = start; i < end; i++) {
        str = str + tokens[i];
      }
      return str;
    }

    #endregion

  }
}
