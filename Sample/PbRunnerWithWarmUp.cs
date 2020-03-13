using System;
using AgeEstimatorSharp.TensorflowHelper;
using NumSharp;

namespace Sample
{
    /// <summary>
    /// Sample class to demonstrate how to extend PbRunner class.
    /// Execute tensor calculation on a dummy tensor right after re-loading model parameters
    /// so that the first time we perform calculation for real
    /// we don't need to wait for session init.
    /// </summary>
    internal class PbRunnerWithWarmUp : PbRunner
    {
        /// <summary>
        /// Name of input node.
        /// </summary>
        private readonly string _inputNode;
        /// <summary>
        /// Name of output nodes.
        /// </summary>
        private readonly ValueTuple<string, string> _outputNodes;
        /// <summary>
        /// Width of face image.
        /// </summary>
        private readonly int _width;
        /// <summary>
        /// Height of face image.
        /// </summary>
        private readonly int _height;
        /// <summary>
        /// Depth of face image.
        /// </summary>
        private readonly int _depth;

        /// <summary>
        /// Initialize object.
        /// Set name of input, output node
        /// as well as size of face image.
        /// </summary>
        /// <param name="inputNode">
        /// Name of input node.
        /// </param>
        /// <param name="outputNodes">
        /// Name of output node.
        /// </param>
        /// <param name="width">
        /// Width of face image.
        /// </param>
        /// <param name="height">
        /// Height of face image.
        /// </param>
        /// <param name="depth">
        /// Depth of face image.
        /// </param>
        public PbRunnerWithWarmUp(string inputNode, ValueTuple<string, string> outputNodes,
            int width, int height, int depth)
        {
            _inputNode = inputNode;
            _outputNodes = outputNodes;
            _width = width;
            _height = height;
            _depth = depth;
        }

        /// <inheritdoc />
        /// <param name="config"></param>
        protected override void LoadModel(ModelConfig config)
        {
            base.LoadModel(config);
            // Warm up the model after reloading parameters.
            WarmUp();
        }

        /// <summary>
        /// Perform calculation on a dummy tensor to warm up the model.
        /// </summary>
        private void WarmUp()
        {
            var dummyInputs = np.ones(1, _width, _height, _depth);
            Run(dummyInputs, _inputNode, _outputNodes);
        }
    }
}
