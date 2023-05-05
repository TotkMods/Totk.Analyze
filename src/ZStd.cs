using SarcLibrary;
using ZstdSharp;

namespace Totk.Analyze;

public class ZStd
{
    private enum DecompressorMode
    {
        Common,
        Pack,
        Map
    }

    private static readonly string _zsDicPath = Path.Combine(Global.Config.GamePath, "Pack", "ZsDic.pack.zs");
    private static readonly Decompressor _commonDecompressor = LoadDicts(DecompressorMode.Common);
    private static readonly Decompressor _mapDecompressor = LoadDicts(DecompressorMode.Map);
    private static readonly Decompressor _packDecompressor = LoadDicts(DecompressorMode.Pack);

    public static void Decompress(string file)
    {
        Span<byte> src = File.ReadAllBytes(file);
        Span<byte> data =
            file.EndsWith(".bcett.byml.zs") ? _mapDecompressor.Unwrap(src) :
            file.EndsWith(".pack.zs") ? _packDecompressor.Unwrap(src) :
            _commonDecompressor.Unwrap(src);

        FileStream fs = File.Create(file.Remove(file.Length - 3, 3));
        fs.Write(data);
    }

    private static Decompressor LoadDicts(DecompressorMode mode)
    {
        Decompressor decompressor = new();

        Span<byte> data = decompressor.Unwrap(File.ReadAllBytes(_zsDicPath));
        SarcFile sarc = SarcFile.FromBinary(data.ToArray()); // use cead to avoid the copy

        decompressor.LoadDictionary(sarc[mode switch {
            DecompressorMode.Map => "bcett.byml.zsdic",
            DecompressorMode.Pack => "pack.zsdic",
            _ => "zs.zsdic"
        }]);

        return decompressor;
    }
}
