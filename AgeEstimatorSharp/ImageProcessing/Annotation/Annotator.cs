using System.Collections.Generic;
using AgeEstimatorSharp.Predictor;
using OpenCvSharp;

namespace AgeEstimatorSharp.ImageProcessing.Annotation
{
    /// <summary>
    /// Simple implementation of IAnnotation.
    /// Predicted result is display on top of each detected face.
    /// </summary>
    public class Annotator : IAnnotation
    {
        /// <inheritdoc />
        public AnnotationOption Option { get; set; } = AnnotationOption.Both;

        /// <inheritdoc />
        public Mat Annotate(string path, List<Result> results)
        {
            var image = Cv2.ImRead(path);

            // Loop through the list of results
            // and draw rectangle/add text for each one.
            foreach (var result in results)
            {
                Annotate(image, result);
            }

            return image;
        }

        /// <summary>
        /// Draw a rectangle around the face
        /// and display predicted result on top.
        /// </summary>
        /// <param name="image">
        /// Input image in OpenCv matrix format.
        /// </param>
        /// <param name="result">
        /// Predicted result from tensorflow network.
        /// </param>
        private void Annotate(Mat image, Result result)
        {
            var topLeft = new Point(result.Loc.Left, result.Loc.Top);
            var bottomRight = new Point(result.Loc.Right, result.Loc.Bottom);
            Cv2.Rectangle(image, topLeft, bottomRight, Scalar.Red, 2);

            string msg;
            if (Option == AnnotationOption.Both)
            {
                msg = $"{result.Gender} - {result.Age:n2}";
            }
            else if (Option == AnnotationOption.Gender)
            {
                msg = $"{result.Gender}";
            }
            else
            {
                msg = $"{result.Age:n2}";
            }

            // Display result 10px above face's rectangle, looks better that way
            var msgPoint = new Point(result.Loc.Left, result.Loc.Top - 10);
            Cv2.PutText(image, msg, msgPoint, HersheyFonts.HersheyPlain, 1.5, Scalar.Red, 1, LineTypes.AntiAlias);
        }
    }
}
