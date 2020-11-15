using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockMeInTime
{
    // TODO: Move all TimeBlock related logic here
    class TimeBlockData
    {
        public int id;

        /*
        public int from_minutes;
        public int to_minutes;

        public int day_of_week;
        */
        public string text { get; set; }
        public Color _original_background_color { get; set; }
        public int col { get; set; }
        public int row { get; set; }
    }

    class TimeBlock : TextBlock
    {
        public TimeBlockData data = new TimeBlockData();

        public TimeBlockData Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
                Message = data.text;
                OriginalBackground = new SolidColorBrush(data._original_background_color);
            }
        }

        public static int next_id = 0;

        public static byte hover_factor = 30;

        public static SolidColorBrush textblock_default_background = Brushes.Black;
        public static SolidColorBrush textblock_hover_background = Brushes.DarkGray;

        public SolidColorBrush _original_background { get; set; }

        public int Col
        {
            get
            {
                return data.col;
            }

            set
            {
                data.col = value;
            }
        }

        public int Row
        {
            get
            {
                return data.row;
            }

            set
            {
                data.row = value;
            }
        }

        public string Message
        {
            get
            {
                return data.text;
            }

            set
            {
                data.text = value;
                Text = value;
            }
        }

        public SolidColorBrush OriginalBackground
        {
            get
            {
                return _original_background;
            }

            set
            {
                _original_background = value;
                Background = value;
                ForegroundBasedOnBackground();

                data._original_background_color = _original_background.Color;
            }
        }

        public TimeBlock(int _col, int _row) : base()
        {
            data.id = TimeBlock.next_id++;

            Col = _col;
            Row = _row;

            OriginalBackground = TimeBlock.textblock_default_background;

            MouseEnter += HoverOver;
            MouseLeave += HoverAway;
        }

        public TimeBlock(TimeBlockData _data) : base()
        {
            _data.id = TimeBlock.next_id++;


            Data = _data;

            MouseEnter += HoverOver;
            MouseLeave += HoverAway;
        }

        private void ForegroundBasedOnBackground()
        {
            //Color color = ((SolidColorBrush)data._original_background).Color;
            Color color = ((SolidColorBrush)OriginalBackground).Color;

            int r = color.R;
            int g = color.G;
            int b = color.B;

            float brightness = (((((float)r) * 299f) + (((float)g) * 587f) + (((float)b) * 114f)) / 1000f);

            Brush foregroundBrush = (brightness > 125f) ? Brushes.Black : Brushes.White;

            Foreground = foregroundBrush;
        }

        public string Serialize()
        {
            Col = Grid.GetColumn(this);
            Row = Grid.GetRow(this);

            string line = JsonSerializer.Serialize(Data);

            return line;
        }

        public static TimeBlock Deserialize(string serial)
        {
            TimeBlockData _data = JsonSerializer.Deserialize<TimeBlockData>(serial);

            TimeBlock tb = new TimeBlock(_data);

            return tb;
        }

        public void HoverOver(object sender, MouseEventArgs e)
        {
            HoverOver();
        }

        public void HoverOver()
        {
            HoverColor();
        }

        public void HoverAway(object sender, MouseEventArgs e)
        {
            HoverAway();
        }

        public void HoverAway()
        {
            if(UserInputState.GetUserInputState().state != UserInputStateEnum.DRAGGING_SELECTION)
            {
                UnHoverColor();
            }
        }

        public void ResetToOriginalBackground()
        {
            Background = OriginalBackground;
            ForegroundBasedOnBackground();
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
            ResetToOriginalBackground();
        }
    }
}
