using System.Collections.Generic;

namespace AgeEstimatorSharp.TensorflowHelper
{
    /// <summary>
    /// Configuration for tensorflow model's runner.
    /// </summary>
    public struct ModelConfig
    {
        /// <summary>
        /// Path to tensorflow model in pb format.
        /// </summary>
        public string ModelPath { get; set; }
        /// <summary>
        /// Name of every output and input nodes of tensorflow graph. 
        /// </summary>
        public List<string> NodeNames { get; set; }
    }
}
