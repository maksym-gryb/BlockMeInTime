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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

// BUG: hover/unhover doesn't save properly the original color

/*  NEXT
 * 
 * # create TimeBlock (class) extends TextBlock
 * 
 * 
 * # make TimeBlocks 30 minute intervals (what is the best way to represent this in code?)
 * # somehow link TimeBlocks together -> TimeBlockChain?
 * 
 * # Top-Left corner can be a fade-in/fade-out message box (i.e. GRID[ 0, 0 ])
 * 
 */

namespace BlockMeInTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    enum MouseState
    {
        DEFAULT,
        DRAGGING_SELECTION,
    }

    public partial class MainWindow : Window
    {
        public static ReadOnlyCollection<string> DaysOfWeek { get; } = new ReadOnlyCollection<string>(new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" });

        private DayLengthQuestion day_length_question_window;
        private BlockDetailsQuestion block_details_question;

        private Grid maingrid;

        private int hours_per_day = 14;
        private int hour_day_starts_at = 8;

        private static Brush textblock_default_background = Brushes.Black;
        private static Brush textblock_hover_background = Brushes.DarkGray;

        private MouseState mouse_state = MouseState.DEFAULT;

        private TextBlock hovering_over = null;
        private TextBlock dragging_object = null;

        private List<TextBlock> dragged_selected_items = new List<TextBlock>();
        private List<TextBlock> timeblocks = new List<TextBlock>();

        private static string default_save_file = "save.bmit";

        public byte hover_factor = 30;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Block Me In Time";

            //AskDayLength();
            //AskDayStartHour();

            GenerateTimeBlocks();

            //LoadFile();
        }

        private string SerializeTimeBlock(TextBlock tb)
        {
            int col = Grid.GetColumn(tb);
            int row = Grid.GetRow(tb);

            if(tb == hovering_over)
            {
                tb.Background = UnHoverColor(tb.Background);
                tb.Foreground = ForegroundBasedOnBackground(tb.Background);
            }

            Color color = ((SolidColorBrush)tb.Background).Color;
            string line = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", tb.Text, col, row, color.R, color.G, color.B, color.A);

            if (tb == hovering_over)
            {
                tb.Background = HoverColor(tb.Background);
                tb.Foreground = ForegroundBasedOnBackground(tb.Background);
            }

            return line;
        }

        /*
         * Base on: https://stackoverflow.com/questions/11867545/change-text-color-based-on-brightness-of-the-covered-background-area
         */
        private Brush ForegroundBasedOnBackground(Brush background)
        {
            return ForegroundBasedOnBackground(((SolidColorBrush)background).Color);
        }

        private Brush ForegroundBasedOnBackground(Color background)
        {
            int r = background.R;
            int g = background.G;
            int b = background.B;

            float brightness = (((((float)r) * 299f) + (((float)g) * 587f) + (((float)b) * 114f)) / 1000f);

            Brush foregroundBrush = (brightness > 125f) ? Brushes.Black : Brushes.White;

            return foregroundBrush;
        }

        private Brush HoverColor(Brush brush)
        {
            return HoverColor(((SolidColorBrush)brush).Color);
        }

        private Brush HoverColor(Color color)
        {
            byte a, r, g, b;
            a = color.A;
            r = color.R;
            g = color.G;
            b = color.B;

            r += hover_factor;
            g += hover_factor;
            b += hover_factor;

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private Brush UnHoverColor(Brush brush)
        {
            return UnHoverColor(((SolidColorBrush)brush).Color);
        }

        private Brush UnHoverColor(Color color)
        {
            byte a, r, g, b;
            a = color.A;
            r = color.R;
            g = color.G;
            b = color.B;

            r -= hover_factor;
            g -= hover_factor;
            b -= hover_factor;

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private TextBlock DeserializeTimeBlock(string serial)
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

            TextBlock tb;

            tb = maingrid.Children.Cast<TextBlock>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
            tb.Text = activity_title;

            Color bgcolor = Color.FromArgb(a, r, g, b);
            SolidColorBrush brush = new SolidColorBrush(bgcolor);
            tb.Background = brush;

            tb.Foreground = ForegroundBasedOnBackground(bgcolor);

            //PlaceInGrid(tb);

            return tb;
        }

        private void PlaceInGrid(TimeBlock tb)
        {
            Grid.SetColumn(tb, tb.Col);
            Grid.SetRow(tb, tb.Row);
            maingrid.Children.Add(tb);
        }

        private void LoadFile()
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(@default_save_file))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    TextBlock tb = DeserializeTimeBlock(line);
                    timeblocks.Add(tb);
                }
            }

            // show information message
            string messageBoxText = "Load file complete";
            string caption = "Load File";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void SaveFile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@default_save_file))
            {
                foreach (TextBlock tb in timeblocks)
                {
                    string line = SerializeTimeBlock(tb);
                    file.WriteLine(line);
                }
            }

            ShowSaveSuccessMessage();
        }

        private void ShowSaveSuccessMessage()
        {
            string messageBoxText = "File was successfully saved";
            string caption = "Save File";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void AskDayLength()
        {
            day_length_question_window = new DayLengthQuestion();
            day_length_question_window.Topmost = true;
            day_length_question_window.ShowDialog();

            hours_per_day = Int32.Parse(day_length_question_window.inputField.Text);
        }

        /* UNTESTED METHOD */
        private Brush GetRandomColour()
        {
            Random r = new Random();
            int min = 1;
            int max = 255;
            return new SolidColorBrush(Color.FromRgb((byte)r.Next(min, max), (byte)r.Next(min, max), (byte)r.Next(min, max)));
        }

        private Brush GetValidBackgroundColor()
        {
            Random r = new Random();
            int min = 50;
            int max = 150;
            return new SolidColorBrush(Color.FromRgb((byte)r.Next(min, max), (byte)r.Next(min, max), (byte)r.Next(min, max)));
        }

        private void GenerateTimeBlocks()
        {
            maingrid = new Grid();
            maingrid.ShowGridLines = false;

            int days_in_week = 5;

            for(int i = days_in_week; i + 1 > 0; i--)
            {
                ColumnDefinition new_column_definition = new ColumnDefinition();
                maingrid.ColumnDefinitions.Add(new_column_definition);
            }

            for(int i = hours_per_day; i + 1 > 0; i--)
            {
                maingrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int y = 0, x = 1; x < days_in_week + 1; x++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = DaysOfWeek[x - 1];
                tb.Background = textblock_default_background;
                Grid.SetColumn(tb, x);
                Grid.SetRow(tb, y);
                maingrid.Children.Add(tb);
            }

            for (int y = 1, x = 0; y < hours_per_day + 1; y++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = (hour_day_starts_at + y - 1).ToString() + " o'clock";
                tb.Background = textblock_default_background;
                tb.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetColumn(tb, x);
                Grid.SetRow(tb, y);
                maingrid.Children.Add(tb);
            }

            for (int x = 1; x < days_in_week + 1; x++)
            {
                for(int y = 1; y < hours_per_day + 1; y++)
                {
                    TextBlock nb = new TextBlock();
                    nb.Text = "--";
                    Grid.SetColumn(nb, x);
                    Grid.SetRow(nb, y);
                    maingrid.Children.Add(nb);
                    nb.Background = textblock_default_background;
                    nb.MouseEnter += TextBlock_MouseEnter;
                    nb.MouseLeave += TextBlock_MouseLeave;
                }
            }

            this.Content = maingrid;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SaveFile();
            }

            if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
            {
                LoadFile();
            }
        }

        private void SetDefaultColor(TextBlock tb)
        {
            tb.Background = textblock_default_background;
        }

        private void SetHoverColor(TextBlock tb)
        {
            tb.Background = textblock_hover_background;
        }

        private void ClearDragging()
        {
            foreach(TimeBlock tb in dragged_selected_items)
            {
                if(tb != hovering_over && !timeblocks.Contains(tb))
                {
                    SetDefaultColor(tb);
                }
            }

            dragged_selected_items.Clear();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            if(hovering_over != null)
            {
                mouse_state = MouseState.DRAGGING_SELECTION;
                dragging_object = hovering_over;
                dragged_selected_items.Add(dragging_object);
            }
        }

        private void AskBlockDetailsQuestion()
        {
            block_details_question = new BlockDetailsQuestion();
            block_details_question.Topmost = true;
            block_details_question.Activate();
            block_details_question.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            block_details_question.ShowDialog();

            if (block_details_question.cancel)
            {
                foreach (TextBlock tb in dragged_selected_items)
                {
                    if (tb != hovering_over)
                    {
                        tb.Background = textblock_default_background;
                        tb.Foreground = ForegroundBasedOnBackground(tb.Background);
                    }
                }
                dragged_selected_items.Clear();
                return;
            }

            string activity_title = block_details_question.inputField.Text;

            foreach (TextBlock tb in dragged_selected_items)
            {
                if (tb != hovering_over)
                {
                    tb.Text = activity_title;
                    tb.Background = GetValidBackgroundColor();
                    tb.Foreground = ForegroundBasedOnBackground(tb.Background);
                }
            }
        }

        private void AddSelectedToTimeBlocks()
        {
            timeblocks.AddRange(dragged_selected_items);
        }

        private void Window_MouseUp(object sender, MouseEventArgs e)
        {
            AskBlockDetailsQuestion();

            AddSelectedToTimeBlocks();

            dragging_object = null;

            ClearDragging();

            mouse_state = MouseState.DEFAULT;
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textblock = sender as TextBlock;

            textblock.Background = HoverColor(textblock.Background);

            hovering_over = textblock;

            if (mouse_state == MouseState.DRAGGING_SELECTION)
            {
                dragged_selected_items.Add(textblock);
            }
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouse_state != MouseState.DRAGGING_SELECTION)
            {
                var textblock = sender as TextBlock;

                textblock.Background = UnHoverColor(textblock.Background);
            }

            hovering_over = null;
        }
    }
}
