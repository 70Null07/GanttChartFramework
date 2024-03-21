using LiveCharts.Defaults;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using static GanttChartFramework.MainWindow;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;

namespace GanttChartFramework
{
	public partial class ControlCenter : Page, INotifyPropertyChanged
	{

		public SeriesCollection Series { get; set; }
		public Func<double, string> Formatter { get; set; }
		public string[] Labels { get; set; }

		public List<int> StartQueue { get; set; }

		private int[,] randomArray;

		private int rndMachines;
		private int rndDetails;

		private double _from;
		private double _to;

		private ChartValues<GanttPoint> _values;

		public ControlCenter()
		{
			InitializeComponent();

			DataContext = this;
		}

		public ControlCenter(int _rndMachines, int _rndDetails, int[,] _array, List<int> _queue)
		{
			InitializeComponent();

			randomArray = _array;
			rndMachines = _rndMachines;
			rndDetails = _rndDetails;
			StartQueue = _queue;

			FillGanttChart();

			DataContext = this;
		}

		public void FillGanttChart()
		{
			_values = new ChartValues<GanttPoint>();
			Series = new SeriesCollection();

			var now = DateTime.Now;

			for (int i = 0; i < rndDetails; i++)
			{
				_values = new ChartValues<GanttPoint>();
				for (int j = 0; j < rndMachines; j++)
				{
					if (j == 0 && i == 0)
						_values.Add(new GanttPoint(now.Ticks, now.AddDays(randomArray[j, i]).Ticks));
					else if (Series.Count == 0)
					{
						DateTime dt = new DateTime((long)_values.Last().EndPoint);
						dt = dt.AddDays(1);
						_values.Add(new GanttPoint(dt.Ticks, dt.AddDays(randomArray[j, i]).Ticks));
					}
					else
					{
						var t = Series.Last();
						ChartValues<GanttPoint> k = t.Values as ChartValues<GanttPoint>;
						DateTime dt = new DateTime((long)k[j].EndPoint);
						if (_values.Count != 0 && dt < new DateTime((long)_values.Last().EndPoint))
							dt = new DateTime((long)_values.Last().EndPoint);
						dt = dt.AddDays(1);
						_values.Add(new GanttPoint(dt.Ticks, dt.AddDays(randomArray[j, i]).Ticks));
					}
				}
				List<SolidColorBrush> colors = new List<SolidColorBrush>
				{
					new SolidColorBrush(Colors.Red),
					new SolidColorBrush(Colors.Green),
					new SolidColorBrush(Colors.Blue),
					new SolidColorBrush (Colors.Magenta),
					new SolidColorBrush (Colors.Yellow),
					new SolidColorBrush (Colors.Purple),
					new SolidColorBrush (Colors.Gray),
					new SolidColorBrush (Colors.PeachPuff),
					new SolidColorBrush (Colors.Pink),
					new SolidColorBrush (Colors.Orange),
					new SolidColorBrush (Colors.Orchid),
				};
				if (i == rndDetails - 1)
				{
					DateTime dt = new DateTime((long)_values.Last().EndPoint);
					DateTime dtNow = DateTime.Now;
					var days = dt - dtNow;
					timeTakesLabel.Content += " " + days.Days;
				}
				if (i < colors.Count)
					Series.Add(new RowSeries
					{
						Values = _values,
						DataLabels = true,
						Fill = colors[i],
						Title = "Деталь " + StartQueue[i],
					});
				else
				{
					Series.Add(new RowSeries
					{
						Values = _values,
						DataLabels = true,
						Fill = new SolidColorBrush(Colors.Gold),
						Title = "Деталь " + i,
					});
				}
			}
			Formatter = value => new DateTime((long)value).ToString("dd MMM");

			var labels = new List<string>();
			for (var i = 0; i < rndMachines; i++)
				labels.Add("Станок " + (i + 1));
			Labels = labels.ToArray();

			//maxValue = 11.0;

			ResetZoomOnClick(null, null);
		}

		public double From
		{
			get { return _from; }
			set
			{
				_from = value;
				OnPropertyChanged("From");
			}
		}

		public double To
		{
			get { return _to; }
			set
			{
				_to = value;
				OnPropertyChanged("To");
			}
		}

		private void ResetZoomOnClick(object sender, RoutedEventArgs e)
		{
			//From = _values.First().StartPoint;
			From = new GanttPoint(DateTime.Now.AddDays(0).Ticks, DateTime.Now.AddDays(0).Ticks).EndPoint;
			//To = _values.Last().EndPoint;
			To = new GanttPoint(DateTime.Now.AddDays(0).Ticks, DateTime.Now.AddDays(70).Ticks).EndPoint;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

        private void ExportValues_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.ShowDialog();

            if (openFolderDialog.SelectedPath != string.Empty)
            {
                try
                {
                    MyData data = new MyData();
                    data.Matrix = randomArray;
					data.Method = this.WindowTitle;
					data.Order = StartQueue;

                    var json = JsonConvert.SerializeObject(data);
                    File.WriteAllText(openFolderDialog.SelectedPath + "\\result.json", json);
                }
                catch { }
            }
        }
    }
}
