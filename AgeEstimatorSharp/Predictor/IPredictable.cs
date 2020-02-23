using System.Collections.Generic;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.ImageProcessing.Resizer;
using AgeEstimatorSharp.Preprocessor;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Read an image, locate all faces in image
    /// and use tensorflow to predict gender/age. 
    /// </summary>
    public interface IPredictable
    {
        /// <summary>
        /// A list of preprocessor to pre-process image
        /// before passing to tensorflow model.
        /// </summary>
        List<IProcessor> Preprocessors { get; set; }
        /// <summary>
        /// Object to detect the positions of all faces in image.
        /// </summary>
        ILocatable Locator { get; set; }
        /// <summary>
        /// Object to resize detected face
        /// to match input size of tensorflow model.
        /// </summary>
        IResizable Resizer { get; set; }

        /// <summary>
        /// Predict gender/age of all people
        /// whose faces can be detected in image.
        /// </summary>
        /// <param name="imagePath">
        /// Path to input image. Should be absolute path.
        /// </param>
        /// <returns>
        /// A list of Result object,
        /// each one is the position and predicted result of a face.
        /// </returns>
        List<Result> Fit(string imagePath);

        /// <summary>
        /// Predict gender/age of all people
        /// whose faces can be detected in image.
        /// </summary>
        /// <param name="data">
        /// Input image, already loaded into memory.
        /// </param>
        /// <returns>
        /// A list of Result object,
        /// each one is the position and predicted result of a face.
        /// </returns>
        List<Result> Fit(byte[] data);
    }
}
