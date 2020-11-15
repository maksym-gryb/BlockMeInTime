using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlockMeInTime
{
    class InfoMessage : TextBlock
    {
        private System.Windows.Threading.DispatcherTimer fadeDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer fadingDispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private Color foreground_color = Colors.Red;

        private Color ForegroundColor
        {
            get
            {
                return foreground_color;
            }

            set
            {
                foreground_color = value;
                Foreground = new SolidColorBrush(foreground_color);
            }
        }

        public InfoMessage() : base()
        {
            Background = Brushes.Black;
            Foreground = Brushes.Red;

            Text = "";
        }

        public void Show(int time_to_fade, int fade_duration, string message)
        {
            Text = message;

            fadeDispatcherTimer.Interval = new TimeSpan(0, 0, 0, time_to_fade, 0);
            fadeDispatcherTimer.Tick += Fade;
            fadeDispatcherTimer.Start();

            fadingDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 * fade_duration / 100));
            fadingDispatcherTimer.Tick += Fading;
        }


        private void Fade(object sender, EventArgs e)
        {
            fadeDispatcherTimer.Stop();
            fadingDispatcherTimer.Start();
        }

        private void Fading(object sender, EventArgs e)
        {
            Color faded_color = new Color();

            byte fade_amount = 20;
            faded_color.A = foreground_color.A;
            faded_color.A -= fade_amount;
            faded_color.R = foreground_color.R;
            faded_color.G = foreground_color.G;
            faded_color.B = foreground_color.B;

            if (faded_color.A < fade_amount)
            {
                faded_color.A = 0;
                fadingDispatcherTimer.Stop();
            }

            ForegroundColor = faded_color;
        }
    }
}
