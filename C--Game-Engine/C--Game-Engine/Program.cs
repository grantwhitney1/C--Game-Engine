using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shaders;
using System.ComponentModel.Design;
using System.Diagnostics;
using Window;

namespace Engine
{
    public class Program
    {
        const double MAXFR = 60.0f;

        public static void Main()
        {
            using Game game = new(800, 800, "LearnOpenTK");
            game.RenderFrequency = MAXFR;
            game.UpdateFrequency = 0.0f;
            game.AspectRatio = new(1, 1);

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
        int frames = 0;

        static int triangles = 360;
        const double TWICE_PI = 2.0f * (float)Math.PI;

        // [x0][y0][z0][r0][g0][b0]...
        float[] vertices = new float[(triangles + 1) * 6];
        // [index0][index1]...
        uint[] indices = new uint[(triangles) * 3];

        Shader? shader;

        public Game(int width, int height, string title)
            : base(GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Size = (width, height),
                      Title = title
                  })
        {
            {
                int j = 0;
                for (int i = 0; i < (triangles + 1) * 6; i++)
                {
                    switch (i % 6)
                    {
                        case 0:
                            if (i == (triangles + 1) * 6 - 6)
                                vertices[i] = 0.0f;
                            else
                                vertices[i] = (float)Math.Cos(TWICE_PI * (double)j / (double)triangles);
                            break;
                        case 1:
                            if (i == (triangles + 1) * 6 - 5)
                                vertices[i] = 0.0f;
                            else
                                vertices[i] = (float)Math.Sin(TWICE_PI * (double)j / (double)triangles);
                            break;
                        case 2:
                            vertices[i] = 1.0f;
                            if (i == (triangles + 1) * 6 - 4)
                                vertices[i] = 1.0f;
                            j++;
                            break;
                        //RGB
                        case 3:

                            if((float)i / (((float)triangles + 1.0f) * 6.0f) <= (1.0f / 3.0f))
                            {
                                vertices[i] = 1.0f - (float)i / (((float)triangles + 1.0f) * 2.0f);
                            }
                            else if ((float)i / (((float)triangles + 1.0f) * 6.0f) > (1.0f / 3.0f)
                                && (float)i / (((float)triangles + 1.0f) * 6.0f) <= (2.0f / 3.0f))
                            {
                                vertices[i] = 0.0f;
                            }
                            else if ((float)i / (((float)triangles + 1.0f) * 6.0f) > 2.0f / 3.0f)
                            {
                                vertices[i] = 0.0f + ((float)i - 2 * (((float)triangles + 1.0f) * 2.0f)) / (((float)triangles + 1.0f) * 2.0f);
                            }

                            //CENTER == WHITE
                            if (i == (triangles + 1) * 6 - 3)
                                vertices[i] = 1.0f;
                            break;
                        case 4:

                            if ((float)i / (((float)triangles + 1.0f) * 6.0f) <= (1.0f / 3.0f))
                            {
                                vertices[i] = 0.0f + (float)i / (((float)triangles + 1.0f) * 2.0f);
                            }
                            else if((float)i / (((float)triangles + 1.0f) * 6.0f) > (1.0f / 3.0f)
                                && (float)i / (((float)triangles + 1.0f) * 6.0f) <= (2.0f / 3.0f))
                            {
                                vertices[i] = 1.0f - ((float)i - (((float)triangles + 1.0f) * 2.0f)) / (((float)triangles + 1.0f) * 2.0f);
                            }
                            else if ((float)i / (((float)triangles + 1.0f) * 6.0f) > 2.0f / 3.0f)
                            {
                                vertices[i] = 0.0f;
                            }

                            //CENTER == WHITE
                            if (i == (triangles + 1) * 6 - 2)
                                vertices[i] = 1.0f;
                            break;
                        case 5:

                            if ((float)i / (((float)triangles + 1.0f) * 6.0f) <= (1.0f / 3.0f))
                            {
                                vertices[i] = 0.0f;
                            }
                            else if ((float)i / (((float)triangles + 1.0f) * 6.0f) > (1.0f / 3.0f)
                                && (float)i / (((float)triangles + 1.0f) * 6.0f) <= (2.0f / 3.0f))
                            {
                                vertices[i] = 0.0f + ((float)i - (((float)triangles + 1.0f) * 2.0f)) / (((float)triangles + 1.0f) * 2.0f);
                            }
                            else if((float)i / (((float)triangles + 1.0f) * 6.0f) > 2.0f / 3.0f)
                            {
                                vertices[i] = 1.0f - ((float)i - 2 * (((float)triangles + 1.0f) * 2.0f)) / (((float)triangles + 1.0f) * 2.0f);
                            }

                            //CENTER == WHITE
                            if (i == (triangles + 1) * 6 - 1)
                                vertices[i] = 1.0f;
                            break;
                    }
                }
            }

            {
                int i = 0;
                foreach (float x in vertices)
                {
                    Console.Write(x + " ");
                    if ((i + 1) % 3 == 0)
                        Console.WriteLine();
                    i++;
                }
            }

            /*
             * 41, 0, 1
             * 41, 1, 2,
             * 41, 2, 3,
             * ...
             * 41, 40, 0,
             * 
             */

            {
                uint j = 0;

                for (uint i = 0; i < (triangles) * 3; i++)
                {
                    switch (i % 3)
                    {
                        case 0:
                            indices[i] = (uint)triangles;
                            break;
                        case 1:
                            indices[i] = j;
                            if (j < triangles - 1)
                                j++;
                            else
                                j = 0;
                            break;
                        case 2:
                            indices[i] = j;
                            break;
                    }
                }
            }

            {
                int i = 0;
                foreach (float x in indices)
                {
                    Console.Write(x + " ");
                    if ((i + 1) % 3 == 0)
                        Console.WriteLine();
                    i++;
                }
            }

        }

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

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            //Element Buffer Objects rely on Vertex Array Objects

