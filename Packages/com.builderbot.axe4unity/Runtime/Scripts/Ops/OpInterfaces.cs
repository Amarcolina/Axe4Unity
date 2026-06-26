
namespace Axe4Unity {

  public interface IOp {
    void Execute(ref MachineStateNative machine);
  }

  public interface IOpRModifier {
    int RMode { get; set; }
  }

  public interface IOpControl : IOp {
    int JumpLine { get; set; }
    int JumpOp { get; set; }
  }

  public interface IOpLoopExit {
    bool ShouldExit { get; }
  }

  public interface IOpGraphic { }

  public interface IOp_Function : IOp {
    int ArgCount { get; set; }
  }

  public interface IOp_Binary_U16 {
    ushort Execute(ref MachineStateNative machine, ushort lhs, ushort rhs);
  }

  public interface IOp_Binary_S16 {
    short Execute(ref MachineStateNative machine, short lhs, short rhs);
  }

  public interface IOp_Binary_U8 {
    byte Execute(ref MachineStateNative machine, byte lhs, byte rhs);
  }

}
