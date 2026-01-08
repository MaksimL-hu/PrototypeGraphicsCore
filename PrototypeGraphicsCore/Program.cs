using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Config;

namespace PrototypeGraphicsCore;

public static class Program
{
    public static void Main()
    {
        var native = new NativeWindowSettings
        {
            Title = AppConfig.Title,
            ClientSize = new OpenTK.Mathematics.Vector2i(AppConfig.Width, AppConfig.Height),
            APIVersion = AppConfig.GLVersion,
            Flags = ContextFlags.ForwardCompatible
        };

        using var window = new SceneWindow(GameWindowSettings.Default, native);
        window.Run();
    }
}