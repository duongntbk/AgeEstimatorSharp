using System.Collections.Generic;
using NumSharp;

namespace AgeEstimatorSharp.TensorflowHelper
{
    /// <summary>
    /// Helper class, perform simple tensor manipulating.
    /// </summary>
    public class TensorUtils
    {
        /// <summary>
        /// Convert list of binary array into one tensor.
        /// To create tensor of shape (n, a, b, c),
        /// pass a list of binary array with n elements
        /// and set (a, b, c) as shape.
        /// </summary>
        /// <param name="dataList">
        /// List of input binary array.
        /// </param>
        /// <param name="shape">
        /// Shape of output tensor.
        /// </param>
        /// <returns>
        /// Output tensor.
        /// </returns>
        public static NDArray CreateTensor(List<byte[]> dataList, int[] shape)
        {
            var tensors = new List<NDArray>();

            foreach (var data in dataList)
            {
                var tensor = np.array(data);
                tensor = tensor.astype(NPTypeCode.Float);
                tensor = tensor.reshape(shape);

                tensors.Add(tensor);
            }

            return np.vstack(tensors.ToArray());
        }
    }
}
