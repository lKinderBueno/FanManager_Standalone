using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using MySettingDLL;
//using CPUInfoDLL;
using OpenHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using Microsoft.Expression.Shapes;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Management;
using System.Runtime.InteropServices;
using AutoUpdaterDotNET;

//using AutoUpdaterDotNET;

namespace Fan_Manager_standalone
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private CPUInfo CPU = new CPUInfo();
        Computer computer = new Computer();
        public MainWindow()
        {
            InitializeComponent();
            Timer_PerfomanceMode_Init();
            computer.Open();
            computer.CPUEnabled = true;

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }



        private DispatcherTimer Timer_PerfomanceMode_Setted;
        private DispatcherTimer Timer_Fan_Setted;

        private void Timer_PerfomanceMode_Init()
        {
            Timer_PerfomanceMode_Setted = new DispatcherTimer();
            Timer_PerfomanceMode_Setted.Tick += Timer_PerfomanceMode_Setted_Tick_1;
            Timer_PerfomanceMode_Setted.Interval = new TimeSpan(0, 0, 0, 5, 0);
            Timer_PerfomanceMode_Setted.Start();

            Timer_Fan_Setted = new DispatcherTimer();
            Timer_Fan_Setted.Tick += Timer_Fan_Setted_Tick_1;
            Timer_Fan_Setted.Interval = new TimeSpan(0, 0, 0, 0, 20);
            Timer_Fan_Setted.Start();


        }
        private int i = 0;

        private void Timer_Fan_Setted_Tick_1(object sender, EventArgs e)
        {
            RotateTransform rotateTransform1 = new RotateTransform(i+=5, 0, 0);
            R_FanLayer2_Copy.RenderTransform = rotateTransform1;
            if (i > 360)
                i = 0;

        }
        private void Timer_PerfomanceMode_Setted_Tick_1(object sender, EventArgs e)
        {
            computer.Hardware[0].Update();
            Temp.Text =computer.Hardware[0].Sensors[13].Value + "°";
        }

        //-------------------START FAN MODE------------------------------
        //FAN AUTO
        private void RB_Fan_Auto_Checked(object sender, RoutedEventArgs e)
        {
            FanMode_set(1);
            Automatic_txt_fk.Visibility = Visibility.Visible;
            Turbo_txt_fk.Visibility = Visibility.Hidden;
            Silent_txt_fk.Visibility = Visibility.Hidden;
            Static_txt_fk.Visibility = Visibility.Hidden;
            Advanced_txt_fk.Visibility = Visibility.Hidden;


            //BTN_fan_custom.Visibility = Visibility.Hidden;
            slider_FAN_AutoOffset.IsEnabled = false;
        }

        //FAN TURBO
        private void RB_Fan_Max_Checked(object sender, RoutedEventArgs e)
        {
            FanMode_set(2);
            Automatic_txt_fk.Visibility = Visibility.Hidden;
            Turbo_txt_fk.Visibility = Visibility.Visible;
            Silent_txt_fk.Visibility = Visibility.Hidden;
            Static_txt_fk.Visibility = Visibility.Hidden;
            Advanced_txt_fk.Visibility = Visibility.Hidden;
            //BTN_fan_custom.Visibility = Visibility.Hidden;
            slider_FAN_AutoOffset.IsEnabled = false;
        }

        //FAN QUIET
        private void RB_Fan_Silent_Checked(object sender, RoutedEventArgs e)
        {
            Automatic_txt_fk.Visibility = Visibility.Hidden;
            Turbo_txt_fk.Visibility = Visibility.Hidden;
            Silent_txt_fk.Visibility = Visibility.Visible;
            Static_txt_fk.Visibility = Visibility.Hidden;
            Advanced_txt_fk.Visibility = Visibility.Hidden;
            FanMode_set(3);
            slider_FAN_AutoOffset.IsEnabled = false;
            //BTN_fan_custom.Visibility = Visibility.Hidden;
        }

        //FAN STATIC
        private void RB_Fan_Static_Checked(object sender, RoutedEventArgs e)
        {
            Automatic_txt_fk.Visibility = Visibility.Hidden;
            Turbo_txt_fk.Visibility = Visibility.Hidden;
            Silent_txt_fk.Visibility = Visibility.Hidden;
            Static_txt_fk.Visibility = Visibility.Visible;
            Advanced_txt_fk.Visibility = Visibility.Hidden;
            FanMode_set(4);
            slider_FAN_AutoOffset.IsEnabled = true;
            //BTN_fan_custom.Visibility = Visibility.Hidden;
        }

        private void RB_Fan_Manual_Checked(object sender, RoutedEventArgs e)
        {

            Automatic_txt_fk.Visibility = Visibility.Hidden;
            Turbo_txt_fk.Visibility = Visibility.Hidden;
            Silent_txt_fk.Visibility = Visibility.Hidden;
            Static_txt_fk.Visibility = Visibility.Hidden;
            Advanced_txt_fk.Visibility = Visibility.Visible;
            if (Convert.ToInt32(Registry.GetValue(keyName + "\\FAN", "First_startup", 1)) == 1)
            {
                Set_defult_FanTable();
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath + "FAN", "First_startup", "0", RegistryValueKind.DWord);
            }
            //BTN_fan_custom.Visibility = Visibility.Visible;
            FanMode_set(5);
        }
  

        //If Fan Speed Slider Value is changed then update FanMode
        private void slider_FAN_AutoOffset_ValueChanged(object sender, MouseButtonEventArgs e)
        {
            FanMode_set(4);
        }

        //Set Fan Mode through FanManger RevCenter2
        private string NamePath1 = @"SOFTWARE\Rev.Center\Rev.Center2.0";

        public void FanMode_set(int mode)
        {
            foreach (var process in Process.GetProcessesByName("FanManager RevCenter2"))
            {
                process.Kill();
            }
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath1, "FanMode", mode, RegistryValueKind.DWord);
            if (mode == 4)
            {
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath1, "StaticFan", slider_FAN_AutoOffset.Value, RegistryValueKind.DWord);

            }

            Process.Start("FanManager RevCenter2.exe");
        }
        //-----------------------END FAN MODE-------------------------------------

        // MainWindow main = new MainWindow();
        private UIElement _lastClickedUIElement;
        private Point? _clickOffset;
        private int[] array_temp = new int[7];
        private int[] array_fan = new int[7];

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastClickedUIElement = sender as UIElement;
            _clickOffset = e.GetPosition(_lastClickedUIElement);
            line12.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line23.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line34.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line45.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line56.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line67.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _lastClickedUIElement = null;
            line12.Stroke = System.Windows.Media.Brushes.Red;
            line23.Stroke = System.Windows.Media.Brushes.Red;
            line34.Stroke = System.Windows.Media.Brushes.Red;
            line45.Stroke = System.Windows.Media.Brushes.Red;
            line56.Stroke = System.Windows.Media.Brushes.Red;
            line67.Stroke = System.Windows.Media.Brushes.Red;
        }

        private void Any_MouseMove(object sender, MouseEventArgs e)
        {
            if (_lastClickedUIElement == null)
                return;

            _lastClickedUIElement.SetValue(Canvas.LeftProperty, e.GetPosition(this).X + _clickOffset.Value.X-283); //- 160); //grande più a sx
            _lastClickedUIElement.SetValue(Canvas.TopProperty, e.GetPosition(this).Y - _clickOffset.Value.Y-53);// - 110);//+ piccolo più in alto

            Canvas.SetLeft(t1, 8);
            Canvas.SetLeft(t7, 460);
            control_check_left();
            control_check_top();
            set_Text();
            set_line();
            set_Text();
            save_data();
        }

        private void control_check_left()
        {

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t6)))
            {
                if (Canvas.GetLeft(t6) > Canvas.GetLeft(t7))
                    Canvas.SetLeft(t6, (Canvas.GetLeft(t7) - 1));
                if (Canvas.GetLeft(t6) < Canvas.GetLeft(t5))
                    Canvas.SetLeft(t6, (Canvas.GetLeft(t5) + 1));
                return;
            }
            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t5)))
            {
                if (Canvas.GetLeft(t5) > Canvas.GetLeft(t6))
                    Canvas.SetLeft(t5, (Canvas.GetLeft(t6) - 1));
                if (Canvas.GetLeft(t5) < Canvas.GetLeft(t4))
                    Canvas.SetLeft(t5, ( Canvas.GetLeft(t4) + 1));
                return;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t4)))
            {
                if (Canvas.GetLeft(t4) > Canvas.GetLeft(t5))
                    Canvas.SetLeft(t4, (Canvas.GetLeft(t5) - 1));
                if (Canvas.GetLeft(t4) < Canvas.GetLeft(t3))
                    Canvas.SetLeft(t4, (Canvas.GetLeft(t3) + 1));
                return;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t3)))
            {
                if (Canvas.GetLeft(t3) > Canvas.GetLeft(t4))
                    Canvas.SetLeft(t3, (Canvas.GetLeft(t4) - 1));
                if (Canvas.GetLeft(t3) < Canvas.GetLeft(t2))
                    Canvas.SetLeft(t3, (Canvas.GetLeft(t2) + 1));
                return;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t2)))
            {
                if (Canvas.GetLeft(t2) > Canvas.GetLeft(t3))
                    Canvas.SetLeft(t2, (Canvas.GetLeft(t3) - 1));
                if (Canvas.GetLeft(t2) < Canvas.GetLeft(t1))
                    Canvas.SetLeft(t2, (Canvas.GetLeft(t1) + 1));
                return;
            }
        }

        private void control_check_top()
        {
            if (Canvas.GetTop(_lastClickedUIElement) < Canvas.GetTop(max))
            {
                Canvas.SetTop(_lastClickedUIElement, Canvas.GetTop(max) - 1);
                return;
            }
            if (Canvas.GetTop(_lastClickedUIElement) > Canvas.GetTop(min))
            {
                Canvas.SetTop(_lastClickedUIElement, Canvas.GetTop(min) + 1);
                return;
            }
        }

        private void set_Text()
        {
            if (Canvas.GetTop(_lastClickedUIElement) < 25)
                T_fan.Text = "100";
            else if (Canvas.GetTop(_lastClickedUIElement) < 60)
                T_fan.Text = "87.5";
            else if (Canvas.GetTop(_lastClickedUIElement) < 98)
                T_fan.Text = "75";
            else if (Canvas.GetTop(_lastClickedUIElement) < 133)
                T_fan.Text = "62.5";
            else if (Canvas.GetTop(_lastClickedUIElement) < 170)
                T_fan.Text = "50";
            else if (Canvas.GetTop(_lastClickedUIElement) < 208)
                T_fan.Text = "37.5";
            else if (Canvas.GetTop(_lastClickedUIElement) < 245)
                T_fan.Text = "25";
            else if (Canvas.GetTop(_lastClickedUIElement) < 283)
                T_fan.Text = "12.5";
            else T_fan.Text = "0";

            T_temp.Text = Math.Round((((Canvas.GetLeft(_lastClickedUIElement) - 8) / 6.953846153846) + 35), 0).ToString();

        }

        private void save_data()
        {
            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t1)))
            {
                array_temp[0] = Convert.ToInt32(T_temp.Text);
                array_fan[0] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t2)))
            {
                array_temp[1] = Convert.ToInt32(T_temp.Text);
                array_fan[1] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t3)))
            {
                array_temp[2] = Convert.ToInt32(T_temp.Text);
                array_fan[2] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t4)))
            {
                array_temp[3] = Convert.ToInt32(T_temp.Text);
                array_fan[3] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t5)))
            {
                array_temp[4] = Convert.ToInt32(T_temp.Text);
                array_fan[4] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t6)))
            {
                array_temp[5] = Convert.ToInt32(T_temp.Text);
                array_fan[5] = conversion(Convert.ToDouble(T_fan.Text));
            }
            else if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t7)))
            {
                array_temp[6] = Convert.ToInt32(T_temp.Text);
                array_fan[6] = conversion(Convert.ToDouble(T_fan.Text));
            }
        }

        //TODO. Se fan a 15, =>1
        private int conversion(double data)
        {
            switch (data)
            {
                case 0:
                    return 0;
                    break;
                case 125:
                    return 1;
                    break;
                case 25:
                    return 2;
                    break;
                case 375:
                    return 3;
                    break;
                case 50:
                    return 4;
                    break;
                case 625:
                    return 5;
                case 75:
                    return 6;
                case 875:
                    return 7;
                default: //100
                    return 8;
                    break;
            }

        }

        private void set_line()
        {

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t1) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t2)))
            {
                line12.X1 = Canvas.GetLeft(t1) + 10;
                line12.Y1 = Canvas.GetTop(t1) + 10;
                line12.X2 = Canvas.GetLeft(t2) + 10;
                line12.Y2 = Canvas.GetTop(t2) + 10;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t2) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t3)))
            {

                line23.X1 = Canvas.GetLeft(t2) + 10;
                line23.Y1 = Canvas.GetTop(t2) + 10;
                line23.X2 = Canvas.GetLeft(t3) + 10;
                line23.Y2 = Canvas.GetTop(t3) + 10;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t3) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t4)))
            {
                line34.X1 = Canvas.GetLeft(t3) + 10;
                line34.Y1 = Canvas.GetTop(t3) + 10;
                line34.X2 = Canvas.GetLeft(t4) + 10;
                line34.Y2 = Canvas.GetTop(t4) + 10;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t4) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t5)))
            {
                line45.X1 = Canvas.GetLeft(t4) + 10;
                line45.Y1 = Canvas.GetTop(t4) + 10;
                line45.X2 = Canvas.GetLeft(t5) + 10;
                line45.Y2 = Canvas.GetTop(t5) + 10;

            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t5) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t6)))
            {
                line56.X1 = Canvas.GetLeft(t5) + 10;
                line56.Y1 = Canvas.GetTop(t5) + 10;
                line56.X2 = Canvas.GetLeft(t6) + 10;
                line56.Y2 = Canvas.GetTop(t6) + 10;
            }

            if ((Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t6) || Canvas.GetLeft(_lastClickedUIElement) == Canvas.GetLeft(t7)))
            {
                line67.X1 = Canvas.GetLeft(t6) + 10;
                line67.Y1 = Canvas.GetTop(t6) + 10;
                line67.X2 = Canvas.GetLeft(t7) + 10;
                line67.Y2 = Canvas.GetTop(t7) + 10;
            }
        }

        private void init()
        {
            line12.X1 = Canvas.GetLeft(t1) + 10;
            line12.Y1 = Canvas.GetTop(t1) + 10;
            line12.X2 = Canvas.GetLeft(t2) + 10;
            line12.Y2 = Canvas.GetTop(t2) + 10;
            line23.X1 = Canvas.GetLeft(t2) + 10;
            line23.Y1 = Canvas.GetTop(t2) + 10;
            line23.X2 = Canvas.GetLeft(t3) + 10;
            line23.Y2 = Canvas.GetTop(t3) + 10;
            line34.X1 = Canvas.GetLeft(t3) + 10;
            line34.Y1 = Canvas.GetTop(t3) + 10;
            line34.X2 = Canvas.GetLeft(t4) + 10;
            line34.Y2 = Canvas.GetTop(t4) + 10;
            line45.X1 = Canvas.GetLeft(t4) + 10;
            line45.Y1 = Canvas.GetTop(t4) + 10;
            line45.X2 = Canvas.GetLeft(t5) + 10;
            line45.Y2 = Canvas.GetTop(t5) + 10;
            line56.X1 = Canvas.GetLeft(t5) + 10;
            line56.Y1 = Canvas.GetTop(t5) + 10;
            line56.X2 = Canvas.GetLeft(t6) + 10;
            line56.Y2 = Canvas.GetTop(t6) + 10;
            line67.X1 = Canvas.GetLeft(t6) + 10;
            line67.Y1 = Canvas.GetTop(t6) + 10;
            line67.X2 = Canvas.GetLeft(t7) + 10;
            line67.Y2 = Canvas.GetTop(t7) + 10;
        }

        const string userRoot = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Rev.Center";
        const string keyName1 = userRoot + "\\" + "Rev.Center2.0";
        const string keyName = userRoot + "\\" + "Rev.Center2.0\\FAN";
        private void load_reg()
        {
            int perf_mode = Convert.ToInt32(Registry.GetValue(keyName1, "PerformanceMode", "1"));
            int fan_mode = Convert.ToInt32(Registry.GetValue(keyName1, "FanMode", "1"));
            double static_fan = Convert.ToDouble(Registry.GetValue(keyName1, "StaticFan", "0"));

            slider_FAN_AutoOffset.Value = static_fan;

            switch (fan_mode)
            {
                case 1:
                    RB_Fan_Auto.IsChecked = true;
                    break;
                case 2:
                    RB_Fan_Max.IsChecked = true;
                    break;
                case 3:
                    RB_Fan_Silent.IsChecked = true;
                    break;
                case 4:
                    RB_Fan_Static.IsChecked = true;
                    break;
                case 5:
                    RB_Fan_Manual.IsChecked = true;
                    break;
            }

            array_temp[0] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_0", 35));
            array_fan[0] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_0", 0));
            Canvas.SetLeft(t1, Convert.ToDouble(Registry.GetValue(keyName, "t1_x", 8)));
            Canvas.SetTop(t1, Convert.ToDouble(Registry.GetValue(keyName, "t1_y", 303)));

            array_temp[1] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_1", 50));
            array_fan[1] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_1", 0));
            Canvas.SetLeft(t2, Convert.ToDouble(Registry.GetValue(keyName, "t2_x", 113)));
            Canvas.SetTop(t2, Convert.ToDouble(Registry.GetValue(keyName, "t2_y", 303)));

            array_temp[2] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_2", 56));
            array_fan[2] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_2", 2));
            Canvas.SetLeft(t3, Convert.ToDouble(Registry.GetValue(keyName, "t3_x", 150)));
            Canvas.SetTop(t3, Convert.ToDouble(Registry.GetValue(keyName, "t3_y", 228)));

            array_temp[3] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_3", 65));
            array_fan[3] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_3", 4));
            Canvas.SetLeft(t4, Convert.ToDouble(Registry.GetValue(keyName, "t4_x", 218)));
            Canvas.SetTop(t4, Convert.ToDouble(Registry.GetValue(keyName, "t4_y", 153)));

            array_temp[4] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_4", 75));
            array_fan[4] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_4", 5));
            Canvas.SetLeft(t5, Convert.ToDouble(Registry.GetValue(keyName, "t5_x", 288)));
            Canvas.SetTop(t5, Convert.ToDouble(Registry.GetValue(keyName, "t5_y", 116.4)));

            array_temp[5] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_5", 85));
            array_fan[5] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_5", 7));
            Canvas.SetLeft(t6, Convert.ToDouble(Registry.GetValue(keyName, "t6_x", 360)));
            Canvas.SetTop(t6, Convert.ToDouble(Registry.GetValue(keyName, "t6_y", 41.5)));

            array_temp[6] = Convert.ToInt32(Registry.GetValue(keyName, "Temp_6", 100));
            array_fan[6] = Convert.ToInt32(Registry.GetValue(keyName, "Fan_6", 7));
            Canvas.SetLeft(t7, Convert.ToDouble(Registry.GetValue(keyName, "t7_x", 458)));
            Canvas.SetTop(t7, Convert.ToDouble(Registry.GetValue(keyName, "t7_y", 40)));
        }


        private string NamePath = @"SOFTWARE\Rev.Center\Rev.Center2.0\FAN";
        public void save_reg()
        {
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "First_startup", "0", RegistryValueKind.DWord);
            for (int i = 0; i < 7; i++)
            {
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_" + i.ToString(), array_temp[i], RegistryValueKind.DWord);
                Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_" + i.ToString(), array_fan[i], RegistryValueKind.DWord);
            }
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t1_x", Canvas.GetLeft(t1), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t1_y", Canvas.GetTop(t1), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t2_x", Canvas.GetLeft(t2), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t2_y", Canvas.GetTop(t2), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t3_x", Canvas.GetLeft(t3), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t3_y", Canvas.GetTop(t3), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t4_x", Canvas.GetLeft(t4), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t4_y", Canvas.GetTop(t4), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t5_x", Canvas.GetLeft(t5), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t5_y", Canvas.GetTop(t5), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t6_x", Canvas.GetLeft(t6), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t6_y", Canvas.GetTop(t6), RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t7_x", Canvas.GetLeft(t7), RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "t7_y", Canvas.GetTop(t7), RegistryValueKind.DWord);

            //main.FanMode_set(5);


            /*
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_0", array_temp[0], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_0", array_fan[0], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_1", array_temp[1], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_1", array_fan[1], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_2", array_temp[2], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_2", array_fan[2], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_3", array_temp[3], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_3", array_fan[3], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_4", array_temp[4], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_4", array_fan[4], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_5", array_temp[5], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_5", array_fan[5], RegistryValueKind.DWord);

            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Temp_6", array_temp[6], RegistryValueKind.DWord);
            Utility.RegistryKeyWrite(RegistryHive.LocalMachine, NamePath, "Fan_6", array_fan[6], RegistryValueKind.DWord);
        */
        }



        public void Set_defult_FanTable()
        {
            string path = @"SOFTWARE\Rev.Center\Rev.Center2.0";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true))
            //Registry.CurrentUser.OpenSubKey(keyName, true))
            {
                if (key == null)
                {
                    // Key doesn't exist. Do whatever you want to handle
                    // this case
                }
                else
                {
                    save_reg();
                    key.DeleteSubKeyTree("FAN");
                    Canvas.SetLeft(t1, 8);
                    Canvas.SetTop(t1, 303);

                    Canvas.SetLeft(t2, 113);
                    Canvas.SetTop(t2, 303);

                    Canvas.SetLeft(t3, 150);
                    Canvas.SetTop(t3, 228);

                    Canvas.SetLeft(t4, 218);
                    Canvas.SetTop(t4, 153);

                    Canvas.SetLeft(t5, 288);
                    Canvas.SetTop(t5, 116.4);

                    Canvas.SetLeft(t6, 360);
                    Canvas.SetTop(t6, 41.5);

                    Canvas.SetLeft(t7, 458);
                    Canvas.SetTop(t7, 40);
                    init();
                    array_temp[0] = 35;
                    array_fan[0] = 0;

                    array_temp[1] = 50;
                    array_fan[1] = 0;

                    array_temp[2] = 56;
                    array_fan[2] = 2;

                    array_temp[3] = 65;
                    array_fan[3] = 4;

                    array_temp[4] = 75;
                    array_fan[4] = 5;

                    array_temp[5] = 85;
                    array_fan[5] = 7;

                    array_temp[6] = 100;
                    array_fan[6] = 7;
                    save_reg();
                }
            }

        }

        public void reset_UI(int FanTableIndex)
        {
            //   this.Update_T3_UI(Get_Fan_Table_duty(FanTableIndex, 4) - 1, Get_Fan_Table_temp(FanTableIndex, 4) - 1);
            // this.Update_T2_UI(Get_Fan_Table_duty(FanTableIndex, 1) + 1, Get_Fan_Table_temp(FanTableIndex, 1) + 1);
        }

        private void PART_Track_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Fan_Loaded(object sender, RoutedEventArgs e)
        {
            Fan_control.Visibility = Visibility.Hidden;
            Main.Visibility = Visibility.Visible;
            //Run_at_Startup.Visibility = Visibility.Hidden;
            load_reg();
            init();
            AutoUpdater.Start("http://sitodielio.000webhostapp.com/Rev.Center2/updater_fan.xml");
            AutoUpdater.OpenDownloadPage = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Fan_control.Visibility = Visibility.Hidden;
            Main.Visibility = Visibility.Visible;
            //Run_at_Startup.Visibility = Visibility.Hidden;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Fan_control.Visibility = Visibility.Visible;
            Main.Visibility = Visibility.Hidden;
            //Run_at_Startup.Visibility = Visibility.Hidden;

        }

        private void Temp_Copy1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Fan_control.Visibility = Visibility.Hidden;
            Main.Visibility = Visibility.Hidden;
            //Run_at_Startup.Visibility = Visibility.Visible;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            save_reg();
            if (RB_Fan_Manual.IsChecked == true)
                FanMode_set(5);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Set_defult_FanTable();
            if (RB_Fan_Manual.IsChecked == true)
                FanMode_set(5);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.paypal.me/EMUNI");

        }
    }
}
