using NumSharp;

namespace AgeEstimatorSharp.Preprocessor
{
    /// <summary>
    /// Divide the value of each pixel by *rate* to scale back input.
    /// </summary>
    public class DividePreprocessor : IProcessor
    {
        private readonly double _rate;

        public DividePreprocessor(double rate)
        {
            _rate = rate;
        }

        /// <inheritdoc />
        public NDArray Process(NDArray inputs)
        {
            return inputs / _rate;
        }
    }
}
