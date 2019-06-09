using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Runtime.InteropServices;

namespace ZaidEy
{
 
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll")]
        public static extern long mciSendString(string command, StringBuilder restring, int retlength, IntPtr callback);
        public Form1()
        {
            InitializeComponent();
            
        }

        private void btnClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mciSendString("Open wave audio", null, 0, IntPtr.Zero);
            button1.Click += new EventHandler(this.Button1_Click);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            mciSendString("Open wave audio", null, 0, IntPtr.Zero);
            button2.Click += new EventHandler(this.Button2_Click);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
           // mciSendString("save sound D:\\786Zaid.wav", null, 0, IntPtr.Zero);
           // mciSendString("close sound", null, 0, IntPtr.Zero);


            mciSendString("pause Som", null, 0, IntPtr.Zero);

            string filename = @"C:\temp11\whatever.wav";
            mciSendString("save Som " + filename, null, 0, IntPtr.Zero);
            mciSendString("close Som", null, 0, IntPtr.Zero);
        }




    }
}
