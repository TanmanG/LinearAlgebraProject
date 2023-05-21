using System.Numerics;
using System.Runtime.InteropServices;
using System.Drawing;
using LinearAlgebraProject;
using OpenTK;
using System.Runtime.CompilerServices;
using OpenTK.Mathematics;



// To-do:
// 0. Add OpenTK visualization
// 1. Add interface and documentation
// 2. Add GPU acceleration
// 3. Add proper driver code for rendering

internal class VertexRenderer : IVertexRenderer
{
    private static void Main(string[] args)
    {

        int VIEWPORT_WIDTH = 1280;
        int VIEWPORT_HEIGHT = 720;

        using (Game game = new(VIEWPORT_WIDTH, VIEWPORT_HEIGHT, "Vertex Renderer"))
        {
            game.Run();
        }
    }

    // PERSPECTIVE RENDERING
    //
    public static float[,] ComputeVertexNDCs(List<Vector3> vertices,Matrix4x4 perspectiveMatrix)
    {
        List<Vector3> processedVectors = GetNdcCoordinates(GetHomogenizedVectors(vertices, perspectiveMatrix));

        return Vector3ListToArray(processedVectors);
    }
    public static Matrix4x4 GetPerspectiveMatrix(float aspectRatio, float fov,
                                               float nearClippingDistance,
                                               float farClippingDistance)
    {
        if (nearClippingDistance <= 0)
            throw new Exception("Near clipping distance must be non-zero and positive!");
        if (farClippingDistance <= 0)
            throw new Exception("Far clipping distance must be non-zero and positive!");
        if (fov <= 0 || fov >= 180)
            throw new Exception("FOV must be between 0 and 180!");
        if (aspectRatio <= 0)
            throw new Exception("Aspect Ratio must be non-zero and positive!");

        Matrix4x4 perspectiveMatrix = new()
        {
            M11 = 1 / (aspectRatio * MathF.Tan(fov / 2)),
            M22 = 1 / (aspectRatio * MathF.Tan(fov / 2)),
            M33 = -(nearClippingDistance + farClippingDistance) / (nearClippingDistance - farClippingDistance),
            M34 = -2 * nearClippingDistance * farClippingDistance / (nearClippingDistance - farClippingDistance),
            M43 = -1,
        };

        return perspectiveMatrix;
    }
    public static List<Vector4> GetHomogenizedVectors(List<Vector3> inputVectors, Matrix4x4 perspectiveMatrix)
    {
        List<Vector4> homogenizedVectors = new();

        foreach (Vector3 vector in inputVectors)
        {
            homogenizedVectors.Add(new()
            {
                X = vector.X * perspectiveMatrix.M11,
                Y = vector.Y * perspectiveMatrix.M22,
                Z = vector.Z * perspectiveMatrix.M33 + perspectiveMatrix.M34,
                W = vector.Z * perspectiveMatrix.M43,
            });
        }

        return homogenizedVectors;
    }
    public static List<Vector3> GetNdcCoordinates(List<Vector4> inputVectors)
    {
        List<Vector3> homogenizedVectors = new();

        // For each homogenous coordinate, apply NDC divide to place it into NDC space
        foreach (Vector4 vector in inputVectors)
        {
            homogenizedVectors.Add(Vector4To3(vector, 1 / vector.W));
        }

        return homogenizedVectors;
    }
    public static List<Tuple<int, int>> GetScreenCoordinates(List<Vector3> inputVectors, int viewportWidth, int viewportHeight)
    {
        Dictionary<Tuple<int, int>, float> screenCoordinates = new();

        foreach (Vector3 vector in inputVectors)
        {
            Tuple<int, int> newPixel = new((int)((vector.X + 1) / 2 * viewportWidth),
                                           (int)((vector.Y + 1) / 2 * viewportHeight));

            // Check if the new pixel is closer than the old one
            if (!screenCoordinates.TryGetValue(newPixel, out float value) || vector.Z < value)
            {
                // Replace/Add the pixel
                screenCoordinates.Remove(newPixel);
                screenCoordinates.Add(newPixel, vector.Z);
            }
        }

        return screenCoordinates.Keys.ToList();
    }

    // HELPER FUNCTIONS
    //
    public static Vector3 Vector4To3(Vector4 inputVector, float scalar = 1)
    {
        return new Vector3(inputVector.X * scalar, inputVector.Y * scalar, inputVector.Z * scalar);
    }
    public static float[,] Vector3ListToArray(List<Vector3> inputVectors)
    {
        float[,] vectorArray = new float[inputVectors.Count, 3];

        for (int row = 0; row < inputVectors.Count - 1; row++)
        {
            vectorArray[row, 0] = inputVectors[row].X;
            vectorArray[row, 1] = inputVectors[row].Y;
            vectorArray[row, 2] = inputVectors[row].Z;
        }

        return vectorArray;
    }

    // OPERATION FUNCTIONS
    //
    public static List<Vector3> RotateVectorsAboutAxis(List<Vector3> vectors, Axis axis, float degrees, bool usingRadians = false)
    {
        List<Vector3> retVector = new();
        // Rotate each vector
        foreach (Vector3 vector in vectors)
        {
            // Store the rotated vector
            retVector.Add(RotateVectorAboutAxis(vector, axis, degrees, usingRadians));
        }

        return retVector;
    }
    public static Vector3 RotateVectorAboutAxis(Vector3 vector, Axis axis, float degrees, bool usingRadians = false)
    {
        // Converts degrees to radians
        if (!usingRadians)
            degrees *= MathF.PI / 180;

        // Branches to which axis is selected
        switch (axis)
        {
            case Axis.X: // Rotate about X-axis
                         // X remains untouched
                vector.Y = vector.Y * MathF.Cos(degrees) - vector.Z * MathF.Sin(degrees);
                vector.Z = vector.Y * MathF.Sin(degrees) + vector.Z * MathF.Cos(degrees);
                break;

            case Axis.Y: // Rotate about Y-axis
                vector.X = vector.X * MathF.Cos(degrees) + vector.Z * MathF.Sin(degrees);
                // Y remains untouched
                vector.Z = -vector.X * MathF.Sin(degrees) + vector.Z * MathF.Cos(degrees);
                break;

            case Axis.Z: // Rotate about Z-axis
                vector.X = vector.X * MathF.Cos(degrees) - vector.Y * MathF.Sin(degrees);
                vector.Y = vector.X * MathF.Sin(degrees) - vector.Y * MathF.Cos(degrees);
                // Z remains untouched
                break;
        }
        return vector;
    }
}

public enum Axis
{
    X,  
    Y,
    Z,

}