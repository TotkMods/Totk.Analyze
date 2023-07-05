using CsRestbl;
using Totk.Analyze.Extensions;

namespace Totk.Analyze.Scripts;

public static class GenHashTable
{
    public static Dictionary<uint, string> HashTable { get; } = new();

    public static void Execute()
    {
        using Restbl restbl = Restbl.FromBinary(
            TotkConfig.GetFileBytes(Path.Combine("System", "Resource", "ResourceSizeTable.Product.111.rsizetable.zs"), decompress: true));

        var fileCache = JsonExtension.LoadJson<Dictionary<string, string?>>("FileCache.json");
        foreach ((var file, var rel) in fileCache) {
            string path = (rel != null ? Path.GetRelativePath(rel, file) : file).Canonical();
            string ext = path.GetExt();
            string entryName = (ext is ".ta" ? file : path).Replace(Path.DirectorySeparatorChar, '/');
            uint hash = Crc32.Compute(entryName);

            if (restbl.NameTable.Contains(entryName)) {
                restbl.NameTable.Remove(entryName);
            }
            else if (restbl.CrcTable.Contains(hash)) {
                restbl.CrcTable.Remove(hash);
                HashTable.Add(hash, entryName);
            }
        }

        HashTable.OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value)
            .StashJson(Path.Combine("Hashes", "HashTable.json"));

        File.WriteAllLines(Path.Combine("D:", "Bin", "Totk", "Hashes", "TotK-Strings.txt"), HashTable
            .OrderBy(x => x.Value)
            .Select(x => x.Value));

        using FileStream fs = File.Create("D:\\Bin\\Totk\\Hashes\\Unmatched.rsizetable");
        fs.Write(restbl.ToBinary());
    }

    public static string GetExt(this string path)
    {
        return Path.GetExtension(path);
    }
}
