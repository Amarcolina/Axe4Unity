using System;
using UnityEngine;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Axe4Unity;
using Axe4Unity.Op;

[BurstCompile]
public class APORTAL_NativeRunner : NativeRunner {

  public override Results Step(Machine machine, int maxSteps, int maxGetKeys) {
    unsafe {
      var statePtr = UnsafeUtility.AddressOf(ref machine.State);
      Execute(statePtr, maxSteps, maxGetKeys, out var lastOpIndex, out var stepsCompleted, out var getKeysCompleted);
      int lineIndex = (int)(lastOpIndex / 10000);
      int opIndex = (int)(lastOpIndex % 10000);
      var lastOp = machine.Program.Lines[lineIndex].Ops[opIndex];
      return new Results() {
        LastOpExecuted = lastOp,
        StepsCompleted = stepsCompleted,
        IsGetKeyTimeout = getKeysCompleted >= maxGetKeys,
      };
    }
  }

  [BurstCompile]
  public static unsafe void Execute(
    void* ptr,
    int maxSteps,
    int maxGetKey,
    out ulong lastOpIndex,
    out int stepsCompleted,
    out int getKeysCompleted) {
    ref MachineStateNative machine = ref UnsafeUtility.AsRef<MachineStateNative>(ptr);
    Execute(ref machine, maxSteps, maxGetKey, out lastOpIndex, out stepsCompleted, out getKeysCompleted);
  }

  public static void Execute(
    ref MachineStateNative machine,
    int maxSteps,
    int maxGetKeys,
    out ulong lastOpIndex,
    out int stepsCompleted,
    out int getKeysCompleted) {
    lastOpIndex = 0;
    stepsCompleted = 0;
    getKeysCompleted = 0;

    while (stepsCompleted < maxSteps && getKeysCompleted < maxGetKeys) {
      switch(machine.PC.GetLongCode()) {
        default:
          throw new InvalidOperationException($"Tried to jump to line {machine.PC.LineIndex} and op {machine.PC.OpIndex} with code {machine.PC.GetLongCode()}");
        case 0_0000:
          machine.PC = new ProgramCounter(12, 0);
          lastOpIndex = 11_0003;
          stepsCompleted += 19;

          //.Portal
          new Nop().Execute(ref machine);

          //|E86D7->^^oPENX
          new Nop().Execute(ref machine);

          //|E86D8->^^oPENY
          new Nop().Execute(ref machine);

          //L1+680->^^oCUT
          new Nop().Execute(ref machine);

          //DiagnosticOff
          new Nop().Execute(ref machine);

          //FnOff 
          new Nop().Execute(ref machine);

          //Fix 5
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //ClrDraw
          new ClrDraw() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //"Portal2"->Str1PP
          new Nop().Execute(ref machine);

          //UnArchive Str1PP
          new Const() { Value = 16384 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new UnArchive() { ArgCount = 1 }.Execute(ref machine);

          //^^o`Y5->^^oPSAVE
          new Nop().Execute(ref machine);

          //!If GetCalc(Str1PP)->PSAVE
          new Const() { Value = 16384 }.Execute(ref machine);
          new GetCalcFromRam().Execute(ref machine);
          new StoreAddress() { Address = 1818, RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 15, JumpOp = 0 }.Execute(ref machine);
          break;
        case 12_0000:
          machine.PC = new ProgramCounter(15, 0);
          lastOpIndex = 14_0000;
          stepsCompleted += 10;

          //GetCalc(Str1PP,1)->PSAVE
          new Const() { Value = 16384 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new GetCalcCreate().Execute(ref machine);
          new StoreAddress() { Address = 1818, RMode = 1 }.Execute(ref machine);

          //1->{PSAVE}
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 15_0000:
          machine.PC = new ProgramCounter(17, 0);
          lastOpIndex = 16_0002;
          stepsCompleted += 4;

          //"PortalPK"->Str0PP
          new Nop().Execute(ref machine);

          //!If GetCalc(Str0PP,`Y2)
          new Const() { Value = 16393 }.Execute(ref machine);
          new GetCalcFromFileSystem() { VarAddress = 1800 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 24, JumpOp = 0 }.Execute(ref machine);
          break;
        case 17_0000:
          machine.PC = new ProgramCounter(18, 0);
          lastOpIndex = 17_0006;
          stepsCompleted += 7;

          //Text(0,0,"A vital data file is missing,
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16403 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 18_0000:
          machine.PC = new ProgramCounter(19, 0);
          lastOpIndex = 18_0006;
          stepsCompleted += 7;

          //Text(0,7,"Make sure all files were
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16433 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 19_0000:
          machine.PC = new ProgramCounter(20, 0);
          lastOpIndex = 19_0006;
          stepsCompleted += 7;

          //Text(0,14,"transfered successfully."
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16458 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 20_0000:
          machine.PC = new ProgramCounter(21, 0);
          lastOpIndex = 20_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 21_0000:
          machine.PC = new ProgramCounter(22, 0);
          lastOpIndex = 21_0000;
          stepsCompleted += 1;
          getKeysCompleted += 1;

          //getKey^^r
          new GetKey() { RMode = 1, ArgCount = 0 }.Execute(ref machine);
          return;
        case 22_0000:
          machine.PC = new ProgramCounter(23, 0);
          lastOpIndex = 22_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 23_0000:
          machine.PC = new ProgramCounter(24, 0);
          lastOpIndex = 23_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 24_0000:
          machine.PC = new ProgramCounter(28, 0);
          lastOpIndex = 27_0000;
          stepsCompleted += 4;

          //..AXE
          new Nop().Execute(ref machine);

          //^^o`Y0->^^oY0
          new Nop().Execute(ref machine);

          //^^o`Y1->^^oY1
          new Nop().Execute(ref machine);

          //^^o`Y2->^^oY2
          new Nop().Execute(ref machine);
          break;
        case 28_0000:
          machine.PC = new ProgramCounter(35, 0);
          lastOpIndex = 34_0001;
          stepsCompleted += 20;

          //Lbl START
          new Label().Execute(ref machine);

          //conj([827C381000000000],L1+8,8
          new Const() { Value = 16483 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2312 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //conj([8080C0E0F8000000],L1,8
          new Const() { Value = 16491 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //DeltaList(32,32,23,20,18,18,27,30)->Str1X
          new Nop().Execute(ref machine);

          //DeltaList(4,1,~2,~2,7,10,13,13)->Str1Y
          new Nop().Execute(ref machine);

          //ClrDraw
          new ClrDraw() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //0->theta
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          break;
        case 35_0000:
          machine.PC = new ProgramCounter(36, 0);
          lastOpIndex = 35_0004;
          stepsCompleted += 5;

          //Repeat theta=23
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 23 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Repeat() { JumpLine = 48, JumpOp = 0 }.Execute(ref machine);
          break;
        case 36_0000:
          machine.PC = new ProgramCounter(37, 0);
          lastOpIndex = 36_0001;
          stepsCompleted += 2;
          getKeysCompleted += 1;

          //If getKey
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 39, JumpOp = 0 }.Execute(ref machine);
          break;
        case 37_0000:
          machine.PC = new ProgramCounter(38, 0);
          lastOpIndex = 37_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 38_0000:
          machine.PC = new ProgramCounter(39, 0);
          lastOpIndex = 38_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 39_0000:
          machine.PC = new ProgramCounter(43, 0);
          lastOpIndex = 42_0000;
          stepsCompleted += 39;

          //Pt-On(int(theta^8+Str1X},int(theta^8+Str1Y},Shade_t(L1)->C
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16499 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemorySignedByte().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16507 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemorySignedByte().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new SpriteTransform<RotCC>() { ArgCount = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //conj(L1+8,L1,8
          new Const() { Value = 2312 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //conj(C,L1+8,8
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2312 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 43_0000:
          machine.PC = new ProgramCounter(44, 0);
          lastOpIndex = 43_0008;
          stepsCompleted += 9;

          //If theta+1->theta<16
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 46, JumpOp = 0 }.Execute(ref machine);
          break;
        case 44_0000:
          machine.PC = new ProgramCounter(46, 0);
          lastOpIndex = 45_0000;
          stepsCompleted += 2;

          //ClrDraw
          new ClrDraw() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 46_0000:
          machine.PC = new ProgramCounter(47, 0);
          lastOpIndex = 46_0002;
          stepsCompleted += 3;

          //Pause 100
          new Const() { Value = 100 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 47_0000:
          machine.PC = new ProgramCounter(48, 0);
          lastOpIndex = 47_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 35, JumpOp = 0 }.Execute(ref machine);
          break;
        case 48_0000:
          machine.PC = new ProgramCounter(50, 0);
          lastOpIndex = 49_0000;
          stepsCompleted += 8;

          //conj(Pic2M,L6,208
          new Const() { Value = 16897 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 208 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 50_0000:
          machine.PC = new ProgramCounter(51, 0);
          lastOpIndex = 50_0002;
          stepsCompleted += 3;

          //Pause 500
          new Const() { Value = 500 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 51_0000:
          machine.PC = new ProgramCounter(54, 0);
          lastOpIndex = 53_0000;
          stepsCompleted += 5;

          //90->B
          new Const() { Value = 90 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //0->A
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //540
          new Const() { Value = 540 }.Execute(ref machine);
          break;
        case 54_0000:
          machine.PC = new ProgramCounter(55, 0);
          lastOpIndex = 54_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 67, JumpOp = 0 }.Execute(ref machine);
          break;
        case 55_0000:
          machine.PC = new ProgramCounter(57, 0);
          lastOpIndex = 56_0001;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //->F
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //If getKey
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 59, JumpOp = 0 }.Execute(ref machine);
          break;
        case 57_0000:
          machine.PC = new ProgramCounter(58, 0);
          lastOpIndex = 57_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 58_0000:
          machine.PC = new ProgramCounter(59, 0);
          lastOpIndex = 58_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 59_0000:
          machine.PC = new ProgramCounter(61, 0);
          lastOpIndex = 60_0008;
          stepsCompleted += 25;

          //conj(Pic2M+216,F+216+L6,552-F
          new Const() { Value = 17113 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 216 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 552 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //If A+B->A>90
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 90 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 64, JumpOp = 0 }.Execute(ref machine);
          break;
        case 61_0000:
          machine.PC = new ProgramCounter(64, 0);
          lastOpIndex = 63_0000;
          stepsCompleted += 11;

          //F-12->F
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //A-90->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 90 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 64_0000:
          machine.PC = new ProgramCounter(66, 0);
          lastOpIndex = 65_0000;
          stepsCompleted += 4;

          //B--
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Dec().Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 66_0000:
          machine.PC = new ProgramCounter(66, 2);
          lastOpIndex = 66_0001;
          stepsCompleted += 2;

          //End!If F
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 67, JumpOp = 0 }.Execute(ref machine);
          break;
        case 66_0002:
          machine.PC = new ProgramCounter(67, 0);
          lastOpIndex = 66_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 54, JumpOp = 0 }.Execute(ref machine);
          break;
        case 67_0000:
          machine.PC = new ProgramCounter(71, 0);
          lastOpIndex = 70_0001;
          stepsCompleted += 11;
          getKeysCompleted += 1;

          //Lbl MEN
          new Label().Execute(ref machine);

          //conj(Pic2M,L6,768
          new Const() { Value = 16897 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //getKey
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //1->theta
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          break;
        case 71_0000:
          machine.PC = new ProgramCounter(72, 0);
          lastOpIndex = 71_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 85, JumpOp = 0 }.Execute(ref machine);
          break;
        case 72_0000:
          machine.PC = new ProgramCounter(76, 0);
          lastOpIndex = 75_0000;
          stepsCompleted += 28;

          //{DeltaList(2,38,75)+theta}->X
          new Const() { Value = 16515 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //rref(X,51,19,7
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 19 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(X+1,50,17,9
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 17 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 76_0000:
          machine.PC = new ProgramCounter(77, 0);
          lastOpIndex = 76_0002;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //Repeat getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new Repeat() { JumpLine = 78, JumpOp = 0 }.Execute(ref machine);
          break;
        case 77_0000:
          machine.PC = new ProgramCounter(78, 0);
          lastOpIndex = 77_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 76, JumpOp = 0 }.Execute(ref machine);
          break;
        case 78_0000:
          machine.PC = new ProgramCounter(82, 0);
          lastOpIndex = 81_0004;
          stepsCompleted += 46;

          //rref(X,51,19,7
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 19 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(X+1,50,17,9
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 17 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //K=3-(K=2)+theta+3^3->theta
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //If K=15
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 84, JumpOp = 0 }.Execute(ref machine);
          break;
        case 82_0000:
          machine.PC = new ProgramCounter(83, 0);
          lastOpIndex = 82_0000;
          stepsCompleted += 1;

          //Goto QUIT
          new Goto() { LabelAddress = 91 }.Execute(ref machine);
          break;
        case 83_0000:
          machine.PC = new ProgramCounter(84, 0);
          lastOpIndex = 83_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 84_0000:
          machine.PC = new ProgramCounter(84, 5);
          lastOpIndex = 84_0004;
          stepsCompleted += 5;

          //EndIf K=54
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 54 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 85, JumpOp = 0 }.Execute(ref machine);
          break;
        case 84_0005:
          machine.PC = new ProgramCounter(85, 0);
          lastOpIndex = 84_0005;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 71, JumpOp = 0 }.Execute(ref machine);
          break;
        case 85_0000:
          machine.PC = new ProgramCounter(86, 0);
          lastOpIndex = 85_0001;
          stepsCompleted += 2;

          //!If theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 88, JumpOp = 0 }.Execute(ref machine);
          break;
        case 86_0000:
          machine.PC = new ProgramCounter(87, 0);
          lastOpIndex = 86_0000;
          stepsCompleted += 1;

          //Goto LOAD
          new Goto() { LabelAddress = 1220 }.Execute(ref machine);
          break;
        case 87_0000:
          machine.PC = new ProgramCounter(88, 0);
          lastOpIndex = 87_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 88_0000:
          machine.PC = new ProgramCounter(89, 0);
          lastOpIndex = 88_0004;
          stepsCompleted += 5;

          //!If theta-1
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 91, JumpOp = 0 }.Execute(ref machine);
          break;
        case 89_0000:
          machine.PC = new ProgramCounter(90, 0);
          lastOpIndex = 89_0000;
          stepsCompleted += 1;

          //Goto SELECT
          new Goto() { LabelAddress = 1290 }.Execute(ref machine);
          break;
        case 90_0000:
          machine.PC = new ProgramCounter(91, 0);
          lastOpIndex = 90_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 91_0000:
          machine.PC = new ProgramCounter(94, 0);
          lastOpIndex = 93_0006;
          stepsCompleted += 9;

          //Lbl QUIT
          new Label().Execute(ref machine);

          //ClrDraw
          new ClrDraw() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //Text(0,0,"Saving...
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16518 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 94_0000:
          machine.PC = new ProgramCounter(95, 0);
          lastOpIndex = 94_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 95_0000:
          machine.PC = new ProgramCounter(97, 0);
          lastOpIndex = 96_0000;
          stepsCompleted += 4;

          //Archive Str1PP
          new Const() { Value = 16384 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Archive() { ArgCount = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 97_0000:
          machine.PC = new ProgramCounter(106, 0);
          lastOpIndex = 105_0001;
          stepsCompleted += 32;

          //Lbl TOP
          new Label().Execute(ref machine);

          //0->{L1+690}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2994, RMode = 1 }.Execute(ref machine);

          //Y->{L1+702}^^r
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 3006, RMode = 1 }.Execute(ref machine);

          //..AXE
          new Nop().Execute(ref machine);

          //0->K->{L5}
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 5632, RMode = 0 }.Execute(ref machine);

          //Fill(L5,120
          new Const() { Value = 5632 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 120 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fill() { ArgCount = 2 }.Execute(ref machine);

          //{^^o`Y0+2}->{^^o`Y1+2}
          new Const() { Value = 1850 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 1796, RMode = 0 }.Execute(ref machine);

          //{{L1+704}*2+`Y0}^^r+Y0->Y1
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new FileHandle() { VarAddress = 1848 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1848, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1848 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1794, RMode = 1 }.Execute(ref machine);

          //L4->I
          new Const() { Value = 4864 }.Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);
          break;
        case 106_0000:
          machine.PC = new ProgramCounter(107, 0);
          lastOpIndex = 106_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 111, JumpOp = 0 }.Execute(ref machine);
          break;
        case 107_0000:
          machine.PC = new ProgramCounter(110, 9);
          lastOpIndex = 110_0008;
          stepsCompleted += 35;

          //{`Y1}->A
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //Y1+1->Y1
          new ReadAddress() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1794, RMode = 1 }.Execute(ref machine);

          //Fill(A/16->{I},A^16+1->B
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fill() { ArgCount = 2 }.Execute(ref machine);

          //EndIf 216+L4<=(I+B->I
          new Const() { Value = 5080 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);
          new Binary_U16<LessEq>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 111, JumpOp = 0 }.Execute(ref machine);
          break;
        case 110_0009:
          machine.PC = new ProgramCounter(111, 0);
          lastOpIndex = 110_0009;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 106, JumpOp = 0 }.Execute(ref machine);
          break;
        case 111_0000:
          machine.PC = new ProgramCounter(113, 2);
          lastOpIndex = 113_0001;
          stepsCompleted += 7;

