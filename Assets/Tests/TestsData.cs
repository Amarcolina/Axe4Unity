using NUnit.Framework;

namespace Axe4Unity {

  public class TestsData : TestBase {

    [Test]
    public void TestConstantValue() {
      Execute("65535->A");

      Assert.That(U16("A"), Is.EqualTo(65535));
    }

    [Test]
    public void TestNegativeConstant() {
      Execute("~5->A");

      Assert.That(S16("A"), Is.EqualTo(-5));
    }

    [Test]
    public void TestDecimalConstant() {
      Execute("5.5->A",
              "A*2->B");

      Assert.That(U16("B") / 256f, Is.EqualTo(11));

      Execute("123.456->A");

      Assert.That(U16("A") / 256f, Is.EqualTo(123.456f).Within(1f / 256f));
    }

    [Test]
    public void TestHexValue() {
      Execute("|EA10F->A");

      Assert.That(U16("A"), Is.EqualTo(0xA10F));
    }

    [Test]
    public void TestBinaryValue() {
      Execute("greek_pi101->A",
              "greek_pi1111->B",
              "greek_pi10101100->C");

      Assert.That(U16("A"), Is.EqualTo(0b101));
      Assert.That(U16("B"), Is.EqualTo(0b1111));
      Assert.That(U16("C"), Is.EqualTo(0b10101100));
    }

    [Test]
    public void TestBinaryData() {
      Execute("[0102FFAB]->Pic1",
              "{Pic1}^^r->A",
              "{Pic1+2}^^r->B");

      Assert.That(U16("A"), Is.EqualTo(0x0201));
      Assert.That(U16("B"), Is.EqualTo(0xABFF));
    }

    [Test]
    public void TestDataSingleByte() {
      Execute("DeltaList(1,2,3)->Str1",
              "{Str1}->A",
              "{Str1+1}->B",
              "{Str1+2}->C");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(2));
      Assert.That(U16("C"), Is.EqualTo(3));
    }

