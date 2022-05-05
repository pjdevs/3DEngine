using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace gl
{
    public class Program
    {
        public static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 800),
                Title = "OpenTK Engine",
                Flags = ContextFlags.ForwardCompatible, // needed for MacOS
            };

            using var window = new GLWindow(GameWindowSettings.Default, nativeWindowSettings);

            window.Run();
        }
    }
}
