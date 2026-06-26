using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Axe4Unity {

  public class TestsCalling : TestBase {

    [Test]
    public void TestCall() {
      Execute("1->A",
              "sub(PP)",
              "A+5->A",
              "Return",
              "Lbl PP",
              "10->A",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(15));
    }

    [Test]
    public void TestCallCanBeUsedAsValue() {
      Execute("sub(FOO)->A",
              "Return",
              "Lbl FOO",
              "5->B",
              "B+B",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(10));
    }

    [Test]
    public void TestCallCanBeUsedAsCondition() {
      Execute("If sub(TRUE)",
              "23->A",
              "End",
              "If sub(FALSE)",
              "44->A",
              "End",
              "Return",
              "Lbl TRUE",
              "1",
              "Return",
              "Lbl FALSE",
              "0",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(23));
    }

    [Test]
    public void TestCallArgOrder() {
      Execute("sub(MAD,2,3,5)->A",
              "Return",
              "Lbl MAD",
              "{r1}*{r2}+{r3}",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(11));
    }

    [Test]
    public void TestCanUseDirectCalling() {
      Execute("ADD(5, 6)->A",
              "Return",
              "Lbl ADD",
              "{r1}+{r2}",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(11));
    }

    [Test]
    public void TestCanUseDirectCallInExpression() {
      Execute("C()+5->A",
              "Return",
              "Lbl C",
              "3",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(8));
    }

    [Test]
    public void TestCanCallByAddress() {
      Execute("|LADD->A",
              "(A)(5, 3)->B",
              "Return",
              "Lbl ADD",
              "{r1}+{r2}",
              "Return");

      Assert.That(U16("B"), Is.EqualTo(8));
    }

    [Test]
    public void TestReturnIf() {
      Execute("sub(FOO)",
              "Return",
              "Lbl FOO",
              "5->A",
              "ReturnIf 1",
              "10->A",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(5));
    }

    [Test]
    public void TestReturnNotIf() {
      Execute("sub(FOO)",
              "Return",
              "Lbl FOO",
              "5->A",
              "Return!If 1",
              "10->A",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(10));
    }
  }
}
