using System;
using System.Collections.Generic;
using System.Linq;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.TensorflowHelper;
using NumSharp;

namespace AgeEstimatorSharp.Predictor
{
    public class AgeAndGenderPredictor : BasePredictor
    {
        /// <summary>
        /// Name of input node of tensorflow graph.
        /// </summary>
        private readonly string _inputNode;
        /// <summary>
        /// Name of output node of age guessing branch.
        /// </summary>
        private readonly string _ageOutputNode;
        /// <summary>
        /// Name of output node of gender prediction branch.
        /// </summary>
        private readonly string _genderOutputNode;

        public AgeAndGenderPredictor(IRunnable runner,
            string inputNode, string ageOutputNode, string genderOutputNode,
            int width = CommonConstants.Image.DefaultWidth,
            int height = CommonConstants.Image.DefaultHeight,
            int depth = CommonConstants.Image.DefaultColorDepth) :
                base(runner, width, height, depth)
        {
            _inputNode = inputNode;
            _ageOutputNode = ageOutputNode;
            _genderOutputNode = genderOutputNode;
        }

        /// <inheritdoc />
        protected override List<Result> Fit(List<Location> faceLocs, NDArray inputs)
        {
            var outputNodes = new ValueTuple<string, string>(_ageOutputNode, _genderOutputNode);
            // Pass input tensor to tensorflow model and retrieve result
            var rawOutputs = Runner.Run(inputs, _inputNode, outputNodes);
            return ConvertToResult(faceLocs, rawOutputs);
        }

        /// <summary>
        /// Convert output of tensorflow model from tensor
        /// to more readable format.
        /// </summary>
        /// <param name="faceLocs">
        /// List of positions of each face.
        /// </param>
        /// <param name="rawOutput">
        /// Predicted result of all faces as a tensor. 
        /// </param>
        /// <returns></returns>
        private List<Result> ConvertToResult(List<Location> faceLocs, ValueTuple<NDArray, NDArray> rawOutput)
        {
            var rs = new List<Result>();

            // If there is only one face
            if (faceLocs.Count == 1)
            {
                // Output tensor is just a scalar value
                rs.Add(ConvertToResult(faceLocs.First(), np.squeeze(rawOutput.Item1), np.squeeze(rawOutput.Item2)));
            }
            // If there are 2 or faces
            else
            {
                // Output tensor is a array
                for (var i = 0; i < faceLocs.Count; i++)
                {
                    rs.Add(ConvertToResult(faceLocs[i], np.squeeze(rawOutput.Item1)[i], 
                        np.squeeze(rawOutput.Item2)[i]));
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
        /// <param name="rawAgeOutput">
        /// Predicted age in float type.
        /// </param>
        /// <param name="rawGenderOutput">
        /// Predicted gender in float type.
        /// </param>
        /// <returns></returns>
        private Result ConvertToResult(Location faceLoc, float rawAgeOutput, float rawGenderOutput)
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
