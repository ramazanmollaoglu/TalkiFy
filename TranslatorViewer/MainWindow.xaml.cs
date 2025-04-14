using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TranslatorViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    Process pythonProc;
    public static MainWindow instance;
    public MainWindow()
    {
        InitializeComponent();
        instance = this;
    }
    
    

    public void StartPython(int inputDevice,int outputDevice,int samplerate,int channels)
    {
        pythonProc = new Process();
        pythonProc.StartInfo.FileName = "python";
        pythonProc.StartInfo.Arguments = $"Translator.py --input {inputDevice} --output {outputDevice} --samplerate {samplerate} --channels {channels}";
        pythonProc.StartInfo.UseShellExecute = false;
        pythonProc.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        pythonProc.StartInfo.RedirectStandardOutput = true;
        pythonProc.StartInfo.RedirectStandardError = true; // Hataları da yakala
        pythonProc.StartInfo.CreateNoWindow = true;
        pythonProc.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Dispatcher.Invoke(() => {
                    DisplayText.Text = "❌ HATA: " + e.Data;
                });
            }
        };
        pythonProc.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Dispatcher.Invoke(() => {
                    if(e.Data.Contains("#tr:"))
                    {
                        DisplayText.Text += $"\n\n{e.Data}";
                    }
                    else
                    {
                        DisplayText.Text = e.Data;
                    }
                        
                });
            }
        };
        pythonProc.Start();
        pythonProc.BeginOutputReadLine();
        pythonProc.BeginErrorReadLine();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        // Genişliği orana göre hesapla
        double overlayWidth = screenWidth;
        double overlayHeight = ((screenHeight * 300) / 1080);

        // Boyut ve konum ayarla
        Width = overlayWidth;
        Height = overlayHeight;
        Left = 0; // sağ alt köşe için
        Top = screenHeight - overlayHeight; // üstten başlasın (dikeyde tam boy olacak)

        // Başlat python
        //StartPython();
        AudioWindow newWindow = new AudioWindow();
        newWindow.Show();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        try
        {
            if (pythonProc != null && !pythonProc.HasExited)
            {
                pythonProc.Kill();     // python.exe'yi öldürür
                pythonProc.Dispose();  // kaynakları temizler
            }
            var pythonProcesses = Process.GetProcessesByName("python");

            foreach (var proc in pythonProcesses)
            {
                try
                {
                    Console.WriteLine($"Kapatılıyor: PID {proc.Id} - {proc.ProcessName}");
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Python kapatılırken hata: " + ex.Message);
        }

    }
}