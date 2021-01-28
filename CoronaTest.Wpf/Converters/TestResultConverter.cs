using CoronaTest.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CoronaTest.Wpf.Converters
{
    public class TestResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestResult testResult)
            {
                switch(testResult)
                {
                    case TestResult.Unknown:
                        return "-";
                    case TestResult.Positive:
                        return "positiv";
                    case TestResult.Negative:
                        return "negativ";
                    default:
                        return testResult.ToString();
                }
            }
            else
            {
                throw new ArgumentException($"Argument is not from type {typeof(TestResult)}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
