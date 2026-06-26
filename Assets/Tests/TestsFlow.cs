using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Axe4Unity {

  public class TestsFlow : TestBase {

    [Test]
    public void TestGoto() {
      Execute("1->A",
              "Goto FOO",
              "2->A",
              "Lbl FOO");

      Assert.That(U16("A"), Is.EqualTo(1));
    }

    [Test]
    public void TestGotoExpr() {
      Execute("|LBAR->A",
              "Goto (A)",
              "Lbl FOO",
              "5->A",
              "Return",
              "Lbl BAR",
              "23->A",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(23));
    }

    [Test]
    public void TestIf() {
      Execute("2->Z",
              "If 1",
              "5->Z",
              "End",
              "If 0",
              "9->Z",
              "End");

      Assert.That(U16("Z"), Is.EqualTo(5));
    }

    [Test]
    public void TestNotIf() {
      Execute("!If 1",
              "5->G",
              "End",
              "!If 0",
              "7->J",
              "End");

      Assert.That(U16("G"), Is.EqualTo(0));
      Assert.That(U16("J"), Is.EqualTo(7));
    }

    [Test]
    public void TestIfElse() {
      Execute("2->A->B",
              "If 1",
              "5->A",
              "Else",
              "8->A",
              "End",
              "If 0",
              "5->B",
              "Else",
              "8->B",
              "End");

      Assert.That(U16("A"), Is.EqualTo(5));
      Assert.That(U16("B"), Is.EqualTo(8));
    }

    [Test]
    public void TestElseIf() {
      Execute("If 0",
              "5->A",
              "ElseIf 1",
              "10->A",
              "End");

      Assert.That(U16("A"), Is.EqualTo(10));

      Execute("If 1",
              "5->B",
              "ElseIf 0",
              "10->B",
              "End");

      Assert.That(U16("B"), Is.EqualTo(5));

      Execute("If 0",
              "1->C",
              "ElseIf 0",
              "2->C",
              "ElseIf 0",
              "3->C",
              "ElseIf 1",
              "4->C",
              "End");

      Assert.That(U16("C"), Is.EqualTo(4));

      Execute("If 0",
              "ElseIf 0",
              "ElseIf 0",
              "Else",
              "4->C",
              "End");

      Assert.That(U16("C"), Is.EqualTo(4));

      Execute("If 1",
              "1->A",
              "ElseIf 0",
              "ElseIf 0",
              "Else",
              "2->B",
              "End");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(0));
    }

    [Test]
    public void TestLoopInsideElseIf() {
      Execute("If 0",
              "55->A",
              "ElseIf 1",
              "For(B,1,5)",
              "C+1->C",
              "End",
              "ElseIf 0",
              "End");

      Assert.That(U16("C"), Is.EqualTo(5));
    }

    [Test]
    public void TestConditional() {
      Execute("(1?5,3)->A",
              "(0?8,9)->B",
              "(1?6)->C",
              "(0?2)->D");

      Assert.That(U16("A"), Is.EqualTo(5));
      Assert.That(U16("B"), Is.EqualTo(9));
      Assert.That(U16("C"), Is.EqualTo(6));
      Assert.That(U16("D"), Is.EqualTo(0));
    }

    [Test]
    public void TestWhile() {
      Execute("10->A",
              "While A",
              "A-1->A",
              "B+2->B",
              "End");

      Assert.That(U16("B"), Is.EqualTo(20));
    }

    [Test]
    public void TestRepeat() {
      Execute("10->A",
              "Repeat A=0",
              "A-1->A",
              "B+3->B",
              "End");

      Assert.That(U16("B"), Is.EqualTo(30));
    }

    [Test]
    public void TestFor() {
      Execute("For(A,1,10)",
              "B+1->B",
              "End");

      Assert.That(U16("B"), Is.EqualTo(10));
    }

    [Test]
    public void TestDoubleFor() {
      Execute("For(A,1,5)",
              "For(B,1,5)",
              "C+1->C",
              "End",
              "End");

      Assert.That(U16("C"), Is.EqualTo(25));
    }

    [Test]
    public void TestForStack() {
      Execute("For(10)",
              "A+1->A",
              "End");

      Assert.That(U16("A"), Is.EqualTo(10));
    }

    [Test]
    public void TestForStackWithExpression() {
      Execute("4->A",
              "2->B",
              "For(A*B)",
              "A+1->A",
              "End");

      Assert.That(U16("A"), Is.EqualTo(12));
    }

    [Test]
    public void TestForStackWithRMod() {
      Execute("For(260)^^r",
              "A+1->A",
              "End");

      Assert.That(U16("A"), Is.EqualTo(4));
    }

    [Test]
    public void TestNestedForStack() {
      Execute("For(5)",
              "For(8)",
              "A+1->A",
              "End",
              "End");

      Assert.That(U16("A"), Is.EqualTo(40));
    }

    [Test]
    public void TestForStackWithinRegularFor() {
      Execute("For(A,0,9)",
              "For(5)",
              "B+1->B",
              "End",
              "End");

      Assert.That(U16("B"), Is.EqualTo(50));
    }

    [Test]
    public void TestDS() {
      Execute("4->B",
              "For(A,1,12)",
              "DS<(B,4)",
              "C+1->C",
              "End",
              "End");

      Assert.That(U16("C"), Is.EqualTo(3));
    }

    [Test]
    public void TestEndIf() {
      Execute("For(A,1,10)",
              "B+1->B",
              "EndIf A=5");

      Assert.That(U16("A"), Is.EqualTo(5));
    }

    [Test]
    public void TestEndNotIf() {
      Execute("For(A,1,10)",
              "B+1->B",
              "End!If A-5");

      Assert.That(U16("A"), Is.EqualTo(5));
    }

    [Test]
    public void TestJumpTable() {
      for (int i = 0; i < 3; i++) {
        Execute($"Z-Test({i},A0,A1,A2)",
                "13->A",
                "Return",
                "Lbl A0",
                "10->A",
                "Return",
                "Lbl A1",
                "11->A",
                "Return",
                "Lbl A2",
                "12->A",
                "Return");

        Assert.That(U16("A"), Is.EqualTo(10 + i));
      }
    }
  }
}
