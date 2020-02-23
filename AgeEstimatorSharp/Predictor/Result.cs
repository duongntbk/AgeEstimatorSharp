using AgeEstimatorSharp.ImageProcessing.Locator;

namespace AgeEstimatorSharp.Predictor
{
    /// <summary>
    /// This struct is used to store the position of individual face,
    /// as well as predicted results from tensorflow model.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Location of face in (top, right, bottom, left) format.
        /// </summary>
        public Location Loc { get; set; }
        /// <summary>
        /// Predicted gender.
        /// </summary>
        public Gender Gender { get; set; } = Gender.NotPredict;
        /// <summary>
        /// Estimated age.
        /// </summary>
        public float Age { get; set; } = CommonConstants.Predictor.NotPredictAge;
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        NotPredict = -1
    }
}
