using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hours
{
    public partial class Form1 : Form
    {
        private List<DateTime> startTimes = new List<DateTime>();
        private List<DateTime> stopTimes = new List<DateTime>();

        private string dataDirPath;
        private string startTimesDataPath;
        private string stopTimesDataPath;

        private int workingStopTimePath;
        private bool running;

        Timer tickTimer = new Timer();

        public Form1()
        {
            InitializeComponent();

            tickTimer.Interval = 200;
            tickTimer.Tick += new EventHandler(Tick);
            tickTimer.Start();


            dataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hours";
            if (!Directory.Exists(dataDirPath))
                Directory.CreateDirectory(dataDirPath);
            startTimesDataPath = dataDirPath + "\\startTimes.txt";
            stopTimesDataPath = dataDirPath + "\\stopTimes.txt";
        }

        private void CreateAndStartNewEntry()
        {
            ReadTimes();
            workingStopTimePath = stopTimes.Count;
            startTimes.Add(DateTime.Now);
            stopTimes.Add(DateTime.Now);
            running = true;
        }

        private void Tick(object sender, EventArgs e)
        {
            if (running)
            {
                stopTimes[workingStopTimePath] = DateTime.Now;
                WriteTimes();
            }

            ReadTimes();
            totalLabel.Text =   "Total: " + Math.Round(GetTotalTime(), 2).ToString() + "h";

            if (running)
            {
                currentLabel.Text = "Current: " + Math.Round(GetCurrentTime(), 2).ToString() + "h";
            }
        }

        private void ReadTimes()
        {
            if (!File.Exists(startTimesDataPath) || !File.Exists(stopTimesDataPath))
                return;

            startTimes.Clear();
            stopTimes.Clear();

            string[] startFileLines = File.ReadAllLines(startTimesDataPath);
            string[] stopFileLines = File.ReadAllLines(stopTimesDataPath);

            foreach (string startTimeLine in startFileLines)
            {
                try
                {
                    startTimes.Add(DateTime.Parse(startTimeLine));
                }
                catch { }
            }

            foreach (string stopTimeLine in stopFileLines)
            {
                try
                {
                    stopTimes.Add(DateTime.Parse(stopTimeLine));
                }
                catch { }
            }
        }

        private void WriteTimes()
        {
            File.WriteAllText(startTimesDataPath, "");
            File.WriteAllText(stopTimesDataPath, "");

            foreach (DateTime startTime in startTimes)
            {
                File.AppendAllText(startTimesDataPath, startTime.ToString() + "\n");
            }

            foreach (DateTime stopTime in stopTimes)
            {
                File.AppendAllText(stopTimesDataPath, stopTime.ToString() + "\n");
            }
        }


        private float GetTotalTime()
        {
            float totalHours = 0;

            for (int i = 0; i < startTimes.Count; i++)
            {
                TimeSpan dif = stopTimes[i].Subtract(startTimes[i]);
                totalHours += (float)dif.TotalHours;
            }

            return totalHours;
        }

        private float GetCurrentTime()
        {
            if (stopTimes.Count <= 0 || startTimes.Count <= 0)
                return 0f;

            TimeSpan dif = stopTimes[stopTimes.Count - 1].Subtract(startTimes[startTimes.Count - 1]);
            return (float)dif.TotalHours;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (running)
            {
                startButton.Text = "Start";
                running = false;
            }
            else
            {
                CreateAndStartNewEntry();
                startButton.Text = "Stop";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
