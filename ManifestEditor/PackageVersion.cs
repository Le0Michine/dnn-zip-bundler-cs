using System.Linq;

namespace ManifestEditor
{
    public class PackageVersion
    {
        private int Year { get; set; }
        private int Sprint { get; set; }
        private int Build { get; set; }

        public PackageVersion(PackageVersion other)
        {
            Year = other.Year;
            Sprint = other.Sprint;
            Build = other.Build;
        }

        public PackageVersion(string version)
        {
            var versionParts = version.Split('.').Select(int.Parse).ToArray();
            Year = versionParts[0];
            Sprint = versionParts[1];
            Build = versionParts[2];
        }

        public PackageVersion IncrementBuild()
        {
            var result = new PackageVersion(this);
            result.Build++;
            return result;
        }

        public PackageVersion IncrementSprint()
        {
            var result = new PackageVersion(this) {Build = 1};
            result.Sprint++;
            return result;
        }

        public override string ToString()
        {
            return $"{Year:D4}.{Sprint:D2}.{Build:D4}";
        }
    }
}