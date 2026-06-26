using NUnit.Framework;

namespace Axe4Unity {

  public class TestsOptimizations : TestBase {

    [Test]
    public void TestNotIfForEquality() {
      Execute("55->A",
              "!If A-55",
              "1->B",
              "End",

              "!If A-54",
              "1->C",
              "End",

              "!If A-56",
              "1->D",
              "End");

      Assert.That(U16("B"), Is.EqualTo(1));
      Assert.That(U16("C"), Is.EqualTo(0));
      Assert.That(U16("D"), Is.EqualTo(0));
    }

    [Test]
    public void TestMultiAssignment() {
      Execute("1->A->B+1->C*2->D");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(1));
      Assert.That(U16("C"), Is.EqualTo(2));
      Assert.That(U16("D"), Is.EqualTo(4));
    }

    [Test]
    public void TestIfConditionCanBeUsedAsValue() {
      Execute("5->A->B->C->D",
              "If A=B",
              "->C",
              "End",
              "!If A-B",
              "->D",
              "End");

      Assert.That(U16("C"), Is.EqualTo(1));
      Assert.That(U16("D"), Is.EqualTo(0));
    }

    [Test]
    public void TestSubroutineCanBeUsedInExpression() {
      Execute("5->A->B",
              "2->C",
              "3->D",

              "If sub(EQ, A, B)",
              "6->X",
              "End",

              "If sub(EQ, sub(ADD, C, D), A)",
              "7->Y",
              "End",

              "If sub(EQ, sub(ADD, A, B), C)",
              "9->Z",
              "End",

              "Return",

              "Lbl EQ",
              "{r1}={r2}",
              "Return",

              "Lbl ADD",
              "{r1}+{r2}",
              "Return");

      Assert.That(U16("X"), Is.EqualTo(6));
      Assert.That(U16("Y"), Is.EqualTo(7));
      Assert.That(U16("Z"), Is.EqualTo(0));
    }

    [Test]
    public void TestWhile1DoesNotSetHL() {
      Execute("123",
              "While 1",
              "->F",
              "EndIf 1");

      Assert.That(U16("F"), Is.EqualTo(123));
    }

    [Test]
    public void TestCanUseStoreWithinFunction() {
      Execute("FOO(1,2->A",
              "Return",
              "Lbl FOO",
              "{r2}->C",
              "123",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(2));
    }

    [Test]
    public void TestStoreToAddressReturnsAddress() {
      Execute("^^oL1->A",
              "23->{A}->B");

      Assert.That(U16("B"), Is.EqualTo(Addr("L1")));
    }

    [Test]
    public void TestStoreU16ToAddressReturnsAddressPlusOne() {
      Execute("^^oL1->A",
              "23->{A}^^r->B");

      Assert.That(U16("B"), Is.EqualTo(Addr("L1") + 1));
    }

    [Test]
    public void TestCanUseAddrFromPreviousLine() {
      Execute("23->{L1+10}",
              "7+3->A",
              "{+L1}->B");

      Assert.That(U16("B"), Is.EqualTo(23));
    }

    [Test]
    public void TestWritingToConstantAddressKeepsHL() {
      Execute("5->{L1+5}->{L1+99}",
              "{L1+99}->A");

      Assert.That(U16("A"), Is.EqualTo(5));
    }

    [Test]
    public void TestWritingToDynamicAddressKeepsAddress() {
      Execute("L1->A",
              "5->{A}->B");

      Assert.That(U16("B"), Is.EqualTo(Addr("L1")));
    }

    [Test]
    public void TestDataExpressionsDontModifyHL() {
      Execute("sub(ADD5, 3)->A",
              "Return",
              "Lbl ADD5",
              "[05]->Str1",
              "+{Str1}",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(8));
    }

    [Test]
    public void TestNoStackBreak() {
      Execute("[1234]->GDB1GS",
              "Pt-Off({r1},{r2},{r3}+8,GDB1GS+768)");
    }

    [Test]
    public void TestCanOmitArgsFromExpression() {
      Execute("SUM(1,,5,)->A",
              "Return",
              "Lbl SUM",
              "{r1}+{r2}+{r3}+{r4}",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(12));
    }

    [Test]
    public void TestCanOmitFirstArgOfSingleArgSubroutine() {
      Execute("5",
              "sub(STORE,)",
              "Return",
              "Lbl STORE",
              "{r1}->A",
              "Return");

      Assert.That(U16("A"), Is.EqualTo(5));
    }

    [Test]
    public void TestCanNestMemoryStoreOperations() {
      Execute("L1->Z",
              "3->{2->{1->{Z}+1}+1}",
              "{L1}->A",
              "{L1+1}->B",
              "{L1+2}->C");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(2));
      Assert.That(U16("C"), Is.EqualTo(3));
    }

    [Test]
    public void TestBigChonker() {
      Execute("AO(5,3,9)",
              "Return",
              "Lbl AO",
              "0->{0->{{r3}->{0->{0->{{r2}*256->{{r1}*256->{O+1->O*11+L1+309}^^r+1}^^r+1}^^r+1}^^r+1}^^r+1}+1}",
              "Return");

      var l1 = Addr("L1") + 309 + 11;

      Assert.That(U16("O"), Is.EqualTo(1));
      Assert.That(Machine.State.Read_U16(l1 + 0) / 256, Is.EqualTo(5));
      Assert.That(Machine.State.Read_U16(l1 + 2) / 256, Is.EqualTo(3));
    }
  }
}
