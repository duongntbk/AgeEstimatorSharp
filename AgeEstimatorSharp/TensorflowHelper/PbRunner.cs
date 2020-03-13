using System;
using System.Collections.Generic;
using System.IO;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;

namespace AgeEstimatorSharp.TensorflowHelper
{
    /// <summary>
    /// Generic class to perform tensor calculation.
    /// </summary>
    public class PbRunner : IRunnable
    {
        /// <summary>
        /// Key is operation name while value is the operation itself.
        /// </summary>
        private readonly Dictionary<string, Operation> _operationDict =
            new Dictionary<string, Operation>();
        /// <summary>
        /// Graph of tensorflow model, loaded from pb file.
        /// </summary>
        private readonly Graph _graph = new Graph();
        /// <summary>
        /// Current tensorflow session.
        /// </summary>
        private Session _session;
        /// <summary>
        /// Path to tensorflow model in pb format.
        /// </summary>
        protected string ModelPath;

        /// <summary>
        /// Configuration of current model.
        /// </summary>
        public ModelConfig Config
        {
            set
            {
                // Check if model is changed before reloading settings
                if (value.ModelPath != ModelPath)
                {
                    try
                    {
                        LoadModel(value);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        throw new Exception(
                            $"Cannot file model folder. Please check if model path is correct. The current model path is: {value.ModelPath}", ex);
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new Exception(
                            $"Cannot find pb file. Please check if model path is correct. The current model path is: {value.ModelPath}", ex);
                    }
                    catch (ValueError ex)
                    {
                        throw new Exception(
                            "Cannot load operations from pb graph. Please check if all node names are correct", ex);
                    }
                }
            }
        }

        /// <inheritdoc />
        public NDArray Run(NDArray inputs, string inputName, string outputName)
        {
            var inputOperation = _operationDict[inputName];
            var outputOperation = _operationDict[outputName];

            var results = _session.run(outputOperation.outputs[0],
                new FeedItem(inputOperation.outputs[0], inputs));
            return np.squeeze(results);
        }

        /// <inheritdoc />
        public ValueTuple<NDArray, NDArray> Run(NDArray inputs, string inputName, ValueTuple<string, string> outputNames)
        {
            var fetches = new ValueTuple<ITensorOrOperation, ITensorOrOperation>
            {
                Item1 = _operationDict[outputNames.Item1].outputs[0],
                Item2 = _operationDict[outputNames.Item2].outputs[0]
            };

            var inputOperation = _operationDict[inputName];
            return _session.run(fetches, new FeedItem(inputOperation.outputs[0], inputs));
        }

        /// <summary>
        /// Load model settings from pb file.
        /// </summary>
        /// <param name="config"></param>
        protected virtual void LoadModel(ModelConfig config)
        {
            // Update current model path
            ModelPath = config.ModelPath;

            // Import graph from pb file
            _graph.Import(ModelPath);

            // Create tensorflow session from current graph
            _session = tf.Session(_graph);

            // Re-initialize operation dictionary
            _operationDict.Clear();

            // Load all input/output operations by name
            // and store in dictionary
            foreach (var name in config.NodeNames)
            {
                var operation = _graph.OperationByName(name);
                _operationDict[name] = operation;
            }
        }
    }
}
