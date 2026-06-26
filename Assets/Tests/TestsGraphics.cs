using NUnit.Framework;

namespace Axe4Unity {

  public class TestsGraphics : TestBase {

    [Test]
    public void TestPxlOn() {
      Execute("Pxl-On(0,0)");

      Assert.That(U8("L6"), Is.EqualTo(0b10000000));
    }

    [Test]
    public void TestPxlTest() {
      Execute("Pxl-On(3,5)",
              "pxl-Test(3,5)->A",
              "pxl-Test(4,5)->B");

      Assert.That(U16("A"), Is.EqualTo(1));
      Assert.That(U16("B"), Is.EqualTo(0));
    }

    [Test]
    public void TestPtOn() {
      Execute("[FFFFFFFFFFFFFFFF]->Str1",
              "Pt-On(9,33,Str1)",
              "For(X,9,17)",
              "For(Y,33,40)",
              "If pxl-Test(X,Y)",
              "A+1->A",
              "End",
              "End",
              "End");

      Assert.That(U16("A"), Is.EqualTo(64));
    }

    [Test]
    public void TestHoriz() {
      Execute("Pxl-On(55,33)",
              "Horizontal +",
              "pxl-Test(56,33)->A");

      Assert.That(U16("A"), Is.EqualTo(1));
    }
  }
}
