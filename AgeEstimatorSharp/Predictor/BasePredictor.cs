using System.Collections.Generic;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.ImageProcessing.Resizer;
using AgeEstimatorSharp.Preprocessor;
using AgeEstimatorSharp.TensorflowHelper;
using NumSharp;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Abstract base class for all predictors.
    /// </summary>
    public abstract class BasePredictor : IPredictable
    {
        /// <summary>
        /// Width of input of tensorflow model.
        /// </summary>
        protected int Width;
        /// <summary>
        /// Height of input of tensorflow model.
        /// </summary>
        protected int Height;
        /// <summary>
        /// Depth of input of tensorflow model.
        /// Equal 1 if model is trained on greyscale images;
        /// equal 3 if model is trained on RGB images.
        /// </summary>
        protected int Depth;
        /// <inheritdoc />
        public ILocatable Locator { get; set; }
        /// <inheritdoc />
        public IResizable Resizer { get; set; }
        /// <inheritdoc />
        public List<IProcessor> Preprocessors { get; set; }

        /// <summary>
        /// Initialize object.
        /// Also set shape of input of tensorflow model.
        /// </summary>
        /// <param name="width">
        /// Width of image used to train tensorflow model.
        /// </param>
        /// <param name="height">
        /// Width of image used to train tensorflow model.
        /// </param>
        /// <param name="depth">
        /// Depth of input of tensorflow model.
        /// Equal 1 if model is trained on greyscale images;
        /// equal 3 if model is trained on RGB images.
        /// </param>
        protected BasePredictor(int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }

        /// <summary>
        /// Predict gender/age for all faces which were detected.
        /// </summary>
        /// <param name="faces">
        /// Data of all faces, converted into binary type. 
        /// </param>
        /// <param name="runner">
        /// Object to perform tensor calculation on input,
        /// using tensorflow model.
        /// </param>
        /// <param name="inputNode">
        /// Name of input node of tensorflow model's graph.
        /// </param>
        /// <param name="outputNode">
        /// Name of output node of tensorflow model's graph.
        /// </param>
        /// <returns>
        /// An tensor stores all results of all faces. 
        /// </returns>
        protected NDArray Fit(List<byte[]> faces, IRunnable runner,
            string inputNode, string outputNode)
        {
            int[] shape =
            {
                1, // We will perform tensor stacking inside CreateTensor function
                Height,
                Width,
                Depth
            };
            // Convert faces' data into one tensor
            var inputs = TensorUtils.CreateTensor(faces, shape);

            // Normalize input tensor before passing to tensorflow model.
            // This could sustainably increase model's accuracy
            if (Preprocessors != null)
            {
                foreach (var preprocessor in Preprocessors)
                {
                    inputs = preprocessor.Process(inputs);
                }
            }

            // Pass input tensor to tensorflow model and retrieve result
            return runner.Run(inputs, inputNode, outputNode);
        }

        /// <inheritdoc />
        public abstract List<Result> Fit(string imagePath);

        /// <inheritdoc />
        public abstract List<Result> Fit(byte[] data);

    }
}
