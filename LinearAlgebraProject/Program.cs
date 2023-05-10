using System.Numerics;
using System.Runtime.InteropServices;
using System.Drawing;

Console.WriteLine("Hello, World!");

float fov = 90;
float aspectRatio = 1.0f;
float nearClippingDistance = 1;
float farClippingDistance = 10f;

Matrix4x4 perspectiveMatrix = GetPerspectiveMatrix(aspectRatio: aspectRatio, 
                                                   fov: fov, 
                                                   nearClippingDistance: nearClippingDistance, 
                                                   farClippingDistance: farClippingDistance);
perspectiveMatrix = RotateMatrixAboutYAxis(perspectiveMatrix, 135, degrees: true);

List<Vector3> vertices = new()
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

List<Vector3> ndcVertices = GetNdcCoordinates(GetHomogenizedVectors(vertices, perspectiveMatrix));

int viewportWidth = 240;
int viewportHeight = 240;

List<Tuple<int, int>> screenCoordinates = GetScreenCoordinates(inputVectors: ndcVertices, viewportWidth: viewportWidth, viewportHeight: viewportHeight);

foreach (var pixel in screenCoordinates)
{
    Console.WriteLine(pixel);
}


// FUNCTIONS

Matrix4x4 GetPerspectiveMatrix(float aspectRatio, 
                               float fov, 
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


/// Converts a list of world coordinates along with a perspective matrix to create a list of homogenized vectors
List<Vector4> GetHomogenizedVectors(List<Vector3> inputVectors, Matrix4x4 perspectiveMatrix)
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

/// Converts a list of homogenized vectors into ones in NDC space.
List<Vector3> GetNdcCoordinates(List<Vector4> inputVectors) 
{
    List<Vector3> homogenizedVectors = new();

    // For each homogenous coordinate, apply NDC divide to place it into NDC space
    foreach (Vector4 vector in inputVectors)
    {
        homogenizedVectors.Add(Vector4To3(vector, 1 / vector.W));
    }

    return homogenizedVectors;
}

/// Converts a Vector4 to Vector3, truncating the W-element and multiplying by a scalar if provided.
Vector3 Vector4To3(Vector4 inputVector, float scalar = 1)
{
    return new Vector3(inputVector.X * scalar, inputVector.Y * scalar, inputVector.Z * scalar);
}

List<Tuple<int, int>> GetScreenCoordinates(List<Vector3> inputVectors, int viewportWidth, int viewportHeight)
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


// EXTRA FUN FUNCTIONS
// This is scuffed invesitgate a proper answer ???
Matrix4x4 RotateMatrixAboutYAxis(Matrix4x4 rotationMatrix, float angle, bool degrees = false)
{
    angle *= degrees ? float.Pi / 180 : 1;

    Matrix4x4 appliedRotation = new()
    {
        M11 = MathF.Cos(angle),
        M13 = MathF.Sin(angle),
        M22 = 1,
        M31 = -MathF.Sin(angle),
        M33 = MathF.Cos(angle),
        M44 = 1
    };

    return rotationMatrix * appliedRotation;
}
