namespace Totk.Analyze;

public static class Global
{
    public static TotkConfig Config { get; } = TotkConfig.Load();
}
