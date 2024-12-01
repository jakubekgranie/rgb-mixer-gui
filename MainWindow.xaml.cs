using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace rgb_mixer_revision2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        List<int[]> hexes = [[255, 255, 255]]; // hex data
        bool[] toMix = [false]; // blend options
        private void registerColor(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string input = hex.Text;
            string[] pattern = ["^#?([0-9A-Fa-f]){6}$", "^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])([ ,]|, )?(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])([ ,]|, )?(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$"];
            if(Regex.Match(input, pattern[0]).Success)
            {
                int[] rgb = { 0, 0, 0, 0, 0, 0 };
                if (input[0] == '#')
                    input = input[1..];
                for(int i = 0; i < 6; i++)
                {
                    int parsed = input[i];
                    if (parsed > 96)
                        parsed -= 32;
                    if (parsed > 64)
                        parsed -= 55;
                    if (parsed > 47)
                        parsed -= 48;
                    rgb[i] = parsed;
                }
                int[] ready = { rgb[0] * 16 + rgb[1], rgb[2] * 16 + rgb[3], rgb[4] * 16 + rgb[5] };
                int cID = ColorID.Content.ToString()[0] - 49;
                if (cID > hexes.Count - 1)
                {
                    hexes.Add(ready);
                    bool[] resized = new bool[toMix.Length + 1];
                    Array.Copy(toMix, resized, toMix.Length);
                    resized[^1] = false;
                    toMix = resized;
                }
                else
                   hexes[cID] = ready;
                supplementaryHex.Background = new SolidColorBrush(Color.FromRgb((byte)(ready[0]), (byte)(ready[1]), (byte)(ready[2])));
                supplementaryHex.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)((ready[0] < 234) ? ready[0] + 32 : ready[0]), (byte)((ready[1] < 234) ? ready[1] + 32 : ready[1]), (byte)((ready[2] < 234) ? ready[2] + 32 : ready[2])));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cID = ColorID.Content.ToString()[0] - 49;
            if (toMix[cID])
            {
                blendButton.Background = new SolidColorBrush(Color.FromRgb(139, 195, 74));
                blendButton.BorderBrush = new SolidColorBrush(Color.FromRgb(78, 109, 43));
                blendImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/blend.png"));
            }
            else
            {
                blendButton.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                blendButton.BorderBrush = new SolidColorBrush(Color.FromRgb(166, 44, 35));
                blendImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/block.png"));
            }
            toMix[cID] = !toMix[cID];
        }
    }
}