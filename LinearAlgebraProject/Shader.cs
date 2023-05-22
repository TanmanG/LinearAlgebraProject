using OpenTK.Graphics.OpenGL4;

namespace LinearAlgebraProject
{
    public class Shader : IDisposable
    {
        public int Handle;

        public Shader(string vertexPath, string fragmentPath)
        {
            // Create the vertex shader
            string VertexShaderSource = File.ReadAllText(vertexPath);
            var VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
    
            // Create the fragment shader
            string fragmentShaderSource = File.ReadAllText(fragmentPath);
            var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);

            // Compile the vertex shader
            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vertexSuccess);
            if (vertexSuccess == 0)
            {
                // Log the status to the console
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            // Compile the fragment shader
            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fragmentSuccess);
            if (fragmentSuccess == 0)
            {
                // Log the status to the console
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                // Log the status to the console
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        /// <summary>
        /// Whether this Shader has been disposed of already.
        /// </summary>
        private bool disposedValue = false;
        /// <summary>
        /// Dispose function, called on destruction.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }
        /// <summary>
        /// Finalizer- called during last step of garbage collection.
        /// </summary>
        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource Leak! Did you forget to call Dispose()?");
            }
        }
        /// <summary>
        /// Helper function to call dispose properly.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
