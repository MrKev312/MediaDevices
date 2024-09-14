using System.Windows;

namespace MediaDevicesApp.Mvvm;

public static class MsgBox
{
	public static bool ShowQuestion(string text)
	{
		return MessageBox.Show(Application.Current.MainWindow, text, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
	}

	public static void ShowError(string text)
	{
		MessageBox.Show(Application.Current.MainWindow, text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
	}
}
