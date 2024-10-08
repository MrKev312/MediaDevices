﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace ExplorerCtrl.Converter;

[ValueConversion(typeof(DateTime?), typeof(string))]
internal sealed class DateTimeToShortStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		DateTime? date = (DateTime?)value;
		return date.HasValue ? $"{date.Value.ToShortDateString()} {date.Value.ToShortTimeString()}" : string.Empty;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
