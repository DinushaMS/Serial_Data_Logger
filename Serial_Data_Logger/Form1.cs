using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Data_Logger
{
    public partial class MainForm : Form
    {
        double[] x = new double[1044];
        byte[] data = new byte[1044];
        byte[] incommingData = new byte[1044];
        double[] processedData = new double[1044];
        Random rnd = new Random();
        int count = 1044;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateData()
        {
            count = Convert.ToInt32(txtCount.Text);
            x = new double[count];
            data = new byte[count];
            incommingData = new byte[count];
            processedData = new double[count];
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ProcessData()
        {
            for (int i = 0; i < count; i++)
                processedData[i] = Convert.ToDouble(incommingData[i]);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            btnClose.Location = new Point(this.Width - 28, btnClose.Location.Y);
            btnMinimize.Location = new Point(this.Width - 56, btnMinimize.Location.Y);
            cmbComPort.Items.AddRange(SerialPort.GetPortNames());
            //serialPort2.BaudRate = 115200;
            //serialPort2.PortName = "COM2";
            //serialPort2.StopBits = StopBits.One;
            //serialPort2.Parity = Parity.None;
            //serialPort2.DataBits = 8;
            //serialPort2.Open();
            //while (!serialPort2.IsOpen) ;
            //MessageBox.Show("Connected!");
            //for (int i = 0; i < 1044; i++)
            //{
            //    x[i] = i;
            //    data[i] = Convert.ToByte(rnd.NextDouble() * 3.5 * (1 + Math.Sin((i / 1043.0) * Math.PI * 2)));
            //    //data[i] = Convert.ToByte(i % 8);
            //}
        }

        private void ReGenData()
        {
            double k = rnd.NextDouble() * 3.5;
            for (int i = 0; i < 1044; i++)
            {
                x[i] = i;
                data[i] = Convert.ToByte(k * (1 + Math.Sin((i / 1043.0) * Math.PI * 2)));
                //data[i] = Convert.ToByte(i % 8);
            }
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //ReGenData();
            //serialPort2.Write(data, 0, data.Length);
            //Thread.Sleep(500);
            UpdateData();
        }

        private void UpdateGraph()
        {
            ProcessData();

            dataPlot.plt.Clear();
            Color figureBgColor = ColorTranslator.FromHtml("#001021");
            Color dataBgColor = ColorTranslator.FromHtml("#021d38");
            dataPlot.plt.Style(figBg: figureBgColor, dataBg: dataBgColor);
            dataPlot.plt.Grid(color: ColorTranslator.FromHtml("#273c51"));
            dataPlot.plt.Ticks(color: Color.LightGray, useMultiplierNotation: false);
            dataPlot.plt.PlotScatter(x, processedData, label: "Series 1");
            dataPlot.plt.Title("");
            dataPlot.plt.XLabel("Pixel Number", color: Color.White);
            dataPlot.plt.YLabel("ADC value", color: Color.White);
            dataPlot.plt.AxisAuto();
            //dataPlot.plt.Legend(true, fontColor: Color.White);
            dataPlot.Render();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                serialPort1.BaudRate = Convert.ToInt32(cmbBAUD.Text);
                serialPort1.PortName = cmbComPort.Text;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;                                            // attempt to connect to USB board
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEvent);
                serialPort1.Open();
                while (!serialPort1.IsOpen) ;
                //MessageBox.Show("Connected!");
                timer1.Start();
                //backgroundWorker1.RunWorkerAsync();
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.LimeGreen;
            }
            else
            {
                serialPort1.Close();
                while (serialPort1.IsOpen) ;

                btnConnect.Text = "Connect";
                btnConnect.BackColor = Color.Red;
            }
        }

        private void DataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            
            //int MSB,LSB,val = 0;
            //int j = 0;

            serialPort1.Read(incommingData, 0, count);

            //while(serialPort1.BytesToRead != 0)
            //{
            //    MSB = serialPort1.ReadByte();
            //    //LSB = serialPort1.ReadByte();
            //    //val = (Convert.ToByte(MSB) << 2) | (Convert.ToByte(LSB) >> 6);
            //    incommingData[j] = Convert.ToDouble(MSB);
            //    j++;
            //    if (j > count - 1)
            //        j = 0;
            //    //Console.WriteLine("Received" + serialPort1.BytesToRead);
            //}
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //serialPort2.Write(data, 0, data.Length);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateGraph();
            //Console.WriteLine("updated!");
        }
    }
}
