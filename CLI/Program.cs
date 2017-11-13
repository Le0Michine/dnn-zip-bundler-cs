using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliParse;
using CLI.BundlerConfigurationSchema;
using ManifestEditor;
using Zipper;
using Zipper.ConfigurationSchema;

namespace CLI
{
    internal static class Program
    {
        private static readonly Params CliParams = new Params();
        private static readonly ConsoleSpiner Spiner = new ConsoleSpiner();
        private static string workingDirectory = Directory.GetCurrentDirectory();

        public static void Main(string[] args)
        {
            var result = CliParams.CliParse(args);

            if (!result.Successful || result.ShowHelp)
            {
                Console.WriteLine(CliParams.GetHelpInfo());
                return;
            }

            if (!File.Exists(CliParams.Config))
            {
                Console.WriteLine($"Unable to find config file: {CliParams.Config}");
                return;
            }

            var wd = Path.GetDirectoryName(Path.GetFullPath(CliParams.Config));
            Console.WriteLine($"Working directory: {wd}");
            Directory.SetCurrentDirectory(wd);

            var config = BundlerConfiguration.FromJson(File.ReadAllText(CliParams.Config));

            config.Manifests.ToList().ForEach(manifest => {
                if (!File.Exists(manifest)) {
                    Console.WriteLine($"Unable to find manifest file: {manifest}");
                }
            });

            var newPackageVersion = UpdateVersion(config);
            GeneratePackages(config, newPackageVersion);
        }

        private static PackageVersion UpdateVersion(BundlerConfiguration config)
        {
            var currentVersion = ManifestVersionReplacer.GetVersionFromManifest(config.Manifests.First());
            var newVersion = CliParams.BumpBuild
                ? currentVersion.IncrementBuild()
                : CliParams.BumpSprint
                    ? currentVersion.IncrementSprint()
                    : !string.IsNullOrWhiteSpace(CliParams.TargetVersion)
                        ? new PackageVersion(CliParams.TargetVersion)
                        : currentVersion;
            Console.WriteLine($"Current version in manifest: {currentVersion}");
            Console.WriteLine($"Target package version: {newVersion}");

            foreach (var manifest in config.Manifests)
            {
                try
                {
                    ManifestVersionReplacer.ReplaceVersion(manifest, newVersion);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to update manifest file: {manifest}, exception occurred: ${e.Message}");
                }
            }

            return newVersion;
        }

        private static void GeneratePackages(BundlerConfiguration config, PackageVersion newPackageVersion)
        {
            Console.Write("Start packaging...");
            var task = Spiner.Start();
            var sw = Stopwatch.StartNew();
            Task.WaitAll(config.Packages
                .Select(package => new ZipFileGenerator(ReplacePlaceholders(Configuration.FromJson(package.ToString()), newPackageVersion)).WriteFileAsync())
                .ToArray()
            );
            Spiner.Stop();
            Console.WriteLine($" \nPackaging finished, elapsed time: {sw.Elapsed}");
        }

        private static Configuration ReplacePlaceholders(Configuration config, PackageVersion newPackageVersion)
        {
            config.Name = config.Name.Replace("[PACKAGE_VERSION]", newPackageVersion.ToString());
            return config;
        }
    }
}
