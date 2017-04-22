using System;
using System.Collections.Generic;
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
        private static readonly Params cliParams = new Params();

        public static void Main(string[] args)
        {
            var result = cliParams.CliParse(args);

            if (!result.Successful || result.ShowHelp)
            {
                Console.WriteLine(cliParams.GetHelpInfo());
                return;
            }

            var sw = Stopwatch.StartNew();
            GeneratePackages();
            Console.Write($"Packaging finished, elapsed time: {sw.Elapsed}");
        }

        private static void GeneratePackages()
        {
            var config = BundlerConfiguration.FromJson(File.ReadAllText(cliParams.Config));
            var tasks = new List<Task>();

            Task.WaitAll(config.Packages
                .Select(package => new ZipFileGenerator(Configuration.FromJson(package.ToString())).WriteFileAsync())
                .ToArray()
            );
        }
    }
}
