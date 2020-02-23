using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.TensorflowHelper;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Predict gender from face image.
    /// </summary>
    public class GenderClassifier : BaseSinglePredictor
    {
        /// <summary>
        /// Initialize object.
        /// Match shape of input and specify name
        /// of input node and output node of tensorflow model.
        /// Pass object to perform calculation using tensorflow model as well.
        /// </summary>
        /// <param name="runner">
        /// Object to perform calculation using tensorflow model.
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
        public GenderClassifier(IRunnable runner,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth,
            string inputNode = CommonConstants.Predictor.GenderInputNode,
            string outputNode = CommonConstants.Predictor.GenderOutputNode) : 
                base(runner, width, height, depth)
        {
            InputNode = inputNode;
            OutputNode = outputNode;
        }

        /// <inheritdoc />
        protected override Result ConvertToResult(Location faceLoc, float rawOutput)
        {
            // This is a 2 classes classification problem,
            // using binary cross entropy as loss function.
            // Therefore the threshold is 0.5
            if (rawOutput < CommonConstants.Predictor.Threshold)
            { 
                return new Result
                {
                    Loc = faceLoc,
                    Gender = Gender.Male
                };
            }

            return new Result
            {
                Loc = faceLoc,
                Gender = Gender.Female
            };
        }
    }
}
