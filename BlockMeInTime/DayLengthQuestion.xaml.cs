using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace BlockMeInTime
{
    /// <summary>
    /// Interaction logic for DayLengthQuestion.xaml
    /// </summary>
    public partial class DayLengthQuestion : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public DayLengthQuestion()
        {
            InitializeComponent();
        }

        public void OnKeyDownHandler(object sender, RoutedEventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Key.Enter)
            {
                Close();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }
    }
}
