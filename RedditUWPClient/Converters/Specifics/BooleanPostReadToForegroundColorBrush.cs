using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace RedditUWPClient.Converters
{
    internal class BooleanPostReadToForegroundColorBrush : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
       
            if (value != null && value is bool v)
            {
                if (v == true)
                    return new SolidColorBrush( Colors.LightGray);
                else
                    return new SolidColorBrush(Colors.White);
            }

            return new SolidColorBrush(Colors.White);
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }

     
    }
}


