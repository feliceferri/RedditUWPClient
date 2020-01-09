﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RedditUWPClient.Converters
{
    internal class IsNullToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language) 
        {
            if(value == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }
        

        public object ConvertBack(object value, Type targetType, object parameter, string language) 
        {
                return null;
        }


    }
}


