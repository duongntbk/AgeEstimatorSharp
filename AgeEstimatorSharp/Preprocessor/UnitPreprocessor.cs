using NumSharp;

namespace AgeEstimatorSharp.Preprocessor
{
    /// <summary>
    /// Normalize input tensor into range [0, 1]
    /// </summary>
    public class UnitPreprocessor : IProcessor
    {
        /// <summary>
        /// Because maximum value for each channel is 255,
        /// normalization rate is 255.
        /// </summary>
        private const float RgbNormalizationRate = 255.0f;

        /// <inheritdoc />
        public NDArray Process(NDArray inputs)
        {
            // Divide all value by 255 to normalize input tensor into range [0, 1]
            return inputs / RgbNormalizationRate;
        }
    }
}
