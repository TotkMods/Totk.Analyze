using Cead.Interop;
using CsRestbl;
using Native.IO.Services;
using Totk.Analyze.Extensions;
using Totk.Analyze.Scripts;

// This should always be called
// when the application starts.
DllManager.LoadCead();
NativeLibraryManager.RegisterPath("D:\\Bin\\native", out bool isCommonLoaded)
    .Register(RestblLibrary.Shared, out bool isRestblLoaded);
JsonExtension.RegisterLocation("D:\\Bin\\Totk");

Console.WriteLine($"""
    Common Loaded: {isCommonLoaded}
    Restbl Loaded: {isRestblLoaded}
    """);

GenHashTable.Execute();