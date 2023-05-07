using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace Totk.Analyze.Models;

public unsafe class TotkFileInfo
{
    public string FilePath { get; set; }
    public string Extension { get; set; }

    public required bool IsCompressed { get; set; }
    public int? CompressedFileSize { get; set; }
    public required int FileSize { get; set; } = -1;
    public string? FourByteBinaryMagicHex { get; set; }
    public string? EightByteBinaryMagicHex { get; set; }
    public string? Magic { get; set; }
    public string? TestResult { get; set; }

    private readonly byte* data;

    [JsonConstructor]
    public TotkFileInfo() { }

    [SetsRequiredMembers]
    public TotkFileInfo(string path, string ext, nint? src = null, int? len = null)
    {
        FilePath = path;
        Extension = ext;
        IsCompressed = path.EndsWith(".zs");

        Span<byte> buffer;
        if (src is nint _src && len is int _len) {
            buffer = new((byte*)_src, _len);
        }
        else {
            using FileStream fs = File.OpenRead(path);
            fs.Read(buffer = new byte[fs.Length]);
        }

        if (buffer.Length == 0) {
            TestResult = "Zero-byte file!";
            return;
        }

        CompressedFileSize = IsCompressed ? buffer.Length : null;

        Span<byte> raw = IsCompressed ? TotkZstd.Decompress(path, buffer) : buffer;
        FileSize = raw.Length;

        StringBuilder magic = new();
        int i = -1; char x;
        while (char.IsAsciiLetter(x = (char)raw[++i])) {
            magic.Append(x);
        }

        if (magic.Length > 0) {
            Magic = magic.ToString();
        }
        else {
            if (raw.Length >= 4) {
                FourByteBinaryMagicHex = Convert.ToHexString(raw[..4]);
            }

            if (raw.Length >= 8) {
                EightByteBinaryMagicHex = Convert.ToHexString(raw[..8]);
            }
        }

        fixed (byte* ptr = raw) {
            data = ptr;
        }
    }

    public Span<byte> GetData()
    {
        return new(data, FileSize);
    }
}
