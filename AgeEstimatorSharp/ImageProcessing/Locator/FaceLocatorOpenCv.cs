using OpenCvSharp;
using System.Collections.Generic;

namespace AgeEstimatorSharp.ImageProcessing.Locator
{
    /// <summary>
    /// This class performs face detection using Haar Cascade
    /// and the OpenCV library.
    /// </summary>
    public class FaceLocatorOpenCv : AbstractFaceLocator
    {
        /// <summary>
        /// Haar Cascade file to be use to detect face location.
        /// </summary>
        private readonly CascadeClassifier _cascade;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="expandRatio">
        /// Experiments show that an expand ratio of 1.00
        /// gives the best result when using Haar Cascade.
        /// </param>
        /// <param name="cascadePath">
        /// Specify the path to Haar Cascade file.
        /// </param>
        public FaceLocatorOpenCv(float expandRatio=1.00f, string cascadePath = CommonConstants.Image.HaarcascadeFrontalFaceAlt2) :
            base(expandRatio)
        {
            // Load the cascade
            _cascade = new CascadeClassifier(cascadePath);
        }

        /// <inheritdoc />
        public override List<Location> GetFaceLocations(string path)
        {
            var image = Cv2.ImRead(path); // Read image from disk
            return GetFaceLocations(image);
        }

        /// <inheritdoc />
        public override List<Location> GetFaceLocations(byte[] bytes)
        {
            var image = Cv2.ImDecode(bytes, ImreadModes.Color); // Convert binary data to image matrix
            return GetFaceLocations(image);
        }

        /// <summary>
        /// Detect positions of all faces in image.
        /// </summary>
        /// <param name="image">
        /// Input image in OpenCV's matrix format.
        /// </param>
        /// <returns></returns>
        protected List<Location> GetFaceLocations(Mat image)
        {
            var rs = new List<Location>();

            var width = image.Width;
            var height = image.Height;

            // Detect faces
            var faces = _cascade.DetectMultiScale(image, 1.1, 3, HaarDetectionType.ScaleImage, new Size(30, 30));

            foreach (var loc in faces)
            {
                rs.Add(ExpandFace(width, height,
                    loc.Top, loc.Right, loc.Bottom, loc.Left));
            }

            return rs;
        }
    }
}
