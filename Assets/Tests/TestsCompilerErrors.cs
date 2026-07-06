using NUnit.Framework;

namespace Axe4Unity {

  public class TestsCompilerErrors : TestBase {
    
    [Test]
    public void TestUsingUndefinedSymbolThrowsException() {
      Assert.That(() => Execute("Str1->A"), Throws.Exception);
    }

    [Test]
    public void TestTryingToCallUndefinedLabelThrowsException() {
      Assert.That(() => Execute("FOO(1,2,3)"), Throws.Exception);
    }

    [Test]
    public void TestTryingToUseUndefinedVariableThrowsException() {
      Assert.That(() => Execute("FOO*BAR->A"), Throws.Exception);
    }

  }
}
