using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DennyRatServer
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string info)
        {
            Form prompt = new Form();
            prompt.Width = 328;
            prompt.Height = 335;
            prompt.Text = caption;

            ListView listView1 = new System.Windows.Forms.ListView();
            Label label1 = new System.Windows.Forms.Label();
            Button button1 = new System.Windows.Forms.Button();
            ColumnHeader columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ColumnHeader columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));

            // 
            // listView1
            // 
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {columnHeader1, columnHeader2});
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HideSelection = false;
            listView1.Location = new System.Drawing.Point(12, 36);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(286, 204);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(13, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(51, 20);
            label1.TabIndex = 1;
            label1.Text = text;

            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(12, 246);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(286, 38);
            button1.TabIndex = 2;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += (sender, e) => {

                prompt.Close();
            
            };

            prompt.FormClosing += (sender, e) => {

                if (listView1.SelectedItems.Count <= 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to close?", "No device selected", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }

            };

            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 222;


            columnHeader2.Text = "ID";

            string[] separateitems = info.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < separateitems.Length; i++)
            {
                string[] thisitminfo = separateitems[i].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem itm = new ListViewItem(thisitminfo[0]);
                itm.SubItems.Add(thisitminfo[1]);
                listView1.Items.Add(itm);
            }

            prompt.Controls.Add(listView1);
            prompt.Controls.Add(label1);
            prompt.Controls.Add(button1);
            prompt.ShowDialog();

            if(listView1.SelectedItems.Count <= 0)
            {
                return "none";
            }

            return listView1.SelectedItems[0].SubItems[1].Text;
        }

        public static string ShowTextDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 300;
            prompt.Height = 165;
            prompt.Text = caption;

            Label label1 = new System.Windows.Forms.Label();
            TextBox textBox1 = new System.Windows.Forms.TextBox();
            Button button1 = new System.Windows.Forms.Button();

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(104, 20);
            label1.TabIndex = 0;
            label1.Text = text;
            // 
            // textBox1
            // 
            textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox1.Location = new System.Drawing.Point(12, 32);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(254, 26);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(12, 65);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(254, 42);
            button1.TabIndex = 2;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;


            button1.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(button1);
            prompt.Controls.Add(textBox1);
            prompt.Controls.Add(label1);
            prompt.ShowDialog();
            return textBox1.Text;
        }
    }
}
