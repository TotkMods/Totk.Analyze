using System.Text.Json;

namespace Totk.Analyze.Extensions;

public static class JsonExtension
{
    private static string _path = Path.Combine(AppContext.BaseDirectory, "Stash", "Json");

    public static void StashJson<T>(this T value, string path, bool format = true)
    {
        string file = Path.Combine(_path, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);

        using FileStream fs = File.Create(file);
        JsonSerializer.Serialize(fs, value, new JsonSerializerOptions {
            WriteIndented = true
        });
    }

    public static T LoadJson<T>(this string path)
    {
        using FileStream fs = File.OpenRead(Path.Combine(_path, path));
        return JsonSerializer.Deserialize<T>(fs)!;
    }

    public static void RegisterLocation(string path)
    {
        _path = path;
    }
}
