using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Zipper.ConfigurationSchema;
using Zipper.Model;

namespace Zipper
{
    public class ZipFileGenerator
    {
        private readonly Configuration _configuration;
        private readonly string _basePath;

        public ZipFileGenerator(Configuration configuration, string basePath = null)
        {
            _configuration = configuration;
            _basePath = basePath;
        }

        public void WriteFile()
        {
            var zip = CreateZip(_configuration.Entries.ToList(), _configuration.IgnoreEntries.ToList());
            var zipDirectoryPath = Path.GetDirectoryName(ResolvePath(_configuration.Name));
            if (!string.IsNullOrEmpty(zipDirectoryPath) && !Directory.Exists(zipDirectoryPath))
            {
                Directory.CreateDirectory(zipDirectoryPath);
            }
            File.WriteAllBytes(ResolvePath(_configuration.Name), zip);
        }

        public async Task WriteFileAsync()
        {
            await Task.Run(() => WriteFile());
        }

        private byte[] CreateZip(List<object> entries, List<string> ignoreEntries)
        {
            var allEntries = entries.SelectMany(entry =>
            {
                var directoryOrFile = entry as string;
                if (directoryOrFile != null)
                {
                    return GetAllEntries(directoryOrFile, null, ignoreEntries);
                }
                var jObject = entry as JObject;
                if (jObject != null)
                {
                    var jsonEntry = jObject;
                    if (jsonEntry.IsFileEntry())
                    {
                        var fileEntry = (FileEntry) jsonEntry;
                        return GetAllEntries(fileEntry.Name, fileEntry.Path, ignoreEntries);
                    }
                    if (jsonEntry.IsZipEntry())
                    {
                        var zipEntry = (ZipEntry) jsonEntry;
                        return new List<IEntry>
                        {
                            new ZipEntryInfo
                            {
                                Name = zipEntry.Name,
                                Bytes = CreateZip(zipEntry.Entries.ToList(), zipEntry.IgnoreEntries.ToList())
                            }
                        };
                    }
                }
                return Enumerable.Empty<IEntry>();
            }).ToList();

            return Compress(allEntries);
        }

        private IEnumerable<IEntry> GetAllEntries(string directoryOrFile, string entryPath, List<string> ignoreEntries)
        {
            if (File.Exists(ResolvePath(directoryOrFile)))
            {
                return new List<IEntry>
                {
                    new FileEntryInfo
                    {
                        Name = entryPath == null ? directoryOrFile : Path.Combine(entryPath, Path.GetFileName(directoryOrFile)),
                        Path = entryPath,
                        Bytes = File.ReadAllBytes(ResolvePath(directoryOrFile))
                    }
                };
            }
            if (Directory.Exists(ResolvePath(directoryOrFile)))
            {
                return GetDirectoriesRecursively(directoryOrFile, entryPath, ignoreEntries);
            }
            return Enumerable.Empty<IEntry>();
        }

        private IEnumerable<IEntry> GetDirectoriesRecursively(string dir, string entryPath, List<string> ignoreEntries,
            string replacePath = null)
        {
            replacePath = replacePath ?? dir;
            var result = new List<IEntry>();
            var dirs = Directory.EnumerateDirectories(ResolvePath(dir))
                .ToList()
                .Where(x => FilterEntries(x, null, ignoreEntries))
                .ToList();
            foreach (var fileSystemDirectory in dirs)
            {
                result.AddRange(GetDirectoriesRecursively(ToRelativePath(fileSystemDirectory), entryPath, ignoreEntries,
                    replacePath));
            }
            foreach (var fileSystemFile in Directory.EnumerateFiles(ResolvePath(dir)).Where(x => FilterEntries(x, null, ignoreEntries)))
            {
                var relPath = ToRelativePath(fileSystemFile);
                var path = entryPath == null ? relPath : ReplacePath(relPath, replacePath, entryPath);
                result.Add(new FileEntryInfo
                {
                    Name = path,
                    Path = path,
                    Bytes = File.ReadAllBytes(ResolvePath(fileSystemFile))
                });
            }
            return result;
        }

        private bool FilterEntries(string entry, string entryType, IReadOnlyCollection<string> ignoreEntries)
        {
            if (ignoreEntries == null || ignoreEntries.Count == 0)
            {
                return true;
            }
            return !ignoreEntries.Any(x => entry.ToUpper().Contains(x.ToUpper()));
        }

        private byte[] Compress(IEnumerable<IEntry> entries)
        {
            byte[] result;
            using (var stream = new MemoryStream())
            {
                using(var zip = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    foreach (var entry in entries)
                    {
                        using (var writeEntryStream = zip.CreateEntry(entry.Name).Open())
                        {
                            using (var streamWriter = new BinaryWriter(writeEntryStream))
                            {
                                streamWriter.Write(entry.Bytes);
                            }
                        }
                    }
                }
                result = new byte[stream.ToArray().Length];
                stream.ToArray().CopyTo(result, 0);
            }

            return result;
        }

        private string ResolvePath(string path)
        {
            return _basePath == null ? path : Path.Combine(_basePath, path);
        }

        private string ToRelativePath(string path)
        {
            if (_basePath == null)
            {
                return path;
            }
            var relPath = path.Replace(_basePath, "");
            return Path.IsPathRooted(relPath) ? relPath.Substring(1) : relPath;
        }

        private string ReplacePath(string path, string pathToReplace, string replaceWith)
        {
            var result = path.Replace(pathToReplace, replaceWith);
            return Path.IsPathRooted(result) ? result.Substring(1) : result;
        }
    }
}