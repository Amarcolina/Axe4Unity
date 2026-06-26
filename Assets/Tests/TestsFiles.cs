using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Axe4Unity {

  public class TestsFiles : TestBase {

    [Test]
    public void TestCanCreateFile() {
      Execute("GetCalc(\"FOO\", 10)->A");

      Assert.That(Machine.State.RamFiles.ContainsKey("FOO"));
      Assert.That(Machine.State.RamFiles["FOO"].ptr, Is.EqualTo(Machine.ADDR_FREE_RAM));
      Assert.That(Machine.State.RamFiles["FOO"].size, Is.EqualTo(10));
      Assert.That(U16("A"), Is.EqualTo(Machine.ADDR_FREE_RAM));
    }

    [Test]
    public void TestCanDeleteFile() {
      Execute("GetCalc(\"FOO\", 10)",
              "DelVar \"FOO\"");

      Assert.That(Machine.State.RamFiles.Count, Is.Zero);
    }

    [Test]
    public void TestCanCreateNonOverlappingFiles() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "GetCalc(\"BAR\", 20)->B");

      Assert.That(Machine.State.RamFiles.ContainsKey("FOO"));
      Assert.That(Machine.State.RamFiles.ContainsKey("BAR"));

      Assert.That(Machine.State.RamFiles["FOO"].ptr, Is.EqualTo(Machine.ADDR_FREE_RAM));
      Assert.That(Machine.State.RamFiles["BAR"].ptr, Is.EqualTo(Machine.ADDR_FREE_RAM + 10));
    }

    [Test]
    public void TestCanCreateNonOverlappingFilesInFragmentedSystem() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "GetCalc(\"BAR\", 20)->B",
              "DelVar \"FOO\"",
              "GetCalc(\"BAZ\", 50)->C");

      Assert.That(Machine.State.RamFiles.ContainsKey("BAR"));
      Assert.That(Machine.State.RamFiles.ContainsKey("BAZ"));

      Assert.That(Machine.State.RamFiles["BAR"].ptr, Is.EqualTo(Machine.ADDR_FREE_RAM + 10));
      Assert.That(Machine.State.RamFiles["BAZ"].ptr, Is.EqualTo(Machine.ADDR_FREE_RAM + 30));
    }

    [Test]
    public void TestCanArchiveFile() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "For(B,0,9)",
              "B->{A+B}",
              "End",
              "Archive \"FOO\"");

      Assert.That(Machine.State.RamFiles.Count, Is.Zero);
      Assert.That(Machine.State.ArchiveFiles.ContainsKey("FOO"));

      var file = Machine.State.ArchiveFiles["FOO"];
      Assert.That(file.Length, Is.EqualTo(10));

      for (int i = 0; i < 10; i++) {
        Assert.That(file[i], Is.EqualTo(i));
      }
    }

    [Test]
    public void TestCanUnArchiveFile() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "For(B,0,9)",
              "B->{A+B}",
              "End",
              "Archive \"FOO\"",
              "UnArchive \"FOO\"",
              "GetCalc(\"FOO\")->B");

      Assert.That(Machine.State.ArchiveFiles.Count, Is.Zero);
      Assert.That(Machine.State.RamFiles.ContainsKey("FOO"));

      for (int i = 0; i < 10; i++) {
        Assert.That(U8(U16("B") + i), Is.EqualTo(i));
      }
    }

    [Test]
    public void TestCanReadFromArchivedFile() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "For(B,0,9)",
              "B->{A+B}",
              "End",
              "Archive \"FOO\"",
              "GetCalc(\"FOO\", `Y0)",
              "For(A,0,9)",
              "{`Y0+A}->{L1+A}",
              "End");

      for (int i = 0; i < 10; i++) {
        Assert.That(U8(Addr("L1") + i), Is.EqualTo(i));
      }
    }

    [Test]
    public void TestCanUseFileHandlesToReadFromRam() {
      Execute("GetCalc(\"FOO\", 10)->A",
              "For(I,0,9)",
              "I->{A+I}",
              "End",
              "GetCalc(\"FOO\", `Y1)->B",
              "For(I,0,9)",
              "{`Y1+I}->{L1+I}",
              "End");

      Assert.That(U16("B"), Is.Not.Zero);

      for (int i = 0; i < 10; i++) {
        Assert.That(U8(Addr("L1") + i), Is.EqualTo(i));
      }
    }

    [Test]
    public void TestLoadingInvalidFileCreatesInvalidHandle() {
      LogAssert.Expect(LogType.Error, new Regex("Encountered error when trying to execute op"));
      Assert.That(() => Execute("GetCalc(\"FOO\", `Y1)",
                                "{`Y1}->A"),
                   Throws.InvalidOperationException);
    }
  }
}
