using OpenCvSharp;

namespace AgeEstimatorSharp.ImageProcessing
{
    /// <summary>
    /// Helper class to manipulate picture using OpenCV.
    /// </summary>
    public class ImageUtils
    {
        /// <summary>
        /// Display image in new window using OpenCV.
        /// </summary>
        /// <param name="image">
        /// Target image in OpenCV matrix format.
        /// </param>
        /// <param name="title">
        /// Window title.
        /// </param>
        public static void DisplayImage(Mat image, string title)
        {
            Cv2.ImShow(title, image);
        }
    }
}
