using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Kleeshee.SftpClient.ValueConverters
{
    public class FileSizeToStringConverter : IValueConverter
    {
        private const ulong Kilo = 1024;

        private const ulong Mega = Kilo * 1024;

        private const ulong Giga = Mega * 1024;

        private const ulong Tera = Giga * 1024;

        private const ulong Peta = Tera * 1024;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var culture = new CultureInfo(language);
            var size = value as ulong?;
            if (size == null)
            {
                return null;
            }
            else if (size < Kilo)
            {
                return string.Format(culture.NumberFormat, "{0} B", size);
            }
            else if (size < Mega)
            {
                return string.Format(culture.NumberFormat, "{0:0.00} KB", size / (double)Kilo);
            }
            else if (size < Giga)
            {
                return string.Format(culture.NumberFormat, "{0:0.00} MB", size / (double)Mega);
            }
            else if (size < Tera)
            {
                return string.Format(culture.NumberFormat, "{0:0.00} GB", size / (double)Giga);
            }
            else if (size < Peta)
            {
                return string.Format(culture.NumberFormat, "{0:0.00} TB", size / (double)Tera);
            }
            else
            {
                return string.Format(culture.NumberFormat, "{0:0.00} PB", size / (double)Peta);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
