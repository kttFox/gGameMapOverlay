using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using gGameMapOverlay.Overlay;

namespace gGameMapOverlay.Update;

/// <summary>更新が見つかったコンポーネント。</summary>
public sealed record AvailableUpdate(UpdateComponent Component, string CurrentVersion, UpdatePackage Package, Uri PackageUri);

/// <summary>
/// 本体とマップ情報 (data フォルダー) の自動更新。GodiNavi の updater と同じ流れで、
/// マニフェストで新しい版を調べ、ダウンロードしてサイズと SHA-256 を検証し、
/// ジャーナル (update\journal.json) に記録しながら差し替える。失敗したらジャーナルを逆にたどって元に戻す。
/// 途中で落ちた場合も、次の起動時 (RecoverInterrupted) に元に戻す。
///
/// 実行中の exe / dll は上書きできないが名前は変えられるので、本体は古いファイルを *.old に改名してから新しいファイルを置き、
/// 再起動後に *.old を消す (CleanUp)。data はフォルダーごと入れ替える。
/// </summary>
public sealed class Updater
{
    public const string DataVersionFileName = "version.txt";
    private const string OldSuffix = ".old";

    /// <summary>本体の更新で置き換えないもの (data は別に更新する。設定とモデルは利用者のもの)。</summary>
    private static readonly string[] AppExcluded = ["data", "models", "update", "config.json"];

    private readonly string root;
    private readonly string executableName;

    /// <param name="root">インストール先 (実行ファイルのフォルダー)。</param>
    /// <param name="executableName">本体の更新ファイルに必ず入っているべき実行ファイルの名前。</param>
    public Updater(string? root = null, string? executableName = null)
    {
        this.root = Path.GetFullPath(root ?? AppContext.BaseDirectory);
        this.executableName = executableName ?? Path.GetFileName(Environment.ProcessPath ?? "gGameMapOverlay.exe");
    }

    public string WorkDirectory => Path.Combine(root, "update");

    private string JournalPath => Path.Combine(WorkDirectory, "journal.json");

    public string DataDirectory => Path.Combine(root, "data");

