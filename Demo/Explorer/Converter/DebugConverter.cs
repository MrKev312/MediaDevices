using System;
using System.Globalization;
using System.Windows.Data;

namespace ExplorerCtrl.Converter;

internal sealed class DebugConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}
