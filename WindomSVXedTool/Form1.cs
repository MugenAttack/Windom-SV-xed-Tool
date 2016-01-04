﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace WindomSVXedTool
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            //AllocConsole();
            InitializeComponent();
           
            
            
        }

        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //private static extern bool AllocConsole();

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                FileInfo f = new FileInfo(txtFile.Text);
                
                switch(f.Extension)
                {
                    case ".xed":
                        XedDecrypt xd = new XedDecrypt();
                        xd.Decrypt(txtFile.Text,txtName.Text);
                        MessageBox.Show("Unpacking is Done");
                        break;
                    case ".txt":
                        XedEncrypt xe = new XedEncrypt();
                        xe.Encrypt(txtFile.Text, f.DirectoryName,txtName.Text);
                        MessageBox.Show("Packing is Done");
                        break;
                    default:
                        MessageBox.Show("Error: Invalid File Type");
                        break;
                }
        //}
            //catch
            //{
            //    MessageBox.Show("Error 2: Invalid File Type");
            //}

   

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog {Filter = "supported Files (*.xed;*.txt)|*.xed;*.txt", Title = "Browse for xed or txt File"};
            if (browseFile.ShowDialog() == DialogResult.OK)
                txtFile.Text = browseFile.FileName;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            FileInfo f = new FileInfo(txtFile.Text);

            switch (f.Extension)
            {
                case ".xed":
                    button1.Text = "Unpack";
                    break;
                case ".txt":
                    button1.Text = "Pack";
                    break;
                default:
                    button1.Text = "Invalid File Type";
                    break;
            }
        }
    }
}
