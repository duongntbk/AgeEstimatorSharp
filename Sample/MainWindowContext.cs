using System.ComponentModel;
using System.Runtime.CompilerServices;
using AgeEstimatorSharp.ImageProcessing.Annotation;

namespace Sample
{
    public class MainWindowContext : INotifyPropertyChanged
    {
        private string _picturePath;

        private bool _isProcessing;

        private string _formTitle;

        public string PicturePath
        {
            get => _picturePath;
            set
            {
                _picturePath = value;
                // Notify UI of change in value of PicturePath.
                NotifyPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                // Notify UI of change in value of SecretFilePath.
                NotifyPropertyChanged();
            }
        }

        public string FormTitle
        {
            get => _formTitle;
            set
            {
                _formTitle = value;
                // Notify UI of change in value of SecretFilePath.
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Binding path for radio buttons used to select annotation mode.
        /// The default annotation mode is Both.
        /// </summary>
        public FaceDetectionOption FaceDetection { get; set; } = FaceDetectionOption.Hog;

        /// <summary>
        /// Binding path for radio buttons used to select face detection mode.
        /// The default annotation mode is Hog Feature.
        /// </summary>
        public AnnotationOption Annotation { get; set; } = AnnotationOption.Both;

        /// <summary>
        /// Implement PropertyChanged of INotifyPropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handler to notify UI of change in binding data.
        /// Use CallerMemberName to avoid hard coding UI elements' in code behind.
        /// </summary>
        /// <param name="propName"></param>
        public void NotifyPropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
