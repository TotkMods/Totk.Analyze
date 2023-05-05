using Totk.Analyze;
using Totk.Analyze.Formats;

const string path = "D:\\Bin\\totk\\ResourceSizeTable.Product.100.rsizetable";
RESTBL restbl = RESTBL.Parse(File.ReadAllBytes(path));

// foreach (var item in restbl.CrcTable) {
//     Console.WriteLine(item.Key);
// }

foreach (var item in restbl.NameTable) {
    Console.WriteLine(item.Key);
}