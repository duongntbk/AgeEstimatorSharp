using System.Windows.Input;

namespace Sample
{
    public static class Commands
    {
        public static readonly RoutedUICommand LoadImage = new RoutedUICommand(
            "LoadImage",
            "LoadImage",
            typeof(Commands)
        );

        public static readonly RoutedUICommand Predict = new RoutedUICommand(
            "Predict",
            "Predict",
            typeof(Commands)
        );
    }
}
