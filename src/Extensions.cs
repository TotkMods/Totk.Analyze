using System.Text.Json;
using ZstdSharp;

namespace Totk.Analyze;

public class Extensions
{
    public static void Collect(string path = "F:\\Games\\Totk\\content")
    {
        Dictionary<string, List<string>> extensions = new();

        foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)) {
            string filePath = Path.GetRelativePath(path, file);
            Console.WriteLine(filePath);

            bool isZS = file.EndsWith(".zs");
            string ext = isZS ? Path.GetExtension(Path.GetFileNameWithoutExtension(filePath)) + ".zs" : Path.GetExtension(filePath);

            if (!extensions.TryGetValue(ext, out List<string>? files)) {
                files = new();
                extensions[ext] = files;
            }

            files.Add(filePath);
        }

        using FileStream fs = File.Create(Path.Combine(Path.GetDirectoryName(path)!, "Research", "Analyze", "Extensions.ZS.json"));
        JsonSerializer.Serialize(fs, extensions.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value), new JsonSerializerOptions() {
            WriteIndented = true
        });

        using FileStream fs2 = File.Create(Path.Combine(Path.GetDirectoryName(path)!, "Research", "Analyze", "Extensions.ZS.List.json"));
        JsonSerializer.Serialize(fs2, extensions.Keys.Order(), new JsonSerializerOptions() {
            WriteIndented = true
        });
    }

    public static Dictionary<string, List<string>> Open(string path = "F:\\Games\\Totk\\content")
    {
        using FileStream fs = File.OpenRead(Path.Combine(Path.GetDirectoryName(path)!, "Research", "Analyze", "Extensions.ZS.json"));
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(fs)!;
    }

    public static void TestZS(string path)
    {
        List<string> extensions = new();
        Dictionary<string, string> extensionsFailed = new();

        foreach ((var ext, var files) in Extensions.Open().Where(x => x.Key.EndsWith(".zs"))) {
            try {
                var src = File.ReadAllBytes(Path.Combine(path, files[0]));
                using Decompressor decompressor = new();
                var decompressed = decompressor.Unwrap(src);
                extensions.Add(ext);
            }
            catch (Exception ex) {
                extensionsFailed.Add(ext, ex.Message);
                continue;
            }
        }

        using FileStream fs = File.Create(Path.Combine(Path.GetDirectoryName(path)!, "Research", "Analyze", "Extensions.ZS.Good.List.json"));
        JsonSerializer.Serialize(fs, extensions.Order(), new JsonSerializerOptions() {
            WriteIndented = true
        });

        using FileStream fs2 = File.Create(Path.Combine(Path.GetDirectoryName(path)!, "Research", "Analyze", "Extensions.ZS.Fail.List.json"));
        JsonSerializer.Serialize(fs2, extensionsFailed.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value), new JsonSerializerOptions() {
            WriteIndented = true
        });
    }
}
