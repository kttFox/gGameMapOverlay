namespace gGameMapOverlay;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        // --encrypt-data <SRC_DIR> <OUT_DIR>: SRC_DIR の *.json を暗号化して OUT_DIR に *.pak を書く (Overlay.DataCipher)
        if (Array.IndexOf(args, "--encrypt-data") is var encrypt and >= 0)
        {
            EncryptData(args.Skip(encrypt + 1).Take(2).ToArray());
            return;
        }
        using var mutex = new Mutex(true, @"Local\gGameMapOverlay", out var created);
        // --updated: 本体の更新後に古い版から起動された。古い版が終わるのを待つ
        if (!created && args.Contains("--updated"))
        {
            try
            {
                created = mutex.WaitOne(TimeSpan.FromSeconds(30));
            }
            catch (AbandonedMutexException)
            {
                created = true;
            }
        }
        if (!created)
        {
            MessageBox.Show("gGame Map Reader はすでに起動しています。", "gGame Map Reader");
            return;
        }
        // 途中で止まった更新を元に戻し、前回の更新の残り (*.old) を消す
        var updater = new Update.Updater();
        try
        {
            updater.RecoverInterrupted();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            MessageBox.Show($"中断した更新を元に戻せませんでした。\n{exception.Message}", "gGame Map Reader");
        }
        updater.CleanUp();
        ApplicationConfiguration.Initialize();
        // --config <FILE>: 設定ファイルを切り替える (既定は実行ファイルと同じフォルダーの config.json)
        // --image <FILE>: 画像モード (スクリーンショットから読み取るデバッグ機能) で起動する
        // --debug: 画像モードのボタンを表示する (--image を付けた場合も表示する)
        var configPath = Option(args, "--config") is { } path ? Path.GetFullPath(path) : AppConfig.DefaultPath;
        var imagePath = Option(args, "--image") is { } image ? Path.GetFullPath(image) : null;
        Application.Run(new MainForm(AppConfig.Load(configPath), configPath, imagePath, debug: args.Contains("--debug")));
    }

    private static void EncryptData(string[] directories)
    {
        if (directories.Length < 2)
        {
            MessageBox.Show("使い方: --encrypt-data <平文 JSON のフォルダー> <出力フォルダー>", "gGame Map Reader");
            return;
        }
        var written = Overlay.DataCipher.EncryptDirectory(Path.GetFullPath(directories[0]), Path.GetFullPath(directories[1]));
        MessageBox.Show($"{written.Count} 件を暗号化しました。\n" + string.Join("\n", written.Select(Path.GetFileName)), "gGame Map Reader");
    }

    private static string? Option(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
