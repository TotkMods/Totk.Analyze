using Cead;
using Cead.Handles;
using System.Text.Json;
using System.Text.Json.Serialization;
using Totk.Analyze.Models;

namespace Totk.Analyze.Components;

public unsafe class FileProcessor
{
    public Dictionary<string, Dictionary<string, TotkFileInfo>> FileInfo { get; } = new();
    public List<string> AllExtensions { get; } = new();

    public void Process(string path, nint? src = null, int? len = null)
    {
        string relPath = Path.GetRelativePath(TotkConfig.Shared.GamePath, path);
        Console.WriteLine($"Processing '{relPath}'");

        (string newExtPath, string ext) = GetExt(relPath);
        TotkFileInfo info = new(path, ext, src, len);
        Add(ext, relPath, info);

        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions", ext, newExtPath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        try {
            RunChecks(path, info, outputPath);
        }
        catch (Exception ex) {
            info.TestResult = ex.Message;
        }

        if (info.TestResult != null) {
            Console.WriteLine($"  {info.TestResult}");
        }

        using FileStream fs = File.Create(outputPath + ".json");
        JsonSerializer.Serialize(fs, info, new JsonSerializerOptions {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }

    public void RunChecks(string path, TotkFileInfo info, string outputPath)
    {
        if (info.Magic == "SARC") {
            using Sarc sarc = Sarc.FromBinary(info.GetData());
            foreach ((var name, var file) in sarc) {
                fixed (byte* ptr = file.AsSpan()) {
                    Process(Path.Combine(path, name), (nint)ptr, file.AsSpan().Length);
                }
            }

            using DataHandle handle = sarc.ToBinary();
            if (info.GetData().SequenceEqual(handle)) {
                info.TestResult = "SARC was byte perfect";
            }
            else {
                info.TestResult = $"SARC size diff from original: {info.GetData().Length - handle.AsSpan().Length}";

                // Append a random sarc extension
                // to fix conflicts with the folder
                using FileStream fs = File.Create(outputPath + ".bactorpack");
                fs.Write(handle);
            }
        }
        else if (info.Magic == "YB" || info.Magic == "BY" && !path.EndsWith(".esetb.byml.zs")) {
            using Byml byml = Byml.FromBinary(info.GetData());
            using DataHandle handle = byml.ToBinary(false, 7);
            if (info.GetData().SequenceEqual(handle)) {
                info.TestResult = "BYML read/write was byte perfect";
            }
            else {
                info.TestResult = $"BYML size diff from original: {info.GetData().Length - handle.AsSpan().Length}";
                using FileStream fs = File.Create(outputPath);
                fs.Write(handle);
            }
        }
    }

    private void Add(string ext, string path, TotkFileInfo info)
    {
        if (!FileInfo.TryGetValue(ext, out Dictionary<string, TotkFileInfo>? files)) {
            FileInfo[ext] = files = new();
        }

        files.Add(path, info);
    }

    private static (string, string) GetExt(string path)
    {
        if (path.EndsWith(".zs")) {
            path = path[..^3];
        }

        return (path, Path.GetExtension(path));
    }
}