            /*
             *In order to limit VRAM usage, you can delete buffer like so:
             *GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // 0 sets to null
             *GL.DeleteBuffer(VertexBufferObject);
             */

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            shader.Use();

            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shader!.Dispose();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader!.Use();

            GL.BufferData(BufferTarget.ArrayBuffer,
                vertices.Length * sizeof(float), vertices,
                BufferUsageHint.DynamicDraw);

            //Code goes here

            if(frames < 180)
            {
                vertices[vertices.Length - 1] -= 1.0f / 180.0f;
                vertices[vertices.Length - 2] -= 1.0f / 180.0f;
                vertices[vertices.Length - 3] -= 1.0f / 180.0f;
            }
            else if(frames >= 180 && frames < 360)
            {
                vertices[vertices.Length - 1] += 1.0f / 180.0f;
                vertices[vertices.Length - 2] += 1.0f / 180.0f;
                vertices[vertices.Length - 3] += 1.0f / 180.0f;
            }
            else if(frames >= 360)
            {
                frames = 0;
                vertices[vertices.Length - 1] -= 1.0f / 180.0f;
                vertices[vertices.Length - 2] -= 1.0f / 180.0f;
                vertices[vertices.Length - 3] -= 1.0f / 180.0f;
            }

            for(int i = 3; i < vertices.Length - 6; i += 6)
            {
                if (i + 6 >= vertices.Length - 4)
                {
                    Console.WriteLine("Debug");
                    vertices[i] = vertices[3];
                    vertices[i + 1] = vertices[4];
                    vertices[i + 2] = vertices[5];
                }
                else
                {
                    vertices[i] = vertices[i + 6];
                    vertices[i + 1] = vertices[i + 7];
                    vertices[i + 2] = vertices[i + 8];
                }
            }

            frames++;

            GL.BindVertexArray(VertexArrayObject);

            GL.DrawElements(PrimitiveType.TriangleFan, indices.Length, DrawElementsType.UnsignedInt, 0);
            
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if(KeyboardState.IsKeyPressed(Keys.F))
            {
                if(!IsFullscreen)
                    WindowState = WindowState.Fullscreen;
                else
                {
                    WindowState = WindowState.Normal;
                    Size = new (800, 600);
                }
            }
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
        public readonly int Handle;

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
            if (!disposedValue)
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