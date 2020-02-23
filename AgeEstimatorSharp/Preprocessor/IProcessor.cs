using NumSharp;

namespace AgeEstimatorSharp.Preprocessor
{
    /// <summary>
    /// Normalize input tensor before passing into tensorflow model.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Perform normalization on input tensor.
        /// </summary>
        /// <param name="inputs">
        /// Input tensor.
        /// </param>
        /// <returns>
        /// Output tensor.
        /// </returns>
        NDArray Process(NDArray inputs);
    }
}
