using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TheIdleScrolls_Core.Utility
{
    public class ResourceAccess
    {
        public static T ParseResourceFile<T>(string moduleName, string resource)
        {
            return ParseResourceFile<T>(moduleName + ".Resources." + resource);
        }

        public static T ParseResourceFile<T>(string resourcePath)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)
                ?? throw new Exception($"Resource {resourcePath} not found");
            var jsonString = new StreamReader(stream).ReadToEnd();
            return JsonSerializer.Deserialize<T>(jsonString)
                ?? throw new Exception($"Failed to deserialize resource at {resourcePath}");
        }
    }
}
