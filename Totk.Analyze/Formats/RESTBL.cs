using System.Text;

namespace Totk.Analyze.Formats;

public class RESTBL
{
    public Dictionary<uint, uint> CrcTable = new();
    public Dictionary<string, uint> NameTable = new();

    public static RESTBL Parse(byte[] data)
    {
        using BinaryReader br = new(new MemoryStream(data));
        RESTBL restbl = new();

        byte[] magic = br.ReadBytes(6);

        uint unknown1 = br.ReadUInt32();
        uint unknown2 = br.ReadUInt32();

        uint crcCount = br.ReadUInt32();
        uint nameCount = br.ReadUInt32();

        for (int i = 0; i < crcCount; i++) {
            restbl.CrcTable.Add(br.ReadUInt32(), br.ReadUInt32());
        }

        for (int i = 0; i < nameCount; i++) {
            byte[] name = br.ReadBytes(160);
            restbl.NameTable.Add(Encoding.UTF8.GetString(name), br.ReadUInt32());
        }

        return restbl;
    }
}
