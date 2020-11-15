using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BlockMeInTime
{
    class InfoMessage : WrapPanel
    {
        private TextBlock text;
        private System.Windows.Threading.DispatcherTimer fadeDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer fadingDispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private float fade_duration_interval;
        InfoMessage() : base()
        {
            this.AddVisualChild(text);
        }

        /*

        public void PopUp(int time_to_fade, int fade_duration, string message)
        {
            text.Text = message;

            fadeDispatcherTimer.Interval = new TimeSpan(0, 0, 0, time_to_fade, 0);
            fadeDispatcherTimer.Tick += Fade;
            fadeDispatcherTimer.Start();

            fadingDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 * fade_duration / 20));

            base.Show();
        }

        private void Fade(object sender, EventArgs e)
        {
            fadingDispatcherTimer.Start();
        }

        private void Fading(object sender, EventArgs e)
        {
            fadeDispatcherTimer.Stop();
            this.Background.alpha; //get alpha

            if (alpha < 20) -> base.Hide(); && fadingDispatcherTimer.Stop();
        }

        */
    }
}
