using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace avaloniakiro20260104.ViewModels;

public class NotConverter : IValueConverter
{
    public static readonly NotConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        
        return true; // 預設為 true，如果不是 bool 類型
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
            return !boolValue;
        
        return false;
    }
}