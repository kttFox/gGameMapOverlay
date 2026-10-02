using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using gGameMapOverlay.Update;

namespace gGameMapOverlay.Tests;

public class VersionOrderTests
{
    [Theory]
    [InlineData("1.0.1", "1.0.0", true)]
    [InlineData("1.10.0", "1.9.9", true)]
    [InlineData("1.0", "1.0.0", false)]
    [InlineData("1.0.0", "1.0.0+abc", false)]
    [InlineData("2026.09.30.2", "2026.09.30.1", true)]
    [InlineData("2026.09.30.1", "2026.10.01.1", false)]
    [InlineData("2026.09.30.1", "0", true)]
    public void IsNewer_ComparesNumericParts(string latest, string current, bool expected) =>
        Assert.Equal(expected, VersionOrder.IsNewer(latest, current));
}

public sealed class UpdaterTests : IDisposable
{
    private readonly string work = Path.Combine(Path.GetTempPath(), "gGameMapOverlayTests", Guid.NewGuid().ToString("N"));
    private readonly string root;
    private readonly string server;

    public UpdaterTests()
    {
        root = Directory.CreateDirectory(Path.Combine(work, "app")).FullName;
        server = Directory.CreateDirectory(Path.Combine(work, "server")).FullName;
        Directory.CreateDirectory(Path.Combine(root, "data"));
        File.WriteAllText(Path.Combine(root, "data", "maps.json"), """{ "maps": [] }""");
        File.WriteAllText(Path.Combine(root, "data", "old-only.json"), "{}");
        File.WriteAllText(Path.Combine(root, "data", Updater.DataVersionFileName), "2026.01.01.1");
        File.WriteAllText(Path.Combine(root, "App.exe"), "old exe");
        File.WriteAllText(Path.Combine(root, "config.json"), "user config");
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(work, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    private Updater NewUpdater() => new(root, "App.exe");

    private Uri ManifestUri => new(Path.Combine(server, "manifest.json"));

    /// <summary>files (zip 内のパス → 中身) の zip を server に置き、その項目を返す。</summary>
    private UpdatePackage Publish(string name, string version, Dictionary<string, string> files, string? sha256 = null)
    {
        var path = Path.Combine(server, name);
        using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            foreach (var (entry, content) in files)
            {
                using var writer = new StreamWriter(archive.CreateEntry(entry).Open());
                writer.Write(content);
            }
        }
        var bytes = File.ReadAllBytes(path);
        return new UpdatePackage { Version = version, Url = name, Sha256 = sha256 ?? Convert.ToHexStringLower(SHA256.HashData(bytes)), Size = bytes.Length };
    }

    private void WriteManifest(UpdatePackage? app, UpdatePackage? data) =>
        File.WriteAllText(Path.Combine(server, "manifest.json"), JsonSerializer.Serialize(new UpdateManifest { Schema = 1, App = app, Data = data }));

    [Fact]
    public async Task Check_FindsOnlyNewerComponents()
    {
        WriteManifest(
            Publish("app.zip", "0.0.1", new() { ["App.exe"] = "x" }),
            Publish("data.zip", "2026.09.30.1", new() { ["maps.json"] = """{ "maps": [] }""" }));
        var updates = await NewUpdater().CheckAsync(ManifestUri);
        var update = Assert.Single(updates);
        Assert.Equal(UpdateComponent.Data, update.Component);
        Assert.Equal("2026.01.01.1", update.CurrentVersion);
    }

    [Fact]
    public async Task ApplyData_ReplacesWholeFolderAndWritesVersion()
    {
        WriteManifest(null, Publish("data.zip", "2026.09.30.1", new() { ["maps.json"] = """{ "maps": [{ "id": "1", "names": {} }] }""" }));
        var updater = NewUpdater();
        await updater.ApplyAsync(Assert.Single(await updater.CheckAsync(ManifestUri)));

        Assert.Contains("\"id\": \"1\"", File.ReadAllText(Path.Combine(root, "data", "maps.json")));
        Assert.False(File.Exists(Path.Combine(root, "data", "old-only.json")));
        Assert.Equal("2026.09.30.1", updater.DataVersion);
        Assert.False(Directory.Exists(Path.Combine(updater.WorkDirectory, "data.old")));
        Assert.Empty(await updater.CheckAsync(ManifestUri));
    }

    [Fact]
    public async Task Apply_WithWrongHash_KeepsCurrentFiles()
    {
        WriteManifest(null, Publish("data.zip", "2026.09.30.1", new() { ["maps.json"] = """{ "maps": [] }""" }, sha256: new string('0', 64)));
        var updater = NewUpdater();
        var update = Assert.Single(await updater.CheckAsync(ManifestUri));

        await Assert.ThrowsAsync<InvalidDataException>(() => updater.ApplyAsync(update));
        Assert.Equal("2026.01.01.1", updater.DataVersion);
        Assert.True(File.Exists(Path.Combine(root, "data", "old-only.json")));
    }

    [Fact]
    public async Task ApplyData_WithUnreadableData_KeepsCurrentFiles()
    {
        WriteManifest(null, Publish("data.zip", "2026.09.30.1", new() { ["special.json"] = "{}" })); // maps.json がない
        var updater = NewUpdater();
        var update = Assert.Single(await updater.CheckAsync(ManifestUri));

        await Assert.ThrowsAsync<FileNotFoundException>(() => updater.ApplyAsync(update));
        Assert.Equal("2026.01.01.1", updater.DataVersion);
    }

    [Fact]
    public async Task ApplyApp_RenamesOldFilesAndSkipsUserFiles()
    {
        WriteManifest(Publish("app.zip", "99.0.0", new()
        {
            ["App.exe"] = "new exe",
            ["lib/new.dll"] = "new dll",
            ["config.json"] = "packaged config",
            ["data/maps.json"] = "packaged data",
        }), null);
        var updater = NewUpdater();
        await updater.ApplyAsync(Assert.Single(await updater.CheckAsync(ManifestUri)));

        Assert.Equal("new exe", File.ReadAllText(Path.Combine(root, "App.exe")));
        Assert.Equal("old exe", File.ReadAllText(Path.Combine(root, "App.exe.old")));
        Assert.Equal("new dll", File.ReadAllText(Path.Combine(root, "lib", "new.dll")));
        Assert.Equal("user config", File.ReadAllText(Path.Combine(root, "config.json")));
        Assert.Equal("""{ "maps": [] }""", File.ReadAllText(Path.Combine(root, "data", "maps.json")));

        updater.CleanUp(); // 再起動後
        Assert.False(File.Exists(Path.Combine(root, "App.exe.old")));
        Assert.False(Directory.Exists(updater.WorkDirectory));
    }

    [Fact]
    public async Task ApplyApp_WithoutExecutable_Fails()
    {
        WriteManifest(Publish("app.zip", "99.0.0", new() { ["other.dll"] = "x" }), null);
        var updater = NewUpdater();
        var update = Assert.Single(await updater.CheckAsync(ManifestUri));

        await Assert.ThrowsAsync<InvalidDataException>(() => updater.ApplyAsync(update));
        Assert.False(File.Exists(Path.Combine(root, "other.dll")));
    }

    [Fact]
    public void RecoverInterrupted_RestoresBackupsAndRemovesNewFiles()
    {
        // App.exe は改名して新しいものを置いた後、new.dll は置いた後に止まった
        File.Move(Path.Combine(root, "App.exe"), Path.Combine(root, "App.exe.old"));
        File.WriteAllText(Path.Combine(root, "App.exe"), "new exe");
        File.WriteAllText(Path.Combine(root, "new.dll"), "new dll");
        var updater = NewUpdater();
        Directory.CreateDirectory(updater.WorkDirectory);
        File.WriteAllText(Path.Combine(updater.WorkDirectory, "journal.json"), JsonSerializer.Serialize(new
        {
            Directory = false,
            Entries = new object[]
            {
                new { Target = Path.Combine(root, "App.exe"), Backup = Path.Combine(root, "App.exe.old") },
                new { Target = Path.Combine(root, "new.dll"), Backup = (string?)null },
            },
        }));

        Assert.True(updater.RecoverInterrupted());
        Assert.Equal("old exe", File.ReadAllText(Path.Combine(root, "App.exe")));
        Assert.False(File.Exists(Path.Combine(root, "App.exe.old")));
        Assert.False(File.Exists(Path.Combine(root, "new.dll")));
        Assert.False(updater.RecoverInterrupted());
    }

    [Fact]
    public void ResolveUri_RejectsHttp()
    {
        Assert.Throws<InvalidDataException>(() => Updater.ResolveUri(new Uri("http://example.com/manifest.json"), "app.zip"));
        Assert.Equal("https://example.com/r/app.zip", Updater.ResolveUri(new Uri("https://example.com/r/manifest.json"), "app.zip").ToString());
    }

    [Fact]
    public void Manifest_RejectsBadHash() =>
        Assert.Throws<InvalidDataException>(() => UpdateManifest.Parse("""
            { "schema": 1, "app": { "version": "1.0.0", "url": "a.zip", "sha256": "xyz", "size": 1 } }
            """));
}
