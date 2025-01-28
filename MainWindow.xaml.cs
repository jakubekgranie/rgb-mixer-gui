using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;

namespace rgb_mixer_revision2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        readonly List<int[]> hexes = [[255, 255, 255]]; // hex data
        readonly List<bool> toMix = [false]; // blend options
        private void Blend()
        {
            int active = 0;
            for (int i = 0; i < toMix.Count; i++)
                if (toMix[i])
                    active++;
            if (active > 0)
            {
                int[] blended = [0, 0, 0];
                for (int i = 0; i < hexes.Count; i++)
                    if (toMix[i])
                        for (int j = 0; j < 3; j++)
                            blended[j] += hexes[i][j];
                int[] parsed = [0, 0, 0];
                for (int i = 0; i < 3; i++)
                {
                    double item = blended[i] / active;
                    parsed[i] = (int)Math.Floor(item);
                }
                rgbOutput.Text = parsed[0] + ", " + parsed[1] + ", " + parsed[2];
                string hex = "#";
                for (int i = 0; i < 3; i++)
                {
                    int[] chars = [parsed[i] / 16, 0];
                    chars[1] = parsed[i] - chars[0] * 16 + 48; ;
                    chars[0] += 48;
                    for (int j = 0; j < 2; j++)
                        if (chars[j] > 57)
                            chars[j] += 7;
                    hex += (char)chars[0] + "" + (char)chars[1];
                }
                hexOutput.Text = hex;
                supplementaryOutput.Background = new SolidColorBrush(Color.FromRgb((byte)parsed[0], (byte)parsed[1], (byte)parsed[2]));
                supplementaryOutput.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)((parsed[0] < 234) ? parsed[0] + 32 : parsed[0]), (byte)((parsed[1] < 234) ? parsed[1] + 32 : parsed[1]), (byte)((parsed[2] < 234) ? parsed[2] + 32 : parsed[2])));
            }
        }
        private void RegisterColor(object? sender, KeyEventArgs? e)
        {
            string input = hex.Text;
            string[] pattern = ["^#?([0-9A-Fa-f]){6}$", "^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])([ ,]|, )(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])([ ,]|, )(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$"];
            for (uint j = 0; j < pattern.Length; j++)
                if (Regex.Match(input, pattern[j]).Success)
                {
                    int[] ready = new int[3];
                    if (j == 0)
                    {
                        int[] rgb = new int[6];
                        if (input[0] == '#')
                            input = input[1..];
                        for (int i = 0; i < 6; i++)
                        {
                            int parsed = input[i];
                            if (parsed > 96)
                                parsed -= 32;
                            if (parsed > 64)
                                parsed -= 55;
                            if (parsed > 47)
                                parsed -= 48;
                            rgb[i] = parsed;
                            ready = [rgb[0] * 16 + rgb[1], rgb[2] * 16 + rgb[3], rgb[4] * 16 + rgb[5]];
                        }
                    }
                    else if (j == 1)
                    {
                        string temp = "";
                        int rgbIndex = -1;
                        for (int k = 0; k < input.Length; k++)
                        {
                            if (input[k] > 47 && input[k] < 58)
                                temp += input[k];
                            else if (temp != "")
                            {
                                ready[++rgbIndex] = Int32.Parse(temp);
                                temp = "";
                            }
                        }
                        ready[2] = Int32.Parse(temp);
                    }
                    int cID = Int32.Parse(ColorID.Content.ToString()) - 1;
                    if (cID > hexes.Count - 1)
                    {
                        hexes.Add(ready);
                        toMix.Add(false);
                    }
                    else
                        hexes[cID] = ready;
                    supplementaryHex.Background = new SolidColorBrush(Color.FromRgb((byte)ready[0], (byte)ready[1], (byte)ready[2]));
                    supplementaryHex.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)((ready[0] < 234) ? ready[0] + 32 : ready[0]), (byte)((ready[1] < 234) ? ready[1] + 32 : ready[1]), (byte)((ready[2] < 234) ? ready[2] + 32 : ready[2])));
                    Blend();
                }
        }
        private void ChangeButton()
        {
            int cID = Int32.Parse(ColorID.Content.ToString()) - 1;
            if (toMix[cID])
            {
                blendButton.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                blendButton.BorderBrush = new SolidColorBrush(Color.FromRgb(166, 44, 35));
                blendImage.Source = new BitmapImage(new Uri("pack://application:,,,/block.png"));
            }
            else
            {
                blendButton.Background = new SolidColorBrush(Color.FromRgb(139, 195, 74));
                blendButton.BorderBrush = new SolidColorBrush(Color.FromRgb(78, 109, 43));
                blendImage.Source = new BitmapImage(new Uri("pack://application:,,,/blend.png"));
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cID = Int32.Parse(ColorID.Content.ToString()) - 1;
            toMix[cID] = !toMix[cID];
            ChangeButton();
            Blend();
        }
        private void RefreshInterface(int cID)
        {
            ColorID.Content = cID + 1;
            hex.Text = hexes[cID][0] + ", " + hexes[cID][1] + ", " + hexes[cID][2];
            RegisterColor(null, null);
            ChangeButton();
        }
        private void ArrowTrigger(object sender, RoutedEventArgs? e)
        {
            RegisterColor(null, null);
            int cID = Int32.Parse(ColorID.Content.ToString()) - 1 + Int32.Parse(((Button)sender).Tag.ToString());
            if (cID > hexes.Count - 1)
                cID = 0;
            else if (cID < 0)
                cID = hexes.Count - 1;
            RefreshInterface(cID);
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            hexes.Add([255, 255, 255]);
            toMix.Add(false);
            ArrowTrigger(new Button() { Tag = hexes.Count - Int32.Parse(ColorID.Content.ToString()) }, null);
        }

        private void RemoveItem(object sender, RoutedEventArgs e)
        {
            if (hexes.Count > 1)
            {
                int cID = Int32.Parse(ColorID.Content.ToString()) - 1;
                hexes.RemoveAt(cID);
                toMix.RemoveAt(cID);
                if (cID == hexes.Count)
                    cID--;
                RefreshInterface(cID);
            }
        }
    }
}