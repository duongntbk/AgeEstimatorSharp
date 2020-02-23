using System.Collections.Generic;

namespace AgeEstimatorSharp.ImageProcessing.Locator
{
    /// <summary>
    /// Detect position of all faces in picture.
    /// Positions will be given in (top, right, bottom, left) format.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Read image from path and detect all faces in image.
        /// </summary>
        /// <param name="path">
        /// Path to input image. If input image is not in the same folder,
        /// an absolute path is required.
        /// </param>
        /// <returns>
        /// An list of Location struct,
        /// store positions of all faces in current picture.
        /// </returns>
        List<Location> GetFaceLocations(string path);

        /// <summary>
        /// Read image from binary data and detect all faces in image.
        /// </summary>
        /// <param name="bytes">
        /// Image file in binary format.
        /// </param>
        /// <returns>
        /// An list of Location struct,
        /// store positions of all faces in current picture.
        /// </returns>
        List<Location> GetFaceLocations(byte[] bytes);
    }
}
