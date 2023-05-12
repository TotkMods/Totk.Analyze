using System.Text.Json;
using System.Text.Json.Serialization;

namespace Totk.Analyze;

public class TotkConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Totk", "config.json");

    [JsonIgnore]
    public static TotkConfig Shared { get; } = Load();

    public required string GamePath { get; set; }

    public static byte[] GetFileBytes(string path, bool decompress)
        => decompress && path.EndsWith(".zs") ? TotkZstd.Decompress(path, File.ReadAllBytes(GetFile(path))).ToArray() : File.ReadAllBytes(GetFile(path));

    public static string GetFile(string path)
        => Path.Combine(Shared.GamePath, path);

    public static TotkConfig Load()
    {
        if (!File.Exists(_path)) {
            return Create();
        }

        using FileStream fs = File.OpenRead(_path);
        return JsonSerializer.Deserialize<TotkConfig>(fs) ?? Create();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using FileStream fs = File.Create(_path);
        JsonSerializer.Serialize(fs, this);
    }

    private static TotkConfig Create()
    {
        TotkConfig config = new() {
            GamePath = string.Empty
        };

        config.Save();
        return config;
    }
}
