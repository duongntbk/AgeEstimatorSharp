using System.Collections.Generic;
using AgeEstimatorSharp.ImageProcessing.Locator;
using OpenCvSharp;

namespace AgeEstimatorSharp.ImageProcessing.Resizer
{
    /// <summary>
    /// Implement of IResizable interface using OpenCV.
    /// </summary>
    public class FaceResizer : IResizable
    {
        /// <inheritdoc />
        public List<byte[]> Resize(string path, List<Location> faceLocs, int targetWidth, int targetHeight)
        {
            // Read image from disk, then pass to Resize(Mat image,...)
            var image = Cv2.ImRead(path);
            return Resize(image, faceLocs, targetWidth, targetHeight);
        }

        /// <inheritdoc />
        public List<byte[]> Resize(byte[] data, List<Location> faceLocs, int targetWidth, int targetHeight)
        {
            // Read image from memory, then pass to Resize(Mat image,...)
            var image = Cv2.ImDecode(data, ImreadModes.Color);
            return Resize(image, faceLocs, targetWidth, targetHeight);
        }


        /// <summary>
        /// Extract all faces area from image matrix.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="faceLocs"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        protected List<byte[]> Resize(Mat image, List<Location> faceLocs, int targetWidth, int targetHeight)
        {
            var rs = new List<byte[]>();

            foreach (var loc in faceLocs)
            {
                var face = image[loc.Top, loc.Bottom, loc.Left, loc.Right];
                // In case target size is bigger that actual size,
                // we use interpolation to increase face's size.
                face = face.Resize(new Size(targetWidth, targetHeight), interpolation: InterpolationFlags.Nearest);

                rs.Add(ImageToArray(face));
            }

            return rs;
        }

        private byte[] ImageToArray(Mat image)
        {
            var rs = new byte[image.Height * image.Width * image.Channels()];
            var rsIndex = 0;

            var indexer = image.GetGenericIndexer<Vec3b>();

            for (var rowIndex = 0; rowIndex < image.Height; rowIndex++)
            {
                for (var colIndex = 0; colIndex < image.Width; colIndex++)
                {
                    // OpenCvSharp reads images using BGR format.
                    // We want to switch back to RGB format here.
                    var color = indexer[rowIndex, colIndex];
                    rs[rsIndex++] = color.Item2;
                    rs[rsIndex++] = color.Item1;
                    rs[rsIndex++] = color.Item0;
                }
            }

            return rs;
        }
    }
}
