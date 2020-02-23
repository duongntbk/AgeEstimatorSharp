namespace AgeEstimatorSharp.Preprocessor
{
    /// <summary>
    /// Mean value of R, G and B channels of training data.
    /// </summary>
    internal struct MeanRgb
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
    }
}
