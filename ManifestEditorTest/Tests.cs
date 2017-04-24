using System;
using System.IO;
using ManifestEditor;
using NUnit.Framework;

namespace ManifestEditorTest
{
    [TestFixture]
    public class Tests
    {
        private string ResolvePathToTestFile(string file)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestFiles", file);
        }

        [Test]
        [TestCase("TestManifest.dnn6")]
        [TestCase("TestManifest.dnn7")]
        public void ReadVersionFromManifest(string manifestFile)
        {
            string pathToManifest = ResolvePathToTestFile(manifestFile);
            Assert.AreEqual("2017.09.0002", ManifestVersionReplacer.GetVersionFromManifest(pathToManifest).ToString());
        }

        [Test]
        [TestCase("TestManifest.dnn6")]
        [TestCase("TestManifest.dnn7")]
        public void ReplaceVersionInManifest(string manifestFile)
        {
            string pathToManifest = ResolvePathToTestFile(manifestFile);
            string pathToUpdatedManifest = ResolvePathToTestFile(Path.Combine("output", manifestFile));
            ManifestVersionReplacer.ReplaceVersion(pathToManifest, new PackageVersion("2018.01.0005"), pathToUpdatedManifest);

            Assert.AreEqual("2018.01.0005", ManifestVersionReplacer.GetVersionFromManifest(pathToUpdatedManifest).ToString());
            StringAssert.Contains("2017.09.0002.SqlDataProvider", File.ReadAllText(pathToUpdatedManifest));
        }
    }
}