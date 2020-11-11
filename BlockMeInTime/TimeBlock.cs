using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlockMeInTime
{
    // TODO: Move all TimeBlock related logic here
    class TimeBlock : TextBlock
    {
        public const int minimum_block_size = 50;// pixels per time block

        private static Brush textblock_default_background = Brushes.Black;
        private static Brush textblock_hover_background = Brushes.DarkGray;

        public int from_minutes;
        public int to_minutes;

        public int day_of_week;

        public String message;

        private Brush _original_background;

        /*
        public Brush Background
        {
            get
            {
                return Background;
            }

            set
            {
                Background = value;
                ForegroundBasedOnBackground();
            }
        }
        */

        private int col;
        public int Col
        {
            get
            {
                return col;
            }
        }
        private int row;
        public int Row
        {
            get
            {
                return row;
            }
        }

        public static byte hover_factor = 30;
        public TimeBlock(int _col, int _row) : base()
        {
            col = _col;
            row = _row;

            _original_background = textblock_default_background;
            Background = textblock_default_background;
        }

        public Brush OriginalBackground
        {
            get
            {
                return _original_background;
            }

            set
            {
                _original_background = value;
            }
        }

        private void ForegroundBasedOnBackground()
        {
            Color color = ((SolidColorBrush)_original_background).Color;

            int r = color.R;
            int g = color.G;
            int b = color.B;

            float brightness = (((((float)r) * 299f) + (((float)g) * 587f) + (((float)b) * 114f)) / 1000f);

            Brush foregroundBrush = (brightness > 125f) ? Brushes.Black : Brushes.White;

            Foreground = foregroundBrush;
        }

        private string SerializeTimeBlock()
        {
            int col = Grid.GetColumn(this);
            int row = Grid.GetRow(this);

            Color color = ((SolidColorBrush)_original_background).Color;
            string line = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", Text, col, row, color.R, color.G, color.B, color.A);

            return line;
        }

        public static TimeBlock DeserializeTimeBlock(string serial)
        {
            string activity_title = "";
            int col;
            int row;

            byte r, g, b, a;

            string[] split = serial.Split(':');
            activity_title = "";
            a = byte.Parse(split[split.Length - 1]);
            b = byte.Parse(split[split.Length - 2]);
            g = byte.Parse(split[split.Length - 3]);
            r = byte.Parse(split[split.Length - 4]);
            row = int.Parse(split[split.Length - 5]);
            col = int.Parse(split[split.Length - 6]);
            for (int i = 0; i < split.Length - 6; i++)
            {
                activity_title += split[i];
            }

            TimeBlock tb = new TimeBlock(col, row);

            tb.Text = activity_title;

            Color bgcolor = Color.FromArgb(a, r, g, b);
            SolidColorBrush brush = new SolidColorBrush(bgcolor);
            tb.Background = brush;
            tb.ForegroundBasedOnBackground();

            return tb;
        }

        public void HoverOver()
        {
            HoverColor();
        }

        public void HocerAway()
        {
            UnHoverColor();
        }

        private void HoverColor()
        {
            Color color = ((SolidColorBrush)Background).Color;

            byte a, r, g, b;
            a = color.A;
            r = color.R;
            g = color.G;
            b = color.B;

            r += hover_factor;
            g += hover_factor;
            b += hover_factor;

            Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            ForegroundBasedOnBackground();
        }

        private void UnHoverColor()
        {
            Background = _original_background;
            ForegroundBasedOnBackground();
        }
    }
}
