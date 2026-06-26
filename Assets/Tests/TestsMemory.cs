using NUnit.Framework;

namespace Axe4Unity {

  public class TestsMemory : TestBase {

    [Test]
    public void TestCanWriteToMemory() {
      Execute("1->{L1}");

      Assert.That(U8("L1"), Is.EqualTo(1));
    }

    [Test]
    public void TestCanWriteWordToMemory() {
      Execute("567->{L2}^^r");

      Assert.That(U16("L2"), Is.EqualTo(567));
    }

    [Test]
    public void TestCanReadWordFromMemory() {
      Execute("999->{L3}^^r",
              "{L3}^^r->B");

      Assert.That(U16("B"), Is.EqualTo(999));
    }

    [Test]
    public void TestCanWriteToMemoryAtExpression() {
      Execute("10->B",
              "1->{L1+B}");

      Assert.That(U8(Addr("L1") + 10), Is.EqualTo(1));
    }

    [Test]
    public void TestMemoryCanBeUsedInExpressions() {
      Execute("5->{L1}",
              "10->{L1+2}",
              "{L1}+{L1+2}->B");

      Assert.That(U8("B"), Is.EqualTo(15));
    }

    [Test]
    public void TestReadSignedByte() {
      Execute("[FEFD]->Str1",
              "int(Str1)->A",
              "int(Str1+1}->B");

      Assert.That(S16("A"), Is.EqualTo(-2));
      Assert.That(S16("B"), Is.EqualTo(-3));
    }
  }
}