    [Test]
    public void TestDataDoubleByte() {
      Execute("DeltaList(1^^r,2^^r,3^^r)->Str1",
              "{Str1}^^r->A",
              "{Str1+2}^^r->B",
              "{Str1+4}^^r->C");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(2));
      Assert.That(U16("C"), Is.EqualTo(3));
    }

    [Test]
    public void TestDataWithExpression() {
      Execute("DeltaList(5+10)->Str1",
              "{Str1}->A");

      Assert.That(U16("A"), Is.EqualTo(15));
    }

    [Test]
    public void TestDataWithStaticVar() {
      Execute("[35]->Str1",
              "DeltaList(Str1^^r)->Str2",
              "{Str2}^^r->A",
              "{A}->B");

      Assert.That(U16("B"), Is.EqualTo(0x35));
    }

    [Test]
    public void TestMultipleDataSingleLine() {
      Execute("[04][09]->Str1",
              "{Str1}->A",
              "{Str1+1}->B");

      Assert.That(U16("A"), Is.EqualTo(4));
      Assert.That(U16("B"), Is.EqualTo(9));
    }

    [Test]
    public void TestStringWithStoreDoesHaveTerminator() {
      Execute("\"FOO\"->Str1",
              "{Str1+3}->A");

      Assert.That(U16("A"), Is.Zero);
    }

    [Test]
    public void TestStringWithoutStoreDoesntHaveTerminator() {
      Execute("\"FOO\"",
              "[3311]->Str1",
              "{Str1-1}->A");

      Assert.That(U16("A"), Is.EqualTo((int)'O'));
    }

    [Test]
    public void TestStringWithDataAfterDoesntHaveTerminator() {
      Execute("\"FOO\"[99]->Str1",
              "{Str1+2}->A",
              "{Str1+3}->B");

      Assert.That(U16("A"), Is.EqualTo((int)'O'));
      Assert.That(U16("B"), Is.EqualTo(0x99));
    }

    [Test]
    public void TestCustomVariable() {
      Execute("^^oL1->^^oFOO",
              "5->FOO",
              "FOO->B");

      Assert.That(U16("FOO"), Is.EqualTo(5));
    }

    [Test]
    public void TestCopyData() {
      Execute("For(A,0,7)",
              "A->{L1+A}",
              "End",
              "conj(L1,L2,8)");

      for (int i = 0; i < 8; i++) {
        Assert.That(U8(Addr("L2") + i), Is.EqualTo(i));
      }
    }

    [Test]
    public void TestCopyDataBackwards() {
      Execute("For(A,0,7)",
              "A->{L1+A}",
              "End",
              "conj(L1+7,L2+7,8)^^r");

      for (int i = 0; i < 8; i++) {
        Assert.That(U8(Addr("L2") + i), Is.EqualTo(i));
      }
    }

    [Test]
    public void TestExchangeData() {
      Execute("For(A,0,10)",
              "A->{L1+A}",
              "A*2+100->{L2+A}",
              "End",
              "expr(L1,L2,5)");

      for (int i = 0; i < 5; i++) {
        Assert.That(U8(Addr("L1") + i), Is.EqualTo(i * 2 + 100));
        Assert.That(U8(Addr("L2") + i), Is.EqualTo(i));
      }

      for (int i = 5; i <= 10; i++) {
        Assert.That(U8(Addr("L1") + i), Is.EqualTo(i));
        Assert.That(U8(Addr("L2") + i), Is.EqualTo(i * 2 + 100));
      }
    }

    [Test]
    public void TestInData() {
      Execute("For(A,0,10)",
              "A+100->{L1+A}",
              "End",
              "0->{L1+11}",
              "inString(105, L1)->A",
              "inString(105, L1, 10)->B",
              "inString(105, L1, 3)->C",
              "inString(120, L1)->D");

      Assert.That(U16("A"), Is.EqualTo(6));
      Assert.That(U16("B"), Is.EqualTo(6));
      Assert.That(U16("C"), Is.EqualTo(0));
      Assert.That(U16("D"), Is.EqualTo(0));
    }

    [Test]
    public void TestLength() {
      Execute("\"0123456789\"->Str1",
              "length(Str1)->A");

      Assert.That(U8("A"), Is.EqualTo(10));
    }

    [Test]
    public void TestStrGet() {
      Execute("\"ABCD\"->Str1",
              "\"EFGH\"[00]",
              "\"0123\"[00]",
              "\"4\"[00]",
              "\"56789\"[00]",
              "stdDev(Str1, 0)->A",
              "stdDev(Str1, 1)->B",
              "stdDev(Str1, 4)->C",
              "{A}->A",
              "{B}->B",
              "{C}->C");

      Assert.That(U8("A"), Is.EqualTo((int)'A'));
      Assert.That(U8("B"), Is.EqualTo((int)'E'));
      Assert.That(U8("C"), Is.EqualTo((int)'5'));
    }

    [Test]
    public void TestStrEq() {
      Execute("\"ABCD\"->Str1",
              "\"ABCD\"->Str2",
              "\"ABC\"->Str3",
              "\"ABCDEF\"->Str4",
              "\"\"->Str5",
              "Equ>String(Str1,Str1)->A",
              "Equ>String(Str1,Str2)->B",
              "Equ>String(Str1,Str3)->C",
              "Equ>String(Str1,Str4)->D",
              "Equ>String(Str1,Str5)->E",
              "Equ>String(Str5,Str5)->F");

      Assert.That(U8("A"), Is.EqualTo(1));
      Assert.That(U8("B"), Is.EqualTo(1));
      Assert.That(U8("C"), Is.EqualTo(0));
      Assert.That(U8("D"), Is.EqualTo(0));
      Assert.That(U8("E"), Is.EqualTo(0));
      Assert.That(U8("F"), Is.EqualTo(1));
    }

    [Test]
    public void TestSort() {
      Execute("[09070506020304010800]->Str1",
              "conj(Str1,L1,10)",
              "SortD(L1,10)");

      for (int i = 0; i < 8; i++) {
        Assert.That(U8(Addr("L1") + i), Is.EqualTo(i));
      }
    }
  }
}
