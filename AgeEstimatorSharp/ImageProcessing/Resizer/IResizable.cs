using System.Collections.Generic;
using AgeEstimatorSharp.ImageProcessing.Locator;
using OpenCvSharp;

namespace AgeEstimatorSharp.ImageProcessing.Resizer
{
    /// <summary>
    /// Resize an area of an image into new size.
    /// This interface is used to resize the face
    /// into suitable size to be passed to tensorflow network. 
    /// </summary>
    public interface IResizable
    {
        /// <summary>
        /// Read image from path, extract all faces area
        /// and resize each ones into specified size.
        /// </summary>
        /// <param name="path">
        /// Path to input image. Should be absolute path.
        /// </param>
        /// <param name="faceLocs">
        /// A list of Location type.
        /// Store position of all faces in current image.
        /// </param>
        /// <param name="targetWidth">
        /// The target width to resize each face into.
        /// </param>
        /// <param name="targetHeight">
        /// The target height to resize each face into.
        /// </param>
        /// <returns>
        /// A list of byte array object.
        /// Each byte array object is a face,
        /// resized into the suitable size
        /// and ready to be converted into numpy array.
        /// </returns>
        List<byte[]> Resize(string path, List<Location> faceLocs, int targetWidth, int targetHeight);

        /// <summary>
        /// Read image from path, extract all faces area
        /// and resize each ones into specified size.
        /// </summary>
        /// <param name="data">
        /// Image as binary array.
        /// </param>
        /// <param name="faceLocs">
        /// A list of Location type.
        /// Store position of all faces in current image.
        /// </param>
        /// <param name="targetWidth">
        /// The target width to resize each face into.
        /// </param>
        /// <param name="targetHeight">
        /// The target height to resize each face into.
        /// </param>
        /// <returns>
        /// A list of byte array object.
        /// Each byte array object is a face,
        /// resized into the suitable size
        /// and ready to be converted into numpy array.
        /// </returns>
        List<byte[]> Resize(byte[] data, List<Location> faceLocs, int targetWidth, int targetHeight);
    }
}
