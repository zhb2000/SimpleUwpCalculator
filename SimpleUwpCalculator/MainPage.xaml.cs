using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleUwpCalculator.Calculate;
using Windows.UI.Popups;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimpleUwpCalculator
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = viewModel;
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Simple UWP Calculator";
        }

        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(0);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(1);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(2);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(3);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(4);
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(5);
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(6);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(7);
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(8);
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendNumber(9);
        }

        private void ButtonLeftBrace_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('(');
        }

        private void ButtonRightBrace_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator(')');
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Clear();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Delete();
        }

        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowResult();
        }

        private void ButtonDivide_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('÷');
        }

        private void ButtonMultiply_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('×');
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('-');
        }

        private void ButtonDot_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('.');
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AppendOperator('+');
        }

        private void ButtonMC_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Memory = null;
        }

        private void ButtonMR_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.Memory.HasValue)
            {
                viewModel.Clear();
                viewModel.AppendNumber(viewModel.Memory.Value);
            }
        }

        private void ButtonMPlus_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumber(viewModel.DisplayText) && viewModel.Memory.HasValue)
                viewModel.Memory = viewModel.Memory.Value + decimal.Parse(viewModel.DisplayText);
        }

        private void ButtonMMinus_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumber(viewModel.DisplayText) && viewModel.Memory.HasValue)
                viewModel.Memory = viewModel.Memory.Value - decimal.Parse(viewModel.DisplayText);
        }

        private void ButtonMS_Click(object sender, RoutedEventArgs e)
        {
            if (IsNumber(viewModel.DisplayText))
                viewModel.Memory = decimal.Parse(viewModel.DisplayText);
        }

        private void ButtonMore_Click(object sender, RoutedEventArgs e)
        {

        }

        private static bool IsNumber(string str)
        {
            try
            {
                decimal.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    class MainPageViewModel : INotifyPropertyChanged
    {
        public string DisplayText { get => sb.ToString(); }

        public Brush TextColor
        {
            get => (Status == DisplayStatus.Ok)
                        ? new SolidColorBrush(Colors.Black)
                        : new SolidColorBrush(Colors.Red);
        }

        private DisplayStatus status = DisplayStatus.Ok;
        public DisplayStatus Status
        {
            get => status;
            private set
            {
                status = value;
                NotifyTextColorChanged();
            }
        }

        public void AppendNumber(decimal number)
        {
            if (Status == DisplayStatus.Error)
                Clear();
            sb.Append(number);
            NotifyDisplayTextChanged();
        }

        public void AppendOperator(char ch)
        {
            if (Status == DisplayStatus.Error)
                Clear();
            sb.Append(ch);
            NotifyDisplayTextChanged();
        }

        public void Delete()
        {
            if (Status == DisplayStatus.Error)
                Clear();
            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - 1, 1);
                NotifyDisplayTextChanged();
            }
        }

        public void Clear()
        {
            sb.Clear();
            Status = DisplayStatus.Ok;
            NotifyDisplayTextChanged();
        }

        public async void ShowResult()
        {
            if (Status == DisplayStatus.Error)
            {
                Clear();
                return;
            }
            try
            {
                decimal result = Calculator.CalculateFromString(sb.ToString());
                sb.Clear();
                sb.Append(result);
                NotifyDisplayTextChanged();
            }
            catch (InvalidExpresionException)
            {
                sb.Clear();
                sb.Append("Invalid Expression");
                NotifyDisplayTextChanged();
                Status = DisplayStatus.Error;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public decimal? Memory { get; set; }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyDisplayTextChanged() => NotifyPropertyChanged("DisplayText");

        private void NotifyTextColorChanged() => NotifyPropertyChanged("TextColor");

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly StringBuilder sb = new StringBuilder();
    }

    enum DisplayStatus
    {
        Ok,
        Error
    }
}
