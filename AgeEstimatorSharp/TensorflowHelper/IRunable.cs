using System;
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

        /// <summary>
        /// Perform calculation on tensor using tensorflow model,
        /// this method is used in case the model has 2 outputs.
        /// </summary>
        /// <param name="inputs">
        /// Input tensor.
        /// </param>
        /// <param name="inputName">
        /// Name of input node of tensorflow model.
        /// </param>
        /// <param name="outputNames">
        /// A value tuple hold the names of all output nodes.
        /// </param>
        /// <returns>
        /// Output tensor
        /// </returns>
        ValueTuple<NDArray, NDArray> Run(NDArray inputs, string inputName, ValueTuple<string, string> outputNames);

        /// <summary>
        /// Call as_default on session and graph
        /// so that session initialization and calculation can run on different threads.
        /// </summary>
        void GetDefault();
    }
}
