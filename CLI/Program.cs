using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CliParse;
using CLI.BundlerConfigurationSchema;
using Zipper;
using Zipper.ConfigurationSchema;

namespace CLI
{
    internal class Program
    {
        private static readonly Params CliParams = new Params();
        private static readonly ConsoleSpiner Spiner = new ConsoleSpiner();

        public static void Main(string[] args)
        {
            var result = CliParams.CliParse(args);

            if (!result.Successful || result.ShowHelp)
            {
                Console.WriteLine(CliParams.GetHelpInfo());
                return;
            }

            Console.Write("Start packaging...");
            var task = Spiner.Start();

            var sw = Stopwatch.StartNew();
            GeneratePackages();
            Spiner.Stop();
            Console.WriteLine($" \nPackaging finished, elapsed time: {sw.Elapsed}");
        }

        private static void GeneratePackages()
        {
            var config = BundlerConfiguration.FromJson(File.ReadAllText(CliParams.Config));

            Task.WaitAll(config.Packages
                .Select(package => new ZipFileGenerator(Configuration.FromJson(package.ToString())).WriteFileAsync())
                .ToArray()
            );
        }
    }
}
