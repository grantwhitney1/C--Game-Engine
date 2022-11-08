using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window;

namespace Engine
{
    public class Program
    {
        const double MAXFR = 60.0;

        public static void Main(string[] args)
        {
            using (Game game = new Game(800, 600, "LearnOpenTK"))
            {
                game.RenderFrequency = MAXFR;
                game.UpdateFrequency = MAXFR;

                game.Run();
            }
        }
    }
}

namespace Window
{
    public class Game : GameWindow
    {
        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Size = (width, height),
                      Title = title
                  })
        { }
    }
}