using System.IO;
using System.Text.RegularExpressions;

namespace ManifestEditor
{
    public static class ManifestVersionReplacer
    {
        private const string VersionGroupName = "version";
        private const string LeftPartGroupName = "left";
        private const string RightPartGroupName = "right";
        private static readonly Regex Dnn6VersionRegex = new Regex("(?<left><version>)(?<version>\\d*?\\.\\d*?\\.\\d*)(?<right></version>)");
        private static readonly Regex Dnn7VersionRegex = new Regex("(?<left><package .*? version=\")(?<version>\\d*?\\.\\d*?\\.\\d*)(?<right>.*?>)");

        public static PackageVersion GetVersionFromManifest(string manifestPath)
        {
            if (!File.Exists(manifestPath))
            {
                return null;
            }
            var manifestContent = File.ReadAllText(manifestPath);
            var dnn6Match = Dnn6VersionRegex.Match(manifestContent);
            var dnn7Match = Dnn7VersionRegex.Match(manifestContent);
            return new PackageVersion(dnn6Match.Success ? dnn6Match.Groups[VersionGroupName].Value : dnn7Match.Groups[VersionGroupName].Value);
        }

        public static void ReplaceVersion(string manifestPath, PackageVersion newVersion, string manifestOutputPath = null)
        {
            if (!File.Exists(manifestPath))
            {
                return;
            }

            var manifestContent = File.ReadAllText(manifestPath);
            var updatedManifestContent = ReplaceVersionGroup(ReplaceVersionGroup(manifestContent, newVersion.ToString(), Dnn6VersionRegex), newVersion.ToString(), Dnn7VersionRegex);

            var resultDirectory = Path.GetDirectoryName(manifestOutputPath ?? manifestPath);
            if (!string.IsNullOrEmpty(resultDirectory) && !Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            File.WriteAllText(manifestOutputPath ?? manifestPath, updatedManifestContent);
        }

        private static string ReplaceVersionGroup(string str, string replaceWith, Regex regex)
        {
            return regex.Replace(str, m => m.Groups[LeftPartGroupName] + replaceWith + m.Groups[RightPartGroupName].Value);
        }
    }
}