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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TranslatorViewer
{
    /// <summary>
    /// AudioWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AudioWindow : Window
    {
        AudioController audioController;
        public AudioWindow()
        {
            InitializeComponent();
            audioController = new AudioController();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BaudRateBox.ItemsSource = audioController.baudRateList;
            BaudRateBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<AudioDevice> filteredInput = audioController.AllDevices.Where(x => x.Name.ToLower().Contains("cable") && x.Channels >= 2 && x.SampleRate == Convert.ToInt32(BaudRateBox.SelectedItem) && Direction.Input == x.direction).ToList();
            List<AudioDevice> filteredOutput = audioController.AllDevices.Where(x => x.Name.ToLower().Contains("free") && x.Channels >= 2 && x.SampleRate == Convert.ToInt32(BaudRateBox.SelectedItem) && Direction.Output == x.direction).ToList();
            debugBox.Items.Clear();
            debugBox.Items.Add("Input Devices");
            for (int i = 0; i < filteredInput.Count; i++)
            {
                debugBox.Items.Add($"{filteredInput[i].Index}---{filteredInput[i].Name}-{filteredInput[i].Channels}");
            }
            debugBox.Items.Add("Output Devices");
            for (int i = 0; i < filteredOutput.Count; i++)
            {
                debugBox.Items.Add($"{filteredOutput[i].Index}---{filteredOutput[i].Name}-{filteredOutput[i].Channels}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int inputIndex = Convert.ToInt32(InputBox.Text);
                int outputIndex = Convert.ToInt32(OutputBox.Text);
                int channelIndex = Convert.ToInt32(ChannelBox.Text);
                int baudrate = Convert.ToInt32(BaudRateBox.SelectedItem);
                MainWindow.instance.StartPython(inputIndex, outputIndex, baudrate, channelIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("Please control all values");
            }
        }
    }
}
