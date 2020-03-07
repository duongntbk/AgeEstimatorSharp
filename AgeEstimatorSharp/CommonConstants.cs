namespace AgeEstimatorSharp
{
    /// <summary>
    /// Constants and default settings of library.
    /// </summary>
    public class CommonConstants
    {
        /// <summary>
        /// Settings regarding image.
        /// </summary>
        internal class Image
        {
            /// <summary>
            /// Default depth is 3 (RGB image).
            /// </summary>
            internal const int DefaultColorDepth = 3;
            /// <summary>
            /// Default width of face image is 150px.
            /// </summary>
            internal const int DefaultWidth = 150;
            /// <summary>
            /// Default height of face image is 150px.
            /// </summary>
            internal const int DefaultHeight = 150;
            /// <summary>
            /// Path to default Haar Cascade file. 
            /// </summary>
            internal const string HaarcascadeFrontalFaceAlt2 = "haarcascades\\haarcascade_frontalface_alt2.xml";
        }

        /// <summary>
        /// Settings regarding predictions using tensorflow.
        /// </summary>
        public class Predictor
        {
            /// <summary>
            /// Dummy age to indicate that age was not estimated.
            /// </summary>
            public const float NotPredictAge = -1.0f;
            /// <summary>
            /// Threshold to predict gender.
            /// </summary>
            internal const double Threshold = 0.5;
        }
    }
}
