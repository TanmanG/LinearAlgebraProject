using System.Numerics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LinearAlgebraProject
{
    public class Game : GameWindow
    {
        float[,] vertices = null;
        int VertexBufferObject;
        int VertexArrayObject;
        Shader shader;

        Matrix4x4 perspectiveMatrix;
        List<Vector3> vertexPositions;
        Vector2 lastPos = Vector2.Zero;

        Vector3 cameraPosition; // World coordinates of Camera Angle
        Vector3 CameraFront
        {
            get {
                return Vector3.Normalize(new(x: MathF.Cos(_yaw),
                           y: MathF.Sin(_pitch),
                           z: MathF.Sin(_yaw)));  
                }
        }
        float _yaw; // Left/Right
        float _pitch; // Up/Down
        Vector3 CameraUp
        {
            get { return Vector3.Normalize(Vector3.Cross(CameraRight, CameraFront)); }
        }
        Vector3 CameraRight
        {
            get { return Vector3.Normalize(new(x: MathF.Cos(_yaw + MathF.PI / 2), 
                                              y: 0, 
                                              z: MathF.Sin(_yaw + MathF.PI / 2))); }
        }


        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set background color
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Perspective Matrix Parameters
            float FOV = 90;
            float ASPECT_RATIO =  Size.X / Size.Y;
            float NEAR_CLIPPING_DISTANCE = 1;
            float FAR_CLIPPING_DISTANCE = 10;

            // Create the perspective matrix
            perspectiveMatrix = VertexRenderer.GetPerspectiveMatrix(aspectRatio: ASPECT_RATIO,
                                                               fov: FOV,
                                                               nearClippingDistance: NEAR_CLIPPING_DISTANCE,
                                                               farClippingDistance: FAR_CLIPPING_DISTANCE);
            // Test Vertices
            vertexPositions = new()
            {
                /* 2x2 Cube sitting at origin BLL(0,0,0), BTL(0,0,2), BLR(0,2,0), BTR(0,2,2)
                                                FLL(2,0,0), FTL(2,0,2), FLR(2,2,0), FTR(2,2,2)
                    BLL - Back Lower Left
                    BTL - Back Top Left
                    BLR - Back Lower Right
                    BTR - Back Top Right
                    FLL - Front Lower Left
                    FTL - Front Top Left
                    FLR - Front Lower Right
                    FTR - Front Top Right
                */
    
                // 2x2 Cube tranlated down by 5 along z-axis
                //new(0,0,-5), //BBL (Back Bottom Left)
                //new(2,0,-5), //BBR
                //new(0,2,-5), //BTL
                //new(2,2,-5), //BTR
                //new(0,0,-3), //FBL
                //new(2,0,-3), //FBR
                //new(0,2,-3), //FTL
                //new(2,2,-3), //FTR
                new(0,2,-5), //BTL
                new(0,0,-5), //BBL // Back Left-half of the triangle
                new(2,0,-5), //BBR

                new(2,0,-5), //BBR
                new(2,2,-5), //BTR // Back Right-half of the triangle
                new(0,2,-5), //BTL
                
                new(0,2,-3), //FTL
                new(0,0,-3), //FBL // Front Left-half of the triangle
                new(2,0,-3), //FBR
                
                new(2,0,-3), //FBR
                new(2,2,-3), //FTR // Front Right-half of the triangle
                new(0,2,-3), //FTL
                
                new(0,2,-5), //BTL
                new(0,2,-3), //FTL // Top Left-half of the triangle
                new(2,2,-3), //FTR
                
                new(2,2,-3), //FTR
                new(2,2,-5), //BTR // Top Right-half of the triangle
                new(0,2,-5), //BTL
                
                new(0,2,-5), //BTL
                new(0,0,-5), //BBL // Left Back-half of the triangle
                new(0,0,-3), //FBL
                
                new(0,0,-3), //FBL
                new(0,2,-3), //FTL // Left Top-half of the triangle
                new(0,2,-5), //BTL
                
                new(2,2,-5), //BTR
                new(2,0,-5), //BBR // Right Back-half of the triangle
                new(2,0,-3), //FBR

                new(2,0,-3), //FBR
                new(2,2,-3), //FTR // Right Front-half of the triangle
                new(2,2,-5), //BTR
                
                new(0,0,-3), //FBL
                new(0,0,-5), //BBL
                new(2,0,-5), //BBR
                
                new(0,0,-3), //FBL
                new(2,0,-3), //FBR
                new(2,0,-5), //BBR
            };

            cameraPosition = new(0, 0, 0);
            _pitch = 0;
            _yaw = -MathF.PI / 2;
            
            CursorState = CursorState.Grabbed;

            // Create a new buffer and bind it
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);


            // Create and Bind the VAO (vertex array object)
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Enable depth testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.FrontFace(FrontFaceDirection.Cw);

            // Create a new shader
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            shader.Use();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Store the vertice information, converted into NDC space
            vertices = VertexRenderer.ComputeVertexNDCs(vertices: vertexPositions,
                                                        perspectiveMatrix: perspectiveMatrix,
                                                        position: cameraPosition,
                                                        front: CameraFront,
                                                        right: CameraRight);
            
            // Push the vertices to the openGL buffer
            GL.BufferData(target: BufferTarget.ArrayBuffer,
                        size: vertices.Length * sizeof(float),
                        data: vertices,
                        usage: BufferUsageHint.DynamicDraw);


            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            // Swap out the screen buffer
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused)
                return;

            KeyboardState input = KeyboardState;

            float movespeedModifier = 1f * (float)args.Time;
            float lookspeedModifier = MathF.PI / 180 * (float)args.Time;

            if (input.IsKeyDown(Keys.LeftControl))
            {
                movespeedModifier *= 2;
            }

            if (input.IsKeyDown(Keys.Right))
            {
                _yaw += 45 * lookspeedModifier;
            }
            if (input.IsKeyDown(Keys.Left))
            {
                _yaw += -45 * lookspeedModifier;
            }
            
            
            if (input.IsKeyDown(Keys.Up))
            {
                float appliedLook = MathF.Sin(_pitch) - 45 * lookspeedModifier;
                if (appliedLook > 1)
                {
                    appliedLook = 1;
                }
                _pitch = MathF.Asin(appliedLook);
            }
            if (input.IsKeyDown(Keys.Down))
            {
                float appliedLook = MathF.Sin(_pitch) + 45 * lookspeedModifier;
                if (appliedLook < -1)
                {
                    appliedLook = -1;
                }
                _pitch = MathF.Asin(appliedLook);
            }

            if (_pitch > MathF.PI / 2)
            {
                _pitch = MathF.PI / 2;
            }
            else if (_pitch < -MathF.PI / 2)
            {
                _pitch = -MathF.PI / 2;
            }


            if (input.IsKeyDown(Keys.D))
            {
                cameraPosition += CameraRight * movespeedModifier;
            }
            if (input.IsKeyDown(Keys.A))
            {
                cameraPosition += -CameraRight * movespeedModifier;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                cameraPosition += -CameraUp * movespeedModifier;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                cameraPosition += CameraUp * movespeedModifier;
            }
            if (input.IsKeyDown(Keys.W))
            {
                cameraPosition += CameraFront * movespeedModifier;
            }
            if (input.IsKeyDown(Keys.S))
            {
                cameraPosition += -CameraFront * movespeedModifier;
            }

            // Update the directional vectors
            


            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            GL.DeleteProgram(shader.Handle);
        }


        public static float MOD(float a, float b)
        {
            float i, j, k;

            i = a / b;
            j = i * b;
            k = a - j;

            return k;
        }
    }
}
