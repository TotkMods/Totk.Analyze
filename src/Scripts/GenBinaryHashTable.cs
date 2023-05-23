using System.Buffers.Binary;
using System.Text;
using Totk.Analyze.Extensions;

namespace Totk.Analyze.Scripts;

public static class GenBinaryHashTable
{
    public static void Execute()
    {
        using FileStream fs = File.Create("D:\\Bin\\Totk\\Hashes\\HashTable.bin");
        Span<byte> dword = stackalloc byte[4];

        var hashTable = JsonExtension.LoadJson<Dictionary<uint, string>>("Hashes/HashTable.json");

        BinaryPrimitives.WriteInt32LittleEndian(dword, hashTable.Count);
        fs.Write(dword);

        foreach ((var hash, var name) in hashTable) {
            BinaryPrimitives.WriteUInt32LittleEndian(dword, hash);
            fs.Write(dword);

            byte[] data = new byte[name.Length + 1];
            data[0] = (byte)name.Length;
            Encoding.UTF8.GetBytes(name).CopyTo(data, 1);
            fs.Write(data.AsSpan());
        }

        Span<byte> buffer = new byte[fs.Length];
        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(buffer);

        using FileStream fsZs = File.Create("D:\\Bin\\Totk\\Hashes\\HashTable.bin.zs");
        fsZs.Write(TotkZstd.Compress(".bin.zs", buffer));
    }
}
