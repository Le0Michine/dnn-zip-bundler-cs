using System.IO;
using NUnit.Framework;
using Zipper;
using Zipper.ConfigurationSchema;

namespace ZipperTest
{
    [TestFixture]
    public class Tests
    {
        private const string TestFilesPath = "TestFiles";
        [Test]
        [TestCase("test1.zip.json", "output/out1.zip")]
        [TestCase("test2.zip.json", "output/out2.zip")]
        [TestCase("test3.zip.json", "output/out3.zip")]
        public void Test1(string testConfig, string outputFile)
        {
            var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesPath, testConfig);
            var config = Configuration.FromJson(File.ReadAllText(configPath));
            var zipper = new ZipFileGenerator(config, Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesPath));
            zipper.WriteFile();
            var outputFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFilesPath, outputFile);
            Assert.True(File.Exists(outputFilePath), $"Unable to find generated file {outputFilePath}");
        }
    }
}