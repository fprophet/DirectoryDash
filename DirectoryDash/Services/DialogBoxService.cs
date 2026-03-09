using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirectoryDash.Services
{
    public class DialogBoxService
    {
        public MessageBoxResult ErrorBox(string message)
        {

            return System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult ConfirmBox(string message)
        {

            return System.Windows.MessageBox.Show(message, "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        }

        public MessageBoxResult InfoBox(string message)
        {
            return System.Windows.MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult Warning(string message)
        {
            return System.Windows.MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public MessageBoxResult WarningConfirmation(string message)
        {
            return System.Windows.MessageBox.Show(message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }
    }
}
