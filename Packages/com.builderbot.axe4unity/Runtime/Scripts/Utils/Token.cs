using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Axe4Unity {

  [Serializable]
  public struct Token : IEquatable<Token> {

    public bool IsTwoByte;
    public int Value;

    public byte LowByte {
      get {
        if (IsTwoByte) {
          return (byte)(Value >> 8);
        } else {
          return (byte)(Value & 0xFF);
        }
      }
    }

    public byte HighByte => (byte)(Value & 0xFF);

    public bool IsDigit => Value >= 0x30 && Value <= 0x39;
    public bool IsUppercaseLetter => Value >= 0x41 && Value <= 0x5B;
    public bool IsLowercaseLetter => (Value >= 0xBBB0 && Value <= 0xBBBA) ||
                                     (Value >= 0xBBBC && Value <= 0xBBCA);
    public bool IsLetter => IsUppercaseLetter || IsLowercaseLetter;
    public bool IsLetterOrDigit => IsLetter || IsDigit;

    public bool IsEOL => Value == 0x3F;

    public bool IsStaticVar => (Value >= 0x6000 && Value <= 0x6009) ||
                               (Value >= 0x6100 && Value <= 0x6109) ||
                               (Value >= 0xAA00 && Value <= 0xAA09);

    public bool IsCallingArg => Value >= 0x5E40 && Value <= 0x5E45;
    public bool IsLMemory => Value >= 0x5D00 && Value <= 0x5D05;
    public bool IsFileHandle => Value >= 0x5E10 && Value <= 0x5E19;

    public bool IsDispArg => Value >= 0x01 && Value <= 0x03;

    public bool IsHex => IsDigit || (Value >= 0x41 && Value <= 0x46);
    public int HexValue {
      get {
        if (IsDigit) {
          return Value - 0x30;
        } else if (IsHex) {
          return Value - 0x41 + 10;
        } else {
          throw new Exception();
        }
      }
    }

    public Token(byte byte0) {
      Value = byte0;
      IsTwoByte = false;
    }

    public Token(byte byte0, byte byte1) {
      Value = (byte0 << 8) | byte1;
      IsTwoByte = true;
    }

    public static Token Read(NativeSlice<byte> buffer, ref int offset) {
      byte byte0 = buffer[offset++];
      if (Starts2Byte(byte0)) {
        byte byte1 = buffer[offset++];
        return new Token(byte0, byte1);
      } else {
        return new Token(byte0);
      }
    }

    public static Token Parse(string t) {
      return TokenLookup.StringToToken[t];
    }

    public static List<Token> ParseLine(string line) {
      List<Token> result = new();

      int i = 0;
      while (i < line.Length) {
        (var str, var tok) = TokenLookup.CharToTokens[line[i]].
                             Where(p => line.AsSpan().Slice(i).StartsWith(p.str)).
                             OrderByDescending(p => p.str.Length).
                             FirstOrDefault();
        result.Add(tok);
        i += str.Length;
      }

      return result;
    }

    public static bool Starts2Byte(byte b) {
      switch (b) {
        case 92:
        case 93:
        case 94:
        case 96:
        case 97:
        case 98:
        case 99:
        case 126:
        case 170:
        case 187:
        case 239:
          return true;
        default:
          return false;
      }
    }

    public static bool operator ==(Token a, Token b) {
      return a.Equals(b);
    }

    public static bool operator !=(Token a, Token b) {
      return !a.Equals(b);
    }

    public static bool operator ==(Token a, string b) {
      return a == Parse(b);
    }

    public static bool operator !=(Token a, string b) {
      return a != Parse(b);
    }

    public bool Equals(Token other) {
      return IsTwoByte == other.IsTwoByte &&
             Value == other.Value;
    }

    public override bool Equals(object obj) {
      if (obj is Token other) {
        return Equals(other);
      } else {
        return false;
      }
    }

    public override int GetHashCode() {
      return Value.GetHashCode();
    }

    public override string ToString() {
      if (!TokenLookup.TokenToString.TryGetValue(this, out var str)) {
        return $"${Value:X4}";
      } else {
        return str;
      }
    }

    public static string ToString(List<Token> tokens) {
      string str = "";
      foreach (var t in tokens) {
        str = str + t.ToString();
      }
      return str;
    }
  }
}
