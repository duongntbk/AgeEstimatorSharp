using System.Collections.Generic;
using DlibDotNet;

namespace AgeEstimatorSharp.ImageProcessing.Locator
{
    /// <summary>
    /// This class performs face detection using HOOG feature
    /// and the Dlib library.
    /// </summary>
    public class FaceLocatorDlib : AbstractFaceLocator
    {
        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="expandRatio">
        /// Experiments show that an expand ratio of 1.2
        /// gives the best result when using HOOG feature.
        /// </param>
        public FaceLocatorDlib(float expandRatio = 1.2f) : base(expandRatio)
        {
        }

        /// <inheritdoc />
        public override List<Location> GetFaceLocations(string path)
        {
            var rs = new List<Location>();

            using (var fd = Dlib.GetFrontalFaceDetector())
            {
                var image = Dlib.LoadImageAsMatrix<RgbPixel>(path);
                var width = image.Columns;
                var height = image.Rows;

                fd.Operator(image, out var faces);
                foreach (var loc in faces)
                {
                    rs.Add(ExpandFace(width, height, 
                        loc.Rect.Top, loc.Rect.Right,loc.Rect.Bottom, loc.Rect.Left));
                }
            }

            return rs;
        }

        /// <inheritdoc />
        public override List<Location> GetFaceLocations(byte[] bytes)
        {
            var msg = $@"Function to detect faces from binary data 
                         using Dlib is not implemented yet.{System.Environment.NewLine}
                         Feel free to create a pull request if you want to contribute.";

            throw new System.NotImplementedException(msg);
        }
    }
}
