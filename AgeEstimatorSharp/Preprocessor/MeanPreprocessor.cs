using System;
using System.IO;
using Newtonsoft.Json;
using NumSharp;

namespace AgeEstimatorSharp.Preprocessor
{
    /// <summary>
    /// Mean normalization class.
    /// </summary>
    public class MeanPreprocessor : IProcessor
    {
        /// <summary>
        /// Mean value of each channel in training data.
        /// </summary>
        private readonly NDArray _mean;

        /// <summary>
        /// Initialize object.
        /// Read mean value of R, G and B channels from json file.
        /// </summary>
        /// <param name="meanJsonPath">
        /// Json file stores mean value of each channel.
        /// </param>
        public MeanPreprocessor(string meanJsonPath)
        {
            try
            {
                using (var r = new StreamReader(meanJsonPath))
                {
                    var json = r.ReadToEnd();
                    var mean = JsonConvert.DeserializeObject<MeanRgb>(json);
                    _mean = np.array(mean.R, mean.G, mean.B);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new Exception(
                    $@"Cannot find json folder. Please check if json path is correct. The current json path is: {meanJsonPath}", ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception(
                    $@"Cannot find json file. Please check if json path is correct. The current json path is: {meanJsonPath}", ex);
            }
        }

        /// <inheritdoc />
        public NDArray Process(NDArray inputs)
        {
            // Subtract mean value of each channel
            // to zero center all channels.
            return inputs - _mean;
        }
    }
}
