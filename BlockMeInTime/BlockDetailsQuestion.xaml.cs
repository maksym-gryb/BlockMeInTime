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

namespace BlockMeInTime
{
    /// <summary>
    /// Interaction logic for BlockDetailsQuestion.xaml
    /// </summary>
    public partial class BlockDetailsQuestion : Window
    {
        public bool cancel = true;
        public BlockDetailsQuestion()
        {
            InitializeComponent();
        }

        public void OnKeyDownHandler(object sender, RoutedEventArgs e)
        {
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Key.Enter)
            {
                cancel = false;
                Close();
            }
            if(ke.Key == Key.Escape)
            {
                cancel = true;
                Close();
            }
        }
    }
}
