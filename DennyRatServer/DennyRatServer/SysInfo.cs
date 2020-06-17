using System;
using System.Windows.Forms;

namespace DennyRatServer
{
    public partial class SysInfo : Form
    {
        string[] splitinf;

        public SysInfo(string info)
        {
            splitinf = info.Split('=');
            InitializeComponent();
        }

        private void SysInfo_Load(object sender, EventArgs e)
        {
            string UserName1 = splitinf[0];
            string LabelOS1 = splitinf[1];
            string MachineTxt1 = splitinf[2];
            string label81 = splitinf[3];

            UserName.Text = "User Name: " + UserName1;
            LabelOS.Text = "OS: " + LabelOS1;
            MachineTxt.Text = "Machine ID Name: " + MachineTxt1;
            label8.Text = "System Type/Processors: " + label81;
        }
    }
}
