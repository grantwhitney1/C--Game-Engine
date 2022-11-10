using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
//using OpenTK.Windowing.GraphicsLibraryFramework;
using Shaders;
using Window;

namespace Engine
{
    public class Program
    {
        const double MAXFR = 60.0;

        public static void Main()
        {
            using Game game = new(800, 600, "LearnOpenTK");
            game.RenderFrequency = MAXFR;
            game.UpdateFrequency = MAXFR;

            game.Run();
        }
    }
}

namespace Window
{
    public class Game : GameWindow
    {
        int VertexBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;
        readonly float[] vertices =
        {
            0.5f, 0.5f, 0.0f, //top right
            0.5f, -0.5f, 0.0f, //bottom right
            -0.5f, -0.5f, 0.0f, //bottom left
            -0.5f, 0.5f, 0.0f //top left
        };

        readonly uint[] indices =
        {
            0, 1, 3, //triangle 1
            1, 2, 3 //triangle 2
        };

        Shader? shader;

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

            shader = new("../../../shader.vert", "../../../shader.frag");

            VertexArrayObject = GL.GenVertexArray();

            GL.UseProgram(VertexArrayObject);

            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            ElementBufferObject = GL.GenBuffer();

            //Element Buffer Objects rely on Vertex Array Objects
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.BufferData(BufferTarget.ArrayBuffer,
                vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            /*
             *In order to limit VRAM usage, you can delete buffer like so:
             *GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // 0 sets to null
             *GL.DeleteBuffer(VertexBufferObject);
             */

            shader.Use();
            
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            if(shader != null)
                shader.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}

namespace Shaders
{
    public class Shader : IDisposable
    {
        readonly int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            int VertexShader, FragmentShader;

            string VertexShaderSource = File.ReadAllText(vertexPath);

            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vsuccess);

            if (vsuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fsuccess);

            if (fsuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int hsuccess);

            if (hsuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public int GetAttributeLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}