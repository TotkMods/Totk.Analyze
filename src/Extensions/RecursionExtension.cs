namespace Totk.Analyze.Extensions;

public static class RecursionExtension
{
    public static void IterateFolder(string path, Action<string> process)
    {
        IterateFiles(path, process);
        foreach (var folder in Directory.EnumerateDirectories(path)) {
            process(folder);
        }
    }

    public static void IterateFiles(string path, Action<string> process)
    {
        foreach (var file in Directory.EnumerateFiles(path)) {
            process(file);
        }
    }

    public static async Task IterateFolderAsync(string path, Func<string, Task> process, bool parallel = true)
    {
        await IterateFilesAsync(path, process, parallel);
        IEnumerable<string> dirs = Directory.EnumerateDirectories(path);
        if (parallel) {
            await Parallel.ForEachAsync(dirs, async (folder, cancellationToken) => {
                await process(folder);
            });
        }
        else {
            foreach (var folder in dirs) {
                await process(folder);
            }
        }
    }

    public static async Task IterateFilesAsync(string path, Func<string, Task> process, bool parallel = true)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(path);
        if (parallel) {
            await Parallel.ForEachAsync(files, async (file, cancellationToken) => {
                await process(file);
            });
        }
        else {
            foreach (var file in files) {
                await process(file);
            }
        }
    }
}
