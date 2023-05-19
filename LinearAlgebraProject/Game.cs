using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }


        }

        protected override void OnLoad()
        {
            base.OnLoad();

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            float FOV = 90;
            float ASPECT_RATIO = 1.0f;
            float NEAR_CLIPPING_DISTANCE = 1;
            float FAR_CLIPPING_DISTANCE = 10f;

            List<Vector3> VERTICES = new()
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
                new(0,0,-5), //BLL
                new(0,0,-3), //BTL
                new(0,2,-5), //BLR
                new(0,2,-3), //BTR
                new(2,0,-5), //FLL
                new(2,0,-3), //FTL
                new(2,2,-5), //FLR
                new(2,2,-3), //FTR
            };

            Matrix4x4 perspectiveMatrix = VertexRenderer.GetPerspectiveMatrix(aspectRatio: ASPECT_RATIO,
                                                               fov: FOV,
                                                               nearClippingDistance: NEAR_CLIPPING_DISTANCE,
                                                               farClippingDistance: FAR_CLIPPING_DISTANCE);

            this.vertices = VertexRenderer.ComputeVertexNDCs(VERTICES, perspectiveMatrix);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            if (vertices != null)
            {
                // Push the vertices to the openGL buffer
                GL.BufferData(target: BufferTarget.ArrayBuffer, 
                            size: vertices.Length * sizeof(float), 
                            data: vertices, 
                            usage: BufferUsageHint.StaticDraw);
            }

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Swap out the screen buffer
            SwapBuffers();
        }
    }
}
