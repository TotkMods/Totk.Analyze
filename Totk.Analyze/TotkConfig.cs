﻿using System.Text.Json;

namespace Totk.Analyze;

public class TotkConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Totk", "config.json");

    public required string GamePath { get; set; }

    public static TotkConfig Load()
    {
        using FileStream fs = File.OpenRead(_path);
        return JsonSerializer.Deserialize<TotkConfig>(fs) ?? Create();
    }

    public void Save()
    {
        FileStream fs = File.Create(_path); 
        JsonSerializer.Serialize(fs, this);
    }

    private static TotkConfig Create()
    {
        TotkConfig config = new() {
            GamePath = string.Empty
        };

        config.Save();
        return config;
    }
}
