using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using AgeEstimatorSharp.ImageProcessing;
using AgeEstimatorSharp.ImageProcessing.Annotation;
using AgeEstimatorSharp.ImageProcessing.Locator;
using AgeEstimatorSharp.ImageProcessing.Resizer;
using AgeEstimatorSharp.Predictor;
using AgeEstimatorSharp.Preprocessor;
using AgeEstimatorSharp.TensorflowHelper;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRunnable _ageRunner;
        private readonly IRunnable _genderRunner;
        private readonly IProcessor _unitPreprocessor;
        private readonly IProcessor _meanPreprocessor;
        private IPredictable _predictor;
        private readonly IResizable _resizer;
        private readonly ILocatable _hogLocator;
        private readonly ILocatable _haarLocator;
        private readonly IAnnotation _annotator;
        private readonly string _ageInputNode;
        private readonly string _ageOutputNode;
        private readonly string _genderInputNode;
        private readonly string _genderOutputNode;

        public MainWindow()
        {
            try
            {
                _ageInputNode = ConfigurationManager.AppSettings["ageinputnode"];
                _ageOutputNode = ConfigurationManager.AppSettings["ageoutputnode"];
                _genderInputNode = ConfigurationManager.AppSettings["genderinputnode"];
                _genderOutputNode = ConfigurationManager.AppSettings["genderoutputnode"];

                var ageModelPath = ConfigurationManager.AppSettings["agemodelpath"];
                _ageRunner = new PbRunnerWithWarmUp(_ageInputNode, _ageOutputNode, 150, 150, 3)
                {
                    Config = new ModelConfig
                    {
                        ModelPath = ageModelPath,
                        NodeNames = new List<string>
                        {
                            _ageInputNode,
                            _ageOutputNode
                        }
                    }
                };

                var genderModelPath = ConfigurationManager.AppSettings["gendermodelpath"];
                _genderRunner = new PbRunnerWithWarmUp(_genderInputNode, _genderOutputNode, 150, 150, 3)
                {
                    Config = new ModelConfig
                    {
                        ModelPath = genderModelPath,
                        NodeNames = new List<string>
                        {
                            _genderInputNode,
                            _genderOutputNode
                        }
                    }
                };

                var meanJsonPath = ConfigurationManager.AppSettings["meanjsonpath"];
                _meanPreprocessor = new MeanPreprocessor(meanJsonPath);

                _unitPreprocessor = new UnitPreprocessor();

                _resizer = new FaceResizer();
                _hogLocator = new FaceLocatorDlib();
                _haarLocator = new FaceLocatorOpenCv();

                _predictor = new AgeAndGenderPredictor(_genderRunner, _ageRunner,
                    _ageInputNode, _ageOutputNode,
                    _genderInputNode, _genderOutputNode)
                {
                    Locator = _hogLocator,
                    Resizer = _resizer,
                    Preprocessors = new List<IProcessor>
                    {
                        _meanPreprocessor,
                        _unitPreprocessor
                    }
                };

                _annotator = new Annotator();
            }
            catch (Exception ex)
            {
                var message = $"An error has occured : {Environment.NewLine}{ex.Message}";

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += $"{Environment.NewLine}{ex.Message}";
                }

                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            InitializeComponent();
        }

        private void BtnPredict_Click(object sender, RoutedEventArgs e)
        {
            var rs = _predictor.Fit(TbtImgSrc.Text);
            if (rs.Count == 0)
            {
                MessageBox.Show("not found");
                return;
            }

            var rsImg = _annotator.Annotate(TbtImgSrc.Text, rs);
            ImageUtils.DisplayImage(rsImg, "Result");
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var imagePath = SelectFile();
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            TbtImgSrc.Text = imagePath;
        }

        private string SelectFile()
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {

                // Set filter for file extension and default file extension 
                DefaultExt = ".png",
                Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|GIF Files (*.gif)|*.gif"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name
            return result == true ? dlg.FileName : null;
        }

        /// <summary>
        /// Change predictor depending on which radio button is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PredictionOptionChecked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            var currentPreprocessor = _predictor.Preprocessors;
            var currentLocator = _predictor.Locator;
            var currentResizer = _predictor.Resizer;

            if (RdGenderOpt.IsChecked.HasValue && RdGenderOpt.IsChecked.Value)
            {
                _predictor = new GenderClassifier(_genderRunner, _genderInputNode, _genderOutputNode);
            }
            else if (RdAgeOpt.IsChecked.HasValue && RdAgeOpt.IsChecked.Value)
            {
                _predictor = new AgeEstimator(_ageRunner, _ageInputNode, _ageOutputNode);
            }
            else if (RdBoth.IsChecked.HasValue && RdBoth.IsChecked.Value)
            {
                _predictor = new AgeAndGenderPredictor(_genderRunner, _ageRunner,
                    _ageInputNode, _ageOutputNode,
                    _genderInputNode, _genderOutputNode);
            }

            _predictor.Locator = currentLocator;
            _predictor.Resizer = currentResizer;
            _predictor.Preprocessors = currentPreprocessor;
        }

        /// <summary>
        /// Change face detector depending on which radio button was selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceDetectionOptionChecked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }

            if (RdHogMethod.IsChecked.HasValue && RdHogMethod.IsChecked.Value)
            {
                _predictor.Locator = _hogLocator;
            }
            else if (RdHaarMethod.IsChecked.HasValue && RdHaarMethod.IsChecked.Value)
            {
                _predictor.Locator = _haarLocator;
            }
        }
    }
}
