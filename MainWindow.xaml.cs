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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku
{
    public class Map
    {
        private int cellSize, mapSize, extendedMapSize;
        public Label[,] map;
        private string[,] solution;
        const string defaultCharSet = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZБГДЁЖЗИЙЛПФЦЧШЩЪЫЬЭЮЯ!@#$%^&*(";
        public bool isAMistakeMade = false;
        public Label lastSelected;
        

        readonly SolidColorBrush foregroundStrong = new SolidColorBrush(Color.FromRgb(33, 33, 33));
        readonly SolidColorBrush foregroundPlaced = new SolidColorBrush(Color.FromRgb(19, 70, 151));
        readonly SolidColorBrush foregroundMistake = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        readonly SolidColorBrush borderStrong = new SolidColorBrush(Color.FromRgb(97, 98, 102));
        readonly SolidColorBrush borderDefault = new SolidColorBrush(Color.FromRgb(193, 198, 204));
        readonly SolidColorBrush backgroundMistake = new SolidColorBrush(Color.FromRgb(253, 212, 220));
        readonly SolidColorBrush backgroundSelected = new SolidColorBrush(Color.FromRgb(226, 231, 237));
        readonly SolidColorBrush backgroundtoPlace = new SolidColorBrush(Color.FromRgb(187, 222, 250));
        readonly SolidColorBrush backgroundDefault = new SolidColorBrush(Color.FromRgb(255,255,255));
        readonly SolidColorBrush backgroundSimilar = new SolidColorBrush(Color.FromRgb(200, 208, 221));


        Random rand = new Random();
        public void Show(Canvas GameField) 
        {
            GameField.Width = cellSize * extendedMapSize;
            GameField.Height = cellSize * extendedMapSize;

            for (int i = 0; i < extendedMapSize; i++)
            {
                
                for (int j = 0; j < extendedMapSize; j++)
                {
                    if (i % mapSize == 0 && j % mapSize == 0)
                    {
                        Path p = new Path
                        {
                            Stroke = borderStrong,
                            StrokeThickness = 2,
                            Data = Geometry.Parse($"M0,0 h{cellSize * mapSize} v{cellSize * mapSize} h-{cellSize * mapSize} Z")
                        };
                        Canvas.SetZIndex(p, 10000);
                        Canvas.SetTop(p, i * cellSize);
                        Canvas.SetLeft(p, j * cellSize);
                        GameField.Children.Add(p);
                    }
                    GameField.Children.Add(map[i, j]);
                }
            }
        }

        public void Transposition()
        {
            for (int i = 0; i < extendedMapSize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    string tmp = solution[i, j];
                    solution[i, j] = solution[j, i];
                    solution[j, i] = tmp;
                }
            }
        }
        public void SwapRowsSmall()
        {
            int r = rand.Next(mapSize);
            int first = rand.Next(mapSize) + r * mapSize;
            int second = rand.Next(mapSize) + r * mapSize;
            while (second == first) second = rand.Next(mapSize) + r * mapSize;

            for (int i = 0; i < extendedMapSize; i++)
            {
                string tmp = solution[i, second];
                solution[i, second] = solution[i, first];
                solution[i, first] = tmp;
            }
        }
        public void SwapRowsArea()
        {
            int r1 = rand.Next(mapSize);
            int r2 = rand.Next(mapSize);
            while (r2 == r1) r2 = rand.Next(mapSize);

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < extendedMapSize; j++)
                {
                    string tmp = solution[j, i + r1 * mapSize];
                    solution[j, i + r1 * mapSize] = solution[j, i + r2 * mapSize];
                    solution[j, i + r2 * mapSize] = tmp;
                }
            }
        }
        public void SwapColumnsSmall() {
            int r = rand.Next(mapSize);
            int first = rand.Next(mapSize) + r * mapSize;
            int second = rand.Next(mapSize) + r * mapSize;
            while (second == first) second = rand.Next(mapSize) + r * mapSize;

            for (int i = 0; i < extendedMapSize; i++)
            {
                string tmp = solution[second, i];
                solution[second, i] = solution[first, i];
                solution[first, i] = tmp;
            }
        }
        public void SwapColumnsArea()
        {
            int r1 = rand.Next(mapSize);
            int r2 = rand.Next(mapSize);
            while (r2 == r1) r2 = rand.Next(mapSize);

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < extendedMapSize; j++)
                {
                    string tmp = solution[i + r1 * mapSize, j];
                    solution[i + r1 * mapSize, j] = solution[i + r2 * mapSize, j];
                    solution[i + r2 * mapSize, j] = tmp;
                }
            }
        }
        public void CreateMatrix()
        {
            string trimmedCharSet = defaultCharSet.Substring(0, extendedMapSize);
            
            string charSet = new string(' ', extendedMapSize); 

            for (int j = 0; j < extendedMapSize; j++)
            {
                int r = rand.Next(extendedMapSize);

                while (charSet.Contains(trimmedCharSet[r])) r = rand.Next(extendedMapSize);
                solution[j, 0] = trimmedCharSet[r].ToString();

                charSet = charSet.Insert(j, trimmedCharSet[r].ToString());
            }
            charSet = charSet.Substring(0, extendedMapSize);

            int count;
            for (int n = 0; n < mapSize; n++)
                for (int m = 0; m < mapSize; m++)
                {
                    count = 0;

                    for (int i = mapSize * m; i < mapSize * (m + 1); i++)
                        for (int j = mapSize * n; j < mapSize * (n + 1); j++)
                            solution[j, i] = charSet[count++].ToString();

                    charSet = charSet.Remove(0, 1) + charSet.Substring(0, 1);
                }

            for (int i = 0; i < 300; i++)
            {
                SwapRowsSmall();
                SwapColumnsArea();
                if (rand.Next(2) == 1)
                    Transposition();
                SwapRowsArea();
                SwapColumnsSmall();
            }
        }
        public Map(int size, int mapSize, int difficulty)
        {
            cellSize = size;
            this.mapSize = mapSize;
            extendedMapSize = mapSize * mapSize;

            map = new Label[extendedMapSize, extendedMapSize];

            solution = new string[extendedMapSize, extendedMapSize];
            
            CreateMatrix();

            for (int i = 0; i < extendedMapSize; i++)
                for (int j = 0; j < extendedMapSize; j++)
                {
                    Label l = new Label()
                    {
                        Width = size,
                        Height = size,
                        Content = solution[i, j],
                        Focusable = true,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        BorderBrush = borderDefault,
                        Foreground = foregroundStrong,
                        Background = backgroundDefault,
                        FontSize = size / 2,
                        BorderThickness = new Thickness(.4)
                    };
                    l.KeyDown += L_KeyDown;
                    l.Tag = $"{i},{j}";
                    if (i != 0)
                        Canvas.SetLeft(l, size * i);
                    if (j != 0)
                        Canvas.SetTop(l, size * j);

                    l.MouseRightButtonDown += L_MouseRightButtonDown;
                    l.MouseLeftButtonDown += L_MouseLeftButtonDown;

                    map[i, j] = l;
                }
            difficulty = extendedMapSize*extendedMapSize - difficulty;
            while (difficulty > 0)
            {
                int x = rand.Next(extendedMapSize);
                int y = rand.Next(extendedMapSize);

                if (map[x,y].Content.ToString() != "")
                {
                    map[x, y].Content = "";
                    map[x, y].Foreground = foregroundPlaced;
                    difficulty--;
                }
            }
            int count = 0;
            foreach (Label label in map)
            {
                if (label.Content.ToString() == "")
                count++;
            }
            MessageBox.Show(count.ToString());
        }

        private void L_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int[] tmp = (sender as Label).Tag.ToString().Split(',').Select(num => int.Parse(num)).ToArray();
            int x = tmp[0];
            int y = tmp[1];

            if (map[x, y].Foreground == foregroundStrong)
                return;

            Clear(x, y);
        }
        private void Clear(int x, int y) {

            map[x, y].Content = "";
            map[x, y].Foreground = foregroundPlaced;
            Select(map[x, y]);
        }

        private void L_KeyDown(object sender, KeyEventArgs e)
        {
            int[] tmp = (sender as Label).Tag.ToString().Split(',').Select(num => int.Parse(num)).ToArray();
            int x = tmp[0];
            int y = tmp[1];

            if (map[x, y].Foreground == foregroundStrong)
                return;

            string key = e.Key.ToString();
            int keyInt;
            if (key.Substring(1) != string.Empty && int.TryParse(key[1].ToString(), out keyInt))
                key = keyInt.ToString();

            if (key.Length > 1 || !defaultCharSet.Substring(0, extendedMapSize).Contains(key))
                return;

            Place(x, y, key);
        }

        private void L_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lastSelected != null) 
                lastSelected.Background = backgroundDefault; 
            Label l = sender as Label; 
            lastSelected = l;
            l.Focus();


            Select(l);
        }
        private void Select(Label l)
        {
            foreach (Label label in map)
            {
                if (label.Background == backgroundSimilar || label.Background == backgroundSelected || label.Background == backgroundMistake) label.Background = backgroundDefault;
                if (label.Content.ToString() == l.Content.ToString() && l.Content.ToString() != "" && label.Foreground != foregroundMistake)
                {
                    label.Background = backgroundSimilar;
                }
            }

            int[] tmp = l.Tag.ToString().Split(',').Select(num => int.Parse(num)).ToArray();

            int x = tmp[0];
            int y = tmp[1];

            int xLarge = x / mapSize;
            int yLarge = y / mapSize;

            for (int i = mapSize * xLarge; i < mapSize * (xLarge+1); i++)
                for (int j = mapSize * yLarge; j < mapSize * (yLarge + 1); j++)
                    map[i, j].Background = backgroundSelected;

            for (int m = 0; m < extendedMapSize; m++)
            {
                map[m, y].Background = backgroundSelected;
                map[x, m].Background = backgroundSelected;
            }
            
            if (map[x,y].Foreground == foregroundMistake)
            {
                for (int m = 0; m < extendedMapSize; m++)
                {
                    if (map[m, y].Content.ToString() == map[x, y].Content.ToString() && map[m, y] != map[x, y]) map[m, y].Background = backgroundMistake;
                    if (map[x, m].Content.ToString() == map[x, y].Content.ToString() && map[x, m] != map[x, y]) map[x, m].Background = backgroundMistake;
                }
                for (int i = mapSize * xLarge; i < mapSize * (xLarge + 1); i++)
                    for (int j = mapSize * yLarge; j < mapSize * (yLarge + 1); j++)
                        if (map[i, j].Content.ToString() == map[x, y].Content.ToString() && map[x, y] != map[i, j])
                            map[i, j].Background = backgroundMistake;

            }

            l.Background = backgroundtoPlace;
        }
        private void Place(int x, int y, string ch)
        {
            if (map[x, y].Content.ToString() == ch)
                Clear(x, y);
            else if (map[x, y].Foreground != foregroundStrong)
            {
                Clear(x, y);
                map[x, y].Content = ch;
                if ((string)map[x, y].Content != "" && map[x, y].Content.ToString() != solution[x, y])
                {
                    map[x, y].Foreground = foregroundMistake;
                }
            }

            //CheckForMistakes();
            Select(map[x, y]);
            
        }
        
    }
    public partial class MainWindow : Window
    {
        const int BOX_SIZE = 30;
        UIElement focused;
        Map map;
        public MainWindow()
        {
            InitializeComponent();
            focused = Menu;
        }

        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            int difficulty;
            //easy 40
            //meidum 33
            //hard 27
            //expert 22
            switch ((sender as Button).Tag.ToString())
            {
                case "easy": map = new Map(30, 3, 40); break;
                case "medium": map = new Map(BOX_SIZE, 3, 33); break;
                case "hard": map = new Map(BOX_SIZE, 3, 27); break;
                case "expert": map = new Map(BOX_SIZE, 3, 22); break;
                case "giant": map = new Map(BOX_SIZE, 4, 25); break;
                case "special": SpecialGameParams.Visibility = Visibility.Visible; return;
            }
            Menu.Visibility = Visibility.Hidden;
            levels.Visibility = Visibility.Hidden;
            gameSession.Visibility = Visibility.Visible;
            map.Show(gameField);
        }
        private void newGameWithParams_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int size = 3;
            foreach (RadioButton radio in specialGameMapSize.Children)
                if ((bool)radio.IsChecked)
                {
                    size = int.Parse(radio.Content.ToString());
                    break;
                }

            map = new Map(BOX_SIZE, size, int.Parse(specialGameFilledCells.Content.ToString()));
            map.Show(gameField);
            SpecialGameParams.Visibility = Visibility.Hidden;
            Menu.Visibility = Visibility.Hidden;
            levels.Visibility = Visibility.Hidden;
            gameSession.Visibility = Visibility.Visible;
        }

        private void newGame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation moveUp = new DoubleAnimation(0, TimeSpan.FromMilliseconds(180))
            {
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            TranslateTransform t = levels.RenderTransform as TranslateTransform;
            t.BeginAnimation(TranslateTransform.YProperty, moveUp);
            focused = levels;
            
        }

        private void Menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (focused as StackPanel == levels) { 
                
                DoubleAnimation moveDown = new DoubleAnimation(290, TimeSpan.FromMilliseconds(180))
                {
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                };
                TranslateTransform t = levels.RenderTransform as TranslateTransform;
                t.BeginAnimation(TranslateTransform.YProperty, moveDown);
            }
            focused = Menu;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (slider == null) return;
            int size = int.Parse((sender as RadioButton).Content.ToString());

            slider.Minimum = (int)(size * size * size * size * 0.1);
            labelMinimum.Content = (int)(size * size * size * size * 0.1);

            slider.Maximum = (int)(size * size * size * size - 1);
            labelMaximum.Content = (int)(size * size * size * size - 1);

            slider.Value = (int)(size * size * size * size * .3);
        }

        private void SpecialGameParams_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as Grid).Children[0] as Border).IsMouseOver)
                SpecialGameParams.Visibility = Visibility.Hidden;
        }
    }
}
