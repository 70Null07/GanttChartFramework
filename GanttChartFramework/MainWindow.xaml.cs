using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GanttChartFramework
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public List<string> JohnsonGeneralizations { get; set; }
		public List<string> PeterSinitsynGeneralizations { get; set; }
		public List<string> ResultsOrders { get; set; }

		public List<int> FirstStartQueue = new List<int>();
		public List<int> SecondStartQueue = new List<int>();
		public List<int> ThirdStartQueue = new List<int>();
		public List<int> FourthStartQueue = new List<int>();
		public List<int> FifthStartQueue = new List<int>();

		public List<int> FirstOrder = new List<int>();
		public List<int> SecondOrder = new List<int>();
		public List<int> ThirdOrder = new List<int>();

		public int SelectedJohnson { get; set; }
		public int SelectedPeter { get; set; }
		public int SelectedResult {  get; set; }

		List<KeyValuePair<string, KeyValuePair<int[,], List<int>>>> resultsList;

		public int[,] randomArray;
		public int[,] orderedArray;

		public event PropertyChangedEventHandler PropertyChanged;

		// Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
		public void RaisePropertyChanged(string propertyName)
		{
			// Если кто-то на него подписан, то вызывем его
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public int rndMachines { get; set; }
		public int rndDetails { get; set; }

		public int RndMachines {
			get { return rndMachines; }
			set
			{
				rndMachines = value;
				RaisePropertyChanged(nameof(rndMachines));
			}
			}
        public int RndDetails
        {
            get { return rndDetails; }
            set
            {
                rndDetails = value;
                RaisePropertyChanged(nameof(rndDetails));
            }
        }

        public MainWindow()
		{
			InitializeComponent();
			JohnsonGeneralizations = new List<string>();
			for (int i = 1; i < 6; i++)
				JohnsonGeneralizations.Add(i + " Обобщение");

			SelectedJohnson = 0;

			PeterSinitsynGeneralizations = new List<string>();
			for (int i = 1; i < 4; i++)
				PeterSinitsynGeneralizations.Add(i + "  Очередь");
			DataContext = this;

			ResultsOrders = new List<string>();
            resultsList = new List<KeyValuePair<string, KeyValuePair<int[,], List<int>>>>();

            SelectedPeter = 0;
		}

		private void CalculateJohnson_Click(object sender, RoutedEventArgs e)
		{
			List<int> DefaultQueue = new List<int>();
			for (int i = 0; i < rndDetails; i++)
				DefaultQueue.Add(i);
			switch (SelectedJohnson)
			{
				// В обработку запускаются детали с мин. временем обработки на 
				// 1 станке в порядке возрастания
				case 0:
					{
						FirstGeneralization();
						ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, DefaultQueue);
						NavigationWindow navigationWindow = new NavigationWindow
						{
							Title = "Исходные данные",
							Content = controlCenter
						};
						navigationWindow.Show();


						ControlCenter _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, FirstStartQueue);
						NavigationWindow _navigationWindow = new NavigationWindow
						{
							Title = "Первое обобщение",
							Content = _controlCenter
						};
						_navigationWindow.Show();
					}
					break;
				//В обработку детали с макс. временем обработки
				//на последнем станке
				case 1:
					{
						SecondGeneralization();

						ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, DefaultQueue);
						NavigationWindow navigationWindow = new NavigationWindow
						{
							Title = "Исходные данные",
							Content = controlCenter
						};
						navigationWindow.Show();


						ControlCenter _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, SecondStartQueue);
						NavigationWindow _navigationWindow = new NavigationWindow
						{
							Title = "Второе обобщение",
							Content = _controlCenter
						};
						_navigationWindow.Show();
					}
					break;
				// В обработку детали у которых узкое место "находится дальше" от начала 
				// процесса обработки. "Узкое место" - стакан с макс. временем обработки для данной
				// детали.
				case 2:
					{
						ThirdGeneralization();

						ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, DefaultQueue);
						NavigationWindow navigationWindow = new NavigationWindow
						{
							Title = "Исходные данные",
							Content = controlCenter
						};
						navigationWindow.Show();


						ControlCenter _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, ThirdStartQueue);
						NavigationWindow _navigationWindow = new NavigationWindow
						{
							Title = "Третье обобщение",
							Content = _controlCenter
						};
						_navigationWindow.Show();
					}
					break;
				// Обрабатываются детали с суммарным временем обработки максимальным
				// в порядке убывания
				case 3:
					{
						FourthGeneralization();

						ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, DefaultQueue);
						NavigationWindow navigationWindow = new NavigationWindow
						{
							Title = "Исходные данные",
							Content = controlCenter
						};
						navigationWindow.Show();


						ControlCenter _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, FourthStartQueue);
						NavigationWindow _navigationWindow = new NavigationWindow
						{
							Title = "Четвертое обобщение",
							Content = _controlCenter
						};
						_navigationWindow.Show();
					}
					break;
				case 4:
					{
						FifthGeneralization();

						ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, DefaultQueue);
						NavigationWindow navigationWindow = new NavigationWindow
						{
							Title = "Исходные данные",
							Content = controlCenter
						};
						navigationWindow.Show();


						ControlCenter _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, FifthStartQueue);
						NavigationWindow _navigationWindow = new NavigationWindow
						{
							Title = "Пятое обобщение",
							Content = _controlCenter
						};
						_navigationWindow.Show();
					}
					break;
				default:
					break;
			}
		}

		public void FirstGeneralization()
		{
			List<KeyValuePair<int, int>> DefaultQueue = new List<KeyValuePair<int, int>>();

			for (int i = 0; i < rndDetails; i++)
				DefaultQueue.Add(new KeyValuePair<int, int>(i, randomArray[0, i]));

			for (int i = 0; i < rndDetails; i++)
			{
				int min = 365;
				int curr_index = 0;
				for (int j = 0; j < DefaultQueue.Count; j++)
				{
					if (DefaultQueue[j].Value <= min)
					{
						min = DefaultQueue[j].Value;
						curr_index = j;
					}
				}
				FirstStartQueue.Add(DefaultQueue[curr_index].Key);
				DefaultQueue.RemoveAt(curr_index);
			}
			orderedArray = new int[rndMachines, rndDetails];
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					orderedArray[i, j] = randomArray[i, FirstStartQueue[j]];
				}
			}
		}

		public void SecondGeneralization()
		{
			List<KeyValuePair<int, int>> DefaultQueue = new List<KeyValuePair<int, int>>();

			for (int i = 0; i < rndDetails; i++)
				DefaultQueue.Add(new KeyValuePair<int, int>(i, randomArray[rndMachines - 1, i]));

			for (int i = 0; i < rndDetails; i++)
			{
				int max = 0;
				int curr_index = 0;
				for (int j = 0; j < DefaultQueue.Count; j++)
				{
					if (DefaultQueue[j].Value >= max)
					{
						max = DefaultQueue[j].Value;
						curr_index = j;
					}
				}
				SecondStartQueue.Add(DefaultQueue[curr_index].Key);
				DefaultQueue.RemoveAt(curr_index);
			}
			orderedArray = new int[rndMachines, rndDetails];
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					orderedArray[i, j] = randomArray[i, SecondStartQueue[j]];
				}
			}
		}

		public void ThirdGeneralization()
		{

			List<KeyValuePair<int, KeyValuePair<int, int>>> DefaultQueue = new List<KeyValuePair<int, KeyValuePair<int, int>>>();

			for (int i = 0; i < rndDetails; i++)
			{
				int max = 0;
				// Номер станка
				int curr_index = 0;
				for (int j = 0; j < rndMachines; j++)
				{
					if (randomArray[j, i] > max)
					{
						max = randomArray[j, i];
						curr_index = j;
					}
				}
				DefaultQueue.Add(new KeyValuePair<int, KeyValuePair<int, int>>(i, new KeyValuePair<int, int>(curr_index, max)));
			}
			for (int i = 0; i < rndDetails; i++)
			{
				List<KeyValuePair<int, int>> sameMax = new List<KeyValuePair<int, int>>();
				int max = 0;
				List<int> curr_index = new List<int>();
				for (int j = 0; j < DefaultQueue.Count; j++)
				{
					if (DefaultQueue[j].Value.Value > max)
					{
						sameMax.Clear();
						curr_index.Clear();
						sameMax.Add(new KeyValuePair<int, int>(DefaultQueue[j].Value.Key, DefaultQueue[j].Value.Value));
						curr_index.Add(j);
						max = DefaultQueue[j].Value.Value;
					}
					else if (DefaultQueue[j].Value.Value == max)
					{
						sameMax.Add(new KeyValuePair<int, int>(DefaultQueue[j].Value.Key, DefaultQueue[j].Value.Value));
						curr_index.Add(j);
					}
				}

				if (sameMax.Count == 1)
				{
					ThirdStartQueue.Add(DefaultQueue[curr_index[0]].Key);
					DefaultQueue.RemoveAt(curr_index[0]);
				}
				else
				{
					int max_place = 0;
					int curr_place = 0;
					for (int j = 0; j < sameMax.Count; j++)
					{
						if (sameMax[j].Key > max_place)
						{
							max_place = sameMax[j].Key;
							curr_place = j;
						}
					}

					ThirdStartQueue.Add(DefaultQueue[curr_index[curr_place]].Key);
					DefaultQueue.RemoveAt(curr_index[curr_place]);
				}
			}

			orderedArray = new int[rndMachines, rndDetails];
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					orderedArray[i, j] = randomArray[i, ThirdStartQueue[j]];
				}
			}
		}

		public void FourthGeneralization()
		{
			List<KeyValuePair<int, int>> DefaultQueue = new List<KeyValuePair<int, int>>();

			for (int i = 0; i < rndDetails; i++)
			{
				int max = 0;
				for (int j = 0; j < rndMachines; j++)
				{
					max += randomArray[j, i];
				}
				DefaultQueue.Add(new KeyValuePair<int, int>(i, max));
			}

			for (int i = 0; i < rndDetails; i++)
			{
				int max = 0;
				int curr_index = 0;
				for (int j = 0; j < DefaultQueue.Count; j++)
				{
					if (DefaultQueue[j].Value >= max)
					{
						max = DefaultQueue[j].Value;
						curr_index = j;
					}
				}
				FourthStartQueue.Add(DefaultQueue[curr_index].Key);
				DefaultQueue.RemoveAt(curr_index);
			}
			orderedArray = new int[rndMachines, rndDetails];
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					orderedArray[i, j] = randomArray[i, FourthStartQueue[j]];
				}
			}
		}

		private void FifthGeneralization()
		{
			if (FirstStartQueue.Count == 0)
				FirstGeneralization();
			if (SecondStartQueue.Count == 0)
				SecondGeneralization();
			if (ThirdStartQueue.Count == 0)
				ThirdGeneralization();
			if (FourthStartQueue.Count == 0)
				FourthGeneralization();

			List<KeyValuePair<int, int>> averageOfGeneralization = new List<KeyValuePair<int, int>>();

			for (int i = 0; i < rndDetails; i++)
			{
				averageOfGeneralization.Add(new KeyValuePair<int, int>(i, (FirstStartQueue.IndexOf(i) +
					SecondStartQueue.IndexOf(i) + ThirdStartQueue.IndexOf(i) + FourthStartQueue.IndexOf(i))));
			}
			for (int i = 0; i < rndDetails; i++)
			{
				int curr_index = 0;
				int min = 100;
				for (int j = 0; j < averageOfGeneralization.Count; j++)
				{
					if (averageOfGeneralization[j].Value < min)
					{
						curr_index = j;
						min = averageOfGeneralization[j].Value;
					}
				}
				FifthStartQueue.Add(averageOfGeneralization[curr_index].Key);
				averageOfGeneralization.RemoveAt(curr_index);
			}
			orderedArray = new int[rndMachines, rndDetails];
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					orderedArray[i, j] = randomArray[i, FifthStartQueue[j]];
				}
			}
		}

		private void CalculatePeterSinitsyn_Click(object sender, RoutedEventArgs e)
		{
            List<KeyValuePair<int, int>> DefaultQueue = new List<KeyValuePair<int, int>>();
            List<int> _DefaultQueue = new List<int>();
            for (int i = 0; i < rndDetails; i++)
                _DefaultQueue.Add(i);

            switch (SelectedPeter)
			{
				case 0:
					{
						for (int i = 0; i < rndDetails; i++)
						{
							int max = 0;
							for (int j = 0; j < rndMachines - 1; j++)
							{
								max += randomArray[j, i];
							}
							DefaultQueue.Add(new KeyValuePair<int, int>(i, max));
						}
						for (int i = 0; i < rndDetails; i++)
						{
							int min = 100;
							int curr_index = 0;
							for (int j = 0; j < DefaultQueue.Count; j++)
							{
								if (DefaultQueue[j].Value <= min)
								{
									min = DefaultQueue[j].Value;
									curr_index = j;
								}
							}
							FirstOrder.Add(DefaultQueue[curr_index].Key);
							DefaultQueue.RemoveAt(curr_index);
						}
						orderedArray = new int[rndMachines, rndDetails];
						for (int i = 0; i < rndMachines; i++)
						{
							for (int j = 0; j < rndDetails; j++)
							{
								orderedArray[i, j] = randomArray[i, FirstOrder[j]];
							}
						}
					}
					break;
				case 1:
					{

						for (int i = 0; i < rndDetails; i++)
						{
							int max = 0;
							for (int j = 1; j < rndMachines; j++)
							{
								max += randomArray[j, i];
							}
							DefaultQueue.Add(new KeyValuePair<int, int>(i, max));
						}
						for (int i = 0; i < rndDetails; i++)
						{
							int max = 0;
							int curr_index = 0;
							for (int j = 0; j < DefaultQueue.Count; j++)
							{
								if (DefaultQueue[j].Value >= max)
								{
									max = DefaultQueue[j].Value;
									curr_index = j;
								}
							}
							SecondOrder.Add(DefaultQueue[curr_index].Key);
							DefaultQueue.RemoveAt(curr_index);
						}
						orderedArray = new int[rndMachines, rndDetails];
						for (int i = 0; i < rndMachines; i++)
						{
							for (int j = 0; j < rndDetails; j++)
							{
								orderedArray[i, j] = randomArray[i, SecondOrder[j]];
							}
						}
					}
					break;
				case 2:
					{

						for (int i = 0; i < rndDetails; i++)
						{
							int max = randomArray[rndMachines - 1, i] - randomArray[0, i];

							DefaultQueue.Add(new KeyValuePair<int, int>(i, max));
						}
						for (int i = 0; i < rndDetails; i++)
						{
							int max = -100;
							int curr_index = 0;
							for (int j = 0; j < DefaultQueue.Count; j++)
							{
								if (DefaultQueue[j].Value >= max)
								{
									max = DefaultQueue[j].Value;
									curr_index = j;
								}
							}
							ThirdOrder.Add(DefaultQueue[curr_index].Key);
							DefaultQueue.RemoveAt(curr_index);
						}
						orderedArray = new int[rndMachines, rndDetails];
						for (int i = 0; i < rndMachines; i++)
						{
							for (int j = 0; j < rndDetails; j++)
							{
								orderedArray[i, j] = randomArray[i, ThirdOrder[j]];
							}
						}
					}
					break;
				default:
					break;
			}
            ControlCenter controlCenter = new ControlCenter(rndMachines, rndDetails, randomArray, _DefaultQueue);
            NavigationWindow navigationWindow = new NavigationWindow
            {
                Title = "Исходные данные",
                Content = controlCenter
            };
            navigationWindow.Show();

			ControlCenter _controlCenter = null;


            switch (SelectedPeter)
			{
				case 0:
                    _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, FirstOrder);
					break;
				case 1:
                    _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, SecondOrder);
					break;
				case 2:
                    _controlCenter = new ControlCenter(rndMachines, rndDetails, orderedArray, ThirdOrder);
					break;
				default:
					break;
            }
			if (_controlCenter != null)
			{
                NavigationWindow _navigationWindow = new NavigationWindow
                {
                    Title = "Метод Петрова-Соколицина",
                    Content = _controlCenter
                };
                _navigationWindow.Show();
            }
        }

		private void GenerateValues_Click(object sender, RoutedEventArgs e)
		{
			Random rndValues = new Random();
			rndMachines = rndValues.Next(10, 15);
			rndDetails = rndValues.Next(10, 15);

            RndDetails = rndDetails;
            RndMachines = rndMachines;

            randomArray = new int[rndMachines, rndDetails];

			// Заполняем массив случайными значениями от 0 до 10
			for (int i = 0; i < rndMachines; i++)
			{
				for (int j = 0; j < rndDetails; j++)
				{
					randomArray[i, j] = rndValues.Next(10); // От 0 до 10
				}
			}
		}

		public class MyData
		{
			public string Method { get; set; }
            public int[,] Matrix { get; set; }
			public List<int> Order { get; set; }
        }

		private void LoadValues_Click(object sender, RoutedEventArgs e)
		{
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != string.Empty)
            {
				try
				{
                    var json = File.ReadAllText(openFileDialog.FileName);

                    var jObject = JObject.Parse(json);
                    var matrixArray = jObject["Matrix"].ToObject<int[,]>();

                    randomArray = matrixArray;

                    rndDetails = matrixArray.GetLength(1);
                    RndDetails = matrixArray.GetLength(1);
                    rndMachines = matrixArray.GetLength(0);
                    RndMachines = matrixArray.GetLength(0);
                }
				catch { }
            }
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

                    var json = JsonConvert.SerializeObject(data);
                    File.WriteAllText(openFolderDialog.SelectedPath + "\\file.json", json);
                }
				catch { }
            }
        }

        private void LoadResultButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            try
            {
                var json = File.ReadAllText(openFileDialog.FileName);

                var jObject = JObject.Parse(json);
				var methodName = jObject["Method"].ToObject<string>();
                var matrixArray = jObject["Matrix"].ToObject<int[,]>();
				var methodOrder = jObject["Order"].ToObject<List<int>>();

				resultsList.Add(new KeyValuePair<string, KeyValuePair<int[,], List<int>>>(methodName, new KeyValuePair<int[,], List<int>>(matrixArray, methodOrder)));

				ResultsOrders.Add(methodName);
            }
            catch { }
        }

        private void ShowResultButton_Click(object sender, RoutedEventArgs e)
        {
			var item = resultsList[SelectedResult];

            ControlCenter controlCenter = new ControlCenter(item.Value.Key.GetLength(0), item.Value.Key.GetLength(1), item.Value.Key, item.Value.Value);
            NavigationWindow navigationWindow = new NavigationWindow
            {
                Title = item.Key,
                Content = controlCenter
            };
            navigationWindow.Show();
        }
    }
}
