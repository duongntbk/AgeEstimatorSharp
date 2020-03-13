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
        private readonly IPredictable _predictor;
        private readonly ILocatable _hogLocator;
        private readonly ILocatable _haarLocator;
        private readonly IAnnotation _annotator;

        public MainWindow()
        {
            try
            {
                var inputNode = ConfigurationManager.AppSettings["inputnode"];
                var ageOutputNode = ConfigurationManager.AppSettings["ageoutputnode"];
                var genderOutputNode = ConfigurationManager.AppSettings["genderoutputnode"];

                var modelPath = ConfigurationManager.AppSettings["modelpath"];
                var outputNodes = new ValueTuple<string, string>(ageOutputNode, genderOutputNode);
                IRunnable runner = new PbRunnerWithWarmUp(inputNode, outputNodes, 150, 150, 3)
                {
                    Config = new ModelConfig
                    {
                        ModelPath = modelPath,
                        NodeNames = new List<string>
                        {
                            inputNode,
                            ageOutputNode,
                            genderOutputNode,
                        }
                    }
                };

                var meanJsonPath = ConfigurationManager.AppSettings["meanjsonpath"];
                IProcessor meanPreprocessor = new MeanPreprocessor(meanJsonPath);

                IProcessor dividePreprocessor = new DividePreprocessor(127.5);

                IResizable resizer = new FaceResizer();
                _hogLocator = new FaceLocatorDlib();
                _haarLocator = new FaceLocatorOpenCv();

                _predictor = new AgeAndGenderPredictor(runner, inputNode,
                    ageOutputNode, genderOutputNode)
                {
                    Locator = _hogLocator,
                    Resizer = resizer,
                    Preprocessors = new List<IProcessor>
                    {
                        meanPreprocessor,
                        dividePreprocessor
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

            if (RdGenderOpt.IsChecked.HasValue && RdGenderOpt.IsChecked.Value)
            {
                _annotator.Option = AnnotationOption.Gender;
            }
            else if (RdAgeOpt.IsChecked.HasValue && RdAgeOpt.IsChecked.Value)
            {
                _annotator.Option = AnnotationOption.Age;
            }
            else if (RdBoth.IsChecked.HasValue && RdBoth.IsChecked.Value)
            {
                _annotator.Option = AnnotationOption.Both;
            }
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
