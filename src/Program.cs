using Cead.Interop;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Totk.Analyze;
using Totk.Analyze.Components;
using Totk.Analyze.Extensions;
using Totk.Analyze.Models;

const string path = "D:\\Bin\\Totk\\Json\\Totk.Analyze.Info.json";
var info = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, TotkFileInfo>>>(File.ReadAllBytes(path))!;

Dictionary<string, string> extMagic = new();

foreach ((var ext, var entries) in info) {
    List<string> found = new();
    foreach ((var _, var tInfo) in entries) {
        if (tInfo.Magic != null) {
            found.Add(tInfo.Magic);
        }
    }

    if (found.Count > 0) {
        extMagic.Add(ext, string.Join(',', found.Distinct().Order()));
    }
}

using FileStream fs = File.Create("D:\\Bin\\Totk\\Json\\Extensions-Magic.json");
JsonSerializer.Serialize(fs, extMagic, new JsonSerializerOptions {
    WriteIndented = true
});

//DllManager.LoadCead();

//Stopwatch watch = Stopwatch.StartNew();
//FileProcessor processor = new();

//try {
//#if DEBUG
//    new RecursionExtension(processor.Process).IterateFolder(TotkConfig.Shared.GamePath);
//#else
//    await new RecursionExtension(processor.Process, parallel: false).IterateFolderAsync(TotkConfig.Shared.GamePath);
//#endif
//}
//catch (Exception ex) {
//    await Console.Out.WriteLineAsync(ex.ToString());
//}
//finally {
//    using FileStream fsExtensions = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Totk.Analyze.Extensions.json"));
//    JsonSerializer.Serialize(fsExtensions, processor.FileInfo.Keys.Order(), new JsonSerializerOptions() {
//        WriteIndented = true,
//        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
//    });
//}

//watch.Stop();
//await Console.Out.WriteLineAsync($"""
//    Processed in {watch.ElapsedMilliseconds / 1000 / 60} minutes
//    - Elapsed Milliseconds: {watch.ElapsedMilliseconds}
//    - Elapsed Ticks: {watch.ElapsedMilliseconds}
//    """);