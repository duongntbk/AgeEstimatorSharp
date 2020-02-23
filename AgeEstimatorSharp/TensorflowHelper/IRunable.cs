using NumSharp;

namespace AgeEstimatorSharp.TensorflowHelper
{
    /// <summary>
    /// Perform calculation on tensor using tensorflow model. 
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Path to model file in pb format
        /// and list of nodes' name.
        /// </summary>
        ModelConfig Config { set; }
        /// <summary>
        /// Perform calculation on tensor using tensorflow model.
        /// </summary>
        /// <param name="inputs">
        /// Input tensor.
        /// </param>
        /// <param name="inputName">
        /// Name of input node of tensorflow model.
        /// </param>
        /// <param name="outputName">
        /// Name of output node of tensorflow model.
        /// </param>
        /// <returns>
        /// Output tensor
        /// </returns>
        NDArray Run(NDArray inputs, string inputName, string outputName);
    }
}
