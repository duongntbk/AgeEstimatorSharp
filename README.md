# About

Ever heard of Microsoft's Age Guesser or any of its clone? Want to try it out but afraid that your picture(s) might be collected without you knowing? Then worry no more, this repository will allow you to guess your own age from your picture, using your own computer.

This is a class library to detect face(s) from a picture and apply deep learning technology to guess the gender and age of all people detected. It also comes with a sample program which you can run without installing.

We use a model called AgeGenderNet, which was trained on the [AFAD](https://afad-dataset.github.io/) dataset using fine-tuning based on the VGG19 architecture (originally trained on the ImageNet database). It has archived a 97.01% accuracy for gender predicting and a Mean Absolute Error of 3.67 for age guessing.

# Menu

* [About](#about)
* [The AFAD dataset](#the-afad-dataset)
* [System requirements and installing](#system-requirements-and-installing)
    - [Software version](#software-version)
    - [Installing](#installing)
    - [Pre-trained model](#pre-trained-model)
* [Manual](#manual)
    - [AgeEstimatorSharp manual](#ageestimatorsharp-manual)
        - [If you only want to predict gender](#if-you-only-want-to-predict-gender)
        - [If you only want to guess age](#if-you-only-want-to-guess-age)
        - [If you want to guess both gender and age](#if-you-want-to-guess-both-gender-and-age)
        - [Use the predictor object created above](#use-the-predictor-object-created-above)
    - [How to use your own models](#how-to-use-your-own-models)
    - [Sample program manual](#sample-program-manual)
* [License](#license)

# The AFAD dataset

This is a Asian faces dataset, which contains more than 160,000 facial images. It has samples belong to both sexes, and the age range is mostly in the 15~40 range.

Because of that, while gender predicting can work reasonably well for people of all races and age groups; age guessing performs best for Asian people in the 15-40 age range. Especially for the 20-30 age range, I have achieved some remarkable results.

See my repository below for the detail's information about AgeGenderNet.

https://github.com/duongntbk/AgeGenderNet

# System requirements and installing

## Software version

- OS: Windows 64 bit
- .NET Framework: 4.6.1 and above
- Tensorflow.NET v0.14.0
- SciSharp v1.15.0
- NumSharp v0.20.5
- DlibDotNet v19.18.0.20191202
- OpenCvSharp3 v4.0.0.20181129

## Installing

Nuget packages is not ready yet. To use this library, clone this repository, add AgeEstimatorSharp project to your solution and add a new reference to it.

On the other hand, the sample program is ready to be used as is (if you have .NET Framework 4.6.1 or above). 
- Download and extract the executable file. Beware that due to all dependent libraries, the sample program takes up nearly 90MB compressed.
- Open *Sample.exe.config*, modify row 333~337 as below:
    - *meanjsonpath*: channel-wise mean value of training data, use to normalize input data.
    - *modelpath*: absolute path to AgeGenderNet model. This model should be in pb format.
    - *inputnode*: name of input node of pb graph.
    - *ageoutputnode*: name of output node of pb graph, age guessing branch.
    - *genderoutputnode*: name of output node of pb graph, gender prediction branch.

All dll can be download from *release* tab.

## Pre-trained model

You can download a pre-trained model of AgeGenderNet from the link below. Please note that while AgeEstimatorSharp is GNUv3 licensed, the model itself was trained on AFAD dataset, which is made available for academic research purpose only. Because of that, the pre-trained model can also only be used for academic research purpose as well.

https://drive.google.com/open?id=14F8kXnru3o7UHYaIZSXiDRVFCbxtHrXR

# Manual

## AgeEstimatorSharp manual

### Create necessary objects

Create a face detector object. You can choose between HOG Feature using DlibDotNet.

    var expandRatio = 1.2f;
    hogLocator = new FaceLocatorDlib(expandRatio);

Or Haar Cascade using OpenCvSharp3.

    var expandRatio = 1.0f;
    var haarCascadePath = "C:\\haarcascade_frontalface_alt2.xml";
    haarLocator = new FaceLocatorOpenCv(expandRatio, haarCascadePath);

You can also add your own face detector by implementing the *ILocatable* interface.

Create an object to extract and resize all face regions from picture.

    resizer = new FaceResizer();

You can also add your own resizer by implementing the *IResizable* interface.

Create a list of preprocessors to perform normalization on input data.

    meanJsonPath = "C:\\mean.json";
    meanPreprocessor = new MeanPreprocessor(meanJsonPath); // mean normalization
    dividePreprocessor = new DividePreprocessor(127.5); // normalize input data into [-1, 1] range
    preprocessors = new List<IProcessor>
    {
        meanPreprocessor,
        dividePreprocessor
    };

You can add your own preprocessor by implementing the *IProcessor* interface.

### If you only want to predict gender

Create a runner object to perform gender predicting.

    var modelPath = "C:\\agegendernet.pb"; // Path to pb graph of gender predicting model
    var inputNode = "root_input"; // name of input node of pb graph above
    var outputNode = "gender_output/Sigmoid"; // name of output node of pb graph above
    runner = new PbRunner
    {
        Config = new ModelConfig
        {
            ModelPath = modelPath,
            NodeNames = new List<string>
            {
                inputNode,
                outputNode
            }
        }
    };

Create an object of class *GenderClassifier* and pass all objects created above to it.

    var faceHeight = 150 // height of the face area in training data
    var faceWidth = 150 // width of the face area in training data
    var faceDepth = 3 // number of color channels of the face area in training data
    predictor = new GenderClassifier(runner, inputNode, outputNode, faceWidth, faceHeight, faceDepth)
    {
        Locator = hogLocator, // or haarLocator
        Resizer = resizer,
        Preprocessors = preprocessors // you can add or remove preprocessors as needed
    };

### If you only want to guess age

Create a runner object to perform age guessing.

    var modelPath = "C:\\agegendernet.pb"; // Path to pb graph of gender predicting model
    var inputNode = "root_input"; // name of input node of pb graph above
    var outputNode = "age_output/BiasAdd"; // name of output node of pb graph above, age guessing branch
    runner = new PbRunner
    {
        Config = new ModelConfig
        {
            ModelPath = modelPath,
            NodeNames = new List<string>
            {
                inputNode,
                outputNode
            }
        }
    };

Create an object of class *AgeEstimator* and pass all objects created above to it.

    var faceHeight = 150 // height of the face area in training data
    var faceWidth = 150 // width of the face area in training data
    var faceDepth = 3 // number of color channels of the face area in training data
    predictor = new AgeEstimator(runner, inputNode, outputNode, faceWidth, faceHeight, faceDepth)
    {
        Locator = hogLocator, // or haarLocator
        Resizer = resizer,
        Preprocessors = preprocessors // you can add or remove preprocessors as needed
    };

### If you want to guess both gender and age

Create a runner object to perform both age guessing and gender predicting.

    var modelPath = "C:\\agegendernet.pb"; // Path to pb graph of gender predicting model
    var inputNode = "root_input"; // name of input node of pb graph above
    var ageOutputNode = "age_output/BiasAdd"; // name of output node of pb graph above, age guessing branch
    var genderOutputNode = "gender_output/Sigmoid"; // name of output node of pb graph above, gender predicting branch
    runner = new PbRunner
    {
        Config = new ModelConfig
        {
            ModelPath = modelPath,
            NodeNames = new List<string>
            {
                inputNode,
                ageOutputNode,
                genderOutputNode
            }
        }
    };

Create an object of class *AgeAndGenderPredictor* and pass all objects created above to it.

    var faceHeight = 150 // height of the face area in training data
    var faceWidth = 150 // width of the face area in training data
    var faceDepth = 3 // number of color channels of the face area in training data
    predictor = new AgeAndGenderPredictor(runner,
                    inputNode, ageOutputNode, genderOutputNode,
                    faceWidth, faceHeight, faceDepth)
    {
        Locator = hogLocator, // or haarLocator
        Resizer = resizer,
        Preprocessors = preprocessors // you can add or remove preprocessors as needed
    };

### Use the predictor object created above

Load input image from disk and perform prediction.

    var imagePath = "C:\\my_picture.jpg"; // PNG or GIF should work as well
    var rs = predictor.Fit(imagePath);

You can also perform prediction on image already loaded into memory.

    // byte[] data = get the data somehow
    var rs = predictor.Fit(data);
    
## How to use your own models

When using your own model, set *faceHeight*, *faceWidth*, *faceDepth*, *inputNode*, *ageOutputNode*, *genderOutputNode* to match the value of your model.

For example: the input of your model are greyscale images of size 28x28. And the node names of your model is *input*, *ageOut*, *genderOut*. 

    var modelPath = "C:\\your_gender_model.pb";
    var inputNode = "input";
    var genderOutputNode = "genderOut";
    var ageOutputNode = "ageOut";
    runner = new PbRunner
    {
        Config = new ModelConfig
        {
            ModelPath = modelPath,
            NodeNames = new List<string>
            {
                input,
                ageOutputNode,
                genderOutputNode
            }
        }
    };
    
    var faceHeight = 28
    var faceWidth = 28
    var faceDepth = 1
    predictor = new AgeAndGenderPredictor(runner,
        inputNode, ageOutputNode, genderOutputNode,
        faceWidth, faceHeight, faceDepth)
    {
        Locator = hogLocator, // or haarLocator
        Resizer = resizer,
        Preprocessors = preprocessors // you can add or remove preprocessors as needed
    };

Now you can use *AgeAndGenderPredictor* to predict gender and guess age.

## Sample program manual

Start the program. Loading might take a few seconds, but after that the program should be quite fast.

<img src = "./Sample/Image/gui.JPG">

- ①: select and load target image from disk.
- ②: Choose face detection method. You can choose between HOG Feature using DlibDotNet library or Haar Cascade using OpenCvSharp3 library. Haar Cascade is more sensitive but is also more prone to false positive.
- ③: Select whether to predict gender, age or both.
- ④: Start prediction.

Some results:

<p align="center">
    <img src = "./Sample/Image/Vietnames_football_team_2019.png">
    <br/>
    <label>The Vietnamese football team, picture taken November 2019</label>
    <br/><br/>
    <img src = "./Sample/Image/Tran_Dinh_Trong_2018.png">
    <br/>
    <label>Vietnamese footballer Trần Đình Trọng, picture taken at Asiad 2018, age around 21.5</label>
    <br/><br/>
    <img src = "./Sample/Image/Do_Duy_Manh_2019.png">
    <br/>
    <label>Vietnamese footballer Đỗ Duy Mạnh, picture taken late 2019, age around 23.3</label>
</p>

# License

GNU General Public License v3.0

https://www.gnu.org/licenses/
