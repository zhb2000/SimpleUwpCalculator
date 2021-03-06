﻿using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimpleUwpCalculator
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainPageViewModel viewModel = new MainPageViewModel();

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
            Splview.IsPaneOpen = !Splview.IsPaneOpen;
        }

        private void Splview_PaneOpening(SplitView sender, object args)
        {
            if (Splview.DisplayMode == SplitViewDisplayMode.Overlay)
                mask.Visibility = Visibility.Visible;
        }

        private void Splview_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            mask.Visibility = Visibility.Collapsed;
        }

        private void HistoryItemClick(object sender, ItemClickEventArgs e)
        {
            viewModel.Clear();
            viewModel.AppendNumber(((HistoryItem)e.ClickedItem).Result);
        }

        private void DeleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Histories.Clear();
        }

        /// <summary>
        /// the string is a valid dicimal or not
        /// </summary>
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
}
