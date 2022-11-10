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
        int VertexBufferObject;
        float[] vertices =
        {
            -0.5f, -0.5f, 0.0f, //Bottom-left
            0.5f, -0.5f, 0.0f, //Bottom-right
            0.0f, 0.5f, 0.0f
        };

        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Size = (width, height),
                      Title = title
                  })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}