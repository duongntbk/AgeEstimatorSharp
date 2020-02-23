using System.Collections.Generic;
using System.Linq;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.TensorflowHelper;
using NumSharp;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// Predict both age and gender from face image.
    /// </summary>
    public class AgeAndGenderPredictor : BasePredictor
    {
        /// <summary>
        /// Object to estimate age using tensorflow model.
        /// </summary>
        private readonly IRunnable _ageRunner;
        /// <summary>
        /// Object to predict gender using tensorflow model.
        /// </summary>
        private readonly IRunnable _genderRunner;
        /// <summary>
        /// Name of input node of age estimating model.
        /// </summary>
        private readonly string _ageInputNode;
        /// <summary>
        /// Name of output node of age estimating model.
        /// </summary>
        private readonly string _ageOutputNode;
        /// <summary>
        /// Name of input node of gender predicting model.
        /// </summary>
        private readonly string _genderInputNode;
        /// <summary>
        /// Name of output node of gender predicting model.
        /// </summary>
        private readonly string _genderOutputNode;

        /// <summary>
        /// Initialize object.
        /// Match shape of input and specify name
        /// of input node and output node of tensorflow model.
        /// Pass object to perform calculation using tensorflow model as well.
        /// </summary>
        /// <param name="genderRunner">
        /// Object to predict gender using tensorflow model.
        /// </param>
        /// <param name="ageRunner">
        /// Object to estimate age using tensorflow model.
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
        /// <param name="ageInputNode">
        /// Name of input node of age estimating model.
        /// </param>
        /// <param name="ageOutputNode">
        /// Name of output node of age estimating model.
        /// </param>
        /// <param name="genderInputNode">
        /// Name of input node of gender predicting model.
        /// </param>
        /// <param name="genderOutputNode">
        /// Name of output node of gender predicting model.
        /// </param>
        public AgeAndGenderPredictor(IRunnable genderRunner, IRunnable ageRunner,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth,
            string ageInputNode = CommonConstants.Predictor.AgeInputNode,
            string ageOutputNode = CommonConstants.Predictor.AgeOutputNode,
            string genderInputNode = CommonConstants.Predictor.GenderInputNode,
            string genderOutputNode = CommonConstants.Predictor.GenderOutputNode) :
                base(width, height, depth)
        {
            _ageInputNode = ageInputNode;
            _ageOutputNode = ageOutputNode;
            _genderInputNode = genderInputNode;
            _genderOutputNode = genderOutputNode;

            _ageRunner = ageRunner;
            _genderRunner = genderRunner;
        }

        /// <inheritdoc />
        public override List<Result> Fit(string imagePath)
        {
            var faceLocs = Locator.GetFaceLocations(imagePath);

            if (faceLocs.Count == 0)
            {
                return new List<Result>();
            }

            var faces = Resizer.Resize(imagePath, faceLocs, Width, Height);

            var rawGenderOutputs = Fit(faces, _genderRunner, _genderInputNode, _genderOutputNode);
            var rawAgeOutputs = Fit(faces, _ageRunner, _ageInputNode, _ageOutputNode);

            return ConvertToResult(faceLocs, rawGenderOutputs, rawAgeOutputs);
        }

        public override List<Result> Fit(byte[] data)
        {
            var faceLocs = Locator.GetFaceLocations(data);

            if (faceLocs.Count == 0)
            {
                return new List<Result>();
            }

            var faces = Resizer.Resize(data, faceLocs, Width, Height);

            var rawGenderOutputs = Fit(faces, _genderRunner, _genderInputNode, _genderOutputNode);
            var rawAgeOutputs = Fit(faces, _ageRunner, _ageInputNode, _ageOutputNode);

            return ConvertToResult(faceLocs, rawGenderOutputs, rawAgeOutputs);
        }

        /// <summary>
        /// Convert output from numpy tensor to more readable format.
        /// </summary>
        /// <param name="faceLocs">
        /// List of positions of each face.
        /// </param>
        /// <param name="rawGenderOutputs">
        /// Predicted gender as numpy tensor.
        /// </param>
        /// <param name="rawAgeOutputs">
        /// Estimated age as numpy tensor.
        /// </param>
        /// <returns>
        /// List of Result object, store the postion of all faces
        /// and their predicted results.
        /// </returns>
        private List<Result> ConvertToResult(List<Location> faceLocs,
            NDArray rawGenderOutputs, NDArray rawAgeOutputs)
        {
            var rs = new List<Result>();

            // If there is only one face
            if (faceLocs.Count == 1)
            {
                // Ouput tensor is just a scalar value
                rs.Add(ConvertToResult(faceLocs.First(), rawGenderOutputs, rawAgeOutputs));
            }
            // If there are 2 or faces
            else
            {
                // Output tensor is a array
                for (var i = 0; i < faceLocs.Count; i++)
                {
                    rs.Add(ConvertToResult(faceLocs[i], rawGenderOutputs[i], rawAgeOutputs[i]));
                }
            }

            return rs;
        }

        /// <summary>
        /// Convert output from float type to more readable format.
        /// </summary>
        /// <param name="faceLoc">
        /// Postion of current face.
        /// </param>
        /// <param name="rawGenderOutput">
        /// Predicted gender in float type.
        /// </param>
        /// <param name="rawAgeOutput">
        /// Estimated age in float type.
        /// </param>
        /// <returns>
        /// An Result object stores both location of face
        /// as well as 
        /// </returns>
        private Result ConvertToResult(Location faceLoc, float rawGenderOutput, float rawAgeOutput)
        {
            // Gender was predicted as a classification problem,
            // using binary cross entropy. Class threshold is 0.5
            var gender = rawGenderOutput < CommonConstants.Predictor.Threshold ?
                Gender.Male : Gender.Female;

            return new Result
            {
                Loc = faceLoc,
                Gender = gender,
                Age = rawAgeOutput // regression problem using mae
            };
        }
    }
}