    /// <summary>実行中の本体のバージョン (リリースではタグの版。release.yml が -p:Version で付ける)。</summary>
    public static string AppVersion =>
        (Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0").Split('+')[0];

    /// <summary>インストール済みのマップ情報のバージョン (data\version.txt)。なければ "000"。</summary>
    public string DataVersion
    {
        get
        {
            try
            {
                var text = File.ReadAllText(Path.Combine(DataDirectory, DataVersionFileName)).Trim();
                return text.Length > 0 ? text : "000";
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                return "000";
            }
        }
    }

    public string CurrentVersion(UpdateComponent component) => component == UpdateComponent.App ? AppVersion : DataVersion;

    // ---- 確認 --------------------------------------------------------------

    /// <summary>マニフェストを取得し、今より新しいコンポーネントを返す (App, Data の順)。</summary>
    public async Task<IReadOnlyList<AvailableUpdate>> CheckAsync(Uri manifestUri, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream();
        await DownloadAsync(manifestUri, stream, UpdateManifest.MaxBytes, cancellationToken);
        var manifest = UpdateManifest.Parse(System.Text.Encoding.UTF8.GetString(stream.ToArray()));
        return FindUpdates(manifest, manifestUri);
    }

    public IReadOnlyList<AvailableUpdate> FindUpdates(UpdateManifest manifest, Uri manifestUri)
    {
        var updates = new List<AvailableUpdate>();
        foreach (var component in Enum.GetValues<UpdateComponent>())
        {
            var current = CurrentVersion(component);
            if (manifest.Get(component) is { } package && VersionOrder.IsNewer(package.Version, current))
            {
                updates.Add(new(component, current, package, ResolveUri(manifestUri, package.Url)));
            }
        }
        return updates;
    }

    /// <summary>url をマニフェストの位置から解決する。https と file (配布前の確認用) だけ許す。</summary>
    public static Uri ResolveUri(Uri manifestUri, string url)
    {
        var uri = new Uri(manifestUri, url);
        if (uri.Scheme != Uri.UriSchemeHttps && !uri.IsFile)
        {
            throw new InvalidDataException($"https 以外の URL からは更新しません ({uri})");
        }
        return uri;
    }

    // ---- 適用 --------------------------------------------------------------

    /// <summary>
    /// ダウンロード・検証・差し替えを行う。失敗したら元に戻して例外を投げる。
    /// 本体の場合は、成功したら RestartAfterAppUpdate で再起動する。
    /// </summary>
    public async Task ApplyAsync(AvailableUpdate update, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        RecoverInterrupted();
        Directory.CreateDirectory(WorkDirectory);
        var name = update.Component == UpdateComponent.App ? "本体" : "マップ情報";
        var package = Path.Combine(WorkDirectory, $"{update.Component.ToString().ToLowerInvariant()}.zip");
        var staging = Path.Combine(WorkDirectory, $"staging-{update.Component.ToString().ToLowerInvariant()}");
        try
        {
            progress?.Report($"{name} {update.Package.Version} をダウンロード中… ({update.Package.Size / 1024.0 / 1024.0:0.0}MB)");
            await using (var file = File.Create(package))
            {
                await DownloadAsync(update.PackageUri, file, update.Package.Size, cancellationToken);
            }
            await VerifyAsync(package, update.Package, cancellationToken);

            progress?.Report($"{name} を展開中…");
            DeleteDirectory(staging);
            ExtractSafely(package, staging);
            if (update.Component == UpdateComponent.App)
            {
                if (!File.Exists(Path.Combine(staging, executableName)))
                {
                    throw new InvalidDataException("更新ファイルに実行ファイルがありません。");
                }
                progress?.Report($"{name} を更新中…");
                InstallApp(staging);
            }
            else
            {
                File.WriteAllText(Path.Combine(staging, DataVersionFileName), update.Package.Version);
                OverlayData.Load(staging); // 読めないデータには入れ替えない
                progress?.Report($"{name} を更新中…");
                InstallData(staging);
            }
        }
        finally
        {
            TryDelete(package);
            DeleteDirectory(staging);
        }
    }

    /// <summary>本体の更新後に新しい版を起動する。呼んだ側はすぐに終了すること (新しい版は古い版の終了を待つ)。</summary>
    public static void RestartAfterAppUpdate()
    {
        var path = Environment.ProcessPath ?? throw new InvalidOperationException("実行ファイルの場所が分かりません。");
        var info = new ProcessStartInfo(path) { UseShellExecute = false };
        info.ArgumentList.Add("--updated");
        foreach (var argument in Environment.GetCommandLineArgs().Skip(1).Where(argument => argument != "--updated"))
        {
            info.ArgumentList.Add(argument);
        }
        Process.Start(info);
    }

    private void InstallApp(string staging)
    {
        var operations = Directory.EnumerateFiles(staging, "*", SearchOption.AllDirectories)
            .Select(source => Path.GetRelativePath(staging, source))
            .Where(relative => !IsAppExcluded(relative))
            .Select(relative =>
            {
                var target = Path.Combine(root, relative);
                return new Operation(Path.Combine(staging, relative), target, File.Exists(target) ? target + OldSuffix : null);
            })
            .ToList();
        Install(operations, directory: false);
    }

    private void InstallData(string staging)
    {
        var backup = Directory.Exists(DataDirectory) ? Path.Combine(WorkDirectory, "data" + OldSuffix) : null;
        Install([new Operation(staging, DataDirectory, backup)], directory: true);
        if (backup is not null)
        {
            DeleteDirectory(backup);
        }
    }

    /// <summary>
    /// 先にジャーナルを書いてから、各ファイル (フォルダー) を「今のものを Backup に改名 → 新しいものを置く」で差し替える。
    /// 失敗したら Rollback。成功したらジャーナルを消す (本体の *.old は実行中なので残し、次の起動で消す)。
    /// </summary>
    private void Install(IReadOnlyList<Operation> operations, bool directory)
    {
        // 前回の残り (*.old) があると、元に戻すときに古いほうを戻してしまうので先に消す (消せなければ何もせずに止める)。
        foreach (var operation in operations)
        {
            if (operation.Backup is { } backup)
            {
                DeleteEntry(backup, directory);
            }
        }
        var journal = new Journal(directory, operations.Select(operation => new JournalEntry(operation.Target, operation.Backup)).ToList());
        File.WriteAllText(JournalPath, JsonSerializer.Serialize(journal));
        try
        {
            foreach (var operation in operations)
            {
                if (operation.Backup is { } backup)
                {
                    Move(operation.Target, backup, directory);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(operation.Target)!);
                Move(operation.Source, operation.Target, directory);
            }
        }
        catch
        {
            Rollback(journal);
            throw;
        }
        File.Delete(JournalPath);
    }

    /// <summary>前回の更新が途中で止まっていたら (ジャーナルが残っていたら) 元に戻す。起動時に呼ぶ。</summary>
    public bool RecoverInterrupted()
    {
        if (!File.Exists(JournalPath))
        {
            return false;
        }
        var journal = JsonSerializer.Deserialize<Journal>(File.ReadAllText(JournalPath));
        if (journal is not null)
        {
            Rollback(journal);
        }
        File.Delete(JournalPath);
        return true;
    }

    /// <summary>
    /// 状態から判断して元に戻すので、どこで止まっていても、何度呼んでもよい。
    /// Backup があれば (改名済み) 置いたものを消して戻す。Backup がない項目 (新しく増えたもの) は置いたものを消す。
    /// </summary>
    private static void Rollback(Journal journal)
    {
        foreach (var entry in Enumerable.Reverse(journal.Entries))
        {
            if (entry.Backup is { } backup)
            {
                if (Exists(backup, journal.Directory))
                {
                    DeleteEntry(entry.Target, journal.Directory);
                    Move(backup, entry.Target, journal.Directory);
                }
            }
            else
            {
                DeleteEntry(entry.Target, journal.Directory);
            }
        }
    }

    /// <summary>本体の更新で残った *.old と作業フォルダーを消す。起動時に呼ぶ (消せないものは次回)。</summary>
    public void CleanUp()
    {
        if (File.Exists(JournalPath))
        {
            return;
        }
        foreach (var old in Directory.EnumerateFiles(root, "*" + OldSuffix, SearchOption.AllDirectories)
            .Where(path => !IsAppExcluded(Path.GetRelativePath(root, path))))
        {
            TryDelete(old);
        }
        try
        {
            DeleteDirectory(WorkDirectory);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // 次の起動で消す
        }
    }

    private static bool IsAppExcluded(string relative)
    {
        var first = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
        return AppExcluded.Contains(first, StringComparer.OrdinalIgnoreCase);
    }

    // ---- ダウンロードと検証 ------------------------------------------------------

    private static async Task DownloadAsync(Uri uri, Stream destination, long maxBytes, CancellationToken cancellationToken)
    {
        Stream source;
        HttpClient? client = null;
        if (uri.IsFile)
        {
            source = File.OpenRead(uri.LocalPath);
        }
        else
        {
            client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"gGameMapOverlay/{AppVersion}");
            source = await client.GetStreamAsync(uri, cancellationToken);
        }
        try
        {
            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
            {
                total += read;
                if (total > maxBytes)
                {
                    throw new InvalidDataException($"ダウンロードしたファイルが大きすぎます ({uri})");
                }
                await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
        }
        finally
        {
            await source.DisposeAsync();
            client?.Dispose();
        }
    }

    private static async Task VerifyAsync(string path, UpdatePackage package, CancellationToken cancellationToken)
    {
        var size = new FileInfo(path).Length;
        if (size != package.Size)
        {
            throw new InvalidDataException($"ファイルのサイズが一致しません (expected {package.Size}, actual {size})");
        }
        await using var file = File.OpenRead(path);
        var actual = Convert.ToHexStringLower(await SHA256.HashDataAsync(file, cancellationToken));
        if (!actual.Equals(package.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"ファイルのハッシュが一致しません (expected {package.Sha256}, actual {actual})");
        }
    }

    /// <summary>zip を展開する。展開先の外を指す項目 (../ など) があれば展開しない。</summary>
    private static void ExtractSafely(string package, string destination)
    {
        var fullDestination = Path.GetFullPath(destination) + Path.DirectorySeparatorChar;
        using var archive = ZipFile.OpenRead(package);
        foreach (var entry in archive.Entries)
        {
            var target = Path.GetFullPath(Path.Combine(fullDestination, entry.FullName));
            if (!target.StartsWith(fullDestination, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"更新ファイルに不正なパスがあります ({entry.FullName})");
            }
        }
        ZipFile.ExtractToDirectory(package, destination);
    }

    // ---- ファイル操作 ----------------------------------------------------------

    private static bool Exists(string path, bool directory) => directory ? Directory.Exists(path) : File.Exists(path);

    private static void Move(string source, string destination, bool directory)
    {
        if (directory)
        {
            Directory.Move(source, destination);
        }
        else
        {
            File.Move(source, destination);
        }
    }

    private static void DeleteEntry(string path, bool directory)
    {
        if (directory)
        {
            DeleteDirectory(path);
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // 実行中の古い版がまだ終わっていないなど。次の起動で消す
        }
    }

    private sealed record Operation(string Source, string Target, string? Backup);

    private sealed record JournalEntry(string Target, string? Backup);

    private sealed record Journal(bool Directory, List<JournalEntry> Entries);
}
