using DirectoryDash.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DirectoryDash.Converters
{
    public class ContainerChainToEnumerableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as ContainerViewModel;
            var list = new List<ContainerViewModel>();
            while (current != null)
            {
                list.Add(current);
                //current = current.ChildContainerViewModel;
            }
            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
