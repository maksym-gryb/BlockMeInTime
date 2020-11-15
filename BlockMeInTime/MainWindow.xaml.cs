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
using System.IO.Packaging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockMeInTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static ReadOnlyCollection<string> DaysOfWeek { get; } = new ReadOnlyCollection<string>(new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" });

        private DayLengthQuestion day_length_question_window;
        private BlockDetailsQuestion block_details_question;

        private Grid maingrid;

        private int hours_per_day = 14;
        private int hour_day_starts_at = 8;

        private TimeBlock hovering_over = null;
        private TimeBlock dragging_object = null;

        private List<TimeBlock> dragged_selected_items = new List<TimeBlock>();
        private List<TimeBlock> timeblocks = new List<TimeBlock>();

        private TimeBlock[][] tb_array;

        private static string default_save_file = "save.bmit";
        private static string default_backup_file = "save.bmit.bck";

        public byte hover_factor = 30;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Block Me In Time";

            //AskDayLength();
            //AskDayStartHour();

            tb_array = new TimeBlock[hours_per_day][];

            GenerateTimeBlocks();

            if(System.IO.File.Exists(default_save_file))
            {
                if (new System.IO.FileInfo(default_save_file).Length == 0 && System.IO.File.Exists(default_backup_file))
                {
                    CopyBackup();
                }

                //LoadFile(default_save_file);
                //SaveFile();
            }
        }

        private SolidColorBrush ForegroundBasedOnBackground(Color background)
        {
            int r = background.R;
            int g = background.G;
            int b = background.B;

            float brightness = (((((float)r) * 299f) + (((float)g) * 587f) + (((float)b) * 114f)) / 1000f);

            SolidColorBrush foregroundBrush = (brightness > 125f) ? Brushes.Black : Brushes.White;

            return foregroundBrush;
        }

        private TimeBlock DeserializeTimeBlock(string serial)
        {
            TimeBlock tb = TimeBlock.Deserialize(serial);

            PlaceInGrid(tb);

            return tb;
        }

        private void PlaceInGrid(TimeBlock tb)
        {
            Grid.SetColumn(tb, tb.Col);
            Grid.SetRow(tb, tb.Row);
            maingrid.Children.Add(tb);
        }

        private void LoadFile(string filename)
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(filename))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    TimeBlock tb = DeserializeTimeBlock(line);
                    timeblocks.Add(tb);
                    tb.MouseEnter += TimeBlock_MouseEnter;
                    tb.MouseLeave += TimeBlock_MouseLeave;
                }
            }

            // show information message
            string messageBoxText = "Load file complete";
            string caption = "Load File";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;

            //MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void CopyBackup()
        {
            if (System.IO.File.Exists(default_backup_file))
            {
                System.IO.File.Delete(default_save_file);
            }

            System.IO.File.Copy(default_backup_file, default_save_file);
        }

        private void BackupFile()
        {
            if(System.IO.File.Exists(default_backup_file))
            {
                System.IO.File.Delete(default_backup_file);
            }

            System.IO.File.Copy(default_save_file, default_backup_file);
        }

        private void SaveFile()
        {
            BackupFile();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@default_save_file))
            {
                foreach (TimeBlock tb in timeblocks)
                {
                    string line = tb.Serialize();
                    file.WriteLine(line);
                }
            }

            //ShowSaveSuccessMessage();
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

        private SolidColorBrush GetValidBackgroundColor()
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
                tb.Background = TimeBlock.textblock_default_background;
                Grid.SetColumn(tb, x);
                Grid.SetRow(tb, y);
                maingrid.Children.Add(tb);
            }

            for (int y = 1, x = 0; y < hours_per_day + 1; y++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = (hour_day_starts_at + y - 1).ToString() + " o'clock";
                tb.Background = TimeBlock.textblock_default_background;
                tb.Margin = new Thickness(5, 0, 0, 0);
                Grid.SetColumn(tb, x);
                Grid.SetRow(tb, y);
                maingrid.Children.Add(tb);
            }

            for (int x = 1; x < days_in_week + 1; x++)
            {
                tb_array[x - 1] = new TimeBlock[hours_per_day];

                for(int y = 1; y < hours_per_day + 1; y++)
                {
                    TimeBlock tb = new TimeBlock(x, y);
                    tb.Message = "--";
                    PlaceInGrid(tb);
                    tb.MouseEnter += TimeBlock_MouseEnter;
                    tb.MouseLeave += TimeBlock_MouseLeave;

                    tb_array[x - 1][y - 1] = tb;
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
                LoadFile(default_save_file);
            }
        }

        private void ClearDragging()
        {
            foreach (TimeBlock tb in dragged_selected_items)
            {
                if (tb != hovering_over)
                {
                    tb.ResetToOriginalBackground();
                }
            }

            dragged_selected_items.Clear();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            if(hovering_over != null)
            {
                UserInputState.GetUserInputState().state = UserInputStateEnum.DRAGGING_SELECTION;
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
                foreach (TimeBlock tb in dragged_selected_items)
                {
                    if (tb != hovering_over)
                    {
                        tb.OriginalBackground = TimeBlock.textblock_default_background;
                    }
                }
                dragged_selected_items.Clear();
                return;
            }

            string activity_title = block_details_question.inputField.Text;

            foreach (TimeBlock tb in dragged_selected_items)
            {
                if (tb != hovering_over)
                {
                    tb.Message = activity_title;
                    tb.OriginalBackground = GetValidBackgroundColor();
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

            UserInputState.GetUserInputState().state = UserInputStateEnum.DEFAULT;
        }

        private void TimeBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var timeblock = sender as TimeBlock;

            hovering_over = timeblock;

            if (UserInputState.GetUserInputState().state == UserInputStateEnum.DRAGGING_SELECTION)
            {
                hovering_over.ResetToOriginalBackground();
                ClearDragging();
                int from_x, to_x, from_y, to_y;
                if(dragging_object.Col > hovering_over.Col)
                {
                    from_x = hovering_over.Col;
                    to_x = dragging_object.Col;
                }
                else
                {
                    from_x = dragging_object.Col;
                    to_x = hovering_over.Col;
                }

                if(dragging_object.Row > hovering_over.Row)
                {
                    from_y = hovering_over.Row;
                    to_y = dragging_object.Row;
                }
                else
                {
                    from_y = dragging_object.Row;
                    to_y = hovering_over.Row;
                }

                for (int x = from_x; x < (to_x + 1); x++)
                {
                    for (int y = from_y; y < (to_y + 1); y++)
                    {
                        TimeBlock tb = tb_array[x-1][y-1];
                        tb.HoverOver();
                        dragged_selected_items.Add(tb);
                    }
                }
            }
        }

        private void TimeBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (UserInputState.GetUserInputState().state != UserInputStateEnum.DRAGGING_SELECTION)
            {
                //var timeblock = sender as TimeBlock;
            }

            hovering_over = null;
        }
    }
}