          //L5+16->L
          new Const() { Value = 5648 }.Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //{`Y1}->{L5}
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 5632, RMode = 0 }.Execute(ref machine);

          //For(F,1,{L5})
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 113_0002:
          machine.PC = new ProgramCounter(114, 0);
          lastOpIndex = 113_0004;
          stepsCompleted += 3;
          new Const() { Value = 5632 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 147, JumpOp = 0 }.Execute(ref machine);
          break;
        case 114_0000:
          machine.PC = new ProgramCounter(116, 0);
          lastOpIndex = 115_0005;
          stepsCompleted += 11;

          //Y1+1->Y1
          new ReadAddress() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1794, RMode = 1 }.Execute(ref machine);

          //!If {`Y1}-1
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 122, JumpOp = 0 }.Execute(ref machine);
          break;
        case 116_0000:
          machine.PC = new ProgramCounter(121, 0);
          lastOpIndex = 120_0009;
          stepsCompleted += 42;

          //+1->{L}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //{`Y1+1}->{r6}->{L+12}^^r
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //11->{L4+{r6}}
          new Const() { Value = 11 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //0->{L+14}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //I8({r6},L+2,256)
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 121_0000:
          machine.PC = new ProgramCounter(122, 0);
          lastOpIndex = 121_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 122_0000:
          machine.PC = new ProgramCounter(123, 0);
          lastOpIndex = 122_0005;
          stepsCompleted += 6;

          //!If {`Y1}-2
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 135, JumpOp = 0 }.Execute(ref machine);
          break;
        case 123_0000:
          machine.PC = new ProgramCounter(126, 0);
          lastOpIndex = 125_0013;
          stepsCompleted += 32;

          //+2->{L}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //{`Y1+2}->{r6}->{L+8}^^r
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //I8({`Y1+1},L+2,5)
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 126_0000:
          machine.PC = new ProgramCounter(129, 0);
          lastOpIndex = 128_0008;
          stepsCompleted += 23;

          //15->{L+6}^^r
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //Pic2O->{L+14}^^r
          new Const() { Value = 16819 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //!If {L4+{r6}+18}
          new Const() { Value = 4864 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 131, JumpOp = 0 }.Execute(ref machine);
          break;
        case 129_0000:
          machine.PC = new ProgramCounter(131, 0);
          lastOpIndex = 130_0000;
          stepsCompleted += 8;

          //Pic2O+24->{L+14}^^r
          new Const() { Value = 16843 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 131_0000:
          machine.PC = new ProgramCounter(134, 0);
          lastOpIndex = 133_0013;
          stepsCompleted += 39;

          //9->{{`Y1+1}+L4}
          new Const() { Value = 9 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //{{r2}+2}^^r-1->{{r2}+2}^^r
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //I8({`Y1+2},L+10,5)
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 134_0000:
          machine.PC = new ProgramCounter(135, 0);
          lastOpIndex = 134_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 135_0000:
          machine.PC = new ProgramCounter(136, 0);
          lastOpIndex = 135_0005;
          stepsCompleted += 6;

          //!If {`Y1}-3
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 144, JumpOp = 0 }.Execute(ref machine);
          break;
        case 136_0000:
          machine.PC = new ProgramCounter(144, 0);
          lastOpIndex = 143_0000;
          stepsCompleted += 95;

          //3->{L}->{L+2}
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //5->{{`Y1+1}+L4}
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //10->{{`Y1+2}+L4}
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //{`Y1+1}->{L+10}
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //{`Y1+3}/16-1*51->{L+12}
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //{`Y1+3}^16-1*51->{L+13}
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 13 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //{`Y1+2}->{L+14}
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 144_0000:
          machine.PC = new ProgramCounter(147, 0);
          lastOpIndex = 146_0000;
          stepsCompleted += 12;

          //{`Y1}+Y1->Y1
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1794 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1794, RMode = 1 }.Execute(ref machine);

          //L+16->L
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 10, JumpLine = 113, JumpOp = 2 }.Execute(ref machine);
          break;
        case 147_0000:
          machine.PC = new ProgramCounter(147, 2);
          lastOpIndex = 147_0001;
          stepsCompleted += 2;

          //For(X,0,17
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);
          break;
        case 147_0002:
          machine.PC = new ProgramCounter(148, 0);
          lastOpIndex = 147_0003;
          stepsCompleted += 2;
          new Const() { Value = 17 }.Execute(ref machine);
          new For() { VarAddress = 46, JumpLine = 237, JumpOp = 0 }.Execute(ref machine);
          break;
        case 148_0000:
          machine.PC = new ProgramCounter(148, 2);
          lastOpIndex = 148_0001;
          stepsCompleted += 2;

          //For(Y,0,11
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);
          break;
        case 148_0002:
          machine.PC = new ProgramCounter(149, 0);
          lastOpIndex = 148_0003;
          stepsCompleted += 2;
          new Const() { Value = 11 }.Execute(ref machine);
          new For() { VarAddress = 48, JumpLine = 236, JumpOp = 0 }.Execute(ref machine);
          break;
        case 149_0000:
          machine.PC = new ProgramCounter(157, 0);
          lastOpIndex = 156_0004;
          stepsCompleted += 76;

          //X*5->C
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //Y*5->D
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{Y*18+X->{r6}+L4}->A
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //{{r6}+1+L4}->V
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 42, RMode = 1 }.Execute(ref machine);

          //{{r6}-1+L4}->T
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 38, RMode = 1 }.Execute(ref machine);

          //{Y+1*18+X+L4}->Z
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //{Y-1*18+X+L4}->P
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 30, RMode = 1 }.Execute(ref machine);

          //If A<=3
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<LessEq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 159, JumpOp = 0 }.Execute(ref machine);
          break;
        case 157_0000:
          machine.PC = new ProgramCounter(158, 0);
          lastOpIndex = 157_0008;
          stepsCompleted += 9;

          //SPT(A!=0*5
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 158_0000:
          machine.PC = new ProgramCounter(159, 0);
          lastOpIndex = 158_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 159_0000:
          machine.PC = new ProgramCounter(160, 0);
          lastOpIndex = 159_0004;
          stepsCompleted += 5;

          //If A>=10
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 168, JumpOp = 0 }.Execute(ref machine);
          break;
        case 160_0000:
          machine.PC = new ProgramCounter(161, 0);
          lastOpIndex = 160_0011;
          stepsCompleted += 12;

          //SPT(A-10*5+100
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 100 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 161_0000:
          machine.PC = new ProgramCounter(163, 0);
          lastOpIndex = 162_0004;
          stepsCompleted += 18;

          //A=10*3->{{r6}+L4}
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //If A=11
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 11 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 167, JumpOp = 0 }.Execute(ref machine);
          break;
        case 163_0000:
          machine.PC = new ProgramCounter(164, 0);
          lastOpIndex = 163_0008;
          stepsCompleted += 9;

          //!If {L4-18+{r6}}-2
          new Const() { Value = 4846 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 166, JumpOp = 0 }.Execute(ref machine);
          break;
        case 164_0000:
          machine.PC = new ProgramCounter(166, 0);
          lastOpIndex = 165_0000;
          stepsCompleted += 8;

          //3->{L4-18+{r6}}
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4846 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 166_0000:
          machine.PC = new ProgramCounter(167, 0);
          lastOpIndex = 166_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 167_0000:
          machine.PC = new ProgramCounter(168, 0);
          lastOpIndex = 167_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 168_0000:
          machine.PC = new ProgramCounter(169, 0);
          lastOpIndex = 168_0004;
          stepsCompleted += 5;

          //!If A-1
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 171, JumpOp = 0 }.Execute(ref machine);
          break;
        case 169_0000:
          machine.PC = new ProgramCounter(170, 0);
          lastOpIndex = 169_0011;
          stepsCompleted += 12;

          //SPT(T=0*5+90
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 90 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 170_0000:
          machine.PC = new ProgramCounter(171, 0);
          lastOpIndex = 170_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 171_0000:
          machine.PC = new ProgramCounter(172, 0);
          lastOpIndex = 171_0004;
          stepsCompleted += 5;

          //!If A-9
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 175, JumpOp = 0 }.Execute(ref machine);
          break;
        case 172_0000:
          machine.PC = new ProgramCounter(173, 0);
          lastOpIndex = 172_0002;
          stepsCompleted += 3;

          //SPT(85
          new Const() { Value = 85 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 173_0000:
          machine.PC = new ProgramCounter(175, 0);
          lastOpIndex = 174_0000;
          stepsCompleted += 8;

          //3->{{r6}+L4}
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 175_0000:
          machine.PC = new ProgramCounter(176, 0);
          lastOpIndex = 175_0007;
          stepsCompleted += 8;

          //!If A-4/2
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 198, JumpOp = 0 }.Execute(ref machine);
          break;
        case 176_0000:
          machine.PC = new ProgramCounter(178, 0);
          lastOpIndex = 177_0004;
          stepsCompleted += 13;

          //A=4*20->theta
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 20 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //If C-85
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 85 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 182, JumpOp = 0 }.Execute(ref machine);
          break;
        case 178_0000:
          machine.PC = new ProgramCounter(179, 0);
          lastOpIndex = 178_0001;
          stepsCompleted += 2;

          //!If V
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 181, JumpOp = 0 }.Execute(ref machine);
          break;
        case 179_0000:
          machine.PC = new ProgramCounter(180, 0);
          lastOpIndex = 179_0005;
          stepsCompleted += 6;

          //SPT(45+theta
          new Const() { Value = 45 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 180_0000:
          machine.PC = new ProgramCounter(181, 0);
          lastOpIndex = 180_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 181_0000:
          machine.PC = new ProgramCounter(182, 0);
          lastOpIndex = 181_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 182_0000:
          machine.PC = new ProgramCounter(183, 0);
          lastOpIndex = 182_0004;
          stepsCompleted += 5;

          //If D-55
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 55 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 187, JumpOp = 0 }.Execute(ref machine);
          break;
        case 183_0000:
          machine.PC = new ProgramCounter(184, 0);
          lastOpIndex = 183_0001;
          stepsCompleted += 2;

          //!If Z
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 186, JumpOp = 0 }.Execute(ref machine);
          break;
        case 184_0000:
          machine.PC = new ProgramCounter(185, 0);
          lastOpIndex = 184_0005;
          stepsCompleted += 6;

          //SPT(50+theta
          new Const() { Value = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 185_0000:
          machine.PC = new ProgramCounter(186, 0);
          lastOpIndex = 185_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 186_0000:
          machine.PC = new ProgramCounter(187, 0);
          lastOpIndex = 186_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 187_0000:
          machine.PC = new ProgramCounter(188, 0);
          lastOpIndex = 187_0001;
          stepsCompleted += 2;

          //If C
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 192, JumpOp = 0 }.Execute(ref machine);
          break;
        case 188_0000:
          machine.PC = new ProgramCounter(189, 0);
          lastOpIndex = 188_0001;
          stepsCompleted += 2;

          //!If T
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 191, JumpOp = 0 }.Execute(ref machine);
          break;
        case 189_0000:
          machine.PC = new ProgramCounter(190, 0);
          lastOpIndex = 189_0005;
          stepsCompleted += 6;

          //SPT(55+theta
          new Const() { Value = 55 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 190_0000:
          machine.PC = new ProgramCounter(191, 0);
          lastOpIndex = 190_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 191_0000:
          machine.PC = new ProgramCounter(192, 0);
          lastOpIndex = 191_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 192_0000:
          machine.PC = new ProgramCounter(193, 0);
          lastOpIndex = 192_0001;
          stepsCompleted += 2;

          //If D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 197, JumpOp = 0 }.Execute(ref machine);
          break;
        case 193_0000:
          machine.PC = new ProgramCounter(194, 0);
          lastOpIndex = 193_0001;
          stepsCompleted += 2;

          //!If P
          new ReadAddress() { VarAddress = 30 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 196, JumpOp = 0 }.Execute(ref machine);
          break;
        case 194_0000:
          machine.PC = new ProgramCounter(195, 0);
          lastOpIndex = 194_0005;
          stepsCompleted += 6;

          //SPT(60+theta
          new Const() { Value = 60 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 195_0000:
          machine.PC = new ProgramCounter(196, 0);
          lastOpIndex = 195_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 196_0000:
          machine.PC = new ProgramCounter(197, 0);
          lastOpIndex = 196_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 197_0000:
          machine.PC = new ProgramCounter(198, 0);
          lastOpIndex = 197_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 198_0000:
          machine.PC = new ProgramCounter(199, 0);
          lastOpIndex = 198_0004;
          stepsCompleted += 5;

          //!If A-7
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 201, JumpOp = 0 }.Execute(ref machine);
          break;
        case 199_0000:
          machine.PC = new ProgramCounter(200, 0);
          lastOpIndex = 199_0011;
          stepsCompleted += 12;

          //SPT(X^2*5+35
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 35 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 200_0000:
          machine.PC = new ProgramCounter(201, 0);
          lastOpIndex = 200_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 201_0000:
          machine.PC = new ProgramCounter(202, 0);
          lastOpIndex = 201_0004;
          stepsCompleted += 5;

          //!If A-8
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 204, JumpOp = 0 }.Execute(ref machine);
          break;
        case 202_0000:
          machine.PC = new ProgramCounter(203, 0);
          lastOpIndex = 202_0014;
          stepsCompleted += 15;

          //SPT(X+Y^2*5+25
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 25 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 203_0000:
          machine.PC = new ProgramCounter(204, 0);
          lastOpIndex = 203_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 204_0000:
          machine.PC = new ProgramCounter(205, 0);
          lastOpIndex = 204_0004;
          stepsCompleted += 5;

          //!If A-6
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 207, JumpOp = 0 }.Execute(ref machine);
          break;
        case 205_0000:
          machine.PC = new ProgramCounter(206, 0);
          lastOpIndex = 205_0011;
          stepsCompleted += 12;

          //SPT(P=0*5+15
          new ReadAddress() { VarAddress = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 257, ArgCount = 1 }.Execute(ref machine);
          break;
        case 206_0000:
          machine.PC = new ProgramCounter(207, 0);
          lastOpIndex = 206_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 207_0000:
          machine.PC = new ProgramCounter(208, 0);
          lastOpIndex = 207_0004;
          stepsCompleted += 5;

          //!If A-3
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 235, JumpOp = 0 }.Execute(ref machine);
          break;
        case 208_0000:
          machine.PC = new ProgramCounter(211, 0);
          lastOpIndex = 210_0001;
          stepsCompleted += 20;

          //C+1->{r3}+2->{r4}
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 774, RMode = 1 }.Execute(ref machine);

          //D+1->{r5}+2->{r6}
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 776, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //If X
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 216, JumpOp = 0 }.Execute(ref machine);
          break;
        case 211_0000:
          machine.PC = new ProgramCounter(211, 3);
          lastOpIndex = 211_0002;
          stepsCompleted += 3;

          //If S(T)
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 249, ArgCount = 1 }.Execute(ref machine);
          break;
        case 211_0003:
          machine.PC = new ProgramCounter(212, 0);
          lastOpIndex = 211_0003;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 215, JumpOp = 0 }.Execute(ref machine);
          break;
        case 212_0000:
          machine.PC = new ProgramCounter(215, 0);
          lastOpIndex = 214_0000;
          stepsCompleted += 11;

          //Pxl-Off({r3},{r5}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Off({r3},{r6}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 215_0000:
          machine.PC = new ProgramCounter(216, 0);
          lastOpIndex = 215_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 216_0000:
          machine.PC = new ProgramCounter(217, 0);
          lastOpIndex = 216_0004;
          stepsCompleted += 5;

          //If X-17
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 17 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 222, JumpOp = 0 }.Execute(ref machine);
          break;
        case 217_0000:
          machine.PC = new ProgramCounter(217, 3);
          lastOpIndex = 217_0002;
          stepsCompleted += 3;

          //If S(V)
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 249, ArgCount = 1 }.Execute(ref machine);
          break;
        case 217_0003:
          machine.PC = new ProgramCounter(218, 0);
          lastOpIndex = 217_0003;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 221, JumpOp = 0 }.Execute(ref machine);
          break;
        case 218_0000:
          machine.PC = new ProgramCounter(221, 0);
          lastOpIndex = 220_0000;
          stepsCompleted += 11;

          //Pxl-Off({r4},{r5}
          new ReadAddress() { VarAddress = 774 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Off({r4},{r6}
          new ReadAddress() { VarAddress = 774 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 221_0000:
          machine.PC = new ProgramCounter(222, 0);
          lastOpIndex = 221_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 222_0000:
          machine.PC = new ProgramCounter(223, 0);
          lastOpIndex = 222_0004;
          stepsCompleted += 5;

          //If Y-11
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 11 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 228, JumpOp = 0 }.Execute(ref machine);
          break;
        case 223_0000:
          machine.PC = new ProgramCounter(223, 3);
          lastOpIndex = 223_0002;
          stepsCompleted += 3;

          //If S(Z)
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 249, ArgCount = 1 }.Execute(ref machine);
          break;
        case 223_0003:
          machine.PC = new ProgramCounter(224, 0);
          lastOpIndex = 223_0003;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 227, JumpOp = 0 }.Execute(ref machine);
          break;
        case 224_0000:
          machine.PC = new ProgramCounter(227, 0);
          lastOpIndex = 226_0000;
          stepsCompleted += 11;

          //Pxl-Off({r3},{r6}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Off({r4},{r6}
          new ReadAddress() { VarAddress = 774 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 227_0000:
          machine.PC = new ProgramCounter(228, 0);
          lastOpIndex = 227_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 228_0000:
          machine.PC = new ProgramCounter(229, 0);
          lastOpIndex = 228_0001;
          stepsCompleted += 2;

          //If Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 234, JumpOp = 0 }.Execute(ref machine);
          break;
        case 229_0000:
          machine.PC = new ProgramCounter(229, 3);
          lastOpIndex = 229_0002;
          stepsCompleted += 3;

          //If S(P)
          new ReadAddress() { VarAddress = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 249, ArgCount = 1 }.Execute(ref machine);
          break;
        case 229_0003:
          machine.PC = new ProgramCounter(230, 0);
          lastOpIndex = 229_0003;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 233, JumpOp = 0 }.Execute(ref machine);
          break;
        case 230_0000:
          machine.PC = new ProgramCounter(233, 0);
          lastOpIndex = 232_0000;
          stepsCompleted += 11;

          //Pxl-Off({r3},{r5}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Off({r4},{r5}
          new ReadAddress() { VarAddress = 774 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterErase>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 233_0000:
          machine.PC = new ProgramCounter(234, 0);
          lastOpIndex = 233_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 234_0000:
          machine.PC = new ProgramCounter(235, 0);
          lastOpIndex = 234_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 235_0000:
          machine.PC = new ProgramCounter(236, 0);
          lastOpIndex = 235_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 48, JumpLine = 148, JumpOp = 2 }.Execute(ref machine);
          break;
        case 236_0000:
          machine.PC = new ProgramCounter(237, 0);
          lastOpIndex = 236_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 46, JumpLine = 147, JumpOp = 2 }.Execute(ref machine);
          break;
        case 237_0000:
          machine.PC = new ProgramCounter(240, 0);
          lastOpIndex = 239_0001;
          stepsCompleted += 20;

          //ref(0,60,96,7
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 60 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 96 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //ref(90,0,6,64
          new Const() { Value = 90 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //63->{r1}
          new Const() { Value = 63 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          break;
        case 240_0000:
          machine.PC = new ProgramCounter(241, 0);
          lastOpIndex = 240_0009;
          stepsCompleted += 10;

          //While pxl-Test(89,{r1}-1->{r1}
          new Const() { Value = 89 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new While() { JumpLine = 242, JumpOp = 0 }.Execute(ref machine);
          break;
        case 241_0000:
          machine.PC = new ProgramCounter(242, 0);
          lastOpIndex = 241_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 240, JumpOp = 0 }.Execute(ref machine);
          break;
        case 242_0000:
          machine.PC = new ProgramCounter(243, 0);
          lastOpIndex = 242_0008;
          stepsCompleted += 9;

          //If {L1+704}^^r-18<6
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 245, JumpOp = 0 }.Execute(ref machine);
          break;
        case 243_0000:
          machine.PC = new ProgramCounter(245, 0);
          lastOpIndex = 244_0000;
          stepsCompleted += 13;

          //rref(90,{r1}-9,7,10
          new Const() { Value = 90 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 248, JumpOp = 0 }.Execute(ref machine);
          break;
        case 245_0000:
          machine.PC = new ProgramCounter(248, 0);
          lastOpIndex = 247_0000;
          stepsCompleted += 22;

          //rref(90,0,5,64)
          new Const() { Value = 90 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //ref(89,{r1}+1,6,64
          new Const() { Value = 89 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 248_0000:
          machine.PC = new ProgramCounter(249, 0);
          lastOpIndex = 248_0000;
          stepsCompleted += 1;

          //Goto EN
          new Goto() { LabelAddress = 260 }.Execute(ref machine);
          break;
        case 249_0000:
          machine.PC = new ProgramCounter(250, 5);
          lastOpIndex = 250_0004;
          stepsCompleted += 6;

          //Lbl S
          new Label().Execute(ref machine);

          //ReturnIf {r1}=0
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 251, JumpOp = 0 }.Execute(ref machine);
          break;
        case 250_0005:
          machine.PC = new ProgramCounter(251, 0);
          lastOpIndex = 250_0005;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 251_0000:
          machine.PC = new ProgramCounter(253, 0);
          lastOpIndex = 252_0000;
          stepsCompleted += 11;

          //{r1}>3 and ({r1}<9
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 253_0000:
          machine.PC = new ProgramCounter(257, 0);
          lastOpIndex = 256_0000;
          stepsCompleted += 25;

          //Lbl I8
          new Label().Execute(ref machine);

          //{r1}^18*{r3}->{{r2}}^^r
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //{r1}/18*{r3}->{{r2}+2}^^r
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 257_0000:
          machine.PC = new ProgramCounter(260, 0);
          lastOpIndex = 259_0000;
          stepsCompleted += 12;

          //Lbl SPT
          new Label().Execute(ref machine);

          //Pt-Off(C,D,{r1}+Pic1
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16568 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOverwrite>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 260_0000:
          machine.PC = new ProgramCounter(263, 0);
          lastOpIndex = 262_0001;
          stepsCompleted += 10;

          //Lbl EN
          new Label().Execute(ref machine);

          //expr(L6,L3,768
          new Const() { Value = 6400 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4096 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Exch() { ArgCount = 3 }.Execute(ref machine);

          //63->theta
          new Const() { Value = 63 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          break;
        case 263_0000:
          machine.PC = new ProgramCounter(264, 0);
          lastOpIndex = 263_0005;
          stepsCompleted += 6;

          //While pxl-Test(0,theta)^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 1, ArgCount = 2 }.Execute(ref machine);
          new While() { JumpLine = 266, JumpOp = 0 }.Execute(ref machine);
          break;
        case 264_0000:
          machine.PC = new ProgramCounter(266, 0);
          lastOpIndex = 265_0000;
          stepsCompleted += 6;

          //theta-1->theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 263, JumpOp = 0 }.Execute(ref machine);
          break;
        case 266_0000:
          machine.PC = new ProgramCounter(267, 0);
          lastOpIndex = 266_0002;
          stepsCompleted += 3;

          //If {L1+692}^^r
          new Const() { Value = 2996 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 418, JumpOp = 0 }.Execute(ref machine);
          break;
        case 267_0000:
          machine.PC = new ProgramCounter(275, 0);
          lastOpIndex = 274_0006;
          stepsCompleted += 51;

          //..AXE
          new Nop().Execute(ref machine);

          //Fix 5
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //theta-4*256->Z
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //90->X
          new Const() { Value = 90 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //{L1+702}^^r*5->Y
          new Const() { Value = 3006 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //ref(85,Y/256,5,5
          new Const() { Value = 85 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(85,Y/256,5,5
          new Const() { Value = 85 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //If {`Y0}^^r<{L1+704}^^r
          new FileHandle() { VarAddress = 1848 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1848, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 277, JumpOp = 0 }.Execute(ref machine);
          break;
        case 275_0000:
          machine.PC = new ProgramCounter(277, 0);
          lastOpIndex = 276_0000;
          stepsCompleted += 3;

          //50*256->Z
          new Const() { Value = 12800 }.Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 277_0000:
          machine.PC = new ProgramCounter(278, 0);
          lastOpIndex = 277_0002;
          stepsCompleted += 3;

          //!If {L1+700}^^r
          new Const() { Value = 3004 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 309, JumpOp = 0 }.Execute(ref machine);
          break;
        case 278_0000:
          machine.PC = new ProgramCounter(286, 0);
          lastOpIndex = 285_0001;
          stepsCompleted += 35;

          //conj(Pic2M,L6,768
          new Const() { Value = 16897 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //70->Y
          new Const() { Value = 70 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //0->X
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //ref(0,63,96,1
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 63 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 96 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(0,63,5,1
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 63 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //0->A
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //0->B
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //0->C
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          break;
        case 286_0000:
          machine.PC = new ProgramCounter(287, 0);
          lastOpIndex = 286_0004;
          stepsCompleted += 5;

          //While C-64
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new While() { JumpLine = 296, JumpOp = 0 }.Execute(ref machine);
          break;
        case 287_0000:
          machine.PC = new ProgramCounter(290, 0);
          lastOpIndex = 289_0004;
          stepsCompleted += 15;

          //A+B->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //B+1->B
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //If A>64
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 294, JumpOp = 0 }.Execute(ref machine);
          break;
        case 290_0000:
          machine.PC = new ProgramCounter(294, 0);
          lastOpIndex = 293_0000;
          stepsCompleted += 12;

          //Vertical -
          new Vertical() { Positive = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //A-64->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //C+1->C
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 294_0000:
          machine.PC = new ProgramCounter(295, 0);
          lastOpIndex = 294_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 295_0000:
          machine.PC = new ProgramCounter(296, 0);
          lastOpIndex = 295_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 286, JumpOp = 0 }.Execute(ref machine);
          break;
        case 296_0000:
          machine.PC = new ProgramCounter(297, 0);
          lastOpIndex = 296_0001;
          stepsCompleted += 2;

          //70*256->Y
          new Const() { Value = 17920 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);
          break;
        case 297_0000:
          machine.PC = new ProgramCounter(298, 0);
          lastOpIndex = 297_0004;
          stepsCompleted += 5;

          //Repeat Y<=Z
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U16<LessEq>().Execute(ref machine);
          new Repeat() { JumpLine = 306, JumpOp = 0 }.Execute(ref machine);
          break;
        case 298_0000:
          machine.PC = new ProgramCounter(304, 0);
          lastOpIndex = 303_0000;
          stepsCompleted += 49;

          //Y-128->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //ref(0,0,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(0,0,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Pt-On(0,Y/256,Pic2P
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16560 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //ref(0,Y/256+5,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 304_0000:
          machine.PC = new ProgramCounter(305, 0);
          lastOpIndex = 304_0002;
          stepsCompleted += 3;

          //Pause 10
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 305_0000:
          machine.PC = new ProgramCounter(306, 0);
          lastOpIndex = 305_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 297, JumpOp = 0 }.Execute(ref machine);
          break;
        case 306_0000:
          machine.PC = new ProgramCounter(307, 0);
          lastOpIndex = 306_0000;
          stepsCompleted += 1;

          //TALK()
          new Call() { LabelAddress = 1449, ArgCount = 0 }.Execute(ref machine);
          break;
        case 307_0000:
          machine.PC = new ProgramCounter(308, 0);
          lastOpIndex = 307_0000;
          stepsCompleted += 1;

          //Goto TR2
          new Goto() { LabelAddress = 373 }.Execute(ref machine);
          break;
        case 308_0000:
          machine.PC = new ProgramCounter(309, 0);
          lastOpIndex = 308_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 309_0000:
          machine.PC = new ProgramCounter(310, 0);
          lastOpIndex = 309_0002;
          stepsCompleted += 3;

          //If {L1+688}^^r
          new Const() { Value = 2992 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 344, JumpOp = 0 }.Execute(ref machine);
          break;
        case 310_0000:
          machine.PC = new ProgramCounter(311, 0);
          lastOpIndex = 310_0008;
          stepsCompleted += 9;

          //If {L1+704}^^r-18<7
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 343, JumpOp = 0 }.Execute(ref machine);
          break;
        case 311_0000:
          machine.PC = new ProgramCounter(315, 0);
          lastOpIndex = 314_0002;
          stepsCompleted += 8;

          //13->GDB0C
          new Const() { Value = 13 }.Execute(ref machine);

          //89->X
          new Const() { Value = 89 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //7->{r6}
          new Const() { Value = 7 }.Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //0->F->A
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          break;
        case 315_0000:
          machine.PC = new ProgramCounter(316, 0);
          lastOpIndex = 315_0004;
          stepsCompleted += 5;

          //Repeat F>=96
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 96 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new Repeat() { JumpLine = 341, JumpOp = 0 }.Execute(ref machine);
          break;
        case 316_0000:
          machine.PC = new ProgramCounter(317, 0);
          lastOpIndex = 316_0015;
          stepsCompleted += 16;

          //48-abs(F-48)+2+A->A
          new Const() { Value = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 48 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Abs() { ArgCount = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          break;
        case 317_0000:
          machine.PC = new ProgramCounter(318, 0);
          lastOpIndex = 317_0004;
          stepsCompleted += 5;

          //While A>GDB0C
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 13 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new While() { JumpLine = 326, JumpOp = 0 }.Execute(ref machine);
          break;
        case 318_0000:
          machine.PC = new ProgramCounter(319, 2);
          lastOpIndex = 319_0001;
          stepsCompleted += 3;

          //Horizontal -
          new Horizontal() { Positive = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //For(G,0,7
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          break;
        case 319_0002:
          machine.PC = new ProgramCounter(320, 0);
          lastOpIndex = 319_0003;
          stepsCompleted += 2;
          new Const() { Value = 7 }.Execute(ref machine);
          new For() { VarAddress = 12, JumpLine = 322, JumpOp = 0 }.Execute(ref machine);
          break;
        case 320_0000:
          machine.PC = new ProgramCounter(322, 0);
          lastOpIndex = 321_0000;
          stepsCompleted += 18;

          //Pt-On(95,G*8,Plot2(F,G*8)^^r
          new Const() { Value = 95 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtGet() { RMode = 1, ArgCount = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 12, JumpLine = 319, JumpOp = 2 }.Execute(ref machine);
          break;
        case 322_0000:
          machine.PC = new ProgramCounter(326, 0);
          lastOpIndex = 325_0000;
          stepsCompleted += 12;

          //F++
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //X--
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Dec().Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //A-GDB0C->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 13 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 317, JumpOp = 0 }.Execute(ref machine);
          break;
        case 326_0000:
          machine.PC = new ProgramCounter(327, 0);
          lastOpIndex = 326_0004;
          stepsCompleted += 5;

          //If F>48
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 48 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 334, JumpOp = 0 }.Execute(ref machine);
          break;
        case 327_0000:
          machine.PC = new ProgramCounter(328, 0);
          lastOpIndex = 327_0004;
          stepsCompleted += 5;

          //If F^2
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 333, JumpOp = 0 }.Execute(ref machine);
          break;
        case 328_0000:
          machine.PC = new ProgramCounter(329, 0);
          lastOpIndex = 328_0001;
          stepsCompleted += 2;

          //If {r6}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 332, JumpOp = 0 }.Execute(ref machine);
          break;
        case 329_0000:
          machine.PC = new ProgramCounter(332, 0);
          lastOpIndex = 331_0000;
          stepsCompleted += 7;

          //X++
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //{r6}--
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Dec().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 332_0000:
          machine.PC = new ProgramCounter(333, 0);
          lastOpIndex = 332_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 333_0000:
          machine.PC = new ProgramCounter(334, 0);
          lastOpIndex = 333_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 334_0000:
          machine.PC = new ProgramCounter(338, 0);
          lastOpIndex = 337_0000;
          stepsCompleted += 46;

          //32-({r6}*32/7/8*8)^32+Pic2->{r1}
          new Const() { Value = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16683 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //Pt-Change(X,Y/256->{r2},{r1}
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 770, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //Pxl-Change(X+3,{r2}+1
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 338_0000:
          machine.PC = new ProgramCounter(341, 0);
          lastOpIndex = 340_0000;
          stepsCompleted += 19;

          //Pxl-Change(X+3,{r2}+1
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pt-Change(X,{r2},{r1}
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 315, JumpOp = 0 }.Execute(ref machine);
          break;
        case 341_0000:
          machine.PC = new ProgramCounter(342, 0);
          lastOpIndex = 341_0000;
          stepsCompleted += 1;

          //Goto TR1
          new Goto() { LabelAddress = 416 }.Execute(ref machine);
          break;
        case 342_0000:
          machine.PC = new ProgramCounter(343, 0);
          lastOpIndex = 342_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 343_0000:
          machine.PC = new ProgramCounter(344, 0);
          lastOpIndex = 343_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 344_0000:
          machine.PC = new ProgramCounter(345, 0);
          lastOpIndex = 344_0002;
          stepsCompleted += 3;

          //0->B->A
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          break;
        case 345_0000:
          machine.PC = new ProgramCounter(346, 0);
          lastOpIndex = 345_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 372, JumpOp = 0 }.Execute(ref machine);
          break;
        case 346_0000:
          machine.PC = new ProgramCounter(347, 0);
          lastOpIndex = 346_0004;
          stepsCompleted += 5;

          //A-B->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          break;
        case 347_0000:
          machine.PC = new ProgramCounter(348, 0);
          lastOpIndex = 347_0004;
          stepsCompleted += 5;

          //While A<<0
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new While() { JumpLine = 351, JumpOp = 0 }.Execute(ref machine);
          break;
        case 348_0000:
          machine.PC = new ProgramCounter(351, 0);
          lastOpIndex = 350_0000;
          stepsCompleted += 7;

          //Vertical +
          new Vertical() { Positive = true, RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //A+256->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 347, JumpOp = 0 }.Execute(ref machine);
          break;
        case 351_0000:
          machine.PC = new ProgramCounter(358, 0);
          lastOpIndex = 357_0007;
          stepsCompleted += 34;

          //B+16->B
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //Horizontal -
          new Horizontal() { Positive = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //Horizontal -
          new Horizontal() { Positive = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //ref(94,0,2,64)
          new Const() { Value = 94 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //X-2->X
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //B/2->{r6}
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //If Y-{r6}>>Z
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_S16<GreaterS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 359, JumpOp = 1 }.Execute(ref machine);
          break;
        case 358_0000:
          machine.PC = new ProgramCounter(359, 1);
          lastOpIndex = 359_0000;
          stepsCompleted += 6;

          //Y-{r6}->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //ElseIf Y+{r6}<<Z
          new Else() { IsElseIf = true, JumpLine = 364, JumpOp = 0 }.Execute(ref machine);
          break;
        case 359_0001:
          machine.PC = new ProgramCounter(360, 0);
          lastOpIndex = 359_0008;
          stepsCompleted += 8;
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 362, JumpOp = 0 }.Execute(ref machine);
          break;
        case 360_0000:
          machine.PC = new ProgramCounter(362, 0);
          lastOpIndex = 361_0000;
          stepsCompleted += 6;

          //Y+{r6}->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 364, JumpOp = 0 }.Execute(ref machine);
          break;
        case 362_0000:
          machine.PC = new ProgramCounter(364, 0);
          lastOpIndex = 363_0000;
          stepsCompleted += 3;

          //Z->Y
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 364_0000:
          machine.PC = new ProgramCounter(367, 0);
          lastOpIndex = 366_0000;
          stepsCompleted += 29;

          //Pt-On(X,Y/256,Pic2P
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16560 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //ref(X-2,Y/256+5,8,32
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 367_0000:
          machine.PC = new ProgramCounter(368, 0);
          lastOpIndex = 367_0002;
          stepsCompleted += 3;

          //Pause 15
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 368_0000:
          machine.PC = new ProgramCounter(369, 0);
          lastOpIndex = 368_0001;
          stepsCompleted += 2;

          //If X
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 371, JumpOp = 0 }.Execute(ref machine);
          break;
        case 369_0000:
          machine.PC = new ProgramCounter(371, 0);
          lastOpIndex = 370_0000;
          stepsCompleted += 11;

          //Pt-Change(X,Y/256,Pic2P
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16560 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 371_0000:
          machine.PC = new ProgramCounter(371, 2);
          lastOpIndex = 371_0001;
          stepsCompleted += 2;

          //End!If X
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 372, JumpOp = 0 }.Execute(ref machine);
          break;
        case 371_0002:
          machine.PC = new ProgramCounter(372, 0);
          lastOpIndex = 371_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 345, JumpOp = 0 }.Execute(ref machine);
          break;
        case 372_0000:
          machine.PC = new ProgramCounter(373, 0);
          lastOpIndex = 372_0000;
          stepsCompleted += 1;

          //TALK()
          new Call() { LabelAddress = 1449, ArgCount = 0 }.Execute(ref machine);
          break;
        case 373_0000:
          machine.PC = new ProgramCounter(375, 0);
          lastOpIndex = 374_0006;
          stepsCompleted += 8;

          //Lbl TR2
          new Label().Execute(ref machine);

          //If {`Y0}<{L1+704}^^r
          new FileHandle() { VarAddress = 1848 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1848, RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 390, JumpOp = 0 }.Execute(ref machine);
          break;
        case 375_0000:
          machine.PC = new ProgramCounter(376, 0);
          lastOpIndex = 375_0004;
          stepsCompleted += 5;

          //Repeat Y<<~1200
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64336 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new Repeat() { JumpLine = 384, JumpOp = 0 }.Execute(ref machine);
          break;
        case 376_0000:
          machine.PC = new ProgramCounter(382, 0);
          lastOpIndex = 381_0000;
          stepsCompleted += 49;

          //Y-128->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //ref(0,0,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(0,0,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Pt-On(0,Y/256,Pic2P
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16560 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //ref(0,Y/256+5,5,66
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 66 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 382_0000:
          machine.PC = new ProgramCounter(383, 0);
          lastOpIndex = 382_0002;
          stepsCompleted += 3;

          //Pause 30
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 383_0000:
          machine.PC = new ProgramCounter(384, 0);
          lastOpIndex = 383_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 375, JumpOp = 0 }.Execute(ref machine);
          break;
        case 384_0000:
          machine.PC = new ProgramCounter(385, 0);
          lastOpIndex = 384_0002;
          stepsCompleted += 3;

          //Pause 1200
          new Const() { Value = 1200 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 385_0000:
          machine.PC = new ProgramCounter(386, 0);
          lastOpIndex = 385_0002;
          stepsCompleted += 3;

          //If {L1+688}^^r
          new Const() { Value = 2992 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 388, JumpOp = 0 }.Execute(ref machine);
          break;
        case 386_0000:
          machine.PC = new ProgramCounter(387, 0);
          lastOpIndex = 386_0000;
          stepsCompleted += 1;

          //Goto CREDITS
          new Goto() { LabelAddress = 577 }.Execute(ref machine);
          break;
        case 387_0000:
          machine.PC = new ProgramCounter(388, 0);
          lastOpIndex = 387_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 388_0000:
          machine.PC = new ProgramCounter(389, 0);
          lastOpIndex = 388_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 389_0000:
          machine.PC = new ProgramCounter(390, 0);
          lastOpIndex = 389_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 390_0000:
          machine.PC = new ProgramCounter(392, 0);
          lastOpIndex = 391_0001;
          stepsCompleted += 4;

          //256*2+210->B
          new Const() { Value = 722 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //63*256->C
          new Const() { Value = 16128 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          break;
        case 392_0000:
          machine.PC = new ProgramCounter(393, 0);
          lastOpIndex = 392_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 416, JumpOp = 0 }.Execute(ref machine);
          break;
        case 393_0000:
          machine.PC = new ProgramCounter(394, 0);
          lastOpIndex = 393_0007;
          stepsCompleted += 8;

          //If Y+128<<Z
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 395, JumpOp = 1 }.Execute(ref machine);
          break;
        case 394_0000:
          machine.PC = new ProgramCounter(395, 1);
          lastOpIndex = 395_0000;
          stepsCompleted += 6;

          //Y+128->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //ElseIf Y-128>>Z
          new Else() { IsElseIf = true, JumpLine = 400, JumpOp = 0 }.Execute(ref machine);
          break;
        case 395_0001:
          machine.PC = new ProgramCounter(396, 0);
          lastOpIndex = 395_0008;
          stepsCompleted += 8;
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_S16<GreaterS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 398, JumpOp = 0 }.Execute(ref machine);
          break;
        case 396_0000:
          machine.PC = new ProgramCounter(398, 0);
          lastOpIndex = 397_0000;
          stepsCompleted += 6;

          //Y-128->Y
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 400, JumpOp = 0 }.Execute(ref machine);
          break;
        case 398_0000:
          machine.PC = new ProgramCounter(400, 0);
          lastOpIndex = 399_0000;
          stepsCompleted += 3;

          //Z->Y
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 400_0000:
          machine.PC = new ProgramCounter(402, 0);
          lastOpIndex = 401_0017;
          stepsCompleted += 38;

          //conj(C/256*12->{r6}+L3,L6,768-{r6}
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4096 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //If (Y/256)-(C/256)-5->{r1}<<0
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 404, JumpOp = 0 }.Execute(ref machine);
          break;
        case 402_0000:
          machine.PC = new ProgramCounter(404, 0);
          lastOpIndex = 403_0000;
          stepsCompleted += 3;

          //0->{r1}
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 404_0000:
          machine.PC = new ProgramCounter(409, 0);
          lastOpIndex = 408_0004;
          stepsCompleted += 43;

          //ref(0,{r1},5,128
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(0,{r1},5,128
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //ref(0,Y/256+5,5,64
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //B-16->B
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //If B<32
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 411, JumpOp = 0 }.Execute(ref machine);
          break;
        case 409_0000:
          machine.PC = new ProgramCounter(411, 0);
          lastOpIndex = 410_0000;
          stepsCompleted += 3;

          //32->B
          new Const() { Value = 32 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 411_0000:
          machine.PC = new ProgramCounter(413, 0);
          lastOpIndex = 412_0000;
          stepsCompleted += 11;

          //Pt-On(X,Y/256,Pic2P
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16560 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 413_0000:
          machine.PC = new ProgramCounter(414, 0);
          lastOpIndex = 413_0002;
          stepsCompleted += 3;

          //Pause 20
          new Const() { Value = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 414_0000:
          machine.PC = new ProgramCounter(415, 5);
          lastOpIndex = 415_0004;
          stepsCompleted += 10;

          //C-B->C
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //EndIf C<<0
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 416, JumpOp = 0 }.Execute(ref machine);
          break;
        case 415_0005:
          machine.PC = new ProgramCounter(416, 0);
          lastOpIndex = 415_0005;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 392, JumpOp = 0 }.Execute(ref machine);
          break;
        case 416_0000:
          machine.PC = new ProgramCounter(418, 0);
          lastOpIndex = 417_0000;
          stepsCompleted += 2;

          //Lbl TR1
          new Label().Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 420, JumpOp = 0 }.Execute(ref machine);
          break;
        case 418_0000:
          machine.PC = new ProgramCounter(420, 0);
          lastOpIndex = 419_0000;
          stepsCompleted += 2;

          //RecallPic 
          new RecallPic().Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 420_0000:
          machine.PC = new ProgramCounter(427, 0);
          lastOpIndex = 426_0000;
          stepsCompleted += 38;

          //1->CUT->{L1+700}^^r->{L1+692}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2984, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 3004, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2996, RMode = 1 }.Execute(ref machine);

          //theta-4*256/5->Y
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //~9->N->O->S->T->Q->V
          new Const() { Value = 65527 }.Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 28, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 36, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 38, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 32, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 42, RMode = 1 }.Execute(ref machine);

          //Fill(L1,40,0
          new Const() { Value = 2304 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fill() { ArgCount = 3 }.Execute(ref machine);

          //2->{L1+14}^^r
          new Const() { Value = 2 }.Execute(ref machine);
          new StoreAddress() { Address = 2318, RMode = 1 }.Execute(ref machine);

          //0->X->F->A->B->K
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 427_0000:
          machine.PC = new ProgramCounter(428, 0);
          lastOpIndex = 427_0000;
          stepsCompleted += 1;

          //Lbl CONT
          new Label().Execute(ref machine);
          break;
        case 428_0000:
          machine.PC = new ProgramCounter(429, 0);
          lastOpIndex = 428_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 466, JumpOp = 0 }.Execute(ref machine);
          break;
        case 429_0000:
          machine.PC = new ProgramCounter(435, 0);
          lastOpIndex = 434_0000;
          stepsCompleted += 10;

          //RecallPic 
          new RecallPic().Execute(ref machine);

          //X->D
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //A->G
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //B->H
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //sub(PL)
          new Call() { LabelAddress = 739, ArgCount = 0 }.Execute(ref machine);
          break;
        case 435_0000:
          machine.PC = new ProgramCounter(436, 0);
          lastOpIndex = 435_0002;
          stepsCompleted += 3;

          //sub(PO,0)
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 713, ArgCount = 1 }.Execute(ref machine);
          break;
        case 436_0000:
          machine.PC = new ProgramCounter(437, 0);
          lastOpIndex = 436_0000;
          stepsCompleted += 1;

          //sub(PL)
          new Call() { LabelAddress = 739, ArgCount = 0 }.Execute(ref machine);
          break;
        case 437_0000:
          machine.PC = new ProgramCounter(438, 0);
          lastOpIndex = 437_0002;
          stepsCompleted += 3;

          //sub(PO,0)
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 713, ArgCount = 1 }.Execute(ref machine);
          break;
        case 438_0000:
          machine.PC = new ProgramCounter(444, 0);
          lastOpIndex = 443_0000;
          stepsCompleted += 10;

          //D->X
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);

          //E->Y
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //G->A
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //H->B
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new StoreAddress() { Address = 2, RMode = 1 }.Execute(ref machine);

          //FullScreen
          new Full().Execute(ref machine);

          //sub(P)
          new Call() { LabelAddress = 833, ArgCount = 0 }.Execute(ref machine);
          break;
        case 444_0000:
          machine.PC = new ProgramCounter(448, 0);
          lastOpIndex = 447_0001;
          stepsCompleted += 52;

          //Normal
          new Normal().Execute(ref machine);

          //Pt-On({L1+16}^^r,{L1+18}^^r,{L1+16}^^r/2^4*8*(A!=0 and {L1+12}^^r)+Pic2
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2316 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16683 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //Pxl-On({L1+16}^^r+1+{L1+14}^^r,{L1+18}^^r+1)
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2318 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterOr>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //If A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 450, JumpOp = 0 }.Execute(ref machine);
          break;
        case 448_0000:
          machine.PC = new ProgramCounter(450, 0);
          lastOpIndex = 449_0000;
          stepsCompleted += 3;

          //2->{L1+14}^^r
          new Const() { Value = 2 }.Execute(ref machine);
          new StoreAddress() { Address = 2318, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 450_0000:
          machine.PC = new ProgramCounter(451, 0);
          lastOpIndex = 450_0004;
          stepsCompleted += 5;

          //If A<<0
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 453, JumpOp = 0 }.Execute(ref machine);
          break;
        case 451_0000:
          machine.PC = new ProgramCounter(453, 0);
          lastOpIndex = 452_0000;
          stepsCompleted += 3;

          //0->{L1+14}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2318, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 453_0000:
          machine.PC = new ProgramCounter(454, 0);
          lastOpIndex = 453_0000;
          stepsCompleted += 1;

          //sub(OB)
          new Call() { LabelAddress = 543, ArgCount = 0 }.Execute(ref machine);
          break;
        case 454_0000:
          machine.PC = new ProgramCounter(455, 0);
          lastOpIndex = 454_0002;
          stepsCompleted += 3;

          //If {L1+690}^^r
          new Const() { Value = 2994 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 457, JumpOp = 0 }.Execute(ref machine);
          break;
        case 455_0000:
          machine.PC = new ProgramCounter(456, 0);
          lastOpIndex = 455_0000;
          stepsCompleted += 1;

          //Goto D
          new Goto() { LabelAddress = 627 }.Execute(ref machine);
          break;
        case 456_0000:
          machine.PC = new ProgramCounter(457, 0);
          lastOpIndex = 456_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 457_0000:
          machine.PC = new ProgramCounter(460, 0);
          lastOpIndex = 459_0000;
          stepsCompleted += 60;

          //Pt-Change(N,O,Q>=2*32+(F/2^4*8)+Pic1P
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16755 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //Pt-Change(S,T,V>=2*32+(F/2/2^4*8)+Pic1P
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16755 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 460_0000:
          machine.PC = new ProgramCounter(463, 0);
          lastOpIndex = 462_0010;
          stepsCompleted += 18;
          getKeysCompleted += 1;

          //F+1->F
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //If K=27 and (S!=~9
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 27 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65527 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 465, JumpOp = 0 }.Execute(ref machine);
          break;
        case 463_0000:
          machine.PC = new ProgramCounter(465, 0);
          lastOpIndex = 464_0000;
          stepsCompleted += 8;

          //expr(^^oN,^^oS,10
          new Const() { Value = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Exch() { ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 465_0000:
          machine.PC = new ProgramCounter(465, 14);
          lastOpIndex = 465_0013;
          stepsCompleted += 14;
          getKeysCompleted += 1;

          //EndIf X>4556 and {L1+12}^^r or getKey(15)
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4556 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2316 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 466, JumpOp = 0 }.Execute(ref machine);
          break;
        case 465_0014:
          machine.PC = new ProgramCounter(466, 0);
          lastOpIndex = 465_0014;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 428, JumpOp = 0 }.Execute(ref machine);
          break;
        case 466_0000:
          machine.PC = new ProgramCounter(467, 0);
          lastOpIndex = 466_0003;
          stepsCompleted += 4;
          getKeysCompleted += 1;

          //If getKey(15)
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 495, JumpOp = 0 }.Execute(ref machine);
          break;
        case 467_0000:
          machine.PC = new ProgramCounter(470, 0);
          lastOpIndex = 469_0002;
          stepsCompleted += 7;

          //..AXE
          new Nop().Execute(ref machine);

          //Fix 5
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //1->theta->K
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          break;
        case 470_0000:
          machine.PC = new ProgramCounter(471, 0);
          lastOpIndex = 470_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 484, JumpOp = 0 }.Execute(ref machine);
          break;
        case 471_0000:
          machine.PC = new ProgramCounter(473, 0);
          lastOpIndex = 472_0006;
          stepsCompleted += 8;

          //ClrDraw
          new ClrDraw() { RMode = 0, ArgCount = 0 }.Execute(ref machine);

          //Text(32,1,"CONTINUE
          new Const() { Value = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16528 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 473_0000:
          machine.PC = new ProgramCounter(474, 0);
          lastOpIndex = 473_0006;
          stepsCompleted += 7;

          //Text(34,10,"RESTART
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16537 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 474_0000:
          machine.PC = new ProgramCounter(475, 0);
          lastOpIndex = 474_0006;
          stepsCompleted += 7;

          //Text(40,19,"EXIT
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 19 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16545 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 475_0000:
          machine.PC = new ProgramCounter(477, 0);
          lastOpIndex = 476_0011;
          stepsCompleted += 21;

          //ref(33,theta,30,1
          new Const() { Value = 33 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //ref(33,theta+6,30,1
          new Const() { Value = 33 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);
          break;
        case 477_0000:
          machine.PC = new ProgramCounter(478, 0);
          lastOpIndex = 477_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 481, JumpOp = 0 }.Execute(ref machine);
          break;
        case 478_0000:
          machine.PC = new ProgramCounter(479, 0);
          lastOpIndex = 478_0000;
          stepsCompleted += 1;

          //DispGraph^^r
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 1, ArgCount = 0 }.Execute(ref machine);
          return;
        case 479_0000:
          machine.PC = new ProgramCounter(480, 2);
          lastOpIndex = 480_0001;
          stepsCompleted += 4;
          getKeysCompleted += 1;

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //EndIf K
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 481, JumpOp = 0 }.Execute(ref machine);
          break;
        case 480_0002:
          machine.PC = new ProgramCounter(481, 0);
          lastOpIndex = 480_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 477, JumpOp = 0 }.Execute(ref machine);
          break;
        case 481_0000:
          machine.PC = new ProgramCounter(483, 11);
          lastOpIndex = 483_0010;
          stepsCompleted += 45;

          //K=1-(K=4)*9+theta->theta
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //theta=~8-(theta=28)*27+theta->theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65528 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 28 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 27 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //EndIf K=15 or (K=54
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 54 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 484, JumpOp = 0 }.Execute(ref machine);
          break;
        case 483_0011:
          machine.PC = new ProgramCounter(484, 0);
          lastOpIndex = 483_0011;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 470, JumpOp = 0 }.Execute(ref machine);
          break;
        case 484_0000:
          machine.PC = new ProgramCounter(485, 0);
          lastOpIndex = 484_0004;
          stepsCompleted += 5;

          //If theta=10
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 490, JumpOp = 0 }.Execute(ref machine);
          break;
        case 485_0000:
          machine.PC = new ProgramCounter(488, 0);
          lastOpIndex = 487_0000;
          stepsCompleted += 4;

          //0->{L1+692}
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2996, RMode = 0 }.Execute(ref machine);

          //RecallPic 
          new RecallPic().Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 488_0000:
          machine.PC = new ProgramCounter(489, 0);
          lastOpIndex = 488_0000;
          stepsCompleted += 1;

          //Goto TOP
          new Goto() { LabelAddress = 97 }.Execute(ref machine);
          break;
        case 489_0000:
          machine.PC = new ProgramCounter(490, 0);
          lastOpIndex = 489_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 490_0000:
          machine.PC = new ProgramCounter(491, 0);
          lastOpIndex = 490_0004;
          stepsCompleted += 5;

          //If theta=19
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 19 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 493, JumpOp = 0 }.Execute(ref machine);
          break;
        case 491_0000:
          machine.PC = new ProgramCounter(492, 0);
          lastOpIndex = 491_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 492_0000:
          machine.PC = new ProgramCounter(493, 0);
          lastOpIndex = 492_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 493_0000:
          machine.PC = new ProgramCounter(494, 0);
          lastOpIndex = 493_0000;
          stepsCompleted += 1;

          //Goto CONT
          new Goto() { LabelAddress = 427 }.Execute(ref machine);
          break;
        case 494_0000:
          machine.PC = new ProgramCounter(495, 0);
          lastOpIndex = 494_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 495_0000:
          machine.PC = new ProgramCounter(497, 0);
          lastOpIndex = 496_0002;
          stepsCompleted += 4;

          //..AXE
          new Nop().Execute(ref machine);

          //If {L1+688}
          new Const() { Value = 2992 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 540, JumpOp = 0 }.Execute(ref machine);
          break;
        case 497_0000:
          machine.PC = new ProgramCounter(498, 0);
          lastOpIndex = 497_0005;
          stepsCompleted += 6;

          //!If {L1+704}-17
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 17 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 539, JumpOp = 0 }.Execute(ref machine);
          break;
        case 498_0000:
          machine.PC = new ProgramCounter(499, 0);
          lastOpIndex = 498_0001;
          stepsCompleted += 2;

          //If CUT
          new ReadAddress() { VarAddress = 2984 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 535, JumpOp = 0 }.Execute(ref machine);
          break;
        case 499_0000:
          machine.PC = new ProgramCounter(500, 2);
          lastOpIndex = 500_0001;
          stepsCompleted += 3;

          //DeltaList(~12,0,12,0)->Str1C89
          new Nop().Execute(ref machine);

          //For(F,0,31
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 500_0002:
          machine.PC = new ProgramCounter(501, 0);
          lastOpIndex = 500_0003;
          stepsCompleted += 2;
          new Const() { Value = 31 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 504, JumpOp = 0 }.Execute(ref machine);
          break;
        case 501_0000:
          machine.PC = new ProgramCounter(502, 0);
          lastOpIndex = 501_0012;
          stepsCompleted += 13;

          //DispGraph(int(F^4+Str1C89}+L6
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16550 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemorySignedByte().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6400 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 1 }.Execute(ref machine);
          return;
        case 502_0000:
          machine.PC = new ProgramCounter(503, 0);
          lastOpIndex = 502_0008;
          stepsCompleted += 9;

          //Pause F+1*10
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 503_0000:
          machine.PC = new ProgramCounter(504, 0);
          lastOpIndex = 503_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 500, JumpOp = 2 }.Execute(ref machine);
          break;
        case 504_0000:
          machine.PC = new ProgramCounter(505, 0);
          lastOpIndex = 504_0002;
          stepsCompleted += 3;

          //Pause 800
          new Const() { Value = 800 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 505_0000:
          machine.PC = new ProgramCounter(505, 2);
          lastOpIndex = 505_0001;
          stepsCompleted += 2;

          //For(F,18,19)
          new Const() { Value = 18 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 505_0002:
          machine.PC = new ProgramCounter(506, 0);
          lastOpIndex = 505_0003;
          stepsCompleted += 2;
          new Const() { Value = 19 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 520, JumpOp = 0 }.Execute(ref machine);
          break;
        case 506_0000:
          machine.PC = new ProgramCounter(507, 0);
          lastOpIndex = 506_0002;
          stepsCompleted += 3;

          //LDSTR(F)
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 1398, ArgCount = 1 }.Execute(ref machine);
          break;
        case 507_0000:
          machine.PC = new ProgramCounter(508, 0);
          lastOpIndex = 507_0000;
          stepsCompleted += 1;

          //CLRTOP()
          new Call() { LabelAddress = 1394, ArgCount = 0 }.Execute(ref machine);
          break;
        case 508_0000:
          machine.PC = new ProgramCounter(511, 0);
          lastOpIndex = 510_0002;
          stepsCompleted += 8;

          //|LCLRTOP->{r4}
          new Const() { Value = 1394 }.Execute(ref machine);
          new StoreAddress() { Address = 774, RMode = 1 }.Execute(ref machine);

          //0->{r1}->{^^oPENX}
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //1->K->{^^oPENY}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);
          break;
        case 511_0000:
          machine.PC = new ProgramCounter(512, 0);
          lastOpIndex = 511_0002;
          stepsCompleted += 3;

          //While {{r3}}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new While() { JumpLine = 518, JumpOp = 0 }.Execute(ref machine);
          break;
        case 512_0000:
          machine.PC = new ProgramCounter(513, 0);
          lastOpIndex = 512_0001;
          stepsCompleted += 2;

          //If K
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 515, JumpOp = 0 }.Execute(ref machine);
          break;
        case 513_0000:
          machine.PC = new ProgramCounter(514, 0);
          lastOpIndex = 513_0000;
          stepsCompleted += 1;

          //TEXT()
          new Call() { LabelAddress = 1417, ArgCount = 0 }.Execute(ref machine);
          break;
        case 514_0000:
          machine.PC = new ProgramCounter(515, 0);
          lastOpIndex = 514_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 517, JumpOp = 0 }.Execute(ref machine);
          break;
        case 515_0000:
          machine.PC = new ProgramCounter(517, 0);
          lastOpIndex = 516_0000;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 517_0000:
          machine.PC = new ProgramCounter(518, 0);
          lastOpIndex = 517_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 511, JumpOp = 0 }.Execute(ref machine);
          break;
        case 518_0000:
          machine.PC = new ProgramCounter(519, 0);
          lastOpIndex = 518_0000;
          stepsCompleted += 1;
          getKeysCompleted += 1;

          //getKey^^r
          new GetKey() { RMode = 1, ArgCount = 0 }.Execute(ref machine);
          return;
        case 519_0000:
          machine.PC = new ProgramCounter(520, 0);
          lastOpIndex = 519_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 505, JumpOp = 2 }.Execute(ref machine);
          break;
        case 520_0000:
          machine.PC = new ProgramCounter(520, 2);
          lastOpIndex = 520_0001;
          stepsCompleted += 2;

          //For(F,65,95
          new Const() { Value = 65 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 520_0002:
          machine.PC = new ProgramCounter(521, 0);
          lastOpIndex = 520_0003;
          stepsCompleted += 2;
          new Const() { Value = 95 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 528, JumpOp = 0 }.Execute(ref machine);
          break;
        case 521_0000:
          machine.PC = new ProgramCounter(521, 2);
          lastOpIndex = 521_0001;
          stepsCompleted += 2;

          //For(G,50,54
          new Const() { Value = 50 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          break;
        case 521_0002:
          machine.PC = new ProgramCounter(522, 0);
          lastOpIndex = 521_0003;
          stepsCompleted += 2;
          new Const() { Value = 54 }.Execute(ref machine);
          new For() { VarAddress = 12, JumpLine = 526, JumpOp = 0 }.Execute(ref machine);
          break;
        case 522_0000:
          machine.PC = new ProgramCounter(524, 0);
          lastOpIndex = 523_0000;
          stepsCompleted += 10;

          //rref(F,G,5,1
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 524_0000:
          machine.PC = new ProgramCounter(525, 0);
          lastOpIndex = 524_0002;
          stepsCompleted += 3;

          //Pause 100
          new Const() { Value = 100 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 525_0000:
          machine.PC = new ProgramCounter(526, 0);
          lastOpIndex = 525_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 12, JumpLine = 521, JumpOp = 2 }.Execute(ref machine);
          break;
        case 526_0000:
          machine.PC = new ProgramCounter(528, 0);
          lastOpIndex = 527_0000;
          stepsCompleted += 6;

          //F+4->F
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 10, JumpLine = 520, JumpOp = 2 }.Execute(ref machine);
          break;
        case 528_0000:
          machine.PC = new ProgramCounter(532, 0);
          lastOpIndex = 531_0002;
          stepsCompleted += 20;

          //RecallPic 
          new RecallPic().Execute(ref machine);

          //rref(65,50,31,5
          new Const() { Value = 65 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 31 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Fill(L4+193,5,0)
          new Const() { Value = 5057 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fill() { ArgCount = 3 }.Execute(ref machine);

          //Pause 300
          new Const() { Value = 300 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 532_0000:
          machine.PC = new ProgramCounter(535, 0);
          lastOpIndex = 534_0000;
          stepsCompleted += 4;

          //StorePic 
          new StorePic().Execute(ref machine);

          //0->CUT
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2984, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 535_0000:
          machine.PC = new ProgramCounter(536, 0);
          lastOpIndex = 535_0004;
          stepsCompleted += 5;

          //If Y<1280
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1280 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 538, JumpOp = 0 }.Execute(ref machine);
          break;
        case 536_0000:
          machine.PC = new ProgramCounter(537, 0);
          lastOpIndex = 536_0000;
          stepsCompleted += 1;

          //Goto CONT
          new Goto() { LabelAddress = 427 }.Execute(ref machine);
          break;
        case 537_0000:
          machine.PC = new ProgramCounter(538, 0);
          lastOpIndex = 537_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 538_0000:
          machine.PC = new ProgramCounter(539, 0);
          lastOpIndex = 538_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 539_0000:
          machine.PC = new ProgramCounter(540, 0);
          lastOpIndex = 539_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 540_0000:
          machine.PC = new ProgramCounter(543, 0);
          lastOpIndex = 542_0000;
          stepsCompleted += 17;

          //{L1+704}+1->{L1+704}
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 3008, RMode = 0 }.Execute(ref machine);

          //max({L1+704},{PSAVE})->{PSAVE}
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Max() { ArgCount = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //Goto TOP
          new Goto() { LabelAddress = 97 }.Execute(ref machine);
          break;
        case 543_0000:
          machine.PC = new ProgramCounter(546, 0);
          lastOpIndex = 545_0003;
          stepsCompleted += 12;
          getKeysCompleted += 1;

          //Lbl OB
          new Label().Execute(ref machine);

          //0->{L1+20}^^r->{L1+36}^^r+1->L
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2324, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2340, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //If getKey(54)
          new Const() { Value = 54 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 551, JumpOp = 0 }.Execute(ref machine);
          break;
        case 546_0000:
          machine.PC = new ProgramCounter(547, 0);
          lastOpIndex = 546_0002;
          stepsCompleted += 3;

          //If {L1+32}^^r
          new Const() { Value = 2336 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 549, JumpOp = 0 }.Execute(ref machine);
          break;
        case 547_0000:
          machine.PC = new ProgramCounter(549, 0);
          lastOpIndex = 548_0000;
          stepsCompleted += 3;

          //1->{L1+36}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2340, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 549_0000:
          machine.PC = new ProgramCounter(551, 0);
          lastOpIndex = 550_0000;
          stepsCompleted += 3;

          //0->{L1+32}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2336, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 553, JumpOp = 0 }.Execute(ref machine);
          break;
        case 551_0000:
          machine.PC = new ProgramCounter(553, 0);
          lastOpIndex = 552_0000;
          stepsCompleted += 3;

          //1->{L1+32}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2336, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 553_0000:
          machine.PC = new ProgramCounter(553, 2);
          lastOpIndex = 553_0001;
          stepsCompleted += 2;

          //For(P,1,{L5}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 30, RMode = 1 }.Execute(ref machine);
          break;
        case 553_0002:
          machine.PC = new ProgramCounter(554, 0);
          lastOpIndex = 553_0004;
          stepsCompleted += 3;
          new Const() { Value = 5632 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new For() { VarAddress = 30, JumpLine = 575, JumpOp = 0 }.Execute(ref machine);
          break;
        case 554_0000:
          machine.PC = new ProgramCounter(561, 0);
          lastOpIndex = 560_0004;
          stepsCompleted += 40;

          //P*16+L5->{r6}
          new ReadAddress() { VarAddress = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5632 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //{{r6}}->{r5}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 776, RMode = 1 }.Execute(ref machine);

          //{{r6}+2}^^r->D
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{{r6}+4}^^r->E
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //{{r6}+6}^^r->G
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //{{r6}+8}^^r->H
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //!If {r5}-1
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 564, JumpOp = 0 }.Execute(ref machine);
          break;
        case 561_0000:
          machine.PC = new ProgramCounter(562, 0);
          lastOpIndex = 561_0002;
          stepsCompleted += 3;

          //sub(O1,1)
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 1056, ArgCount = 1 }.Execute(ref machine);
          break;
        case 562_0000:
          machine.PC = new ProgramCounter(563, 0);
          lastOpIndex = 562_0002;
          stepsCompleted += 3;

          //sub(O1,0)
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 1056, ArgCount = 1 }.Execute(ref machine);
          break;
        case 563_0000:
          machine.PC = new ProgramCounter(564, 0);
          lastOpIndex = 563_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 564_0000:
          machine.PC = new ProgramCounter(565, 0);
          lastOpIndex = 564_0004;
          stepsCompleted += 5;

          //!If {r5}-2
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 567, JumpOp = 0 }.Execute(ref machine);
          break;
        case 565_0000:
          machine.PC = new ProgramCounter(566, 0);
          lastOpIndex = 565_0000;
          stepsCompleted += 1;

          //sub(O2)
          new Call() { LabelAddress = 1144, ArgCount = 0 }.Execute(ref machine);
          break;
        case 566_0000:
          machine.PC = new ProgramCounter(567, 0);
          lastOpIndex = 566_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 567_0000:
          machine.PC = new ProgramCounter(568, 0);
          lastOpIndex = 567_0004;
          stepsCompleted += 5;

          //!If {r5}-3
          new ReadAddress() { VarAddress = 776 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 570, JumpOp = 0 }.Execute(ref machine);
          break;
        case 568_0000:
          machine.PC = new ProgramCounter(569, 0);
          lastOpIndex = 568_0000;
          stepsCompleted += 1;

          //sub(O3)
          new Call() { LabelAddress = 1008, ArgCount = 0 }.Execute(ref machine);
          break;
        case 569_0000:
          machine.PC = new ProgramCounter(570, 0);
          lastOpIndex = 569_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 570_0000:
          machine.PC = new ProgramCounter(575, 0);
          lastOpIndex = 574_0000;
          stepsCompleted += 29;

          //D->{{r6}+2}^^r
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //E->{{r6}+4}^^r
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //G->{{r6}+6}^^r
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //H->{{r6}+8}^^r
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 30, JumpLine = 553, JumpOp = 2 }.Execute(ref machine);
          break;
        case 575_0000:
          machine.PC = new ProgramCounter(576, 0);
          lastOpIndex = 575_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 576_0000:
          machine.PC = new ProgramCounter(577, 0);
          lastOpIndex = 576_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 577_0000:
          machine.PC = new ProgramCounter(580, 2);
          lastOpIndex = 580_0001;
          stepsCompleted += 8;

          //Lbl CREDITS
          new Label().Execute(ref machine);

          //~30->G
          new Const() { Value = 65506 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //Fix 3
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //For(F,34,38
          new Const() { Value = 34 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 580_0002:
          machine.PC = new ProgramCounter(581, 0);
          lastOpIndex = 580_0003;
          stepsCompleted += 2;
          new Const() { Value = 38 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 602, JumpOp = 0 }.Execute(ref machine);
          break;
        case 581_0000:
          machine.PC = new ProgramCounter(582, 0);
          lastOpIndex = 581_0002;
          stepsCompleted += 3;

          //LDSTR(F)
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 1398, ArgCount = 1 }.Execute(ref machine);
          break;
        case 582_0000:
          machine.PC = new ProgramCounter(585, 0);
          lastOpIndex = 584_0000;
          stepsCompleted += 7;

          //3->{r1}->{^^oPENX}
          new Const() { Value = 3 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //1->K->{^^oPENY}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //CLRCR()
          new Call() { LabelAddress = 1391, ArgCount = 0 }.Execute(ref machine);
          break;
        case 585_0000:
          machine.PC = new ProgramCounter(586, 0);
          lastOpIndex = 585_0001;
          stepsCompleted += 2;

          //|LCLRCR->{r4}
          new Const() { Value = 1391 }.Execute(ref machine);
          new StoreAddress() { Address = 774, RMode = 1 }.Execute(ref machine);
          break;
        case 586_0000:
          machine.PC = new ProgramCounter(587, 0);
          lastOpIndex = 586_0002;
          stepsCompleted += 3;

          //While {{r3}}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new While() { JumpLine = 595, JumpOp = 0 }.Execute(ref machine);
          break;
        case 587_0000:
          machine.PC = new ProgramCounter(588, 0);
          lastOpIndex = 587_0000;
          stepsCompleted += 1;

          //CRSLOT()
          new Call() { LabelAddress = 617, ArgCount = 0 }.Execute(ref machine);
          break;
        case 588_0000:
          machine.PC = new ProgramCounter(589, 0);
          lastOpIndex = 588_0001;
          stepsCompleted += 2;

          //If K
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 591, JumpOp = 0 }.Execute(ref machine);
          break;
        case 589_0000:
          machine.PC = new ProgramCounter(590, 0);
          lastOpIndex = 589_0000;
          stepsCompleted += 1;

          //TEXT()
          new Call() { LabelAddress = 1417, ArgCount = 0 }.Execute(ref machine);
          break;
        case 590_0000:
          machine.PC = new ProgramCounter(591, 0);
          lastOpIndex = 590_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 594, JumpOp = 0 }.Execute(ref machine);
          break;
        case 591_0000:
          machine.PC = new ProgramCounter(593, 0);
          lastOpIndex = 592_0000;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 593_0000:
          machine.PC = new ProgramCounter(594, 0);
          lastOpIndex = 593_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 594_0000:
          machine.PC = new ProgramCounter(595, 0);
          lastOpIndex = 594_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 586, JumpOp = 0 }.Execute(ref machine);
          break;
        case 595_0000:
          machine.PC = new ProgramCounter(596, 0);
          lastOpIndex = 595_0001;
          stepsCompleted += 2;

          //0->K
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          break;
        case 596_0000:
          machine.PC = new ProgramCounter(597, 0);
          lastOpIndex = 596_0001;
          stepsCompleted += 2;

          //Repeat K
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new Repeat() { JumpLine = 601, JumpOp = 0 }.Execute(ref machine);
          break;
        case 597_0000:
          machine.PC = new ProgramCounter(599, 0);
          lastOpIndex = 598_0000;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //CRSLOT()
          new Call() { LabelAddress = 617, ArgCount = 0 }.Execute(ref machine);
          break;
        case 599_0000:
          machine.PC = new ProgramCounter(600, 0);
          lastOpIndex = 599_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 600_0000:
          machine.PC = new ProgramCounter(601, 0);
          lastOpIndex = 600_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 596, JumpOp = 0 }.Execute(ref machine);
          break;
        case 601_0000:
          machine.PC = new ProgramCounter(602, 0);
          lastOpIndex = 601_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 580, JumpOp = 2 }.Execute(ref machine);
          break;
        case 602_0000:
          machine.PC = new ProgramCounter(603, 0);
          lastOpIndex = 602_0000;
          stepsCompleted += 1;

          //CLRCR()
          new Call() { LabelAddress = 1391, ArgCount = 0 }.Execute(ref machine);
          break;
        case 603_0000:
          machine.PC = new ProgramCounter(604, 0);
          lastOpIndex = 603_0004;
          stepsCompleted += 5;

          //Repeat G>=>=64
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_S16<GreaterEqS>().Execute(ref machine);
          new Repeat() { JumpLine = 607, JumpOp = 0 }.Execute(ref machine);
          break;
        case 604_0000:
          machine.PC = new ProgramCounter(605, 0);
          lastOpIndex = 604_0000;
          stepsCompleted += 1;

          //CRSLOT()
          new Call() { LabelAddress = 617, ArgCount = 0 }.Execute(ref machine);
          break;
        case 605_0000:
          machine.PC = new ProgramCounter(606, 0);
          lastOpIndex = 605_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 606_0000:
          machine.PC = new ProgramCounter(607, 0);
          lastOpIndex = 606_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 603, JumpOp = 0 }.Execute(ref machine);
          break;
        case 607_0000:
          machine.PC = new ProgramCounter(609, 0);
          lastOpIndex = 608_0002;
          stepsCompleted += 6;

          //Fix 2
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //Pause 800
          new Const() { Value = 800 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 609_0000:
          machine.PC = new ProgramCounter(609, 2);
          lastOpIndex = 609_0001;
          stepsCompleted += 2;

          //For(F,0,165
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 609_0002:
          machine.PC = new ProgramCounter(610, 0);
          lastOpIndex = 609_0003;
          stepsCompleted += 2;
          new Const() { Value = 165 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 616, JumpOp = 0 }.Execute(ref machine);
          break;
        case 610_0000:
          machine.PC = new ProgramCounter(610, 2);
          lastOpIndex = 610_0001;
          stepsCompleted += 2;

          //For(G,0,63
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          break;
        case 610_0002:
          machine.PC = new ProgramCounter(611, 0);
          lastOpIndex = 610_0003;
          stepsCompleted += 2;
          new Const() { Value = 63 }.Execute(ref machine);
          new For() { VarAddress = 12, JumpLine = 614, JumpOp = 0 }.Execute(ref machine);
          break;
        case 611_0000:
          machine.PC = new ProgramCounter(614, 0);
          lastOpIndex = 613_0000;
          stepsCompleted += 18;

          //rref(F-G,G,1,4
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //G+3->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 12, JumpLine = 610, JumpOp = 2 }.Execute(ref machine);
          break;
        case 614_0000:
          machine.PC = new ProgramCounter(615, 0);
          lastOpIndex = 614_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 615_0000:
          machine.PC = new ProgramCounter(616, 0);
          lastOpIndex = 615_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 609, JumpOp = 2 }.Execute(ref machine);
          break;
        case 616_0000:
          machine.PC = new ProgramCounter(617, 0);
          lastOpIndex = 616_0000;
          stepsCompleted += 1;

          //Goto START
          new Goto() { LabelAddress = 28 }.Execute(ref machine);
          break;
        case 617_0000:
          machine.PC = new ProgramCounter(623, 0);
          lastOpIndex = 622_0008;
          stepsCompleted += 36;

          //Lbl CRSLOT
          new Label().Execute(ref machine);

          //Pxl-Change(1,G+20
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 20 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Change(1,G
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Change(94,G+20
          new Const() { Value = 94 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 20 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Change(94,G
          new Const() { Value = 94 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //If G+1->G=80
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 80 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 625, JumpOp = 0 }.Execute(ref machine);
          break;
        case 623_0000:
          machine.PC = new ProgramCounter(625, 0);
          lastOpIndex = 624_0000;
          stepsCompleted += 3;

          //~20->G
          new Const() { Value = 65516 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 625_0000:
          machine.PC = new ProgramCounter(626, 0);
          lastOpIndex = 625_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 626_0000:
          machine.PC = new ProgramCounter(627, 0);
          lastOpIndex = 626_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 627_0000:
          machine.PC = new ProgramCounter(628, 2);
          lastOpIndex = 628_0001;
          stepsCompleted += 3;

          //Lbl D
          new Label().Execute(ref machine);

          //For(F,0,40
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 628_0002:
          machine.PC = new ProgramCounter(629, 0);
          lastOpIndex = 628_0003;
          stepsCompleted += 2;
          new Const() { Value = 40 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 635, JumpOp = 0 }.Execute(ref machine);
          break;
        case 629_0000:
          machine.PC = new ProgramCounter(635, 0);
          lastOpIndex = 634_0000;
          stepsCompleted += 61;

          //F*8+L1+200->L
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 200 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //rand^256+X->{L}^^r
          new Rand().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //rand^256+Y->{L+2}^^r
          new Rand().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //rand^32-16->{L+4}^^r
          new Rand().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //rand^32-16->{L+6}^^r
          new Rand().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 10, JumpLine = 628, JumpOp = 2 }.Execute(ref machine);
          break;
        case 635_0000:
          machine.PC = new ProgramCounter(635, 2);
          lastOpIndex = 635_0001;
          stepsCompleted += 2;

          //For(F,0,60
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 635_0002:
          machine.PC = new ProgramCounter(636, 0);
          lastOpIndex = 635_0003;
          stepsCompleted += 2;
          new Const() { Value = 60 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 647, JumpOp = 0 }.Execute(ref machine);
          break;
        case 636_0000:
          machine.PC = new ProgramCounter(637, 2);
          lastOpIndex = 637_0001;
          stepsCompleted += 3;

          //RecallPic 
          new RecallPic().Execute(ref machine);

          //For(G,0,40
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          break;
        case 637_0002:
          machine.PC = new ProgramCounter(638, 0);
          lastOpIndex = 637_0003;
          stepsCompleted += 2;
          new Const() { Value = 40 }.Execute(ref machine);
          new For() { VarAddress = 12, JumpLine = 644, JumpOp = 0 }.Execute(ref machine);
          break;
        case 638_0000:
          machine.PC = new ProgramCounter(644, 0);
          lastOpIndex = 643_0000;
          stepsCompleted += 78;

          //G*8+L1+200->L
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 200 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //{L}^^r+{L+4}^^r->{L}^^r
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //{L+6}^^r+1->{L+6}^^r
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //{L+2}^^r+{L+6}^^r->{L+2}^^r
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //Pxl-On({L}^^r*5/256,{L+2}^^r*5/256
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterOr>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 12, JumpLine = 637, JumpOp = 2 }.Execute(ref machine);
          break;
        case 644_0000:
          machine.PC = new ProgramCounter(645, 0);
          lastOpIndex = 644_0000;
          stepsCompleted += 1;

          //sub(OB)
          new Call() { LabelAddress = 543, ArgCount = 0 }.Execute(ref machine);
          break;
        case 645_0000:
          machine.PC = new ProgramCounter(646, 0);
          lastOpIndex = 645_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 646_0000:
          machine.PC = new ProgramCounter(647, 0);
          lastOpIndex = 646_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 635, JumpOp = 2 }.Execute(ref machine);
          break;
        case 647_0000:
          machine.PC = new ProgramCounter(649, 0);
          lastOpIndex = 648_0000;
          stepsCompleted += 3;

          //0->{L1+692}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2996, RMode = 1 }.Execute(ref machine);

          //Goto TOP
          new Goto() { LabelAddress = 97 }.Execute(ref machine);
          break;
        case 649_0000:
          machine.PC = new ProgramCounter(650, 0);
          lastOpIndex = 649_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 650_0000:
          machine.PC = new ProgramCounter(651, 0);
          lastOpIndex = 650_0000;
          stepsCompleted += 1;

          //..Portal
          new Nop().Execute(ref machine);
          break;
        case 651_0000:
          machine.PC = new ProgramCounter(653, 0);
          lastOpIndex = 652_0002;
          stepsCompleted += 4;

          //Lbl C
          new Label().Execute(ref machine);

          //If {L1+20}^^r
          new Const() { Value = 2324 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 655, JumpOp = 0 }.Execute(ref machine);
          break;
        case 653_0000:
          machine.PC = new ProgramCounter(655, 0);
          lastOpIndex = 654_0000;
          stepsCompleted += 19;

          //E->{L1+24}^^r/5*6*3+(D->{L1+22}^^r/5)
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 2328, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new StoreAddress() { Address = 2326, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 659, JumpOp = 0 }.Execute(ref machine);
          break;
        case 655_0000:
          machine.PC = new ProgramCounter(659, 0);
          lastOpIndex = 658_0000;
          stepsCompleted += 29;

          //D*5/256->{L1+22}^^r
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 2326, RMode = 1 }.Execute(ref machine);

          //E*5/256->{L1+24}^^r
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 2328, RMode = 1 }.Execute(ref machine);

          //{^^oE+1}*6*3+{^^oD+1}
          new Const() { Value = 9 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 7 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 659_0000:
          machine.PC = new ProgramCounter(661, 0);
          lastOpIndex = 660_0004;
          stepsCompleted += 11;

          //{+L4}->M->{L1+26}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2330, RMode = 1 }.Execute(ref machine);

          //If D<<0
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 663, JumpOp = 0 }.Execute(ref machine);
          break;
        case 661_0000:
          machine.PC = new ProgramCounter(663, 0);
          lastOpIndex = 662_0000;
          stepsCompleted += 4;

          //2->M->{L1+26}^^r
          new Const() { Value = 2 }.Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2330, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 663_0000:
          machine.PC = new ProgramCounter(664, 0);
          lastOpIndex = 663_0005;
          stepsCompleted += 6;

          //If {L1+22}^^r>200
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 200 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 666, JumpOp = 0 }.Execute(ref machine);
          break;
        case 664_0000:
          machine.PC = new ProgramCounter(666, 0);
          lastOpIndex = 665_0000;
          stepsCompleted += 3;

          //3->{L1+26}^^r
          new Const() { Value = 3 }.Execute(ref machine);
          new StoreAddress() { Address = 2330, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 666_0000:
          machine.PC = new ProgramCounter(667, 0);
          lastOpIndex = 666_0014;
          stepsCompleted += 15;

          //If {L1+22}^^r-89-1+256/256
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 89 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 669, JumpOp = 0 }.Execute(ref machine);
          break;
        case 667_0000:
          machine.PC = new ProgramCounter(669, 0);
          lastOpIndex = 668_0000;
          stepsCompleted += 19;

          //{L1+22}^^r-93-1+256/256*3->M
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 93 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 669_0000:
          machine.PC = new ProgramCounter(670, 0);
          lastOpIndex = 669_0008;
          stepsCompleted += 9;

          //If {L1+24}^^r*4/256
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 672, JumpOp = 0 }.Execute(ref machine);
          break;
        case 670_0000:
          machine.PC = new ProgramCounter(672, 0);
          lastOpIndex = 671_0000;
          stepsCompleted += 5;

          //+2->M
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 672_0000:
          machine.PC = new ProgramCounter(673, 0);
          lastOpIndex = 672_0004;
          stepsCompleted += 5;

          //!If L/2
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 678, JumpOp = 0 }.Execute(ref machine);
          break;
        case 673_0000:
          machine.PC = new ProgramCounter(675, 0);
          lastOpIndex = 674_0004;
          stepsCompleted += 13;

          //M-1<5->M
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //!If S+9
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 677, JumpOp = 0 }.Execute(ref machine);
          break;
        case 675_0000:
          machine.PC = new ProgramCounter(676, 0);
          lastOpIndex = 675_0000;
          stepsCompleted += 1;

          //Goto RTO
          new Goto() { LabelAddress = 709 }.Execute(ref machine);
          break;
        case 676_0000:
          machine.PC = new ProgramCounter(677, 0);
          lastOpIndex = 676_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 677_0000:
          machine.PC = new ProgramCounter(678, 0);
          lastOpIndex = 677_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 678_0000:
          machine.PC = new ProgramCounter(679, 0);
          lastOpIndex = 678_0001;
          stepsCompleted += 2;

          //!If L
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 681, JumpOp = 0 }.Execute(ref machine);
          break;
        case 679_0000:
          machine.PC = new ProgramCounter(680, 0);
          lastOpIndex = 679_0000;
          stepsCompleted += 1;

          //Goto RTO
          new Goto() { LabelAddress = 709 }.Execute(ref machine);
          break;
        case 680_0000:
          machine.PC = new ProgramCounter(681, 0);
          lastOpIndex = 680_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 681_0000:
          machine.PC = new ProgramCounter(682, 0);
          lastOpIndex = 681_0003;
          stepsCompleted += 4;

          //!If -3
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 685, JumpOp = 0 }.Execute(ref machine);
          break;
        case 682_0000:
          machine.PC = new ProgramCounter(684, 0);
          lastOpIndex = 683_0000;
          stepsCompleted += 8;

          //M-2<6
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);

          //Goto RT1
          new Goto() { LabelAddress = 696 }.Execute(ref machine);
          break;
        case 684_0000:
          machine.PC = new ProgramCounter(685, 0);
          lastOpIndex = 684_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 685_0000:
          machine.PC = new ProgramCounter(686, 0);
          lastOpIndex = 685_0004;
          stepsCompleted += 5;

          //!If Q/2
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 690, JumpOp = 0 }.Execute(ref machine);
          break;
        case 686_0000:
          machine.PC = new ProgramCounter(687, 0);
          lastOpIndex = 686_0021;
          stepsCompleted += 22;

          //!If {L1+22}^^r-N-1>=5+({L1+24}^^r-O>=8
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 689, JumpOp = 0 }.Execute(ref machine);
          break;
        case 687_0000:
          machine.PC = new ProgramCounter(689, 0);
          lastOpIndex = 688_0000;
          stepsCompleted += 2;

          //->M
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 689_0000:
          machine.PC = new ProgramCounter(690, 0);
          lastOpIndex = 689_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 694, JumpOp = 0 }.Execute(ref machine);
          break;
        case 690_0000:
          machine.PC = new ProgramCounter(691, 0);
          lastOpIndex = 690_0021;
          stepsCompleted += 22;

          //!If {L1+22}^^r-N>=8+({L1+24}^^r-O-1>=5
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 693, JumpOp = 0 }.Execute(ref machine);
          break;
        case 691_0000:
          machine.PC = new ProgramCounter(693, 0);
          lastOpIndex = 692_0000;
          stepsCompleted += 2;

          //->M
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 693_0000:
          machine.PC = new ProgramCounter(694, 0);
          lastOpIndex = 693_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 694_0000:
          machine.PC = new ProgramCounter(695, 0);
          lastOpIndex = 694_0004;
          stepsCompleted += 5;

          //!If L-2
          new ReadAddress() { VarAddress = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 700, JumpOp = 0 }.Execute(ref machine);
          break;
        case 695_0000:
          machine.PC = new ProgramCounter(696, 0);
          lastOpIndex = 695_0003;
          stepsCompleted += 4;

          //M=2
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          break;
        case 696_0000:
          machine.PC = new ProgramCounter(699, 0);
          lastOpIndex = 698_0000;
          stepsCompleted += 3;

          //Lbl RT1
          new Label().Execute(ref machine);

          //->M
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 699_0000:
          machine.PC = new ProgramCounter(700, 0);
          lastOpIndex = 699_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 700_0000:
          machine.PC = new ProgramCounter(701, 0);
          lastOpIndex = 700_0004;
          stepsCompleted += 5;

          //!If V/2
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 705, JumpOp = 0 }.Execute(ref machine);
          break;
        case 701_0000:
          machine.PC = new ProgramCounter(702, 0);
          lastOpIndex = 701_0021;
          stepsCompleted += 22;

          //!If {L1+22}^^r-S-1>=5+({L1+24}^^r-T>=8
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 704, JumpOp = 0 }.Execute(ref machine);
          break;
        case 702_0000:
          machine.PC = new ProgramCounter(704, 0);
          lastOpIndex = 703_0000;
          stepsCompleted += 2;

          //->M
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 704_0000:
          machine.PC = new ProgramCounter(705, 0);
          lastOpIndex = 704_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 709, JumpOp = 0 }.Execute(ref machine);
          break;
        case 705_0000:
          machine.PC = new ProgramCounter(706, 0);
          lastOpIndex = 705_0021;
          stepsCompleted += 22;

          //!If {L1+22}^^r-S>=8+({L1+24}^^r-T-1>=5
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 708, JumpOp = 0 }.Execute(ref machine);
          break;
        case 706_0000:
          machine.PC = new ProgramCounter(708, 0);
          lastOpIndex = 707_0000;
          stepsCompleted += 2;

          //->M
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 708_0000:
          machine.PC = new ProgramCounter(709, 0);
          lastOpIndex = 708_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 709_0000:
          machine.PC = new ProgramCounter(712, 0);
          lastOpIndex = 711_0000;
          stepsCompleted += 3;

          //Lbl RTO
          new Label().Execute(ref machine);

          //M
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 712_0000:
          machine.PC = new ProgramCounter(713, 0);
          lastOpIndex = 712_0000;
          stepsCompleted += 1;

          //..Portal
          new Nop().Execute(ref machine);
          break;
        case 713_0000:
          machine.PC = new ProgramCounter(718, 5);
          lastOpIndex = 718_0004;
          stepsCompleted += 126;

          //Lbl PO
          new Label().Execute(ref machine);

          //1-(Q<2->theta)->Z
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //{L1+16}^^r+2+Z>N and (N+5+Z>{L1+16}^^r) and ({L1+18}^^r+2+theta>O) and (O+5+theta>{L1+18}^^r)->R
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new StoreAddress() { Address = 34, RMode = 1 }.Execute(ref machine);

          //1-(V<2->theta)->Z
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //{L1+16}^^r+2+Z>S and (S+5+Z>{L1+16}^^r) and ({L1+18}^^r+2+theta>T) and (T+5+theta>{L1+18}^^r)->W
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new StoreAddress() { Address = 44, RMode = 1 }.Execute(ref machine);

          //Return!If R or W
          new ReadAddress() { VarAddress = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 44 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 719, JumpOp = 0 }.Execute(ref machine);
          break;
        case 718_0005:
          machine.PC = new ProgramCounter(719, 0);
          lastOpIndex = 718_0005;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 719_0000:
          machine.PC = new ProgramCounter(720, 0);
          lastOpIndex = 719_0000;
          stepsCompleted += 1;

          //2
          new Const() { Value = 2 }.Execute(ref machine);
          break;
        case 720_0000:
          machine.PC = new ProgramCounter(721, 0);
          lastOpIndex = 720_0001;
          stepsCompleted += 2;

          //While ->theta
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new While() { JumpLine = 737, JumpOp = 0 }.Execute(ref machine);
          break;
        case 721_0000:
          machine.PC = new ProgramCounter(722, 0);
          lastOpIndex = 721_0001;
          stepsCompleted += 2;

          //If R
          new ReadAddress() { VarAddress = 34 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 734, JumpOp = 0 }.Execute(ref machine);
          break;
        case 722_0000:
          machine.PC = new ProgramCounter(724, 0);
          lastOpIndex = 723_0000;
          stepsCompleted += 3;

          //H->{L1+30}^^r
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new StoreAddress() { Address = 2334, RMode = 1 }.Execute(ref machine);

          //sub(GO)
          new Call() { LabelAddress = 990, ArgCount = 0 }.Execute(ref machine);
          break;
        case 724_0000:
          machine.PC = new ProgramCounter(726, 0);
          lastOpIndex = 725_0000;
          stepsCompleted += 6;

          //1-I->I
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //sub(RO)
          new Call() { LabelAddress = 973, ArgCount = 0 }.Execute(ref machine);
          break;
        case 726_0000:
          machine.PC = new ProgramCounter(728, 0);
          lastOpIndex = 727_0004;
          stepsCompleted += 17;

          //Pt-On({L1+22}^^r,{L1+24}^^r,Pic2+{r1}
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16683 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //!If I-1
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 733, JumpOp = 0 }.Execute(ref machine);
          break;
        case 728_0000:
          machine.PC = new ProgramCounter(732, 0);
          lastOpIndex = 731_0000;
          stepsCompleted += 38;

          //E-(O*256/5)->{L1+28}^^r
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2332, RMode = 1 }.Execute(ref machine);

          //{L1+22}^^r->{L1+16}^^r*256/5+1->D
          new Const() { Value = 2326 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2320, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{L1+24}^^r->{L1+18}^^r*256/5+1->E
          new Const() { Value = 2328 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2322, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(M)
          new Call() { LabelAddress = 950, ArgCount = 0 }.Execute(ref machine);
          break;
        case 732_0000:
          machine.PC = new ProgramCounter(733, 0);
          lastOpIndex = 732_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 733_0000:
          machine.PC = new ProgramCounter(734, 0);
          lastOpIndex = 733_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 734_0000:
          machine.PC = new ProgramCounter(737, 0);
          lastOpIndex = 736_0000;
          stepsCompleted += 12;

          //expr(^^oN,^^oS,10
          new Const() { Value = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Exch() { ArgCount = 3 }.Execute(ref machine);

          //theta-1
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 720, JumpOp = 0 }.Execute(ref machine);
          break;
        case 737_0000:
          machine.PC = new ProgramCounter(738, 0);
          lastOpIndex = 737_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 738_0000:
          machine.PC = new ProgramCounter(739, 0);
          lastOpIndex = 738_0000;
          stepsCompleted += 1;

          //..Portal
          new Nop().Execute(ref machine);
          break;
        case 739_0000:
          machine.PC = new ProgramCounter(741, 0);
          lastOpIndex = 740_0003;
          stepsCompleted += 5;
          getKeysCompleted += 1;

          //Lbl PL
          new Label().Execute(ref machine);

          //If getKey(48)
          new Const() { Value = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 745, JumpOp = 0 }.Execute(ref machine);
          break;
        case 741_0000:
          machine.PC = new ProgramCounter(742, 0);
          lastOpIndex = 741_0005;
          stepsCompleted += 6;

          //!If {PSAVE}-39
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 39 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 744, JumpOp = 0 }.Execute(ref machine);
          break;
        case 742_0000:
          machine.PC = new ProgramCounter(744, 0);
          lastOpIndex = 743_0000;
          stepsCompleted += 6;

          //H-1->H
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 744_0000:
          machine.PC = new ProgramCounter(745, 0);
          lastOpIndex = 744_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 745_0000:
          machine.PC = new ProgramCounter(748, 0);
          lastOpIndex = 747_0004;
          stepsCompleted += 19;

          //0->I->theta->{L1+20}^^r+1->L
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2324, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //E->Y-52->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //!If H//32768
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 750, JumpOp = 0 }.Execute(ref machine);
          break;
        case 748_0000:
          machine.PC = new ProgramCounter(750, 0);
          lastOpIndex = 749_0000;
          stepsCompleted += 6;

          //E+307->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 307 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 750_0000:
          machine.PC = new ProgramCounter(752, 0);
          lastOpIndex = 751_0000;
          stepsCompleted += 7;

          //D->X-52->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //5
          new Const() { Value = 5 }.Execute(ref machine);
          break;
        case 752_0000:
          machine.PC = new ProgramCounter(753, 0);
          lastOpIndex = 752_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 757, JumpOp = 0 }.Execute(ref machine);
          break;
        case 753_0000:
          machine.PC = new ProgramCounter(755, 1);
          lastOpIndex = 755_0000;
          stepsCompleted += 7;

          //->Z
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //D+52->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //C()+I->I
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 755_0001:
          machine.PC = new ProgramCounter(756, 5);
          lastOpIndex = 756_0004;
          stepsCompleted += 9;
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //End!If Z-1
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 757, JumpOp = 0 }.Execute(ref machine);
          break;
        case 756_0005:
          machine.PC = new ProgramCounter(757, 0);
          lastOpIndex = 756_0005;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 752, JumpOp = 0 }.Execute(ref machine);
          break;
        case 757_0000:
          machine.PC = new ProgramCounter(758, 0);
          lastOpIndex = 757_0002;
          stepsCompleted += 3;

          //!If I->{L1+12}^^r
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new StoreAddress() { Address = 2316, RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 770, JumpOp = 0 }.Execute(ref machine);
          break;
        case 758_0000:
          machine.PC = new ProgramCounter(759, 0);
          lastOpIndex = 758_0004;
          stepsCompleted += 5;

          //If F^2
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 763, JumpOp = 0 }.Execute(ref machine);
          break;
        case 759_0000:
          machine.PC = new ProgramCounter(760, 0);
          lastOpIndex = 759_0004;
          stepsCompleted += 5;

          //If H-51
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 762, JumpOp = 0 }.Execute(ref machine);
          break;
        case 760_0000:
          machine.PC = new ProgramCounter(762, 0);
          lastOpIndex = 761_0000;
          stepsCompleted += 6;

          //H+1->H
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 762_0000:
          machine.PC = new ProgramCounter(763, 0);
          lastOpIndex = 762_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 763_0000:
          machine.PC = new ProgramCounter(764, 0);
          lastOpIndex = 763_0007;
          stepsCompleted += 8;

          //!If F^4-1
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 769, JumpOp = 0 }.Execute(ref machine);
          break;
        case 764_0000:
          machine.PC = new ProgramCounter(766, 0);
          lastOpIndex = 765_0001;
          stepsCompleted += 3;

          //Lbl GAD
          new Label().Execute(ref machine);

          //If G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 768, JumpOp = 0 }.Execute(ref machine);
          break;
        case 766_0000:
          machine.PC = new ProgramCounter(768, 0);
          lastOpIndex = 767_0000;
          stepsCompleted += 14;

          //<<0*2-1+G->G
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 768_0000:
          machine.PC = new ProgramCounter(769, 0);
          lastOpIndex = 768_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 769_0000:
          machine.PC = new ProgramCounter(770, 0);
          lastOpIndex = 769_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 783, JumpOp = 0 }.Execute(ref machine);
          break;
        case 770_0000:
          machine.PC = new ProgramCounter(771, 0);
          lastOpIndex = 770_0003;
          stepsCompleted += 4;
          getKeysCompleted += 1;

          //If getKey(4)
          new Const() { Value = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 775, JumpOp = 0 }.Execute(ref machine);
          break;
        case 771_0000:
          machine.PC = new ProgramCounter(772, 0);
          lastOpIndex = 771_0004;
          stepsCompleted += 5;

          //If H<10
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 774, JumpOp = 0 }.Execute(ref machine);
          break;
        case 772_0000:
          machine.PC = new ProgramCounter(774, 0);
          lastOpIndex = 773_0000;
          stepsCompleted += 2;

          //~17
          new Const() { Value = 65519 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 774_0000:
          machine.PC = new ProgramCounter(775, 0);
          lastOpIndex = 774_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 775_0000:
          machine.PC = new ProgramCounter(777, 0);
          lastOpIndex = 776_0007;
          stepsCompleted += 9;

          //->H
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //If Y^256<52
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 779, JumpOp = 0 }.Execute(ref machine);
          break;
        case 777_0000:
          machine.PC = new ProgramCounter(779, 0);
          lastOpIndex = 778_0000;
          stepsCompleted += 3;

          //51->{^^oY}
          new Const() { Value = 51 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 779_0000:
          machine.PC = new ProgramCounter(780, 0);
          lastOpIndex = 779_0001;
          stepsCompleted += 2;

          //If A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 782, JumpOp = 0 }.Execute(ref machine);
          break;
        case 780_0000:
          machine.PC = new ProgramCounter(781, 0);
          lastOpIndex = 780_0000;
          stepsCompleted += 1;

          //Goto GAD
          new Goto() { LabelAddress = 764 }.Execute(ref machine);
          break;
        case 781_0000:
          machine.PC = new ProgramCounter(782, 0);
          lastOpIndex = 781_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 782_0000:
          machine.PC = new ProgramCounter(783, 0);
          lastOpIndex = 782_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 783_0000:
          machine.PC = new ProgramCounter(785, 0);
          lastOpIndex = 784_0000;
          stepsCompleted += 7;

          //Y+H->Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //Y56()
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 785_0000:
          machine.PC = new ProgramCounter(787, 0);
          lastOpIndex = 786_0002;
          stepsCompleted += 5;

          //16->{r1}
          new Const() { Value = 16 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //!If {L1+12}^^r
          new Const() { Value = 2316 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 789, JumpOp = 0 }.Execute(ref machine);
          break;
        case 787_0000:
          machine.PC = new ProgramCounter(789, 0);
          lastOpIndex = 788_0000;
          stepsCompleted += 3;

          //11->{r1}
          new Const() { Value = 11 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 789_0000:
          machine.PC = new ProgramCounter(790, 0);
          lastOpIndex = 789_0003;
          stepsCompleted += 4;
          getKeysCompleted += 1;

          //If getKey(2)
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 794, JumpOp = 0 }.Execute(ref machine);
          break;
        case 790_0000:
          machine.PC = new ProgramCounter(791, 0);
          lastOpIndex = 790_0010;
          stepsCompleted += 11;

          //!If G+{r1}-1//32768
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 793, JumpOp = 0 }.Execute(ref machine);
          break;
        case 791_0000:
          machine.PC = new ProgramCounter(793, 0);
          lastOpIndex = 792_0000;
          stepsCompleted += 6;

          //G-2->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 793_0000:
          machine.PC = new ProgramCounter(794, 0);
          lastOpIndex = 793_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 794_0000:
          machine.PC = new ProgramCounter(795, 0);
          lastOpIndex = 794_0003;
          stepsCompleted += 4;
          getKeysCompleted += 1;

          //If getKey(3)
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 799, JumpOp = 0 }.Execute(ref machine);
          break;
        case 795_0000:
          machine.PC = new ProgramCounter(796, 0);
          lastOpIndex = 795_0007;
          stepsCompleted += 8;

          //If G-{r1}//32768
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 798, JumpOp = 0 }.Execute(ref machine);
          break;
        case 796_0000:
          machine.PC = new ProgramCounter(798, 0);
          lastOpIndex = 797_0000;
          stepsCompleted += 6;

          //G+2->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 798_0000:
          machine.PC = new ProgramCounter(799, 0);
          lastOpIndex = 798_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 799_0000:
          machine.PC = new ProgramCounter(801, 0);
          lastOpIndex = 800_0004;
          stepsCompleted += 10;

          //X-52->D
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //If G>=>=0
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<GreaterEqS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 803, JumpOp = 0 }.Execute(ref machine);
          break;
        case 801_0000:
          machine.PC = new ProgramCounter(803, 0);
          lastOpIndex = 802_0000;
          stepsCompleted += 6;

          //D+307->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 307 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 803_0000:
          machine.PC = new ProgramCounter(804, 1);
          lastOpIndex = 804_0000;
          stepsCompleted += 3;

          //Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //C()->I
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 804_0001:
          machine.PC = new ProgramCounter(806, 1);
          lastOpIndex = 806_0000;
          stepsCompleted += 7;
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //E+204->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 204 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //C()+I->I
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 806_0001:
          machine.PC = new ProgramCounter(808, 0);
          lastOpIndex = 807_0001;
          stepsCompleted += 6;
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //If I
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 810, JumpOp = 0 }.Execute(ref machine);
          break;
        case 808_0000:
          machine.PC = new ProgramCounter(810, 0);
          lastOpIndex = 809_0000;
          stepsCompleted += 3;

          //0->G
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 810_0000:
          machine.PC = new ProgramCounter(813, 0);
          lastOpIndex = 812_0000;
          stepsCompleted += 9;

          //Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //X+G->X->D
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 46, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //X56()
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 813_0000:
          machine.PC = new ProgramCounter(816, 0);
          lastOpIndex = 815_0000;
          stepsCompleted += 11;

          //E+100->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 100 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //D+100->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 100 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //C()
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 816_0000:
          machine.PC = new ProgramCounter(817, 0);
          lastOpIndex = 816_0005;
          stepsCompleted += 6;

          //!If {L1+26}^^r-6
          new Const() { Value = 2330 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 819, JumpOp = 0 }.Execute(ref machine);
          break;
        case 817_0000:
          machine.PC = new ProgramCounter(819, 0);
          lastOpIndex = 818_0000;
          stepsCompleted += 4;

          //~9->S->N
          new Const() { Value = 65527 }.Execute(ref machine);
          new StoreAddress() { Address = 36, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 819_0000:
          machine.PC = new ProgramCounter(822, 0);
          lastOpIndex = 821_0000;
          stepsCompleted += 11;

          //D-100->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 100 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E+103->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 103 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //DTH()
          new Call() { LabelAddress = 828, ArgCount = 0 }.Execute(ref machine);
          break;
        case 822_0000:
          machine.PC = new ProgramCounter(824, 0);
          lastOpIndex = 823_0000;
          stepsCompleted += 6;

          //D+203->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 203 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //DTH()
          new Call() { LabelAddress = 828, ArgCount = 0 }.Execute(ref machine);
          break;
        case 824_0000:
          machine.PC = new ProgramCounter(826, 0);
          lastOpIndex = 825_0000;
          stepsCompleted += 6;

          //E-203->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 203 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //DTH()
          new Call() { LabelAddress = 828, ArgCount = 0 }.Execute(ref machine);
          break;
        case 826_0000:
          machine.PC = new ProgramCounter(828, 0);
          lastOpIndex = 827_0000;
          stepsCompleted += 6;

          //D-203->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 203 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 828_0000:
          machine.PC = new ProgramCounter(830, 0);
          lastOpIndex = 829_0000;
          stepsCompleted += 2;

          //Lbl DTH
          new Label().Execute(ref machine);

          //C()
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 830_0000:
          machine.PC = new ProgramCounter(832, 0);
          lastOpIndex = 831_0000;
          stepsCompleted += 11;

          //{L1+26}^^r>6+{L1+690}^^r->{L1+690}^^r
          new Const() { Value = 2330 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2994 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2994, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 832_0000:
          machine.PC = new ProgramCounter(833, 0);
          lastOpIndex = 832_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 833_0000:
          machine.PC = new ProgramCounter(836, 0);
          lastOpIndex = 835_0002;
          stepsCompleted += 6;

          //Lbl P
          new Label().Execute(ref machine);

          //1->{L1+20}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2324, RMode = 1 }.Execute(ref machine);

          //If {L1+8}^^r
          new Const() { Value = 2312 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 849, JumpOp = 0 }.Execute(ref machine);
          break;
        case 836_0000:
          machine.PC = new ProgramCounter(839, 0);
          lastOpIndex = 838_0001;
          stepsCompleted += 8;

          //{L1}^^r->D
          new Const() { Value = 2304 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{L1+2}^^r->E
          new Const() { Value = 2306 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //{L1+10}^^r
          new Const() { Value = 2314 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          break;
        case 839_0000:
          machine.PC = new ProgramCounter(840, 0);
          lastOpIndex = 839_0007;
          stepsCompleted += 8;

          //Repeat -1->Z//32768
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new Repeat() { JumpLine = 847, JumpOp = 0 }.Execute(ref machine);
          break;
        case 840_0000:
          machine.PC = new ProgramCounter(841, 0);
          lastOpIndex = 840_0008;
          stepsCompleted += 9;

          //If {L1+8}^^r>(rand^1024
          new Const() { Value = 2312 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rand().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1024 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 843, JumpOp = 0 }.Execute(ref machine);
          break;
        case 841_0000:
          machine.PC = new ProgramCounter(843, 0);
          lastOpIndex = 842_0000;
          stepsCompleted += 10;

          //ref(D,E,2,2
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 843_0000:
          machine.PC = new ProgramCounter(847, 0);
          lastOpIndex = 846_0000;
          stepsCompleted += 17;

          //{L1+4}^^r+D->D
          new Const() { Value = 2308 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{L1+6}^^r+E->E
          new Const() { Value = 2310 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //Z-1
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 839, JumpOp = 0 }.Execute(ref machine);
          break;
        case 847_0000:
          machine.PC = new ProgramCounter(849, 0);
          lastOpIndex = 848_0000;
          stepsCompleted += 7;

          //{L1+8}^^r-256->{L1+8}^^r
          new Const() { Value = 2312 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2312, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 849_0000:
          machine.PC = new ProgramCounter(849, 2);
          lastOpIndex = 849_0001;
          stepsCompleted += 2;

          //ReturnIf R
          new ReadAddress() { VarAddress = 34 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 850, JumpOp = 0 }.Execute(ref machine);
          break;
        case 849_0002:
          machine.PC = new ProgramCounter(850, 0);
          lastOpIndex = 849_0002;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 850_0000:
          machine.PC = new ProgramCounter(850, 2);
          lastOpIndex = 850_0001;
          stepsCompleted += 2;

          //ReturnIf W
          new ReadAddress() { VarAddress = 44 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 851, JumpOp = 0 }.Execute(ref machine);
          break;
        case 850_0002:
          machine.PC = new ProgramCounter(851, 0);
          lastOpIndex = 850_0002;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 851_0000:
          machine.PC = new ProgramCounter(852, 0);
          lastOpIndex = 851_0063;
          stepsCompleted += 64;
          getKeysCompleted += 12;

          //!If getKey(20) or getKey(19) or getKey(18)-(getKey(36) or getKey(35) or getKey(34))->G*2+(getKey(34) or getKey(26) or getKey(18)-(getKey(36) or getKey(28) or getKey(20))->H)
          new Const() { Value = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 19 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 35 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new GetKey() { RMode = 0, ArgCount = 1 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 855, JumpOp = 0 }.Execute(ref machine);
          break;
        case 852_0000:
          machine.PC = new ProgramCounter(854, 0);
          lastOpIndex = 853_0000;
          stepsCompleted += 2;

          //->{L1+38}^^r
          new StoreAddress() { Address = 2342, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 854_0000:
          machine.PC = new ProgramCounter(855, 0);
          lastOpIndex = 854_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 855_0000:
          machine.PC = new ProgramCounter(855, 3);
          lastOpIndex = 855_0002;
          stepsCompleted += 3;

          //ReturnIf {L1+38}^^r
          new Const() { Value = 2342 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 856, JumpOp = 0 }.Execute(ref machine);
          break;
        case 855_0003:
          machine.PC = new ProgramCounter(856, 0);
          lastOpIndex = 855_0003;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 856_0000:
          machine.PC = new ProgramCounter(864, 0);
          lastOpIndex = 863_0004;
          stepsCompleted += 36;

          //+1->{L1+38}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2342, RMode = 1 }.Execute(ref machine);

          //{L1+16}^^r+2->D->{L1}^^r
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2304, RMode = 1 }.Execute(ref machine);

          //{L1+18}^^r+2->E->{L1+2}^^r
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2306, RMode = 1 }.Execute(ref machine);

          //3->L
          new Const() { Value = 3 }.Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //1024->{L1+8}^^r
          new Const() { Value = 1024 }.Execute(ref machine);
          new StoreAddress() { Address = 2312, RMode = 1 }.Execute(ref machine);

          //0->{L1+10}^^r->I->J
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2314, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);

          //G*2->{L1+4}^^r
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 2308, RMode = 1 }.Execute(ref machine);

          //H*2->{L1+6}^^r
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 2310, RMode = 1 }.Execute(ref machine);
          break;
        case 864_0000:
          machine.PC = new ProgramCounter(865, 0);
          lastOpIndex = 864_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 870, JumpOp = 0 }.Execute(ref machine);
          break;
        case 865_0000:
          machine.PC = new ProgramCounter(868, 0);
          lastOpIndex = 867_0000;
          stepsCompleted += 19;

          //{L1+4}^^r*2+D->D
          new Const() { Value = 2308 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //{L1+6}^^r*2+E->E
          new Const() { Value = 2310 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 868_0000:
          machine.PC = new ProgramCounter(869, 14);
          lastOpIndex = 869_0013;
          stepsCompleted += 20;

          //{L1+10}^^r+4->{L1+10}^^r
          new Const() { Value = 2314 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2314, RMode = 1 }.Execute(ref machine);

          //EndIf M or (D<<0 or (E<<0
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 870, JumpOp = 0 }.Execute(ref machine);
          break;
        case 869_0014:
          machine.PC = new ProgramCounter(870, 0);
          lastOpIndex = 869_0014;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 864, JumpOp = 0 }.Execute(ref machine);
          break;
        case 870_0000:
          machine.PC = new ProgramCounter(871, 0);
          lastOpIndex = 870_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 876, JumpOp = 0 }.Execute(ref machine);
          break;
        case 871_0000:
          machine.PC = new ProgramCounter(874, 0);
          lastOpIndex = 873_0000;
          stepsCompleted += 11;

          //D-G->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E-H->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 874_0000:
          machine.PC = new ProgramCounter(874, 7);
          lastOpIndex = 874_0006;
          stepsCompleted += 7;

          //Return!If {L1+10}^^r-1->{L1+10}^^r
          new Const() { Value = 2314 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2314, RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 875, JumpOp = 0 }.Execute(ref machine);
          break;
        case 874_0007:
          machine.PC = new ProgramCounter(875, 0);
          lastOpIndex = 874_0007;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 875_0000:
          machine.PC = new ProgramCounter(875, 2);
          lastOpIndex = 875_0001;
          stepsCompleted += 2;

          //End!If M
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 876, JumpOp = 0 }.Execute(ref machine);
          break;
        case 875_0002:
          machine.PC = new ProgramCounter(876, 0);
          lastOpIndex = 875_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 870, JumpOp = 0 }.Execute(ref machine);
          break;
        case 876_0000:
          machine.PC = new ProgramCounter(877, 1);
          lastOpIndex = 877_0000;
          stepsCompleted += 6;

          //D+1->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 877_0001:
          machine.PC = new ProgramCounter(878, 0);
          lastOpIndex = 877_0001;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 880, JumpOp = 0 }.Execute(ref machine);
          break;
        case 878_0000:
          machine.PC = new ProgramCounter(880, 0);
          lastOpIndex = 879_0000;
          stepsCompleted += 6;

          //->I-1->C
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 880_0000:
          machine.PC = new ProgramCounter(881, 1);
          lastOpIndex = 881_0000;
          stepsCompleted += 6;

          //D-2->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 881_0001:
          machine.PC = new ProgramCounter(882, 0);
          lastOpIndex = 881_0001;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 884, JumpOp = 0 }.Execute(ref machine);
          break;
        case 882_0000:
          machine.PC = new ProgramCounter(884, 0);
          lastOpIndex = 883_0000;
          stepsCompleted += 6;

          //->C-2->I
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 884_0000:
          machine.PC = new ProgramCounter(886, 1);
          lastOpIndex = 886_0000;
          stepsCompleted += 11;

          //D+1->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E+1->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 886_0001:
          machine.PC = new ProgramCounter(887, 0);
          lastOpIndex = 886_0001;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 889, JumpOp = 0 }.Execute(ref machine);
          break;
        case 887_0000:
          machine.PC = new ProgramCounter(889, 0);
          lastOpIndex = 888_0000;
          stepsCompleted += 6;

          //->J+1->C
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 889_0000:
          machine.PC = new ProgramCounter(890, 1);
          lastOpIndex = 890_0000;
          stepsCompleted += 6;

          //E-2->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 890_0001:
          machine.PC = new ProgramCounter(891, 0);
          lastOpIndex = 890_0001;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 894, JumpOp = 0 }.Execute(ref machine);
          break;
        case 891_0000:
          machine.PC = new ProgramCounter(894, 0);
          lastOpIndex = 893_0000;
          stepsCompleted += 7;

          //-2->J
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);

          //3->C
          new Const() { Value = 3 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 894_0000:
          machine.PC = new ProgramCounter(895, 8);
          lastOpIndex = 895_0007;
          stepsCompleted += 13;

          //E+1->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //Return!If I*2+J
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 896, JumpOp = 0 }.Execute(ref machine);
          break;
        case 895_0008:
          machine.PC = new ProgramCounter(896, 0);
          lastOpIndex = 895_0008;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 896_0000:
          machine.PC = new ProgramCounter(899, 0);
          lastOpIndex = 898_0006;
          stepsCompleted += 21;

          //C!=2-1+D->D->G
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //E->H
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //sub(BMP,~3,+2)
          new Const() { Value = 65533 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 928, ArgCount = 2 }.Execute(ref machine);
          break;
        case 899_0000:
          machine.PC = new ProgramCounter(900, 0);
          lastOpIndex = 899_0004;
          stepsCompleted += 5;

          //sub(BMP,4,1)
          new Const() { Value = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 928, ArgCount = 2 }.Execute(ref machine);
          break;
        case 900_0000:
          machine.PC = new ProgramCounter(901, 0);
          lastOpIndex = 900_0006;
          stepsCompleted += 7;

          //sub(BMP,~3,+2)
          new Const() { Value = 65533 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 928, ArgCount = 2 }.Execute(ref machine);
          break;
        case 901_0000:
          machine.PC = new ProgramCounter(901, 5);
          lastOpIndex = 901_0004;
          stepsCompleted += 5;

          //ReturnIf theta-1
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 902, JumpOp = 0 }.Execute(ref machine);
          break;
        case 901_0005:
          machine.PC = new ProgramCounter(902, 0);
          lastOpIndex = 901_0005;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 902_0000:
          machine.PC = new ProgramCounter(904, 1);
          lastOpIndex = 904_0000;
          stepsCompleted += 11;

          //G+I->D
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //H+J->E
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //Return!If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 904_0001:
          machine.PC = new ProgramCounter(904, 2);
          lastOpIndex = 904_0001;
          stepsCompleted += 1;
          new If() { Negated = true, JumpLine = 905, JumpOp = 0 }.Execute(ref machine);
          break;
        case 904_0002:
          machine.PC = new ProgramCounter(905, 0);
          lastOpIndex = 904_0002;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 905_0000:
          machine.PC = new ProgramCounter(906, 0);
          lastOpIndex = 905_0000;
          stepsCompleted += 1;

          //sub(EXX)
          new Call() { LabelAddress = 925, ArgCount = 0 }.Execute(ref machine);
          break;
        case 906_0000:
          machine.PC = new ProgramCounter(909, 1);
          lastOpIndex = 909_0000;
          stepsCompleted += 13;

          //G-2->N
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //H-4->O
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 28, RMode = 1 }.Execute(ref machine);

          //C->Q
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new StoreAddress() { Address = 32, RMode = 1 }.Execute(ref machine);

          //Return!If 
          new If() { Negated = true, JumpLine = 910, JumpOp = 0 }.Execute(ref machine);
          break;
        case 909_0001:
          machine.PC = new ProgramCounter(910, 0);
          lastOpIndex = 909_0001;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 910_0000:
          machine.PC = new ProgramCounter(911, 0);
          lastOpIndex = 910_0003;
          stepsCompleted += 4;

          //!If -1
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 915, JumpOp = 0 }.Execute(ref machine);
          break;
        case 911_0000:
          machine.PC = new ProgramCounter(914, 0);
          lastOpIndex = 913_0000;
          stepsCompleted += 11;

          //N-3->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //O+1->O
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 28, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 914_0000:
          machine.PC = new ProgramCounter(915, 0);
          lastOpIndex = 914_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 915_0000:
          machine.PC = new ProgramCounter(916, 0);
          lastOpIndex = 915_0003;
          stepsCompleted += 4;

          //!If -1
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 920, JumpOp = 0 }.Execute(ref machine);
          break;
        case 916_0000:
          machine.PC = new ProgramCounter(919, 0);
          lastOpIndex = 918_0000;
          stepsCompleted += 11;

          //N-1->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //O+2->O
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 28, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 919_0000:
          machine.PC = new ProgramCounter(920, 0);
          lastOpIndex = 919_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 920_0000:
          machine.PC = new ProgramCounter(921, 0);
          lastOpIndex = 920_0003;
          stepsCompleted += 4;

          //!If -1
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 925, JumpOp = 0 }.Execute(ref machine);
          break;
        case 921_0000:
          machine.PC = new ProgramCounter(924, 0);
          lastOpIndex = 923_0000;
          stepsCompleted += 11;

          //N-2->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //O-1->O
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 28, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 924_0000:
          machine.PC = new ProgramCounter(925, 0);
          lastOpIndex = 924_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 925_0000:
          machine.PC = new ProgramCounter(928, 0);
          lastOpIndex = 927_0000;
          stepsCompleted += 9;

          //Lbl EXX
          new Label().Execute(ref machine);

          //expr(^^oN,^^oS,10
          new Const() { Value = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Exch() { ArgCount = 3 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 928_0000:
          machine.PC = new ProgramCounter(930, 0);
          lastOpIndex = 929_0001;
          stepsCompleted += 3;

          //Lbl BMP
          new Label().Execute(ref machine);

          //0->theta
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          break;
        case 930_0000:
          machine.PC = new ProgramCounter(931, 0);
          lastOpIndex = 930_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 947, JumpOp = 0 }.Execute(ref machine);
          break;
        case 931_0000:
          machine.PC = new ProgramCounter(935, 0);
          lastOpIndex = 934_0000;
          stepsCompleted += 23;

          //0->Z->L
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //J*{r1}+G->D
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //I*{r1}*~1+H->E
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65535 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 935_0000:
          machine.PC = new ProgramCounter(940, 0);
          lastOpIndex = 939_0000;
          stepsCompleted += 30;

          //M->Z
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //2->L
          new Const() { Value = 2 }.Execute(ref machine);
          new StoreAddress() { Address = 22, RMode = 1 }.Execute(ref machine);

          //J*{r2}+D+I->D
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //I*{r2}*~1+E+J->E
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65535 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 940_0000:
          machine.PC = new ProgramCounter(941, 0);
          lastOpIndex = 940_0008;
          stepsCompleted += 9;

          //If M=0 or Z->Z
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 945, JumpOp = 0 }.Execute(ref machine);
          break;
        case 941_0000:
          machine.PC = new ProgramCounter(945, 0);
          lastOpIndex = 944_0000;
          stepsCompleted += 21;

          //.If sub(C) or (2->LJ*{r2}+D+I->DI*{r2}*~1+E+J->Esub(C)=0)->Z
          new Nop().Execute(ref machine);

          //J*{r2}*~1+G->G
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65535 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //I*{r2}+H->H
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 945_0000:
          machine.PC = new ProgramCounter(945, 9);
          lastOpIndex = 945_0008;
          stepsCompleted += 9;

          //Return!If theta+1->theta-9
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 946, JumpOp = 0 }.Execute(ref machine);
          break;
        case 945_0009:
          machine.PC = new ProgramCounter(946, 0);
          lastOpIndex = 945_0009;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 946_0000:
          machine.PC = new ProgramCounter(946, 2);
          lastOpIndex = 946_0001;
          stepsCompleted += 2;

          //End!If Z
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 947, JumpOp = 0 }.Execute(ref machine);
          break;
        case 946_0002:
          machine.PC = new ProgramCounter(947, 0);
          lastOpIndex = 946_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 930, JumpOp = 0 }.Execute(ref machine);
          break;
        case 947_0000:
          machine.PC = new ProgramCounter(948, 0);
          lastOpIndex = 947_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 948_0000:
          machine.PC = new ProgramCounter(950, 0);
          lastOpIndex = 949_0000;
          stepsCompleted += 2;

          // 
          new Nop().Execute(ref machine);

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 950_0000:
          machine.PC = new ProgramCounter(953, 0);
          lastOpIndex = 952_0010;
          stepsCompleted += 20;

          //Lbl M
          new Label().Execute(ref machine);

          //Q*10+V->C
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);

          //If C=11 or (C=0
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 11 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 955, JumpOp = 0 }.Execute(ref machine);
          break;
        case 953_0000:
          machine.PC = new ProgramCounter(955, 0);
          lastOpIndex = 954_0000;
          stepsCompleted += 4;

          //~G->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Negate().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 955_0000:
          machine.PC = new ProgramCounter(956, 0);
          lastOpIndex = 955_0010;
          stepsCompleted += 11;

          //If C=22 or (C=33
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 22 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 33 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 962, JumpOp = 0 }.Execute(ref machine);
          break;
        case 956_0000:
          machine.PC = new ProgramCounter(959, 0);
          lastOpIndex = 958_0004;
          stepsCompleted += 33;

          //~1*{L1+30}^^r->H
          new Const() { Value = 65535 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2334 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //T*256/5+{L1+28}^^r+H->E*256/5->{L1+18}^^r
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2332 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 2322, RMode = 1 }.Execute(ref machine);

          //If H>~13
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65523 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 961, JumpOp = 0 }.Execute(ref machine);
          break;
        case 959_0000:
          machine.PC = new ProgramCounter(961, 0);
          lastOpIndex = 960_0000;
          stepsCompleted += 3;

          //~13->H
          new Const() { Value = 65523 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 961_0000:
          machine.PC = new ProgramCounter(962, 0);
          lastOpIndex = 961_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 962_0000:
          machine.PC = new ProgramCounter(963, 0);
          lastOpIndex = 962_0022;
          stepsCompleted += 23;

          //If C=12 or (C=3 or (C=20 or (C=31
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 20 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 31 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 967, JumpOp = 0 }.Execute(ref machine);
          break;
        case 963_0000:
          machine.PC = new ProgramCounter(967, 0);
          lastOpIndex = 966_0000;
          stepsCompleted += 8;

          //~H->Z
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Negate().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //G->H
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //Z->G
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 967_0000:
          machine.PC = new ProgramCounter(968, 0);
          lastOpIndex = 967_0022;
          stepsCompleted += 23;

          //If C=2 or (C=13 or (C=21 or (C=30
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 13 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 21 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 972, JumpOp = 0 }.Execute(ref machine);
          break;
        case 968_0000:
          machine.PC = new ProgramCounter(972, 0);
          lastOpIndex = 971_0000;
          stepsCompleted += 8;

          //~G->Z
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Negate().Execute(ref machine);
          new StoreAddress() { Address = 50, RMode = 1 }.Execute(ref machine);

          //H->G
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //Z->H
          new ReadAddress() { VarAddress = 50 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 972_0000:
          machine.PC = new ProgramCounter(973, 0);
          lastOpIndex = 972_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 973_0000:
          machine.PC = new ProgramCounter(975, 0);
          lastOpIndex = 974_0004;
          stepsCompleted += 6;

          //Lbl RO
          new Label().Execute(ref machine);

          //If V<2
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 982, JumpOp = 0 }.Execute(ref machine);
          break;
        case 975_0000:
          machine.PC = new ProgramCounter(977, 0);
          lastOpIndex = 976_0001;
          stepsCompleted += 10;

          //T+J-2->{L1+24}^^r
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2328, RMode = 1 }.Execute(ref machine);

          //!If V
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 979, JumpOp = 0 }.Execute(ref machine);
          break;
        case 977_0000:
          machine.PC = new ProgramCounter(979, 0);
          lastOpIndex = 978_0000;
          stepsCompleted += 9;

          //S+1-I->{L1+22}^^r
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2326, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 981, JumpOp = 0 }.Execute(ref machine);
          break;
        case 979_0000:
          machine.PC = new ProgramCounter(981, 0);
          lastOpIndex = 980_0000;
          stepsCompleted += 9;

          //S+2+I->{L1+22}^^r
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2326, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 981_0000:
          machine.PC = new ProgramCounter(982, 0);
          lastOpIndex = 981_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 989, JumpOp = 0 }.Execute(ref machine);
          break;
        case 982_0000:
          machine.PC = new ProgramCounter(984, 0);
          lastOpIndex = 983_0004;
          stepsCompleted += 13;

          //S+5-J->{L1+22}^^r
          new ReadAddress() { VarAddress = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2326, RMode = 1 }.Execute(ref machine);

          //!If V-2
          new ReadAddress() { VarAddress = 42 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 986, JumpOp = 0 }.Execute(ref machine);
          break;
        case 984_0000:
          machine.PC = new ProgramCounter(986, 0);
          lastOpIndex = 985_0000;
          stepsCompleted += 9;

          //T+1-I->{L1+24}^^r
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 2328, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 988, JumpOp = 0 }.Execute(ref machine);
          break;
        case 986_0000:
          machine.PC = new ProgramCounter(988, 0);
          lastOpIndex = 987_0000;
          stepsCompleted += 9;

          //T+2+I->{L1+24}^^r
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 2328, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 988_0000:
          machine.PC = new ProgramCounter(989, 0);
          lastOpIndex = 988_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 989_0000:
          machine.PC = new ProgramCounter(990, 0);
          lastOpIndex = 989_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 990_0000:
          machine.PC = new ProgramCounter(992, 0);
          lastOpIndex = 991_0004;
          stepsCompleted += 6;

          //Lbl GO
          new Label().Execute(ref machine);

          //If Q<2
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 999, JumpOp = 0 }.Execute(ref machine);
          break;
        case 992_0000:
          machine.PC = new ProgramCounter(994, 0);
          lastOpIndex = 993_0001;
          stepsCompleted += 11;

          //{L1+18}^^r+2-O->J
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);

          //!If Q
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 996, JumpOp = 0 }.Execute(ref machine);
          break;
        case 994_0000:
          machine.PC = new ProgramCounter(996, 0);
          lastOpIndex = 995_0000;
          stepsCompleted += 10;

          //N+1-{L1+16}^^r->I
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 998, JumpOp = 0 }.Execute(ref machine);
          break;
        case 996_0000:
          machine.PC = new ProgramCounter(998, 0);
          lastOpIndex = 997_0000;
          stepsCompleted += 10;

          //{L1+16}^^r-N-2->I
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 998_0000:
          machine.PC = new ProgramCounter(999, 0);
          lastOpIndex = 998_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 1006, JumpOp = 0 }.Execute(ref machine);
          break;
        case 999_0000:
          machine.PC = new ProgramCounter(1001, 0);
          lastOpIndex = 1000_0004;
          stepsCompleted += 14;

          //N+5-{L1+16}^^r->J
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);

          //!If Q-2
          new ReadAddress() { VarAddress = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1003, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1001_0000:
          machine.PC = new ProgramCounter(1003, 0);
          lastOpIndex = 1002_0000;
          stepsCompleted += 10;

          //O+1-{L1+18}^^r->I
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 1005, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1003_0000:
          machine.PC = new ProgramCounter(1005, 0);
          lastOpIndex = 1004_0000;
          stepsCompleted += 10;

          //{L1+18}^^r-O-2->I
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 28 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1005_0000:
          machine.PC = new ProgramCounter(1006, 0);
          lastOpIndex = 1005_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1006_0000:
          machine.PC = new ProgramCounter(1007, 0);
          lastOpIndex = 1006_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1007_0000:
          machine.PC = new ProgramCounter(1008, 0);
          lastOpIndex = 1007_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1008_0000:
          machine.PC = new ProgramCounter(1010, 0);
          lastOpIndex = 1009_0008;
          stepsCompleted += 10;

          //Lbl O3
          new Label().Execute(ref machine);

          //If F^1024->{r3}<32
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1024 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1020, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1010_0000:
          machine.PC = new ProgramCounter(1011, 0);
          lastOpIndex = 1010_0004;
          stepsCompleted += 5;

          //If {r3}<8
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1013, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1011_0000:
          machine.PC = new ProgramCounter(1011, 2);
          lastOpIndex = 1011_0001;
          stepsCompleted += 2;

          //Pt-On(Dsub(X56),Esub(Y56),{r3}/2*8+40+Pic2
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1011_0002:
          machine.PC = new ProgramCounter(1011, 5);
          lastOpIndex = 1011_0004;
          stepsCompleted += 3;
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1011_0005:
          machine.PC = new ProgramCounter(1013, 0);
          lastOpIndex = 1012_0000;
          stepsCompleted += 17;
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 40 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16683 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1013_0000:
          machine.PC = new ProgramCounter(1014, 0);
          lastOpIndex = 1013_0004;
          stepsCompleted += 5;

          //If {r3}=29
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 29 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1018, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1014_0000:
          machine.PC = new ProgramCounter(1015, 0);
          lastOpIndex = 1014_0010;
          stepsCompleted += 11;

          //sub(I8,{{r6}+10},^^oD,256
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1015_0000:
          machine.PC = new ProgramCounter(1018, 0);
          lastOpIndex = 1017_0000;
          stepsCompleted += 33;

          //int({r6}+12}->G*6+D+25->D
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemorySignedByte().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 25 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //int({r6}+13}->H*6+E+25->E
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 13 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemorySignedByte().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 25 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1018_0000:
          machine.PC = new ProgramCounter(1019, 0);
          lastOpIndex = 1018_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1019_0000:
          machine.PC = new ProgramCounter(1020, 0);
          lastOpIndex = 1019_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1020_0000:
          machine.PC = new ProgramCounter(1021, 0);
          lastOpIndex = 1020_0001;
          stepsCompleted += 2;

          //!If D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 1028, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1021_0000:
          machine.PC = new ProgramCounter(1024, 0);
          lastOpIndex = 1023_0004;
          stepsCompleted += 26;

          //E+1->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //Pt-On(G,H,E/4*8+Pic2O
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16819 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //If E=11
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 11 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1026, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1024_0000:
          machine.PC = new ProgramCounter(1026, 0);
          lastOpIndex = 1025_0000;
          stepsCompleted += 5;

          //0->{{r6}}
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1026_0000:
          machine.PC = new ProgramCounter(1027, 0);
          lastOpIndex = 1026_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1027_0000:
          machine.PC = new ProgramCounter(1028, 0);
          lastOpIndex = 1027_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1028_0000:
          machine.PC = new ProgramCounter(1029, 0);
          lastOpIndex = 1028_0005;
          stepsCompleted += 6;

          //D+G->Dsub(X56)
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1029_0000:
          machine.PC = new ProgramCounter(1030, 0);
          lastOpIndex = 1029_0005;
          stepsCompleted += 6;

          //E+H->Esub(Y56)
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1030_0000:
          machine.PC = new ProgramCounter(1033, 0);
          lastOpIndex = 1032_0028;
          stepsCompleted += 39;

          //D+103->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 103 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E+103->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 103 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //If D>X and (X+256>D and (E>Y and (Y+256>E
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1035, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1033_0000:
          machine.PC = new ProgramCounter(1035, 0);
          lastOpIndex = 1034_0000;
          stepsCompleted += 3;

          //1->{L1+690}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2994, RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1035_0000:
          machine.PC = new ProgramCounter(1036, 0);
          lastOpIndex = 1035_0000;
          stepsCompleted += 1;

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1036_0000:
          machine.PC = new ProgramCounter(1039, 0);
          lastOpIndex = 1038_0001;
          stepsCompleted += 12;

          //D-103->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 103 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E-103->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 103 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //If M
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1048, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1039_0000:
          machine.PC = new ProgramCounter(1040, 0);
          lastOpIndex = 1039_0005;
          stepsCompleted += 6;

          //!If {L1+26}^^r-4
          new Const() { Value = 2330 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1045, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1040_0000:
          machine.PC = new ProgramCounter(1042, 0);
          lastOpIndex = 1041_0010;
          stepsCompleted += 23;

          //->D->E->{{{r6}+14}+L4}
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //sub(I8,{{r6}+14},^^oG,5
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1042_0000:
          machine.PC = new ProgramCounter(1044, 0);
          lastOpIndex = 1043_0000;
          stepsCompleted += 8;

          //Pt-Change(G,H,Pic2O)^^r
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16819 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 1, ArgCount = 3 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1044_0000:
          machine.PC = new ProgramCounter(1045, 0);
          lastOpIndex = 1044_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1045_0000:
          machine.PC = new ProgramCounter(1046, 0);
          lastOpIndex = 1045_0007;
          stepsCompleted += 8;

          //~G->G+D->Dsub(X56)
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Negate().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1046_0000:
          machine.PC = new ProgramCounter(1047, 0);
          lastOpIndex = 1046_0007;
          stepsCompleted += 8;

          //~H->H+E->Esub(Y56)
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Negate().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1047_0000:
          machine.PC = new ProgramCounter(1048, 0);
          lastOpIndex = 1047_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1048_0000:
          machine.PC = new ProgramCounter(1049, 0);
          lastOpIndex = 1048_0002;
          stepsCompleted += 3;

          //sub(PO,40)
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 713, ArgCount = 1 }.Execute(ref machine);
          break;
        case 1049_0000:
          machine.PC = new ProgramCounter(1051, 0);
          lastOpIndex = 1050_0000;
          stepsCompleted += 10;

          //Pt-On({L1+16}^^r,{L1+18}^^r,Pic2+40
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16723 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1051_0000:
          machine.PC = new ProgramCounter(1052, 0);
          lastOpIndex = 1051_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1052_0000:
          machine.PC = new ProgramCounter(1056, 0);
          lastOpIndex = 1055_0000;
          stepsCompleted += 13;

          //Lbl 0O0
          new Label().Execute(ref machine);

          //DeltaList(51,204)->GDB0O0
          new Nop().Execute(ref machine);

          //{//32768+GDB0O0+1}
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16554 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1056_0000:
          machine.PC = new ProgramCounter(1058, 0);
          lastOpIndex = 1057_0001;
          stepsCompleted += 3;

          //Lbl O1
          new Label().Execute(ref machine);

          //Dsub(X56)
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1058_0000:
          machine.PC = new ProgramCounter(1059, 0);
          lastOpIndex = 1058_0001;
          stepsCompleted += 2;

          //Esub(Y56)
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1059_0000:
          machine.PC = new ProgramCounter(1060, 5);
          lastOpIndex = 1060_0004;
          stepsCompleted += 11;

          //D->theta+102->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 102 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E->C+(Hsub(0O0))->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Call() { LabelAddress = 1052, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1060_0005:
          machine.PC = new ProgramCounter(1061, 1);
          lastOpIndex = 1061_0000;
          stepsCompleted += 3;
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C) or (D+51->Dsub(C))->M
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1061_0001:
          machine.PC = new ProgramCounter(1061, 8);
          lastOpIndex = 1061_0007;
          stepsCompleted += 7;
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 51 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1061_0008:
          machine.PC = new ProgramCounter(1065, 0);
          lastOpIndex = 1064_0005;
          stepsCompleted += 12;
          new Binary_U8<Or_U8>().Execute(ref machine);
          new StoreAddress() { Address = 24, RMode = 1 }.Execute(ref machine);

          //theta->D
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //C->E
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //!If {L1+26}^^r-6
          new Const() { Value = 2330 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1069, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1065_0000:
          machine.PC = new ProgramCounter(1066, 0);
          lastOpIndex = 1065_0005;
          stepsCompleted += 6;

          //!If {{r6}+14}^^r
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 1068, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1066_0000:
          machine.PC = new ProgramCounter(1068, 0);
          lastOpIndex = 1067_0000;
          stepsCompleted += 10;

          //+1->{{r6}+14}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1068_0000:
          machine.PC = new ProgramCounter(1069, 0);
          lastOpIndex = 1068_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1069_0000:
          machine.PC = new ProgramCounter(1070, 0);
          lastOpIndex = 1069_0005;
          stepsCompleted += 6;

          //If {{r6}+14}^^r
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1081, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1070_0000:
          machine.PC = new ProgramCounter(1072, 0);
          lastOpIndex = 1071_0004;
          stepsCompleted += 20;

          //{{r6}+14}^^r+1->{r1}->{{r6}+14}^^r
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //!If {r1}-64
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1080, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1072_0000:
          machine.PC = new ProgramCounter(1075, 0);
          lastOpIndex = 1074_0010;
          stepsCompleted += 20;

          //->{{r6}+14}^^r
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //0->G->H
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //I8({{r6}+12}^^r,^^oD,256)
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 253, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1075_0000:
          machine.PC = new ProgramCounter(1076, 0);
          lastOpIndex = 1075_0008;
          stepsCompleted += 9;

          //!If {{r6}+10}^^r-2
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1079, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1076_0000:
          machine.PC = new ProgramCounter(1079, 0);
          lastOpIndex = 1078_0000;
          stepsCompleted += 10;

          //0->{{r6}+10}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 1 }.Execute(ref machine);

          //0->{L1+34}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2338, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1079_0000:
          machine.PC = new ProgramCounter(1080, 0);
          lastOpIndex = 1079_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1080_0000:
          machine.PC = new ProgramCounter(1081, 0);
          lastOpIndex = 1080_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1081_0000:
          machine.PC = new ProgramCounter(1082, 0);
          lastOpIndex = 1081_0008;
          stepsCompleted += 9;

          //!If {{r6}+10}-1
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1084, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1082_0000:
          machine.PC = new ProgramCounter(1084, 0);
          lastOpIndex = 1083_0000;
          stepsCompleted += 8;

          //M->{{r6}+10}
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1084_0000:
          machine.PC = new ProgramCounter(1085, 0);
          lastOpIndex = 1084_0005;
          stepsCompleted += 6;

          //!If {{r6}+10}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 1107, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1085_0000:
          machine.PC = new ProgramCounter(1086, 0);
          lastOpIndex = 1085_0001;
          stepsCompleted += 2;

          //If {r1}
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1090, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1086_0000:
          machine.PC = new ProgramCounter(1087, 0);
          lastOpIndex = 1086_0004;
          stepsCompleted += 5;

          //If H-50
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 50 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1089, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1087_0000:
          machine.PC = new ProgramCounter(1089, 0);
          lastOpIndex = 1088_0000;
          stepsCompleted += 6;

          //H+1->H
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1089_0000:
          machine.PC = new ProgramCounter(1090, 0);
          lastOpIndex = 1089_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1090_0000:
          machine.PC = new ProgramCounter(1091, 0);
          lastOpIndex = 1090_0001;
          stepsCompleted += 2;

          //If M
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1098, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1091_0000:
          machine.PC = new ProgramCounter(1092, 0);
          lastOpIndex = 1091_0001;
          stepsCompleted += 2;

          //!If G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new If() { Negated = true, JumpLine = 1094, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1092_0000:
          machine.PC = new ProgramCounter(1094, 0);
          lastOpIndex = 1093_0000;
          stepsCompleted += 11;

          //H>>0->{{r6}+10}
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<GreaterS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //Else
          new Else() { IsElseIf = false, JumpLine = 1096, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1094_0000:
          machine.PC = new ProgramCounter(1096, 0);
          lastOpIndex = 1095_0000;
          stepsCompleted += 17;

          //<<0*2-1*2+G->G
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1096_0000:
          machine.PC = new ProgramCounter(1098, 0);
          lastOpIndex = 1097_0000;
          stepsCompleted += 3;

          //0->H
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1098_0000:
          machine.PC = new ProgramCounter(1099, 5);
          lastOpIndex = 1099_0004;
          stepsCompleted += 14;

          //E+H->theta+153->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 153 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //D->C+(Gsub(0O0))->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new StoreAddress() { Address = 4, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Call() { LabelAddress = 1052, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1099_0005:
          machine.PC = new ProgramCounter(1100, 1);
          lastOpIndex = 1100_0000;
          stepsCompleted += 3;
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //If sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1100_0001:
          machine.PC = new ProgramCounter(1101, 0);
          lastOpIndex = 1100_0001;
          stepsCompleted += 1;
          new If() { Negated = false, JumpLine = 1103, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1101_0000:
          machine.PC = new ProgramCounter(1103, 0);
          lastOpIndex = 1102_0000;
          stepsCompleted += 5;

          //-1->G
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1103_0000:
          machine.PC = new ProgramCounter(1104, 0);
          lastOpIndex = 1103_0005;
          stepsCompleted += 6;

          //C+G->Dsub(X56)
          new ReadAddress() { VarAddress = 4 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1500, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1104_0000:
          machine.PC = new ProgramCounter(1105, 0);
          lastOpIndex = 1104_0002;
          stepsCompleted += 3;

          //theta->Esub(Y56)
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);
          new Call() { LabelAddress = 1503, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1105_0000:
          machine.PC = new ProgramCounter(1106, 0);
          lastOpIndex = 1105_0002;
          stepsCompleted += 3;

          //sub(PO,32)
          new Const() { Value = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Call() { LabelAddress = 713, ArgCount = 1 }.Execute(ref machine);
          break;
        case 1106_0000:
          machine.PC = new ProgramCounter(1107, 0);
          lastOpIndex = 1106_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1107_0000:
          machine.PC = new ProgramCounter(1108, 0);
          lastOpIndex = 1107_0008;
          stepsCompleted += 9;

          //!If {{r6}+10}-2
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1112, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1108_0000:
          machine.PC = new ProgramCounter(1112, 0);
          lastOpIndex = 1111_0000;
          stepsCompleted += 14;

          //DeltaList(~153^^r,153^^r)->GDB0D0
          new Nop().Execute(ref machine);

          //{{L1+14}^^r+GDB0D0}^^r+X->D
          new Const() { Value = 2318 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16556 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1112_0000:
          machine.PC = new ProgramCounter(1113, 0);
          lastOpIndex = 1112_0002;
          stepsCompleted += 3;

          //If {L1+36}^^r
          new Const() { Value = 2340 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1139, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1113_0000:
          machine.PC = new ProgramCounter(1114, 0);
          lastOpIndex = 1113_0015;
          stepsCompleted += 16;

          //If {{r6}+10} xor {L1+34}^^r-2//32768
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2338 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U8<Xor_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32768 }.Execute(ref machine);
          new Binary_S16<DivS>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1138, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1114_0000:
          machine.PC = new ProgramCounter(1115, 0);
          lastOpIndex = 1114_0007;
          stepsCompleted += 8;

          //!If D+256<X
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1137, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1115_0000:
          machine.PC = new ProgramCounter(1116, 0);
          lastOpIndex = 1115_0007;
          stepsCompleted += 8;

          //!If X+256<D
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1136, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1116_0000:
          machine.PC = new ProgramCounter(1117, 0);
          lastOpIndex = 1116_0007;
          stepsCompleted += 8;

          //!If E+128<Y
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 128 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1135, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1117_0000:
          machine.PC = new ProgramCounter(1118, 0);
          lastOpIndex = 1117_0007;
          stepsCompleted += 8;

          //!If Y+256<E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1134, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1118_0000:
          machine.PC = new ProgramCounter(1124, 1);
          lastOpIndex = 1124_0000;
          stepsCompleted += 40;

          //->{L1+36}^^r
          new StoreAddress() { Address = 2340, RMode = 1 }.Execute(ref machine);

          //{{r6}+10}-2<<0*2->{L1+34}^^r->{{r6}+10}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_S16<LessS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new StoreAddress() { Address = 2338, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //A->G
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //B-5->H
          new ReadAddress() { VarAddress = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 14, RMode = 1 }.Execute(ref machine);

          //D+102->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 102 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //E+204->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 204 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //sub(C)->J
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1124_0001:
          machine.PC = new ProgramCounter(1127, 0);
          lastOpIndex = 1126_0000;
          stepsCompleted += 7;
          new StoreAddress() { Address = 18, RMode = 1 }.Execute(ref machine);

          //D+52->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //sub(C)
          new Call() { LabelAddress = 651, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1127_0000:
          machine.PC = new ProgramCounter(1130, 0);
          lastOpIndex = 1129_0004;
          stepsCompleted += 15;

          //E-204->E
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 204 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //D-154->D
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 154 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //If M or J
          new ReadAddress() { VarAddress = 24 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 18 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1133, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1130_0000:
          machine.PC = new ProgramCounter(1133, 0);
          lastOpIndex = 1132_0000;
          stepsCompleted += 5;

          //X->D
          new ReadAddress() { VarAddress = 46 }.Execute(ref machine);
          new StoreAddress() { Address = 6, RMode = 1 }.Execute(ref machine);

          //Y->E
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new StoreAddress() { Address = 8, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1133_0000:
          machine.PC = new ProgramCounter(1134, 0);
          lastOpIndex = 1133_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1134_0000:
          machine.PC = new ProgramCounter(1135, 0);
          lastOpIndex = 1134_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1135_0000:
          machine.PC = new ProgramCounter(1136, 0);
          lastOpIndex = 1135_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1136_0000:
          machine.PC = new ProgramCounter(1137, 0);
          lastOpIndex = 1136_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1137_0000:
          machine.PC = new ProgramCounter(1138, 0);
          lastOpIndex = 1137_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1138_0000:
          machine.PC = new ProgramCounter(1139, 0);
          lastOpIndex = 1138_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1139_0000:
          machine.PC = new ProgramCounter(1140, 0);
          lastOpIndex = 1139_0017;
          stepsCompleted += 18;

          //!If {{r6}+14}^^r/2/2/2^2
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1142, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1140_0000:
          machine.PC = new ProgramCounter(1142, 0);
          lastOpIndex = 1141_0000;
          stepsCompleted += 18;

          //ref({L1+16}^^r+2,{L1+18}^^r+2,2,2
          new Const() { Value = 2320 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2322 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1142_0000:
          machine.PC = new ProgramCounter(1143, 0);
          lastOpIndex = 1142_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1143_0000:
          machine.PC = new ProgramCounter(1144, 0);
          lastOpIndex = 1143_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1144_0000:
          machine.PC = new ProgramCounter(1147, 0);
          lastOpIndex = 1146_0045;
          stepsCompleted += 53;

          //Lbl O2
          new Label().Execute(ref machine);

          //{{r6}+14}^^r->{r1}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //If pxl-Test(D,E) or pxl-Test(D+1,E) or pxl-Test(D+2,E) or pxl-Test(D+3,E) or pxl-Test(D+4,E)
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlTest() { RMode = 0, ArgCount = 2 }.Execute(ref machine);
          new Binary_U8<Or_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1153, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1147_0000:
          machine.PC = new ProgramCounter(1150, 0);
          lastOpIndex = 1149_0004;
          stepsCompleted += 20;

          //0->{L4+H}
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //G!=16+G->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //!If G-2
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1152, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1150_0000:
          machine.PC = new ProgramCounter(1152, 0);
          lastOpIndex = 1151_0000;
          stepsCompleted += 16;

          //Pt-Change({{r6}+10},{{r6}+12},{r1})^^r
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterInvert>() { RMode = 1, ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1152_0000:
          machine.PC = new ProgramCounter(1153, 0);
          lastOpIndex = 1152_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 1159, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1153_0000:
          machine.PC = new ProgramCounter(1156, 0);
          lastOpIndex = 1155_0004;
          stepsCompleted += 20;

          //G-(G!=0)->G
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 12, RMode = 1 }.Execute(ref machine);

          //3->{L4+H}
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4864 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 14 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //!If G-1
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1158, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1156_0000:
          machine.PC = new ProgramCounter(1158, 0);
          lastOpIndex = 1157_0000;
          stepsCompleted += 16;

          //Pt-On({{r6}+10},{{r6}+12},{r1})^^r
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 1, ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1158_0000:
          machine.PC = new ProgramCounter(1159, 0);
          lastOpIndex = 1158_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1159_0000:
          machine.PC = new ProgramCounter(1160, 0);
          lastOpIndex = 1159_0010;
          stepsCompleted += 11;

          //If G<12 and (G!=0
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1162, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1160_0000:
          machine.PC = new ProgramCounter(1162, 0);
          lastOpIndex = 1161_0000;
          stepsCompleted += 25;

          //Pt-On({{r6}+10},{{r6}+12},G/4*8+{r1}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 12 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PtSprite<PlotterOr>() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1162_0000:
          machine.PC = new ProgramCounter(1164, 0);
          lastOpIndex = 1163_0000;
          stepsCompleted += 16;

          //ref(D,G!=0+E,5,1
          new ReadAddress() { VarAddress = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 12 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 8 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1164_0000:
          machine.PC = new ProgramCounter(1220, 0);
          lastOpIndex = 1219_0000;
          stepsCompleted += 56;

          //..AXE
          new Nop().Execute(ref machine);

          //[2030207088000000]->Pic2P
          new Nop().Execute(ref machine);

          //[]->Pic1
          new Nop().Execute(ref machine);

          //.0
          new Nop().Execute(ref machine);

          //[0000000000]
          new Nop().Execute(ref machine);

          //.5
          new Nop().Execute(ref machine);

          //[F8F8F8F8F8]
          new Nop().Execute(ref machine);

          //.10
          new Nop().Execute(ref machine);

          //[F8F8F8F8F8]
          new Nop().Execute(ref machine);

          //.15
          new Nop().Execute(ref machine);

          //[5000500050]
          new Nop().Execute(ref machine);

          //.20
          new Nop().Execute(ref machine);

          //[00A800A800]
          new Nop().Execute(ref machine);

          //.25
          new Nop().Execute(ref machine);

          //[A850A850A8]
          new Nop().Execute(ref machine);

          //.30
          new Nop().Execute(ref machine);

          //[50A850A850]
          new Nop().Execute(ref machine);

          //.35
          new Nop().Execute(ref machine);

          //[00A8A8F8F8]
          new Nop().Execute(ref machine);

          //.40
          new Nop().Execute(ref machine);

          //[005050F8F8]
          new Nop().Execute(ref machine);

          //.45
          new Nop().Execute(ref machine);

          //[F0E0F0E0F0]
          new Nop().Execute(ref machine);

          //.50
          new Nop().Execute(ref machine);

          //[F8F8F8A800]
          new Nop().Execute(ref machine);

          //.55
          new Nop().Execute(ref machine);

          //[7838783878]
          new Nop().Execute(ref machine);

          //.60
          new Nop().Execute(ref machine);

          //[00A8F8F8F8]
          new Nop().Execute(ref machine);

          //.65
          new Nop().Execute(ref machine);

          //[F880C080F8]
          new Nop().Execute(ref machine);

          //.70
          new Nop().Execute(ref machine);

          //[F8A8888888]
          new Nop().Execute(ref machine);

          //.75
          new Nop().Execute(ref machine);

          //[F8081808F8]
          new Nop().Execute(ref machine);

          //.80
          new Nop().Execute(ref machine);

          //[888888A8F8]
          new Nop().Execute(ref machine);

          //.85
          new Nop().Execute(ref machine);

          //[5070F8F8F8]
          new Nop().Execute(ref machine);

          //.90
          new Nop().Execute(ref machine);

          //[F8005000F8]
          new Nop().Execute(ref machine);

          //.95
          new Nop().Execute(ref machine);

          //[88A888A888]
          new Nop().Execute(ref machine);

          //.100
          new Nop().Execute(ref machine);

          //[8888F88888]
          new Nop().Execute(ref machine);

          //.105
          new Nop().Execute(ref machine);

          //[A8F8700000]
          new Nop().Execute(ref machine);

          //.110
          new Nop().Execute(ref machine);

          //[F888F888F8]
          new Nop().Execute(ref machine);

          //..AXE
          new Nop().Execute(ref machine);

          //[2020207088000000202020F0100000002020202020000000202020784000000000006060000000000020502000000000007050700000000020508850200000008800000088000000]->Pic2
          new Nop().Execute(ref machine);

          //[0808101008081010081010080810100810100808101008081008081010080810000000CC33000000000000669900000000000033CC0000000000009966000000]->Pic1P
          new Nop().Execute(ref machine);

          //[8888F8888800000088F800F888000000F8000000F8000000F8202020F8000000D8505050D80000008888888888000000]->Pic2O
          new Nop().Execute(ref machine);

          //[0000303000000000]->Pic1O
          new Nop().Execute(ref machine);

          //[A850A850A800000050A850A850000000]->Pic4O
          new Nop().Execute(ref machine);

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1220_0000:
          machine.PC = new ProgramCounter(1225, 0);
          lastOpIndex = 1224_0002;
          stepsCompleted += 16;

          //Lbl LOAD
          new Label().Execute(ref machine);

          //0->N->{L1}
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2304, RMode = 0 }.Execute(ref machine);

          //conj("",L3,1)
          new Const() { Value = 16891 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4096 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //L1->theta
          new Const() { Value = 2304 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //{|E9830}^^r->A
          new Const() { Value = 38960 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);
          break;
        case 1225_0000:
          machine.PC = new ProgramCounter(1226, 0);
          lastOpIndex = 1225_0005;
          stepsCompleted += 6;

          //Repeat A<{|E982E}^^r
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 38958 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new Repeat() { JumpLine = 1244, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1226_0000:
          machine.PC = new ProgramCounter(1232, 0);
          lastOpIndex = 1231_0011;
          stepsCompleted += 35;

          //Fill(theta,9,0)
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fill() { ArgCount = 3 }.Execute(ref machine);

          //{A}->T
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 38, RMode = 1 }.Execute(ref machine);

          //A-6->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //{A}->I
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //A-1->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //If T=21 and ({A}>64
          new ReadAddress() { VarAddress = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 21 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1242, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1232_0000:
          machine.PC = new ProgramCounter(1232, 2);
          lastOpIndex = 1232_0001;
          stepsCompleted += 2;

          //For(F,0,I-1
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 1232_0002:
          machine.PC = new ProgramCounter(1233, 0);
          lastOpIndex = 1232_0006;
          stepsCompleted += 5;
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 1235, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1233_0000:
          machine.PC = new ProgramCounter(1235, 0);
          lastOpIndex = 1234_0000;
          stepsCompleted += 12;

          //{A-F}->{theta+F}
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new EndFor() { VarAddress = 10, JumpLine = 1232, JumpOp = 2 }.Execute(ref machine);
          break;
        case 1235_0000:
          machine.PC = new ProgramCounter(1238, 0);
          lastOpIndex = 1237_0005;
          stepsCompleted += 15;

          //conj(theta,L3+1,9
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4097 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //GetCalc(L3,`Y1)
          new Const() { Value = 4096 }.Execute(ref machine);
          new GetCalcFromFileSystem() { VarAddress = 1794 }.Execute(ref machine);

          //If {`Y1}^^r=64222
          new FileHandle() { VarAddress = 1794 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1794, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64222 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1241, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1238_0000:
          machine.PC = new ProgramCounter(1241, 0);
          lastOpIndex = 1240_0000;
          stepsCompleted += 11;

          //theta+9->theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //N+1->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1241_0000:
          machine.PC = new ProgramCounter(1242, 0);
          lastOpIndex = 1241_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1242_0000:
          machine.PC = new ProgramCounter(1244, 0);
          lastOpIndex = 1243_0000;
          stepsCompleted += 6;

          //A-I->A
          new ReadAddress() { VarAddress = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 16 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 0, RMode = 1 }.Execute(ref machine);

          //End
          new EndLoop() { JumpLine = 1225, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1244_0000:
          machine.PC = new ProgramCounter(1246, 0);
          lastOpIndex = 1245_0004;
          stepsCompleted += 7;

          //theta->I
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new StoreAddress() { Address = 16, RMode = 1 }.Execute(ref machine);

          //!If theta-L1
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1248, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1246_0000:
          machine.PC = new ProgramCounter(1247, 0);
          lastOpIndex = 1246_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 1247_0000:
          machine.PC = new ProgramCounter(1248, 0);
          lastOpIndex = 1247_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1248_0000:
          machine.PC = new ProgramCounter(1250, 0);
          lastOpIndex = 1249_0001;
          stepsCompleted += 4;

          //0->theta
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //32->Y
          new Const() { Value = 32 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);
          break;
        case 1250_0000:
          machine.PC = new ProgramCounter(1251, 0);
          lastOpIndex = 1250_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 1281, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1251_0000:
          machine.PC = new ProgramCounter(1256, 2);
          lastOpIndex = 1256_0001;
          stepsCompleted += 41;

          //ref(21,29,36,31
          new Const() { Value = 21 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 29 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 31 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(21,29,36,31
          new Const() { Value = 21 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 29 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 36 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 31 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Pxl-On(23,Y
          new Const() { Value = 23 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterOr>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-On(22,Y+1
          new Const() { Value = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterOr>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-On(22,Y-1
          new Const() { Value = 22 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterOr>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //For(F,0,4
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 1256_0002:
          machine.PC = new ProgramCounter(1257, 0);
          lastOpIndex = 1256_0003;
          stepsCompleted += 2;
          new Const() { Value = 4 }.Execute(ref machine);
          new For() { VarAddress = 10, JumpLine = 1261, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1257_0000:
          machine.PC = new ProgramCounter(1258, 0);
          lastOpIndex = 1257_0007;
          stepsCompleted += 8;

          //If F+theta<N
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1260, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1258_0000:
          machine.PC = new ProgramCounter(1259, 0);
          lastOpIndex = 1258_0021;
          stepsCompleted += 22;

          //Text(25,F*6+29,F+theta*9+L1
          new Const() { Value = 25 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 29 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1259_0000:
          machine.PC = new ProgramCounter(1260, 0);
          lastOpIndex = 1259_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1260_0000:
          machine.PC = new ProgramCounter(1261, 0);
          lastOpIndex = 1260_0000;
          stepsCompleted += 1;

          //End
          new EndFor() { VarAddress = 10, JumpLine = 1256, JumpOp = 2 }.Execute(ref machine);
          break;
        case 1261_0000:
          machine.PC = new ProgramCounter(1262, 0);
          lastOpIndex = 1261_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 1262_0000:
          machine.PC = new ProgramCounter(1263, 0);
          lastOpIndex = 1262_0002;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //Repeat getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new Repeat() { JumpLine = 1264, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1263_0000:
          machine.PC = new ProgramCounter(1264, 0);
          lastOpIndex = 1263_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 1262, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1264_0000:
          machine.PC = new ProgramCounter(1266, 0);
          lastOpIndex = 1265_0004;
          stepsCompleted += 40;

          //(K=1)*(Y-32/6+theta+1<N)-(K=4)*6+Y->Y
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //If Y<32
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1271, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1266_0000:
          machine.PC = new ProgramCounter(1268, 0);
          lastOpIndex = 1267_0001;
          stepsCompleted += 4;

          //32->Y
          new Const() { Value = 32 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //If theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1270, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1268_0000:
          machine.PC = new ProgramCounter(1270, 0);
          lastOpIndex = 1269_0000;
          stepsCompleted += 6;

          //theta-1->theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1270_0000:
          machine.PC = new ProgramCounter(1271, 0);
          lastOpIndex = 1270_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1271_0000:
          machine.PC = new ProgramCounter(1272, 0);
          lastOpIndex = 1271_0004;
          stepsCompleted += 5;

          //If Y>56
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 56 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1277, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1272_0000:
          machine.PC = new ProgramCounter(1274, 0);
          lastOpIndex = 1273_0007;
          stepsCompleted += 10;

          //56->Y
          new Const() { Value = 56 }.Execute(ref machine);
          new StoreAddress() { Address = 48, RMode = 1 }.Execute(ref machine);

          //If theta+5<N
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new Binary_U16<Less>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1276, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1274_0000:
          machine.PC = new ProgramCounter(1276, 0);
          lastOpIndex = 1275_0000;
          stepsCompleted += 6;

          //theta+1->theta
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 52, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1276_0000:
          machine.PC = new ProgramCounter(1277, 0);
          lastOpIndex = 1276_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1277_0000:
          machine.PC = new ProgramCounter(1278, 0);
          lastOpIndex = 1277_0004;
          stepsCompleted += 5;

          //If K=15
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1280, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1278_0000:
          machine.PC = new ProgramCounter(1279, 0);
          lastOpIndex = 1278_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 1279_0000:
          machine.PC = new ProgramCounter(1280, 0);
          lastOpIndex = 1279_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1280_0000:
          machine.PC = new ProgramCounter(1280, 5);
          lastOpIndex = 1280_0004;
          stepsCompleted += 5;

          //EndIf K=54
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 54 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1281, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1280_0005:
          machine.PC = new ProgramCounter(1281, 0);
          lastOpIndex = 1280_0005;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 1250, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1281_0000:
          machine.PC = new ProgramCounter(1289, 0);
          lastOpIndex = 1288_0000;
          stepsCompleted += 38;

          //conj(Y-32/6+theta*9+L1,L3+1,8)
          new ReadAddress() { VarAddress = 48 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 52 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 9 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2304 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4097 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 8 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //GetCalc(L3,`Y0)
          new Const() { Value = 4096 }.Execute(ref machine);
          new GetCalcFromFileSystem() { VarAddress = 1848 }.Execute(ref machine);

          //Y0+2->Y0
          new ReadAddress() { VarAddress = 1848 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1848, RMode = 1 }.Execute(ref machine);

          //0->{L1+688}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 2992, RMode = 1 }.Execute(ref machine);

          //1->{L1+704}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 3008, RMode = 1 }.Execute(ref machine);

          //0->{L1+700}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 3004, RMode = 1 }.Execute(ref machine);

          //1->{L1+692}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2996, RMode = 1 }.Execute(ref machine);

          //Goto TOP
          new Goto() { LabelAddress = 97 }.Execute(ref machine);
          break;
        case 1289_0000:
          machine.PC = new ProgramCounter(1290, 0);
          lastOpIndex = 1289_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1290_0000:
          machine.PC = new ProgramCounter(1294, 0);
          lastOpIndex = 1293_0006;
          stepsCompleted += 26;

          //Lbl SELECT
          new Label().Execute(ref machine);

          //ref(40,30,15,15
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(40,30,15,15
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Text(40,34,"<
          new Const() { Value = 40 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16893 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1294_0000:
          machine.PC = new ProgramCounter(1295, 0);
          lastOpIndex = 1294_0001;
          stepsCompleted += 2;

          //1->N
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);
          break;
        case 1295_0000:
          machine.PC = new ProgramCounter(1296, 0);
          lastOpIndex = 1295_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 1315, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1296_0000:
          machine.PC = new ProgramCounter(1297, 0);
          lastOpIndex = 1296_0010;
          stepsCompleted += 11;

          //Text(44,34,N/10>Dec
          new Const() { Value = 44 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new ToStringNumber().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1297_0000:
          machine.PC = new ProgramCounter(1298, 0);
          lastOpIndex = 1297_0010;
          stepsCompleted += 11;

          //Text(49,34,N^10>Dec
          new Const() { Value = 49 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 10 }.Execute(ref machine);
          new Binary_U16<Mod>().Execute(ref machine);
          new ToStringNumber().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1298_0000:
          machine.PC = new ProgramCounter(1299, 0);
          lastOpIndex = 1298_0006;
          stepsCompleted += 7;

          //Text(53,34,">
          new Const() { Value = 53 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 16895 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = false, ArgCount = 3 }.Execute(ref machine);
          break;
        case 1299_0000:
          machine.PC = new ProgramCounter(1300, 0);
          lastOpIndex = 1299_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 1300_0000:
          machine.PC = new ProgramCounter(1301, 0);
          lastOpIndex = 1300_0002;
          stepsCompleted += 3;
          getKeysCompleted += 1;

          //Repeat getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new Repeat() { JumpLine = 1302, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1301_0000:
          machine.PC = new ProgramCounter(1302, 0);
          lastOpIndex = 1301_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 1300, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1302_0000:
          machine.PC = new ProgramCounter(1303, 0);
          lastOpIndex = 1302_0004;
          stepsCompleted += 5;

          //If K=38
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 38 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1305, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1303_0000:
          machine.PC = new ProgramCounter(1305, 0);
          lastOpIndex = 1304_0000;
          stepsCompleted += 6;

          //38->N->{PSAVE}
          new Const() { Value = 38 }.Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1305_0000:
          machine.PC = new ProgramCounter(1306, 0);
          lastOpIndex = 1305_0015;
          stepsCompleted += 16;

          //If K=3 and (N!=min(38,{PSAVE})
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 38 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1818 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Min() { ArgCount = 2 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1308, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1306_0000:
          machine.PC = new ProgramCounter(1308, 0);
          lastOpIndex = 1307_0000;
          stepsCompleted += 6;

          //N+1->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1308_0000:
          machine.PC = new ProgramCounter(1309, 0);
          lastOpIndex = 1308_0010;
          stepsCompleted += 11;

          //If K=2 and (N!=1
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<NEq>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1311, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1309_0000:
          machine.PC = new ProgramCounter(1311, 0);
          lastOpIndex = 1310_0000;
          stepsCompleted += 6;

          //N-1->N
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 26, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1311_0000:
          machine.PC = new ProgramCounter(1312, 0);
          lastOpIndex = 1311_0004;
          stepsCompleted += 5;

          //If K=15
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1314, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1312_0000:
          machine.PC = new ProgramCounter(1313, 0);
          lastOpIndex = 1312_0000;
          stepsCompleted += 1;

          //Goto MEN
          new Goto() { LabelAddress = 67 }.Execute(ref machine);
          break;
        case 1313_0000:
          machine.PC = new ProgramCounter(1314, 0);
          lastOpIndex = 1313_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1314_0000:
          machine.PC = new ProgramCounter(1314, 5);
          lastOpIndex = 1314_0004;
          stepsCompleted += 5;

          //EndIf K=54
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 54 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1315, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1314_0005:
          machine.PC = new ProgramCounter(1315, 0);
          lastOpIndex = 1314_0005;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 1295, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1315_0000:
          machine.PC = new ProgramCounter(1322, 0);
          lastOpIndex = 1321_0000;
          stepsCompleted += 17;

          //1->{L1+688}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2992, RMode = 1 }.Execute(ref machine);

          //N->{L1+704}^^r
          new ReadAddress() { VarAddress = 26 }.Execute(ref machine);
          new StoreAddress() { Address = 3008, RMode = 1 }.Execute(ref machine);

          //0->{L1+700}^^r
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 3004, RMode = 1 }.Execute(ref machine);

          //Y2+2->Y0
          new ReadAddress() { VarAddress = 1800 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1848, RMode = 1 }.Execute(ref machine);

          //{^^o`Y2+2}->{^^o`Y0+2}
          new Const() { Value = 1802 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 1850, RMode = 0 }.Execute(ref machine);

          //1->{L1+692}^^r
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 2996, RMode = 1 }.Execute(ref machine);

          //Goto TOP
          new Goto() { LabelAddress = 97 }.Execute(ref machine);
          break;
        case 1322_0000:
          machine.PC = new ProgramCounter(1388, 0);
          lastOpIndex = 1387_0000;
          stepsCompleted += 66;

          //..AXE
          new Nop().Execute(ref machine);

          //[000000000000000000000000]->Pic2M
          new Nop().Execute(ref machine);

          //[0000003E0000000000000000]
          new Nop().Execute(ref machine);

          //[0000008E8000000000000000]
          new Nop().Execute(ref machine);

          //[000001C64000000000000000]
          new Nop().Execute(ref machine);

          //[000F83E261F81FE010030000]
          new Nop().Execute(ref machine);

          //[000FE41271FC1FE038030000]
          new Nop().Execute(ref machine);

          //[000C6000618E030038030000]
          new Nop().Execute(ref machine);

          //[000C67C0498603006C030000]
          new Nop().Execute(ref machine);

          //[000C6700898E03006C030000]
          new Nop().Execute(ref machine);

          //[000FE60019FC03007C030000]
          new Nop().Execute(ref machine);

          //[000F844039F00300FE030000]
          new Nop().Execute(ref machine);

          //[000C0480F9B80300FE030000]
          new Nop().Execute(ref machine);

          //[000C0180019C0301C7030000]
          new Nop().Execute(ref machine);

          //[000C0392098E03018303F000]
          new Nop().Execute(ref machine);

          //[000C0191F18703038383F000]
          new Nop().Execute(ref machine);

          //[00000098E000000000000000]
          new Nop().Execute(ref machine);

          //[001FFE5C43FFFFFFFFFFF800]
          new Nop().Execute(ref machine);

          //[0000001F0000000000000000]
          new Nop().Execute(ref machine);

          //[000000000000000000000000]
          new Nop().Execute(ref machine);

          //[00000003FFFFFFFFE0000000]
          new Nop().Execute(ref machine);

          //[0000000739C6F59C70000000]
          new Nop().Execute(ref machine);

          //[000000075ADEF5ADF0000000]
          new Nop().Execute(ref machine);

          //[0000000739CEF5ACF0000000]
          new Nop().Execute(ref machine);

          //[000000077ADEF5ADF0000000]
          new Nop().Execute(ref machine);

          //[000000077AC6319C70000000]
          new Nop().Execute(ref machine);

          //[00000007FFFFFFFFF0000000]
          new Nop().Execute(ref machine);

          //[0000000FFFFFFFFFF8000000]
          new Nop().Execute(ref machine);

          //[3FFFFFFFFFFFFFFFFFFFFFFC]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[70001C00070000E00038000E]
          new Nop().Execute(ref machine);

          //[608A08000200004000100006]
          new Nop().Execute(ref machine);

          //[608A08040202004000500016]
          new Nop().Execute(ref machine);

          //[608A0804020400400090C026]
          new Nop().Execute(ref machine);

          //[60FE0815023FFE460092CE26]
          new Nop().Execute(ref machine);

          //[60000811026C1A4660903026]
          new Nop().Execute(ref machine);

          //[60540811025CE2418093CF26]
          new Nop().Execute(ref machine);

          //[6054080402470E41C0900026]
          new Nop().Execute(ref machine);

          //[6010080A0242324264900026]
          new Nop().Execute(ref machine);

          //[600008040242C44258900026]
          new Nop().Execute(ref machine);

          //[606C08000243184040500016]
          new Nop().Execute(ref machine);

          //[604408000242604020100006]
          new Nop().Execute(ref machine);

          //[601008000243804007D001F6]
          new Nop().Execute(ref machine);

          //[604408404232004004100106]
          new Nop().Execute(ref machine);

          //[606C0864C20C004FE413F906]
          new Nop().Execute(ref machine);

          //[600008358200005014140506]
          new Nop().Execute(ref machine);

          //[70001C00070000E00038000E]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[77113FFFFF1715FFFFF8A88E]
          new Nop().Execute(ref machine);

          //[77555FFFFF5755FFFFFBADDE]
          new Nop().Execute(ref machine);

          //[77515FFFFF1711FFFFF9DDDE]
          new Nop().Execute(ref machine);

          //[77555FFFFF775BFFFFFBADDE]
          new Nop().Execute(ref machine);

          //[71153FFFFF715BFFFFF8A8DE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[7FFFFFFFFFFFFFFFFFFFFFFE]
          new Nop().Execute(ref machine);

          //[3FFFFFFFFFFFFFFFFFFFFFFC]
          new Nop().Execute(ref machine);

          //[000000000000000000000000]
          new Nop().Execute(ref machine);

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1388_0000:
          machine.PC = new ProgramCounter(1391, 0);
          lastOpIndex = 1390_0000;
          stepsCompleted += 11;

          //Lbl CLRIN
          new Label().Execute(ref machine);

          //ref(7,0,89,64
          new Const() { Value = 7 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 89 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1391_0000:
          machine.PC = new ProgramCounter(1394, 0);
          lastOpIndex = 1393_0000;
          stepsCompleted += 11;

          //Lbl CLRCR
          new Label().Execute(ref machine);

          //ref(2,0,92,64
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 92 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 64 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1394_0000:
          machine.PC = new ProgramCounter(1398, 0);
          lastOpIndex = 1397_0000;
          stepsCompleted += 20;

          //Lbl CLRTOP
          new Label().Execute(ref machine);

          //ref(0,0,96,30
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 96 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 30 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterOr>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //rref(0,0,96,29
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 96 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 29 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Rect<PlotterInvert>() { RMode = 0, ArgCount = 4 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1398_0000:
          machine.PC = new ProgramCounter(1407, 0);
          lastOpIndex = 1406_0001;
          stepsCompleted += 42;

          //Lbl LDSTR
          new Label().Execute(ref machine);

          //^^o`Y6->^^oY6
          new Nop().Execute(ref machine);

          //^^o`Y7->^^oY7
          new Nop().Execute(ref machine);

          //conj(^^o`Y2,^^o`Y6,3
          new Const() { Value = 1800 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1824 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //conj(^^o`Y2,^^o`Y7,3
          new Const() { Value = 1800 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1830 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Copy() { RMode = 0, ArgCount = 3 }.Execute(ref machine);

          //({`Y6}^^r)+Y6->Y6
          new FileHandle() { VarAddress = 1824 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1824, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1824 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1824, RMode = 1 }.Execute(ref machine);

          //{r1}*2+Y6->Y7
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1824 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1830, RMode = 1 }.Execute(ref machine);

          //({`Y7}^^r)+Y6+2->Y6
          new FileHandle() { VarAddress = 1830 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1830, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 1824 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 1824, RMode = 1 }.Execute(ref machine);

          //L2->{r1}
          new Const() { Value = 3072 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          break;
        case 1407_0000:
          machine.PC = new ProgramCounter(1408, 0);
          lastOpIndex = 1407_0000;
          stepsCompleted += 1;

          //While 1
          new WhileTrue() { JumpLine = 1412, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1408_0000:
          machine.PC = new ProgramCounter(1411, 2);
          lastOpIndex = 1411_0001;
          stepsCompleted += 14;

          //{`Y6}->{r2}->{{r1}}
          new FileHandle() { VarAddress = 1824 }.Execute(ref machine);
          new ReadFile() { VarAddress = 1824, RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 770, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new StoreMemory() { RMode = 0 }.Execute(ref machine);

          //Y6++
          new ReadAddress() { VarAddress = 1824 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 1824, RMode = 1 }.Execute(ref machine);

          //{r1}++
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //End!If {r2}
          new ReadAddress() { VarAddress = 770 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1412, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1411_0002:
          machine.PC = new ProgramCounter(1412, 0);
          lastOpIndex = 1411_0002;
          stepsCompleted += 1;
          new EndLoop() { JumpLine = 1407, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1412_0000:
          machine.PC = new ProgramCounter(1414, 0);
          lastOpIndex = 1413_0000;
          stepsCompleted += 3;

          //L2->{r3}
          new Const() { Value = 3072 }.Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1414_0000:
          machine.PC = new ProgramCounter(1417, 0);
          lastOpIndex = 1416_0000;
          stepsCompleted += 3;

          //.X START {r1}
          new Nop().Execute(ref machine);

          //.Str1 IN {r3}
          new Nop().Execute(ref machine);

          //.CLR FUNC {r4}
          new Nop().Execute(ref machine);
          break;
        case 1417_0000:
          machine.PC = new ProgramCounter(1420, 0);
          lastOpIndex = 1419_0005;
          stepsCompleted += 13;

          //Lbl TEXT
          new Label().Execute(ref machine);

          //{{r3}}=' '->{r6}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 32 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new StoreAddress() { Address = 778, RMode = 1 }.Execute(ref machine);

          //!If {{r3}}-'*'
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 42 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1424, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1420_0000:
          machine.PC = new ProgramCounter(1423, 0);
          lastOpIndex = 1422_0000;
          stepsCompleted += 6;

          //{r3}++
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);

          //0->K
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1423_0000:
          machine.PC = new ProgramCounter(1424, 0);
          lastOpIndex = 1423_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1424_0000:
          machine.PC = new ProgramCounter(1425, 0);
          lastOpIndex = 1424_0005;
          stepsCompleted += 6;

          //!If {{r3}}-'+'
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 43 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1432, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1425_0000:
          machine.PC = new ProgramCounter(1426, 0);
          lastOpIndex = 1425_0005;
          stepsCompleted += 6;

          //If {^^oPENX}-{r1}
          new Const() { Value = 34519 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1428, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1426_0000:
          machine.PC = new ProgramCounter(1428, 0);
          lastOpIndex = 1427_0000;
          stepsCompleted += 7;

          //{^^oPENY}+6->{^^oPENY}
          new Const() { Value = 34520 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1428_0000:
          machine.PC = new ProgramCounter(1432, 0);
          lastOpIndex = 1431_0000;
          stepsCompleted += 12;

          //{^^oPENY}+2->{^^oPENY}
          new Const() { Value = 34520 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 2 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //{r1}->{^^oPENX}
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //{r3}++
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1432_0000:
          machine.PC = new ProgramCounter(1433, 0);
          lastOpIndex = 1432_0005;
          stepsCompleted += 6;

          //!If {{r3}}-'^'
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 94 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1438, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1433_0000:
          machine.PC = new ProgramCounter(1434, 0);
          lastOpIndex = 1433_0001;
          stepsCompleted += 2;

          //({r4})()
          new ReadAddress() { VarAddress = 774 }.Execute(ref machine);
          new CallAddr() { ArgCount = 0 }.Execute(ref machine);
          break;
        case 1434_0000:
          machine.PC = new ProgramCounter(1438, 0);
          lastOpIndex = 1437_0000;
          stepsCompleted += 8;

          //{r1}->{^^oPENX}
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //1->{^^oPENY}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //{r3}++
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1438_0000:
          machine.PC = new ProgramCounter(1439, 0);
          lastOpIndex = 1438_0004;
          stepsCompleted += 5;

          //DrawF {{r3}}>Frac
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new ToStringChar().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Text() { IsTextOnly = true, ArgCount = 1 }.Execute(ref machine);
          break;
        case 1439_0000:
          machine.PC = new ProgramCounter(1440, 0);
          lastOpIndex = 1439_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 1440_0000:
          machine.PC = new ProgramCounter(1442, 0);
          lastOpIndex = 1441_0001;
          stepsCompleted += 5;

          //{r3}++
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new Inc().Execute(ref machine);
          new StoreAddress() { Address = 772, RMode = 1 }.Execute(ref machine);

          //If {r6}
          new ReadAddress() { VarAddress = 778 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1447, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1442_0000:
          machine.PC = new ProgramCounter(1443, 0);
          lastOpIndex = 1442_0015;
          stepsCompleted += 16;

          //If inString(' ',{r3})*4+{^^oPENX}>92
          new Const() { Value = 32 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new InData() { ArgCount = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 4 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 34519 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 92 }.Execute(ref machine);
          new Binary_U16<Greater>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1446, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1443_0000:
          machine.PC = new ProgramCounter(1446, 0);
          lastOpIndex = 1445_0000;
          stepsCompleted += 9;

          //{r1}->{^^oPENX}
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //{^^oPENY}+6->{^^oPENY}
          new Const() { Value = 34520 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 6 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1446_0000:
          machine.PC = new ProgramCounter(1447, 0);
          lastOpIndex = 1446_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1447_0000:
          machine.PC = new ProgramCounter(1448, 0);
          lastOpIndex = 1447_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1448_0000:
          machine.PC = new ProgramCounter(1449, 0);
          lastOpIndex = 1448_0000;
          stepsCompleted += 1;

          //..AXE
          new Nop().Execute(ref machine);
          break;
        case 1449_0000:
          machine.PC = new ProgramCounter(1451, 0);
          lastOpIndex = 1450_0002;
          stepsCompleted += 4;

          //Lbl TALK
          new Label().Execute(ref machine);

          //If {L1+688}^^r
          new Const() { Value = 2992 }.Execute(ref machine);
          new ReadMemory() { RMode = 1 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1492, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1451_0000:
          machine.PC = new ProgramCounter(1452, 11);
          lastOpIndex = 1452_0010;
          stepsCompleted += 14;

          //{L1+704}->{r1}
          new Const() { Value = 3008 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //ReturnIf {r1}>=18 and ({r1}<=24
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 24 }.Execute(ref machine);
          new Binary_U16<LessEq>().Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1453, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1452_0011:
          machine.PC = new ProgramCounter(1453, 0);
          lastOpIndex = 1452_0011;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1453_0000:
          machine.PC = new ProgramCounter(1453, 5);
          lastOpIndex = 1453_0004;
          stepsCompleted += 5;

          //Return!If {r1}-31
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 31 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1454, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1453_0005:
          machine.PC = new ProgramCounter(1454, 0);
          lastOpIndex = 1453_0005;
          stepsCompleted += 1;
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1454_0000:
          machine.PC = new ProgramCounter(1455, 0);
          lastOpIndex = 1454_0004;
          stepsCompleted += 5;

          //If {r1}>=18
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 18 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1457, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1455_0000:
          machine.PC = new ProgramCounter(1457, 0);
          lastOpIndex = 1456_0000;
          stepsCompleted += 6;

          //{r1}-5->{r1}
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1457_0000:
          machine.PC = new ProgramCounter(1458, 0);
          lastOpIndex = 1457_0004;
          stepsCompleted += 5;

          //If {r1}>=26
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 26 }.Execute(ref machine);
          new Binary_U16<GreaterEq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1460, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1458_0000:
          machine.PC = new ProgramCounter(1460, 0);
          lastOpIndex = 1459_0000;
          stepsCompleted += 4;

          //{r1}--
          new ReadAddress() { VarAddress = 768 }.Execute(ref machine);
          new Dec().Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1460_0000:
          machine.PC = new ProgramCounter(1461, 0);
          lastOpIndex = 1460_0002;
          stepsCompleted += 3;

          //Pause 500
          new Const() { Value = 500 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Pause() { ArgCount = 1 }.Execute(ref machine);
          return;
        case 1461_0000:
          machine.PC = new ProgramCounter(1462, 0);
          lastOpIndex = 1461_0000;
          stepsCompleted += 1;

          //LDSTR()
          new Call() { LabelAddress = 1398, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1462_0000:
          machine.PC = new ProgramCounter(1464, 0);
          lastOpIndex = 1463_0000;
          stepsCompleted += 4;

          //Fix 3
          new Const() { Value = 3 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //CLRIN()
          new Call() { LabelAddress = 1388, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1464_0000:
          machine.PC = new ProgramCounter(1468, 0);
          lastOpIndex = 1467_0001;
          stepsCompleted += 10;

          //|LCLRIN->{r4}
          new Const() { Value = 1388 }.Execute(ref machine);
          new StoreAddress() { Address = 774, RMode = 1 }.Execute(ref machine);

          //8->{r1}->{^^oPENX}
          new Const() { Value = 8 }.Execute(ref machine);
          new StoreAddress() { Address = 768, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34519, RMode = 0 }.Execute(ref machine);

          //1->K->{^^oPENY}
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 34520, RMode = 0 }.Execute(ref machine);

          //~30->F
          new Const() { Value = 65506 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          break;
        case 1468_0000:
          machine.PC = new ProgramCounter(1469, 0);
          lastOpIndex = 1468_0002;
          stepsCompleted += 3;

          //While {{r3}}
          new ReadAddress() { VarAddress = 772 }.Execute(ref machine);
          new ReadMemory() { RMode = 0 }.Execute(ref machine);
          new While() { JumpLine = 1480, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1469_0000:
          machine.PC = new ProgramCounter(1470, 0);
          lastOpIndex = 1469_0000;
          stepsCompleted += 1;

          //SLOT()
          new Call() { LabelAddress = 1493, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1470_0000:
          machine.PC = new ProgramCounter(1471, 0);
          lastOpIndex = 1470_0001;
          stepsCompleted += 2;

          //If K
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1473, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1471_0000:
          machine.PC = new ProgramCounter(1472, 0);
          lastOpIndex = 1471_0000;
          stepsCompleted += 1;

          //TEXT()
          new Call() { LabelAddress = 1417, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1472_0000:
          machine.PC = new ProgramCounter(1473, 0);
          lastOpIndex = 1472_0000;
          stepsCompleted += 1;

          //Else
          new Else() { IsElseIf = false, JumpLine = 1479, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1473_0000:
          machine.PC = new ProgramCounter(1475, 0);
          lastOpIndex = 1474_0004;
          stepsCompleted += 7;
          getKeysCompleted += 1;

          //getKey->K
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //If K=15
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Eq>().Execute(ref machine);
          new If() { Negated = false, JumpLine = 1477, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1475_0000:
          machine.PC = new ProgramCounter(1476, 0);
          lastOpIndex = 1475_0000;
          stepsCompleted += 1;

          //Goto SKPL
          new Goto() { LabelAddress = 1489 }.Execute(ref machine);
          break;
        case 1476_0000:
          machine.PC = new ProgramCounter(1477, 0);
          lastOpIndex = 1476_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1477_0000:
          machine.PC = new ProgramCounter(1478, 0);
          lastOpIndex = 1477_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 1478_0000:
          machine.PC = new ProgramCounter(1479, 0);
          lastOpIndex = 1478_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1479_0000:
          machine.PC = new ProgramCounter(1480, 0);
          lastOpIndex = 1479_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 1468, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1480_0000:
          machine.PC = new ProgramCounter(1481, 0);
          lastOpIndex = 1480_0001;
          stepsCompleted += 2;

          //0->K
          new Const() { Value = 0 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);
          break;
        case 1481_0000:
          machine.PC = new ProgramCounter(1482, 0);
          lastOpIndex = 1481_0007;
          stepsCompleted += 8;

          //Repeat F>>65 and K
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 65 }.Execute(ref machine);
          new Binary_S16<GreaterS>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 20 }.Execute(ref machine);
          new Binary_U8<And_U8>().Execute(ref machine);
          new Repeat() { JumpLine = 1489, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1482_0000:
          machine.PC = new ProgramCounter(1483, 0);
          lastOpIndex = 1482_0000;
          stepsCompleted += 1;

          //SLOT()
          new Call() { LabelAddress = 1493, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1483_0000:
          machine.PC = new ProgramCounter(1484, 0);
          lastOpIndex = 1483_0001;
          stepsCompleted += 2;
          getKeysCompleted += 1;

          //If getKey
          new GetKey() { RMode = 0, ArgCount = 0 }.Execute(ref machine);
          new If() { Negated = false, JumpLine = 1487, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1484_0000:
          machine.PC = new ProgramCounter(1486, 0);
          lastOpIndex = 1485_0000;
          stepsCompleted += 3;

          //1->K
          new Const() { Value = 1 }.Execute(ref machine);
          new StoreAddress() { Address = 20, RMode = 1 }.Execute(ref machine);

          //CLRIN()
          new Call() { LabelAddress = 1388, ArgCount = 0 }.Execute(ref machine);
          break;
        case 1486_0000:
          machine.PC = new ProgramCounter(1487, 0);
          lastOpIndex = 1486_0000;
          stepsCompleted += 1;

          //End
          new End().Execute(ref machine);
          break;
        case 1487_0000:
          machine.PC = new ProgramCounter(1488, 0);
          lastOpIndex = 1487_0000;
          stepsCompleted += 1;

          //DispGraph
          new DispGraph() { DoClrDraw = false, DoRecalPic = false, RMode = 0, ArgCount = 0 }.Execute(ref machine);
          return;
        case 1488_0000:
          machine.PC = new ProgramCounter(1489, 0);
          lastOpIndex = 1488_0000;
          stepsCompleted += 1;

          //End
          new EndLoop() { JumpLine = 1481, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1489_0000:
          machine.PC = new ProgramCounter(1492, 0);
          lastOpIndex = 1491_0000;
          stepsCompleted += 5;

          //Lbl SKPL
          new Label().Execute(ref machine);

          //Fix 2
          new Const() { Value = 2 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Fix() { ArgCount = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1492_0000:
          machine.PC = new ProgramCounter(1493, 0);
          lastOpIndex = 1492_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1493_0000:
          machine.PC = new ProgramCounter(1497, 0);
          lastOpIndex = 1496_0008;
          stepsCompleted += 23;

          //Lbl SLOT
          new Label().Execute(ref machine);

          //Pxl-Change(6,F
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //Pxl-Change(6,F+15
          new Const() { Value = 6 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 15 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new PxlPlot<PlotterInvert>() { RMode = 0, ArgCount = 2 }.Execute(ref machine);

          //!If F+1->F-80
          new ReadAddress() { VarAddress = 10 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 1 }.Execute(ref machine);
          new Binary_U16<Add>().Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 80 }.Execute(ref machine);
          new Binary_U16<Sub>().Execute(ref machine);
          new If() { Negated = true, JumpLine = 1499, JumpOp = 0 }.Execute(ref machine);
          break;
        case 1497_0000:
          machine.PC = new ProgramCounter(1499, 0);
          lastOpIndex = 1498_0000;
          stepsCompleted += 3;

          //~30->F
          new Const() { Value = 65506 }.Execute(ref machine);
          new StoreAddress() { Address = 10, RMode = 1 }.Execute(ref machine);

          //End
          new End().Execute(ref machine);
          break;
        case 1499_0000:
          machine.PC = new ProgramCounter(1500, 0);
          lastOpIndex = 1499_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1500_0000:
          machine.PC = new ProgramCounter(1503, 0);
          lastOpIndex = 1502_0000;
          stepsCompleted += 9;

          //Lbl X56
          new Label().Execute(ref machine);

          //*5/256->{L1+16}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 2320, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1503_0000:
          machine.PC = new ProgramCounter(1506, 0);
          lastOpIndex = 1505_0000;
          stepsCompleted += 9;

          //Lbl Y56
          new Label().Execute(ref machine);

          //*5/256->{L1+18}^^r
          new PushArg().Execute(ref machine);
          new Const() { Value = 5 }.Execute(ref machine);
          new Binary_U16<Mul>().Execute(ref machine);
          new PushArg().Execute(ref machine);
          new Const() { Value = 256 }.Execute(ref machine);
          new Binary_U16<Div>().Execute(ref machine);
          new StoreAddress() { Address = 2322, RMode = 1 }.Execute(ref machine);

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
        case 1506_0000:
          machine.PC = new ProgramCounter(0, 0);
          lastOpIndex = 1506_0000;
          stepsCompleted += 1;

          //Return
          new Return() { RMode = 0 }.Execute(ref machine);
          break;
      }
    }
    return;
  }
}
