using Cead.Interop;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Totk.Analyze;
using Totk.Analyze.Components;
using Totk.Analyze.Extensions;

DllManager.LoadCead();

Stopwatch watch = Stopwatch.StartNew();
FileProcessor processor = new();

try {
#if DEBUG
    new RecursionExtension(processor.Process).IterateFolder(TotkConfig.Shared.GamePath);
#else
    await new RecursionExtension(processor.Process, parallel: false).IterateFolderAsync(TotkConfig.Shared.GamePath);
#endif
}
catch (Exception ex) {
    await Console.Out.WriteLineAsync(ex.ToString());
}
finally {
    using FileStream fsExtensions = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Totk.Analyze.Extensions.json"));
    JsonSerializer.Serialize(fsExtensions, processor.FileInfo.Keys.Order(), new JsonSerializerOptions() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });
}

watch.Stop();
await Console.Out.WriteLineAsync($"""
    Processed in {watch.ElapsedMilliseconds / 1000 / 60} minutes
    - Elapsed Milliseconds: {watch.ElapsedMilliseconds}
    - Elapsed Ticks: {watch.ElapsedMilliseconds}
    """);