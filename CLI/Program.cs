using System;
using System.Collections.Generic;
using System.IO;
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

            GeneratePackages().Wait();
        }

        private static async Task GeneratePackages()
        {
            var config = BundlerConfiguration.FromJson(File.ReadAllText(cliParams.Config));

            foreach (var package in config.Packages)
            {
                var packageConfig = Configuration.FromJson(package.ToString());
//                await new ZipFileGenerator(packageConfig).WriteFileAsync();
                new ZipFileGenerator(packageConfig).WriteFile();
            }
        }
    }
}
