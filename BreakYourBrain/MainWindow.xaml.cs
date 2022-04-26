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
using System.IO;

namespace BreakYourBrain
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TextBox> allTxt = new List<TextBox>();
        List<TextBox> XTxt = new List<TextBox>();
        List<TextBox> YTxt = new List<TextBox>();

        Button but;

        int status = 1;
        double width, height, tempX, tempY;
        double[] xPoint = new double[6];
        double[] yPoint = new double[6];
        double[] voidNum = new double[6];

        string value;
        public MainWindow()
        {
            InitializeComponent();
            Calculate.Click += Bloop;
            Save.Click += Bloop;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int val = 1;
            int str = 5;
            for (int i = 0; i < 940; i = i + 163)
            {
                allTxt.Add(new TextBox());
                allTxt.Last().Height = 42.5;
                allTxt.Last().Width = 163;
                allTxt.Last().ToolTip = $"X{val}";
                allTxt.Last().FontSize = 32;
                allTxt.Last().PreviewTextInput += CheckText;
                allTxt.Last().KeyUp += Enter;
                Canvas.SetBottom(allTxt.Last(), 42.5);
                Canvas.SetLeft(allTxt.Last(), i);
                Main.Children.Add(allTxt.Last());     
                XTxt.Add(allTxt.Last());

                allTxt.Add(new TextBox());
                allTxt.Last().Height = 42.5;
                allTxt.Last().Width = 163;
                allTxt.Last().ToolTip = $"Y{val}";
                allTxt.Last().FontSize = 32;
                allTxt.Last().PreviewTextInput += CheckText;
                allTxt.Last().KeyUp += Enter;
                Canvas.SetBottom(allTxt.Last(), 0);
                Canvas.SetLeft(allTxt.Last(), i);
                Main.Children.Add(allTxt.Last());
                YTxt.Add(allTxt.Last());

                val++;
            }
            allTxt[0].Focus();
            for (int i = 600; i > 0; i = i - 40)
            {
                Line lineX = new Line();
                lineX.X1 = 0;
                lineX.Y1 = i;
                lineX.X2 = 1280;
                lineX.Y2 = i;
                lineX.Stroke = Brushes.Black;
                lineX.StrokeThickness = str;
                Main.Children.Add(lineX);
                str = 1;
            }
            str = 10;
            for (int i = 0; i < 1280; i = i + 40)
            {
                Line lineY = new Line();
                lineY.X1 = i;
                lineY.Y1 = 0;
                lineY.X2 = i;
                lineY.Y2 = 600;
                lineY.Stroke = Brushes.Black;
                lineY.StrokeThickness = str;
                Main.Children.Add(lineY);
                str = 1;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Enter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                int pos = allTxt.IndexOf(sender as TextBox);
                if (pos == allTxt.Count - 1)
                    pos = -1;
                allTxt[pos + 1].Focus();
            }
        } 

        private async void CheckText(object sender, TextCompositionEventArgs e)
        {
            TextBox ftxt = ((TextBox)sender);

            if (!int.TryParse(e.Text, out int c) && Convert.ToChar(e.Text) != 44)
            {
                e.Handled = true;
                return;
            }
                
            await Task.Delay(5);
            value =ftxt.Text;


            if (Convert.ToDouble(value) > 12 )
            {
                value = value.Remove(1, ftxt.Text.Length - 1);
                ftxt.Text = value;
                ftxt.Select(ftxt.Text.Length, 0);
            }


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(System.IO.Path.GetFullPath("../../Formula.txt"), true))
            {
                sw.WriteLine(Output.Text);
                for(int i = 0; i < 6; i++)
                {
                    sw.Write($"X{i}:{xPoint[i]} ");
                }
                sw.WriteLine("");
                for (int i = 0; i < 6; i++)
                {
                    sw.Write($"Y{i}:{yPoint[i]} ");
                }
                sw.WriteLine("");
            }
        }


        private async void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for(int i = 0; i < 6; i++)
                {
                    xPoint[i] = Convert.ToDouble(XTxt[i].Text);
                    yPoint[i] = Convert.ToDouble(YTxt[i].Text);
                    voidNum[i] = 1;
                }
            }
            catch
            {
                await Task.Delay(350);
                MessageBox.Show("Insufficient input data");
                return;
            }

            Graph.Children.Clear();

            double sumY = Sum(yPoint, voidNum);
            double sumX = Sum(xPoint, voidNum);
            double sumYX = Sum(yPoint, xPoint);
            double sumXX = Sum(xPoint, xPoint);

            double Sum(double[] F, double[] S)
            {
                double totalSum = 0;
                for(int i = 0; i < 6; i++)
                {   
                    totalSum += F[i] * S[i];
                }
                return totalSum;
            }

            double b = Math.Round((sumY* sumXX - sumX*sumYX) / (6*sumXX-sumX*sumX), 4);
            double a = Math.Round((6*sumYX -sumX*sumY) / (6*sumXX-sumX*sumX), 4);

            for (int i = 0; i < 6; i++)
            {
                for (int j = i + 1; j < 6; j++)
                {
                    if (xPoint[i] > xPoint[j])
                    {
                        tempX = xPoint[i];
                        tempY = yPoint[i];

                        xPoint[i] = xPoint[j];
                        xPoint[j] = tempX;

                        yPoint[i] = yPoint[j];
                        yPoint[j] = tempY;
                    }
                }
            }

            for(int i = 0; i < 6; i++)
            {
                Ellipse elBef = new Ellipse();
                elBef.Height = 10;
                elBef.Width = 10;
                elBef.Fill = Brushes.Red;
                elBef.ToolTip = $"{xPoint[i]};{yPoint[i]}";
                Panel.SetZIndex(elBef, 0);
                Canvas.SetBottom(elBef, (yPoint[i] * 40) - 5);
                Canvas.SetLeft(elBef, (xPoint[i] * 40) - 5);
                Graph.Children.Add(elBef);
            }

            Line lineX = new Line();
            lineX.X1 = xPoint[0] * 40;
            lineX.Y1 = 600 - (yPoint[0] - Yi(0)) * 40;
            lineX.X2 = xPoint[5] * 40;
            lineX.Y2 = 600 -(yPoint[5 ] - Yi(5)) * 40;
            lineX.Stroke = Brushes.Blue;
            lineX.StrokeThickness = 5;
            Graph.Children.Add(lineX);

            for (int i = 0; i < 6; i++)
            {
                Ellipse elAf = new Ellipse();
                elAf.Height = 15;
                elAf.Width = 15;
                elAf.Fill = Brushes.White;
                elAf.Stroke = Brushes.Blue;
                elAf.StrokeThickness = 2;
                elAf.ToolTip = $"{xPoint[i]};{yPoint[i] - Yi(i)}";
                Panel.SetZIndex(elAf, 0);
                Canvas.SetBottom(elAf, ((yPoint[i] - Yi(i)) * 40) - 7.5);
                Canvas.SetLeft(elAf, (xPoint[i] * 40) - 7.5);
                Graph.Children.Add(elAf);
            }


            Output.Text = $"Y = {a}X + {b}";

            double Yi(int i)
            {
               return  yPoint[i] - (a * xPoint[i] + b);
            }
        }
        
        private async void Bloop(object sender, RoutedEventArgs e)
        {
            if (status == 0)
                return;

            status = 0;
            but = sender as Button;
            width = but.Width / 12;
            height = but.Height / 6;

            DoubleAnimation widthAn = new DoubleAnimation();
            widthAn.From = but.Width - width;
            widthAn.To = but.Width;
            widthAn.Duration = TimeSpan.FromSeconds(0.35);
            but.BeginAnimation(Button.WidthProperty, widthAn);

            DoubleAnimation heightAn = new DoubleAnimation();
            heightAn.From = but.Height - height;
            heightAn.To = but.Height;
            heightAn.Duration = TimeSpan.FromSeconds(0.35);
            but.BeginAnimation(Button.HeightProperty, heightAn);
            await Task.Delay(350);
            status = 1;

        }
    }
}
