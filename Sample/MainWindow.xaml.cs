using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        private readonly IRunnable _runner;

        public MainWindowContext Context { get; set; } = new MainWindowContext();

        public MainWindow()
        {
            try
            {
                var inputNode = ConfigurationManager.AppSettings["inputnode"];
                var ageOutputNode = ConfigurationManager.AppSettings["ageoutputnode"];
                var genderOutputNode = ConfigurationManager.AppSettings["genderoutputnode"];
                
                var outputNodes = new ValueTuple<string, string>(ageOutputNode, genderOutputNode);
                _runner = new PbRunnerWithWarmUp(inputNode, outputNodes, 150, 150, 3);

                var meanJsonPath = ConfigurationManager.AppSettings["meanjsonpath"];
                IProcessor meanPreprocessor = new MeanPreprocessor(meanJsonPath);

                IProcessor dividePreprocessor = new DividePreprocessor(127.5);

                IResizable resizer = new FaceResizer();
                _hogLocator = new FaceLocatorDlib();
                _haarLocator = new FaceLocatorOpenCv();

                _predictor = new AgeAndGenderPredictor(_runner, inputNode,
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
            DataContext = Context;
        }

        private void PredictCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Context.IsProcessing &&
                !string.IsNullOrEmpty(Context.PicturePath);
        }

        private async void PredictCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                DisplayStandByGraphic("Predicting...");

                _predictor.Locator = Context.FaceDetection == FaceDetectionOption.Hog ? _hogLocator : _haarLocator;                            
                var rs = new List<Result>();
                await Task.Run(() =>
                {
                    // Tensorflow session was created in a different thread.
                    _predictor.GetDefault();
                    rs = _predictor.Fit(Context.PicturePath);
                });

                if (rs.Count == 0)
                {
                    MessageBox.Show("not found");
                    return;
                }

                _annotator.Option = Context.Annotation;
                var rsImg = _annotator.Annotate(Context.PicturePath, rs);
                ImageUtils.DisplayImage(rsImg, "Result");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK);
            }
            finally
            {
                DisplayDefaultGraphic();
            }
        }

        private void LoadImageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Context.IsProcessing;
        }

        private void LoadImageCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var imagePath = SelectFile();
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            Context.PicturePath = imagePath;
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
        /// Display animation gif when background task is running.
        /// </summary>
        /// <param name="message"></param>
        private void DisplayStandByGraphic(string message)
        {
            Context.IsProcessing = true;
            Context.FormTitle = message;
            GifCtrl.StartAnimate();
        }

        /// <summary>
        /// Restore original layout of program.
        /// </summary>
        private void DisplayDefaultGraphic()
        {
            GifCtrl.StopAnimate();
            Context.FormTitle = "Sample";
            Context.IsProcessing = false;
        }

        private async void MainWindow_OnLoaded(object sender, EventArgs e)
        {
            var modelPath = ConfigurationManager.AppSettings["modelpath"];
            var inputNode = ConfigurationManager.AppSettings["inputnode"];
            var ageOutputNode = ConfigurationManager.AppSettings["ageoutputnode"];
            var genderOutputNode = ConfigurationManager.AppSettings["genderoutputnode"];

            DisplayStandByGraphic("Loading model...");

            await Task.Run(() =>
            {
                _runner.Config = new ModelConfig
                {
                    ModelPath = modelPath,
                    NodeNames = new List<string>
                    {
                        inputNode,
                        ageOutputNode,
                        genderOutputNode,
                    }
                };
            });

            DisplayDefaultGraphic();
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
