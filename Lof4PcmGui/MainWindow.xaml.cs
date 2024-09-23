using System.Collections.Generic;
using Lof4PcmGui.ViewModels;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lof4PcmGui.Lof4Pcm;
using Microsoft.Win32;
using System.Drawing.Printing;
using ScottPlot;

namespace Lof4PcmGui
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).AppWindowMain = this;
        }

        private void NumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        #region perfectly PCM
        private void ButtonPerfectlyPcmGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                var txt = vm.GenPerfectlyPcm();
                vm.PerfectlyPcmText = txt;
            }
        }

        #endregion perfectly PCM

        #region UFO PCM
        private void ButtonUfoPcmGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                var txt = vm.GenOneUfoPcm();
                vm.UfoPcmText = txt;
            }
        }

        private void ButtonBrowserUfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "XML (*.xml) |*.xml" };
                saveFileDialog.Title = "Select file to save data";
                var result = saveFileDialog.ShowDialog(this);
                if (result == true)
                    vm.UfoMultiSaveFilePathname = saveFileDialog.FileName;
            }
        }

        private void ButtonUfoMultiStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                if (!int.TryParse(vm.NumberOfGeneratedPcmsUfoStr, out var number))
                {
                    MessageBox.Show("The generation amount is invalidate!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (vm.UfoMultiSaveFilePathname == null || vm.UfoMultiSaveFilePathname.Length <= 0)
                {
                    MessageBox.Show("A file for saving data must be selected!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                vm.UpdateUfoMessage("");
                vm.UpdateUfoProcessingInfo(0);
                vm.UpdateUfoCalculatStatus(true);

                vm.StartGenMultiUfo();
            }
        }

        private void ButtonUfoMultiStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.SetUfoStopFlag(true);
            }
        }

        #endregion UFO PCM

        #region EA PCM

        private void ButtonBrowserEa_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "XML (*.xml) |*.xml" };
                saveFileDialog.Title = "Select file to save data";
                var result = saveFileDialog.ShowDialog(this);
                if (result == true)
                    vm.EaMultiSaveFilePathname = saveFileDialog.FileName;
            }
        }

        private void ButtonEaPcmGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                var txt = vm.GenOneEaPcm();
                vm.EaPcmText = txt;
            }
        }

        private void ButtonEaMultiStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                if (!int.TryParse(vm.NumberOfGeneratedPcmsEaStr, out var number))
                {
                    MessageBox.Show("The generation amount is invalidate!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (vm.EaMultiSaveFilePathname == null || vm.EaMultiSaveFilePathname.Length <= 0)
                {
                    MessageBox.Show("A file for saving data must be selected!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                vm.UpdateEaMessage("");
                vm.UpdateEaProcessingInfo(0);
                vm.UpdateEaCalculatStatus(true);

                vm.StartGenMultiEa();
            }
        }

        private void ButtonEaMultiStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.SetEaStopFlag(true);
            }
        }

        #endregion EA PCM

        #region EA & UFO PCM
        private void ButtonEaUfoPcmGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                var txt = vm.GenOneEaUfoPcm();
                vm.EaPcmText = txt;
            }
        }

        private void ButtonBrowserEaUfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "XML (*.xml) |*.xml" };
                saveFileDialog.Title = "Select file to save data";
                var result = saveFileDialog.ShowDialog(this);
                if (result == true)
                    vm.EaUfoMultiSaveFilePathname = saveFileDialog.FileName;
            }
        }

        private void ButtonEaUfoMultiStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                if (!int.TryParse(vm.NumberOfGeneratedPcmsEaUfoStr, out var number))
                {
                    MessageBox.Show("The generation amount is invalidate!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (vm.EaUfoMultiSaveFilePathname == null || vm.EaUfoMultiSaveFilePathname.Length <= 0)
                {
                    MessageBox.Show("A file for saving data must be selected!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                vm.UpdateEaMessage("");
                vm.UpdateEaProcessingInfo(0);
                vm.UpdateEaCalculatStatus(true);

                vm.StartGenMultiEaUfo();
            }
        }

        private void ButtonEaUfoMultiStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.SetEaUfoStopFlag(true);
            }
        }

        #endregion EA & UFO PCM

        #region LOF base

        #endregion LOF base

        #region LOF of UFO

        private void ButtonLoadUfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "XML (*.xml) |*.xml" };
                openFileDialog.Title = "Select file to load data";
                openFileDialog.Multiselect = false;
                var result = openFileDialog.ShowDialog(this);
                if (result == true)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        vm.FilePathname = openFileDialog.FileName;

                        vm.UpdateLoadDataMessage("");
                        vm.UpdateLoadDataProcessingInfo(0);
                        vm.UpdateLoadDataCalculatStatus(true);

                        vm.StartLoadData();
                    }
                }
            }
        }

        private void ButtonStartCalcLofUfo_OnClick(object sender, RoutedEventArgs e)
        {

            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                if (vm.UfoDataLoaded4Lof == null)
                {
                    MessageBox.Show("Matrices hasn't loaded!",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    vm.OutlierResults = null;

                    vm.UpdateEaMessage("");
                    vm.UpdateCalcLofProcessingInfo(0);
                    vm.UpdateCalcLofCalculatStatus(true);

                    vm.StartCalcLof();
                }
            }
        }

        private void ButtonStopCalcLofUfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.SetLoadAndCalcLofStopFlag(true);
            }
        }


        private void OutlierResults_OnSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is LofResult)
                {
                    LofResult data = (LofResult)e.AddedItems[0];

                    // 从MinOuterlier到MaxOuterlier分为100个区间
                    // 每个区间的宽度
                    double range = (data.MaxOuterlier - data.MinOuterlier) / 99.0;
                    // 每个区间的上限
                    double[] uppers = new double[100];
                    uppers[0] = data.MinOuterlier;
                    for (int i = 1; i < 100; i++)
                        uppers[i] = uppers[i - 1] + range;
                    uppers[99] = double.MaxValue;

                    int[] countInRange = new int[100];
                    for (int i = 0; i < 100; i++)
                        countInRange[i] = 0;
                    foreach (var outlier in data.Outliers)
                    {
                        // 找到在哪一个区间内
                        int index = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            if (outlier < uppers[i])
                            {
                                index = i;
                                break;
                            }
                        }
                        countInRange[index]++;
                    }

                    double xMargin = (data.MaxOuterlier - data.MinOuterlier) / 33.0;
                    uppers[99] = data.MaxOuterlier + xMargin;

                    // plot
                    PlotOutlier.Plot.Clear();
                    var scatter = PlotOutlier.Plot.Add.Scatter(uppers, countInRange);
                    double max = 0, min = 0;
                    for (int i = 0; i < 100; i++)
                    {
                        if (countInRange[i] > max)
                            max = countInRange[i];
                        else if (countInRange[i] < min)
                            min = countInRange[i];
                    }

                    scatter.LegendText = tbLegend.Text;
                    PlotOutlier.Plot.Legend.Alignment = Alignment.UpperRight;
                    PlotOutlier.Plot.Axes.SetLimits(data.MinOuterlier - xMargin, data.MaxOuterlier + 2.0 * xMargin,
                        min - 100, max + 100);
                    PlotOutlier.Refresh();
                }
            }

        }

        #endregion LOF of UFO

        #region Unconsistent Reason Check

        private void ButtonCheckInconsistenceType_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                vm.UpdateReasonCheckMessage(-1,-1,-1);
                vm.UpdateReasonCheckProcessingInfo(0);
                vm.UpdateReasonCheckCalculatStatus(true);

                vm.StartReasonCheck();
            }
        }

        #endregion Unconsistent Reason Check

        private void ButtonCheckOptimalDirectionAdjust_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                vm.UpdateAdjustCheckMessage(null);
                vm.UpdateAdjustCheckProcessingInfo(0);
                vm.UpdateAdjustCheckCalculatStatus(true);

                vm.StartAdjustCheck();
            }
        }

        private void ButtonCheckMinChangeAdjust_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                vm.UpdateMinChangeCheckMessage(null);
                vm.UpdateMinChangeCheckProcessingInfo(0);
                vm.UpdateMinChangeCheckCalculatStatus(true);

                vm.StartMinChangeCheck();
            }
        }

    }
}
