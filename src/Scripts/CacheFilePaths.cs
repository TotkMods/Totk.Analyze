using Cead;
using Totk.Analyze.Extensions;

namespace Totk.Analyze.Scripts;

public static class CacheFilePaths
{
    public static Dictionary<string, string?> FilePaths { get; } = new();

    public static void Execute()
    {
        string[] topLevelFiles = Directory.GetFiles(TotkConfig.Shared.GamePath, "*.*", SearchOption.AllDirectories);
        foreach (var path in topLevelFiles) {
            string relativePath = Path.GetRelativePath(TotkConfig.Shared.GamePath, path);
            FilePaths.Add(relativePath, null);

            if (Path.GetExtension(path.Canonical()) is ".bfarc" or ".blarc" or ".genvb" or ".pack" or ".sarc" or ".ta") {
                using Sarc sarc = Sarc.FromBinary(path.Decompress());
                foreach ((var file, _) in sarc) {
                    FilePaths.Add(Path.Combine(relativePath, file), relativePath);
                }
            }
        }

        FilePaths.OrderBy(x => x.Value ?? x.Key)
            .ToDictionary(x => x.Key, x => x.Value)
            .StashJson("FileCache.json");
    }

    public static string Canonical(this string file)
    {
        return file.EndsWith(".zs") || file.EndsWith(".mc") ? file[..(^3)] : file;
    }

    public static byte[] Decompress(this string file)
    {
        return file.EndsWith(".zs")
            ? TotkZstd.Decompress(file, File.ReadAllBytes(file)).ToArray()
            : File.ReadAllBytes(file);
    }
}
