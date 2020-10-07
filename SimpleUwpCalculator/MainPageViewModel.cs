using SimpleUwpCalculator.Calculate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SimpleUwpCalculator
{
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
        private DisplayStatus Status
        {
            get => status;
            set
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

        /// <summary>
        /// clear text on display panel, then set status to Ok
        /// </summary>
        public void Clear()
        {
            sb.Clear();
            Status = DisplayStatus.Ok;
            NotifyDisplayTextChanged();
        }

        /// <summary>
        /// calculate and show result
        /// </summary>
        public async void ShowResult()
        {
            if (Status == DisplayStatus.Error)
            {
                Clear();
                return;
            }
            try
            {
                string expression = sb.ToString();
                decimal result = Calculator.CalculateFromString(expression);
                sb.Clear();
                sb.Append(result);
                NotifyDisplayTextChanged();
                Histories.Add(new HistoryItem(expression, result));
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

        private readonly StringBuilder sb = new StringBuilder();

        private decimal? memory;
        /// <summary>
        /// the number stored in memory
        /// </summary>
        public decimal? Memory
        {
            get => memory;
            set
            {
                memory = value;
                NotifyMemoryPropertiesChanged();
            }
        }

        public Visibility MemoryVisibility
        {
            get => Memory.HasValue ? Visibility.Visible
                                   : Visibility.Collapsed;
        }

        public decimal MemoryText { get => Memory ?? 0; }

        public ObservableCollection<HistoryItem> Histories { get; }
            = new ObservableCollection<HistoryItem>();

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyDisplayTextChanged() => NotifyPropertyChanged("DisplayText");

        private void NotifyTextColorChanged() => NotifyPropertyChanged("TextColor");

        private void NotifyMemoryPropertiesChanged()
        {
            NotifyPropertyChanged("MemoryVisibility");
            NotifyPropertyChanged("MemoryText");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private enum DisplayStatus
        {
            Ok,
            Error
        }
    }
}
