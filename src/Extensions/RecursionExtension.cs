namespace Totk.Analyze.Extensions;

public class RecursionExtension
{
    private readonly Action<string, nint?, int?>? _process;
    private readonly bool _parallel;

    public RecursionExtension(Action<string, nint?, int?> process, bool parallel = true)
    {
        _process = process;
        _parallel = parallel;
    }

    public void IterateFolder(string path)
    {
        IterateFiles(path);
        foreach (var folder in Directory.EnumerateDirectories(path)) {
            IterateFolder(folder);
        }
    }

    public void IterateFiles(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path)) {
            _process!(file, null, null);
        }
    }

    public async Task IterateFolderAsync(string path)
    {
        await IterateFilesAsync(path);
        IEnumerable<string> dirs = Directory.EnumerateDirectories(path);
        if (_parallel) {
            await Parallel.ForEachAsync(dirs, async (folder, cancellationToken) => {
                await IterateFolderAsync(folder);
            });
        }
        else {
            foreach (var folder in dirs) {
                await IterateFolderAsync(folder);
            }
        }
    }

    public async Task IterateFilesAsync(string path)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(path);
        if (_parallel) {
            await Parallel.ForEachAsync(files, async (file, cancellationToken) => {
                await Task.Run(() => _process!(file, null, null), cancellationToken);
            });
        }
        else {
            foreach (var file in files) {
                await Task.Run(() => _process!(file, null, null));
            }
        }
    }
}
