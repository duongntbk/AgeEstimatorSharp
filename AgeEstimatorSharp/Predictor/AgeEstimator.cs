using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.TensorflowHelper;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Estimate age from face image.
    /// </summary>
    public class AgeEstimator : BaseSinglePredictor
    {
        /// <summary>
        /// Initialize object.
        /// Match shape of input and specify name
        /// of input node and output node of tensorflow model.
        /// Pass object to perform calculation using tensorflow model as well.
        /// </summary>
        /// <param name="runner">
        /// Object to perform calculation using tensorflow model
        /// </param>
        /// <param name="width">
        /// Width of image used to train model.
        /// </param>
        /// <param name="height">
        /// Width of image used to train model.
        /// </param>
        /// <param name="depth">
        /// Depth of image used to train model.
        /// </param>
        /// <param name="inputNode">
        /// Name of input node of tensorflow model.
        /// </param>
        /// <param name="outputNode">
        /// Name of output node of tensorflow model.
        /// </param>
        public AgeEstimator(IRunnable runner,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth,
            string inputNode = CommonConstants.Predictor.AgeInputNode,
            string outputNode = CommonConstants.Predictor.AgeOutputNode) :
                base(runner, width, height, depth)
        {
            InputNode = inputNode;
            OutputNode = outputNode;
        }

        /// <inheritdoc />
        protected override Result ConvertToResult(Location faceLoc, float rawOutput)
        {
            // This is a regression problem,
            // using mean absolute error as loss function.
            // Therefore the output of model is the estimated result
            return new Result
            {
                Loc = faceLoc,
                Age = rawOutput
            };
        }
    }
}
