using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Zipper.ConfigurationSchema
{
    public partial class ZipEntry
    {
        public static explicit operator ZipEntry(JObject json)
        {
            var type = json.Property("type")?.Value.ToString();
            return type != null && type.ToUpper().Equals("ZIP") ? FromJson(json.ToString()) : null;
        }
    }

    public partial class FileEntry
    {
        public static explicit operator FileEntry(JObject json)
        {
            var type = json.Property("type")?.Value.ToString();
            return type == null || type.ToUpper().Equals("FILE") ? FromJson(json.ToString()) : null;
        }
    }

    public static class JObjectHelper
    {
        public static bool IsFileEntry(this JObject jObject)
        {
            return (FileEntry) jObject != null;
        }

        public static bool IsZipEntry(this JObject jObject)
        {
            return (ZipEntry) jObject != null;
        }
    }
}