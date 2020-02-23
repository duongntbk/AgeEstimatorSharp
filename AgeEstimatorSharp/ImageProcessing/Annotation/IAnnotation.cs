using System.Collections.Generic;
using AgeEstimatorSharp.Predictor;
using OpenCvSharp;

namespace AgeEstimatorSharp.ImageProcessing.Annotation
{
    /// <summary>
    /// Add annotation to image.
    /// This interface can be used to display predicted results on input image.
    /// </summary>
    public interface IAnnotation
    {
        /// <summary>
        /// Annotate input image with predicted results.
        /// </summary>
        /// <param name="path">
        /// Path to input image. If input image is not in the same folder,
        /// an absolute path is required.
        /// </param>
        /// <param name="results">
        /// Predicted result from tensorflow network.
        /// May contain gender or age or both.
        /// </param>
        /// <returns>
        /// an OpenCv image matrix as output.
        /// </returns>
        Mat Annotate(string path, List<Result> results);
    }
}
