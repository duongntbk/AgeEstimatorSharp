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
        /// Object to perform tensor calculation using tensorflow model.
        /// </summary>
        protected IRunnable Runner;

        /// <summary>
        /// Initialize object.
        /// Also set shape of input of tensorflow model.
        /// </summary>
        /// <param name="runner">
        /// Object to perform calculation using tensorflow model
        /// </param>
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
        protected BasePredictor(IRunnable runner,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth)
        {
            Runner = runner;
            Width = width;
            Height = height;
            Depth = depth;
        }

        /// <inheritdoc />
        public List<Result> Fit(string imagePath)
        {
            // Detect all faces in image
            var faceLocs = Locator.GetFaceLocations(imagePath);

            // Return empty result list if no face was detected
            if (faceLocs.Count == 0)
            {
                return new List<Result>();
            }

            // Resize face to match input of tensorflow model
            var faces = Resizer.Resize(imagePath, faceLocs, Width, Height);

            // Calculate on all faces
            return Fit(faceLocs, faces);
        }

        /// <inheritdoc />
        public List<Result> Fit(byte[] data)
        {
            // Detect all faces from byte array
            var faceLocs = Locator.GetFaceLocations(data);

            // Return empty result list if no face was detected
            if (faceLocs.Count == 0)
            {
                return new List<Result>();
            }

            // Resize face to match input of tensorflow model
            var faces = Resizer.Resize(data, faceLocs, Width, Height);

            // Calculate on all faces
            return Fit(faceLocs, faces);
        }

        /// <inheritdoc />
        public void GetDefault() => Runner.GetDefault();

        /// <summary>
        /// Predict gender/age for all faces which were detected.
        /// </summary>
        /// <param name="faceLocs">
        /// Locations of all detected faces.
        /// </param>
        /// <param name="faces">
        /// Data of all detected faces.
        /// </param>
        /// <returns>
        /// A list of Result object,
        /// each one is the position and predicted result of a face.
        /// </returns>
        protected List<Result> Fit(List<Location> faceLocs, List<byte[]> faces)
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

            return Fit(faceLocs, inputs);
        }

        /// <summary>
        /// Predict gender/age for all faces which were detected.
        /// </summary>
        /// <param name="faceLocs">
        /// Locations of all detected faces.
        /// </param>
        /// <param name="inputs">
        /// Data of all detected faces as tensor.
        /// </param>
        /// <returns>
        /// A list of Result object,
        /// each one is the position and predicted result of a face.
        /// </returns>
        protected abstract List<Result> Fit(List<Location> faceLocs, NDArray inputs);
    }
}
