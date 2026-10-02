using System.Runtime.InteropServices;

namespace gGameMapOverlay.Native;

/// <summary>
/// 移動キー (WASD と矢印キー) の押し下げ・離しを見る。グローバルな低レベルキーボードフックを使うが、移動キーだけを見て、他のキーは記録しない。
/// フックはインストールしたスレッド (UI スレッド) のメッセージループで呼ばれるので、Changed も UI スレッドで起きる。
/// </summary>
internal sealed class MoveKeyWatcher : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    /// <summary>キー (W A S D) と、押されているキーの組み合わせ (Held) のビット。</summary>
    public static readonly (int VirtualKey, char Name)[] Keys = [(0x57, 'W'), (0x41, 'A'), (0x53, 'S'), (0x44, 'D')];

    // 実際に見るキーと、そのキーが表す Keys の番号。矢印キーは同じ方向の WASD と同じキーとして扱う (↑ = W, ← = A, ↓ = S, → = D)。
    private static readonly (int VirtualKey, int Index)[] PhysicalKeys =
        [.. Keys.Select((key, index) => (key.VirtualKey, index)), (0x26, 0), (0x25, 1), (0x28, 2), (0x27, 3)];

    // 押されている実際のキー (PhysicalKeys の順のビット)。W と ↑ を両方押して片方を離しても、もう片方で押されたままにする。
    private int pressed;

    private delegate nint HookProc(int code, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, HookProc callback, nint module, uint threadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(nint hook);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hook, int code, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? name);

    private readonly HookProc hookProc; // フックを解除するまで GC されないようフィールドで持つ
    private nint hook;

    public MoveKeyWatcher() => hookProc = OnKey;

    /// <summary>押されている移動キーの組み合わせ (Keys の順のビット)。</summary>
    public int Held { get; private set; }

    /// <summary>押されている移動キーの組み合わせが変わった (前の組み合わせ, 今の組み合わせ)。</summary>
    public event Action<int, int>? Changed;

    /// <summary>スキルのキー (仮想キーコード、0 ならなし)。</summary>
    public int SkillKey { get; set; }

    /// <summary>スキルのキーが押された (押しっぱなしの繰り返しでは起きない)。</summary>
    public event Action? SkillPressed;

    private bool skillHeld;

    public bool Enabled
    {
        get => hook != 0;
        set
        {
            if (value == Enabled)
            {
                return;
            }
            if (value)
            {
                hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, GetModuleHandle(null), 0);
            }
            else
            {
                UnhookWindowsHookEx(hook);
                hook = 0;
                pressed = 0;
                Held = 0;
            }
        }
    }

    /// <summary>組み合わせの名前 (例: "W", "WD")。押されていなければ空。</summary>
    public static string Name(int held) => new(Keys.Where((_, index) => (held & (1 << index)) != 0).Select(key => key.Name).ToArray());

    private nint OnKey(int code, nint wParam, nint lParam)
    {
        if (code >= 0)
        {
            var vk = Marshal.ReadInt32(lParam);
            if (SkillKey != 0 && vk == SkillKey)
            {
                var down = (int)wParam is WM_KEYDOWN or WM_SYSKEYDOWN;
                if (down && !skillHeld)
                {
                    SkillPressed?.Invoke();
                }
                skillHeld = down;
            }
            var physical = Array.FindIndex(PhysicalKeys, key => key.VirtualKey == vk);
            if (physical >= 0)
            {
                pressed = (int)wParam switch
                {
                    WM_KEYDOWN or WM_SYSKEYDOWN => pressed | (1 << physical),
                    WM_KEYUP or WM_SYSKEYUP => pressed & ~(1 << physical),
                    _ => pressed,
                };
                var held = 0;
                for (var i = 0; i < PhysicalKeys.Length; i++)
                {
                    if ((pressed & (1 << i)) != 0)
                    {
                        held |= 1 << PhysicalKeys[i].Index;
                    }
                }
                // 押しっぱなしの繰り返しでは変わらないので知らせない。
                if (held != Held)
                {
                    var previous = Held;
                    Held = held;
                    Changed?.Invoke(previous, held);
                }
            }
        }
        return CallNextHookEx(hook, code, wParam, lParam);
    }

    public void Dispose() => Enabled = false;
}
