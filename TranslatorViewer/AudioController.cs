using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Newtonsoft.Json;


namespace TranslatorViewer
{
    public enum Direction
    {
        Input, Output
    }
    public class AudioDevice
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public Direction direction { get; set; } // "Input" veya "Output"
    }

    public class AudioController
    {
        public List<AudioDevice> AllDevices = new List<AudioDevice>();
        public List<int> baudRateList = new List<int>();

        public AudioController()
        {
            LoadDevices();
        }

        public List<AudioDevice> GetDevices()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "getdevice.py", // Python dosyanın adı
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            try
            {
                var devices = JsonConvert.DeserializeObject<List<AudioDevice>>(output);
                return devices;
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON parse hatası: " + ex.Message);
                return null;
            }
        }
        private void LoadDevices()
        {
            AllDevices = new List<AudioDevice>();
            baudRateList = new List<int>();
            // 🎧 Output Devices
            AllDevices = GetDevices();


            baudRateList = AllDevices
            .Select(d => d.SampleRate)     // sadece SampleRate değerlerini seç
            .Distinct()                    // tekrar edenleri çıkar
            .OrderBy(rate => rate)         // küçükten büyüğe sırala (opsiyonel)
            .ToList();
        }
    }
}
