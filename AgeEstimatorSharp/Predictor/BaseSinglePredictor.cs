using System.Collections.Generic;
using System.Linq;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.TensorflowHelper;
using NumSharp;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Abstract base class for predictors which only predict either age or gender.
    /// </summary>
    public abstract class BaseSinglePredictor : BasePredictor
    {
        /// <summary>
        /// Name of input node of tensorflow graph.
        /// </summary>
        protected string InputNode;
        /// Name of output node of tensorflow graph.
        protected string OutputNode;

        /// <summary>
        /// Initialize object.
        /// Match shape of tensorflow model.
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
        protected BaseSinglePredictor(IRunnable runner,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth) :
                base(runner, width, height, depth)
        {
        }

        /// <inheritdoc />
        protected override List<Result> Fit(List<Location> faceLocs, NDArray inputs)
        {
            // Pass input tensor to tensorflow model and retrieve result
            var rawOutputs = Runner.Run(inputs, InputNode, OutputNode);

            // Convert result tensor to more readable format
            return ConvertToResult(faceLocs, rawOutputs);
        }

        /// <summary>
        /// Convert output of tensorflow model from tensor
        /// to more readable format.
        /// </summary>
        /// <param name="faceLocs">
        /// List of positions of each face.
        /// </param>
        /// <param name="tensor">
        /// Predicted result of all faces as a tensor. 
        /// </param>
        /// <returns></returns>
        private List<Result> ConvertToResult(List<Location> faceLocs, NDArray tensor)
        {
            var rs = new List<Result>();

            switch (faceLocs.Count)
            {
                // Not face was detected.
                // But this case should not happen
                // because it is already checked in Fit(string imagePath)
                case 0:
                    return rs;
                // If only one face was detected,
                // then output tensor itself will be a scalar
                case 1:
                    rs.Add(ConvertToResult(faceLocs.First(), tensor));
                    break;
                // If more than one faces were detected,
                // then output tensor will be a array
                default:
                {
                    for (var i = 0; i < faceLocs.Count; i++)
                    {
                        rs.Add(ConvertToResult(faceLocs[i], tensor[i]));
                    }

                    break;
                }
            }

            return rs;
        }

        /// <summary>
        /// Convert output from float type to more readable format.
        /// </summary>
        /// <param name="faceLoc">
        /// Location of current face.
        /// </param>
        /// <param name="rawOutput">
        /// Predicted result in float type.
        /// </param>
        /// <returns></returns>
        protected abstract Result ConvertToResult(Location faceLoc, float rawOutput);
    }
}
