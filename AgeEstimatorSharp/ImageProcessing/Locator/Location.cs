namespace AgeEstimatorSharp.ImageProcessing.Locator
{
    /// <summary>
    /// This struct is used to store the top, right,
    /// bottom and left position of faces.
    /// Unit is pixel.
    /// </summary>
    public struct Location
    {
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
    }
}
