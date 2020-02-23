using System;
using System.Collections.Generic;

namespace AgeEstimatorSharp.ImageProcessing.Locator
{
    /// <summary>
    /// Abstract base class for all face locators.
    /// </summary>
    public abstract class AbstractFaceLocator : ILocatable
    {
        /// <summary>
        /// Ratio to be used in ExpandFace method.
        /// Each face detector method has its own optimized ratio.
        /// </summary>
        protected float ExpandFactor;

        /// <summary>
        /// Initialize object and set ExpandRatio.
        /// </summary>
        /// <param name="expandRatio"></param>
        protected AbstractFaceLocator(float expandRatio)
        {
            ExpandFactor = expandRatio;
        }

        /// <summary>
        /// Common face detector algorithm only detect the face area without chin, ears or hair.
        /// However those details can be useful to determine both age and gender.
        /// This method is used to widen the detection box
        /// so that chin, ears and hair can also be included.
        /// </summary>
        /// <param name="width">
        /// The width of input image.
        /// </param>
        /// <param name="height">
        /// The height of input image.
        /// </param>
        /// <param name="top">
        /// Top position of detected face.
        /// </param>
        /// <param name="right">
        /// Right position of detected face.
        /// </param>
        /// <param name="bottom">
        /// Bottom position of detected face.
        /// </param>
        /// <param name="left">
        /// Left position of detected face. </param>
        /// <returns></returns>
        protected Location ExpandFace(int width, int height,
            int top, int right, int bottom, int left)
        {
            // Calculate origin height of face area
            var fHeight = bottom - top;
            // Calculate the amount of padding needed for height
            var paddingHeight = (int)Math.Round(fHeight * (ExpandFactor - 1) / 2);
            // Pad the face area at both the top and bottom,
            // to ensure that the face is still horizontally centered.
            // Also make sure that the new rectangle does not go out of bound.
            var exTop = Math.Max(0, top - paddingHeight);
            var exBottom = Math.Min(bottom + paddingHeight, height);

            // Calculate origin width of face area
            var fWidth = right - left;
            // Calculate the amount of padding needed for height
            var paddingWidth = (int)Math.Round(fWidth * (ExpandFactor - 1) / 2);
            // Pad the face area at both the left and right,
            // to ensure that the face is still vertically centered
            // Also make sure that the new rectangle does not go out of bound.
            var exLeft = Math.Max(0, left - paddingWidth);
            var exRight = Math.Min(right + paddingWidth, width);

            return new Location
            {
                Top = exTop,
                Right = exRight,
                Bottom = exBottom,
                Left = exLeft
            };
        }

        /// <inheritdoc />
        public abstract List<Location> GetFaceLocations(string path);

        /// <inheritdoc />
        public abstract List<Location> GetFaceLocations(byte[] bytes);
    }
}
