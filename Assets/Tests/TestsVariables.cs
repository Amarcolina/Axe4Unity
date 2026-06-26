using NUnit.Framework;

namespace Axe4Unity {

  public class TestsVariables : TestBase {

    [Test]
    public void TestVariableCanBeStoredTo() {
      Execute("123->A");

      Assert.That(U16("A"), Is.EqualTo(123));
    }

    [Test]
    public void TestVariableCanBeReadFrom() {
      Execute("123->A",
              "A->B");

      Assert.That(U16("B"), Is.EqualTo(123));
    }

    [Test]
    public void TestCanReAllocateVariables() {
      Execute("2->A",
              "real(L4)",
              "5->A",
              "real()",
              "{L4}->B",
              "A->C");

      Assert.That(U16("B"), Is.EqualTo(5));
      Assert.That(U16("C"), Is.EqualTo(2));
    }

    [Test]
    public void TestAddressOfVariablesIsCorrectAfterReAllocation() {
      Execute("real(L4)",
              "^^oB->{L1}^^r");

      Assert.That(U16("L1"), Is.EqualTo(Addr("L4") + 2));
    }

    [Test]
    public void TestCanReAllocateVariablesToStaticLocations() {
      Execute("2->A",
              "L4->Str1",
              "real(Str1)",
              "5->A",
              "real()",
              "{L4}->B",
              "A->C");

      Assert.That(U16("B"), Is.EqualTo(5));
      Assert.That(U16("C"), Is.EqualTo(2));
    }

    [Test]
    public void TestCanReAllocateVariablesMidLine() {
      Execute("real(L1)^^oA->{r1}real(L2)^^oA->{r2}");

      Assert.That(U16("{r1}"), Is.EqualTo(Addr("L1")));
      Assert.That(U16("{r2}"), Is.EqualTo(Addr("L2")));
    }
  }
}
