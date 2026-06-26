using NUnit.Framework;

namespace Axe4Unity {

  public class TestsConstantFolding : TestBase {

    [Test]
    public void TestPrimitivesAreConstant() {
      AssertConstants("65535", 65535);
      AssertConstants("123", 123);
      AssertConstants("0", 0);
    }

    [Test]
    public void TestNegativesAreConstant() {
      unchecked {
        AssertConstants("~123", (ushort)(-123));
        AssertConstants("~1", (ushort)-1);
        AssertConstants("~5000", (ushort)-5000);
      }
    }

    [Test]
    public void TestConstantExpressionsAreConstant() {
      AssertConstants("1+1", 2);
      AssertConstants("5*2", 10);
      AssertConstants("100/2", 50);
      AssertConstants("512*2", 1024);
      AssertConstants("10/3", 3);
    }

    [Test]
    public void TestConstantMultiExpressionsAreConstant() {
      unchecked {
        AssertConstants("1+1*2", 4);
        AssertConstants("1+1*~5", (ushort)-10);
        AssertConstants("~10/~10-10", (ushort)-9);
      }
    }

    [Test]
    public void TestExpressionsWithBuiltInStaticVariablesAreConstant() {
      var program = Execute("L1+10");
      AssertFirstOpIsConstant(program, Addr("L1") + 10);
    }

    [Test]
    public void TestExpressionWithUserDefinedStaticVariablesAreConstant() {
      var program = Execute("Str1+5",
                            "[001122]->Str1");

      AssertFirstOpIsConstant(program, Addr("Str1") + 5);
    }

    [Test]
    public void TestForLoopWorksWithFoldedConstants() {
      Execute("For(A,5+5+5-5-5-5+1,10+10+10-10-10)",
              "B+1->B",
              "End");

      Assert.That(U16("B"), Is.EqualTo(10));

      Execute("For(B,L6+12,L6+768)",
              "A+1->A",
              "End");
    }

    private void AssertConstants(string expr, int value) {
      var program = Execute(expr);
      AssertFirstOpIsConstant(program, value);
    }

    private void AssertFirstOpIsConstant(Program program, int value) {
      Assert.That(program.Lines[0].Ops.Count, Is.EqualTo(1));

      var op = (Op.Const)program.Lines[0].Ops[0].Op;
      Assert.That(op.Value, Is.EqualTo(value));
    }
  }
}
