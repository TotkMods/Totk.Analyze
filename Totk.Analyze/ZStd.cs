using ZstdSharp;

namespace Totk.Analyze;

public class ZStd
{
    public static void Decompress(string file)
    {
        var src = File.ReadAllBytes(file);
        using Decompressor decompressor = new();
        var decompressed = decompressor.Unwrap(src);

        string outputFile = file.Remove(file.Length - 4, 3);
        FileStream fs = File.Create(outputFile);
        fs.Write(decompressed);
    }
}
