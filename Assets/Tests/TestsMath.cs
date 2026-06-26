using NUnit.Framework;

namespace Axe4Unity {

  public class TestMath : TestBase {

    [Test]
    public void TestAddition() {
      Execute("10->A",
              "15->B",
              "A+B+10+B+1+0->C");

      Assert.That(U16("C"), Is.EqualTo(51));
    }

    [Test]
    public void TestDivision() {
      Execute("10/2->A",
              "50/5->B",
              "2560/10->C",
              "10/3->D");

      Assert.That(U16("A"), Is.EqualTo(5));
      Assert.That(U16("B"), Is.EqualTo(10));
      Assert.That(U16("C"), Is.EqualTo(256));
      Assert.That(U16("D"), Is.EqualTo(3));
    }

    [Test]
    public void TestSignedDivision() {
      Execute("~10//2->A",
              "50//~5->B",
              "~2560//~10->C");

      Assert.That(S16("A"), Is.EqualTo(-5));
      Assert.That(S16("B"), Is.EqualTo(-10));
      Assert.That(S16("C"), Is.EqualTo(256));
    }

    [Test]
    public void TestSignedDivisionRoundsDown() {
      Execute("~1//100->A",
              "10//~3->B");

      Assert.That(S16("A"), Is.EqualTo(-1));
      Assert.That(S16("B"), Is.EqualTo(-4));
    }

    [Test]
    public void TestIncrementVariables() {
      Execute("5->A",
              "900->B",
              "A++",
              "B++");

      Assert.That(U16("A"), Is.EqualTo(6));
      Assert.That(U16("B"), Is.EqualTo(901));
    }

    [Test]
    public void TestIncrementMemoryLocations() {
      Execute("^^oL4->Z",

              "5->{L1+5}",
              "900->{L2+25}^^r",
              "805->{Z}^^r",

              "{L1+5}++",
              "{L2+25}^^r++",
              "{Z}^^r++",

              "{L1+5}->A",
              "{L2+25}^^r->B",
              "{Z}^^r->C");

      Assert.That(U16("A"), Is.EqualTo(6));
      Assert.That(U16("B"), Is.EqualTo(901));
      Assert.That(U16("C"), Is.EqualTo(806));
    }

    [Test]
    public void TestSquare() {
      Execute("5->A",
              "A^^2->B");

      Assert.That(U16("B"), Is.EqualTo(25));
    }

    [Test]
    public void TestSquareFixed() {
      Execute("5.0->A",
              "A^^2^^r->B");

      Assert.That(U16("B") / 256f, Is.EqualTo(25));
    }

    [Test]
    public void TestRecip() {
      Execute("2.0->A",
              "A^^-1->A",
              "23.0->C",
              "C^^-1->C");

      Assert.That(U16("A") / 256f, Is.EqualTo(1f / 2f));
      Assert.That(U16("C") / 256f, Is.EqualTo(1f / 23f).Within(1f / 256f));
    }

  }
}
