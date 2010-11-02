using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tortoise.Shared.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace NetworkTest_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Connection Side1;

        Thread Side1T;
        
        private void button1_Click(object sender, EventArgs e)
        {

            Side1 = new Connection(IPAddress.Parse("10.255.255.1"), 9999);


            timer1.Start();

            Side1T = new Thread((ThreadStart)(() =>
            {
                while (true)
                {
                    Side1.Poll();
                    Thread.Sleep(0);
                }
            }));

            Side1T.Start();

            this.FormClosing += (FormClosingEventHandler)((object senderx, FormClosingEventArgs ex) =>
            {
                Side1T.Abort();
            });

        }

        Random R = new Random();
        byte[] randomBytes()
        {
            byte[] b = new Byte[(int)numericUpDown5.Value];
            for (int i = 0; i < numericUpDown5.Value; i++)
                b[i] = 0;
            return b;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "Que Size: " + Side1.PacketQueSize + "; " + Side1.LastPacketSpeed + " packets/sec";

            if (checkBox1.Checked)
            {
                if (Side1.PacketQueSize == 0)
                {
                    ByteWriter bw = new ByteWriter();
                    bw.Write(randomBytes());
                    for (int i = 0; i < numericUpDown3.Value; i++)
                        Side1.Write_ModulePacket(bw.GetArray(), 101);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ByteWriter bw = new ByteWriter();
            bw.Write(randomBytes());
            for (int i = 0; i < numericUpDown3.Value; i++)
                Side1.Write_ModulePacket(bw.GetArray(), 101);
        }

    }
}
