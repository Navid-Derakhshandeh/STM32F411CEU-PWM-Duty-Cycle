using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace serialm_onitor
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        List<float> dutyData = new List<float>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxPorts.Items.AddRange(SerialPort.GetPortNames());
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxPorts.SelectedItem == null) return;

            serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(), 9600);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = serialPort.ReadLine();

            if (line.Contains("Duty Cycle:"))
            {
                string valueStr = line.Split(':')[1].Replace("%", "").Trim();
                if (float.TryParse(valueStr, out float duty))
                {
                    dutyData.Add(duty);
                    if (dutyData.Count > 100) dutyData.RemoveAt(0);

                    Invoke(new Action(() =>
                    {
                        labelDuty.Text = $"Duty Cycle: {duty}%";
                        graphPanel.Invalidate();
                    }));
                }
            }
        }

        private void graphPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Black);

            if (dutyData.Count < 2) return;

            Pen pen = new Pen(Color.Lime, 2);
            int w = graphPanel.Width;
            int h = graphPanel.Height;

            for (int i = 1; i < dutyData.Count; i++)
            {
                float x1 = (i - 1) * w / (float)dutyData.Count;
                float y1 = h - (dutyData[i - 1] * h / 100f);
                float x2 = i * w / (float)dutyData.Count;
                float y2 = h - (dutyData[i] * h / 100f);
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }
    }
}
