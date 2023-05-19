using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebraProject
{
    internal interface IVertexRenderer
    {
        /// <summary>
        /// Compute the NDC space coordinates for the vertices in inputVectors and returns them as an array.
        /// </summary>
        /// <param name="vertices">The world coordinates of the vertices.s</param>
        /// <param name="perspectiveMatrix">The perspective matrix to be used.</param>
        /// <returns>A 2D array with 3 columns for each NDC coordinate, with a vertex's coordinates on each row.</returns>
        public static abstract float[,] ComputeVertexNDCs(List<Vector3> vertices, Matrix4x4 perspectiveMatrix);
        /// <summary>
        /// Converts inputVectors into a list of homogenized vectors (perspective-divided), using given perspective matrix.
        /// </summary>
        /// <param name="inputVectors">Vectors to apply the homogenization on.</param>
        /// <param name="perspectiveMatrix">Perspective matrix to apply homogenization with.</param>
        /// <returns>Homogenized vectors.</returns>
        public static abstract List<Vector4> GetHomogenizedVectors(List<Vector3> inputVectors, Matrix4x4 perspectiveMatrix);
        /// <summary>
        /// Converts inputVectors into a list of -1 to 1 clamped vectors.
        /// </summary>
        /// <param name="inputVectors">Vectors to be converted into NDC coordinates</param>
        /// <returns>A list of coordinates ranging from -1 to 1.</returns>
        public static abstract List<Vector3> GetNdcCoordinates(List<Vector4> inputVectors);
        /// <summary>
        /// Generates a perspective matrix with the given parameters.
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio of the theorhetical camera.</param>
        /// <param name="fov">Field of view of the theorhetifcal camera.</param>
        /// <param name="nearClippingDistance">Near cutoff point for the camera.</param>
        /// <param name="farClippingDistance">Far cutoff point for the camera.</param>
        /// <returns>A perspective matrix with the aspect ratio, FOV, near/far clipping plane provided.</returns>
        public static abstract Matrix4x4 GetPerspectiveMatrix(float aspectRatio, float fov, float nearClippingDistance, float farClippingDistance);
        /// <summary>
        /// Converts inputVectors into a list of screen space coordinates.
        /// </summary>
        /// <param name="inputVectors"></param>
        /// <param name="viewportWidth">Width of the output screen.</param>
        /// <param name="viewportHeight">Height of the output screen.</param>
        /// <returns>A list of pixel coordinates for each inputVector.</returns>
        public static abstract List<Tuple<int, int>> GetScreenCoordinates(List<Vector3> inputVectors, int viewportWidth, int viewportHeight);
        /// <summary>
        /// Rotates vector about the given axis by the given angle in degrees (or radians if specified).
        /// </summary>
        /// <param name="vector">The vector to be rotated.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="degrees">Angle to rotate, in degrees (or radians if usingRadians is set to true).</param>
        /// <param name="usingRadians">True when passing an angle in radians for degrees, False when using degrees.</param>
        /// <returns>The vector rotated about the given axis and angle.</returns>
        public static abstract Vector3 RotateVectorAboutAxis(Vector3 vector, Axis axis, float degrees, bool usingRadians = false);
        /// <summary>
        /// Rotates the given list of vectors about the given axis by the given angle in degrees (or radians if specified).
        /// </summary>
        /// <param name="vectors">The vectors to be rotated.</param>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="degrees">Angle to rotate, in degrees (or radians if usingRadians is set to true).</param>
        /// <param name="usingRadians">True when passing an angle in radians for degrees, False when using degrees.</param>
        /// <returns>The vectors rotated about the given axis and angle.</returns>
        public static abstract List<Vector3> RotateVectorsAboutAxis(List<Vector3> vectors, Axis axis, float degrees, bool usingRadians = false);
        /// <summary>
        /// Converts the given inputVector into a Vector 3, truncating W and applying a scalar.
        /// </summary>
        /// <param name="inputVector">The Vector4 to convert into a Vector3.</param>
        /// <param name="scalar">A scalar to apply to the converted Vector3.</param>
        /// <returns>A Vector3 created from truncating W.</returns>
        public static abstract Vector3 Vector4To3(Vector4 inputVector, float scalar = 1);
        /// <summary>
        /// Creates an array representation of inputVectors, where each Vector3 is a row in the output matrix.
        /// </summary>
        /// <param name="inputVectors">The list of Vector3s to be converted into an array.</param>
        /// <returns>An array representation the Vector3s in inputVectors.</returns>
        public static abstract float[,] Vector3ListToArray(List<Vector3> inputVectors);
    }
}
