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

namespace Network_Test
{
    public partial class Form1 : Form
    {
        public static Form1 instance;

        public Form1()
        {
            instance = this;
            InitializeComponent();
        }

        Connection Side1;
        Connection Side2;

        Thread Side1T, Side2T;

        //Connect
        private void button1_Click(object sender, EventArgs e)
        {
            TcpListener tmpl;
            tmpl = new TcpListener(IPAddress.Parse("10.255.255.1"), 9999);
            tmpl.Start();

            Side1 = new Connection(IPAddress.Parse("10.255.255.1"), 9999);

            Side2 = new Connection(tmpl.AcceptSocket());

            tmpl.Stop();


            Connection.AddModuleHandle(101, new ModuleRecive());

            timer1.Start();

            Side1T = new Thread((ThreadStart)(() =>
            {
                while (true)
                {
                    Side2.Poll();
                }
            }));

           Side2T = new Thread((ThreadStart)(() =>
            {
                while (true)
                {
                    Side1.Poll();
                }
            }));

           Side1T.Start();
           Side2T.Start();

            this.FormClosing += (FormClosingEventHandler)((object senderx, FormClosingEventArgs ex) =>
                {
                    Side1T.Abort();
                    Side2T.Abort();
                });

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "Que Size: " + Side1.PacketQueSize + "; " + Side1.LastPacketSpeed + " packets/sec";
            label2.Text = "Que Size: " + Side2.PacketQueSize + "; " + Side2.LastPacketSpeed + " packets/sec";

            if (checkBox1.Checked)
            {
                if (Side1.PacketQueSize == 0)
                {
                    ByteWriter bw = new ByteWriter();
                    bw.Write(Convert.ToInt32(numericUpDown1.Value));
                    bw.Write(DateTime.Now.ToBinary());
                    bw.Write(randomBytes());
                    for (int i = 0; i < numericUpDown3.Value; i++)
                        Side1.Write_ModulePacket(bw.GetArray(), 101, Convert.ToInt32(numericUpDown1.Value));
                }
            }
        }


        //Class for receiving data
        private class ModuleRecive : IComModule
        {
            #region IComModule Members

            public void Communication(Connection Sender, ByteReader data)
            {
                var ptmp = data.ReadInt();
                if (!ptmp.Sucess)
                {
                    MessageBox.Show("An error occurred reading the Integer????");
                    return;
                }
                var ttmp = data.ReadLong();
                if (!ttmp.Sucess)
                {
                    MessageBox.Show("An error occurred reading the Long????");
                    return;
                }
                if (Sender == instance.Side1)
                    instance.reciveds1(ptmp.Result, DateTime.FromBinary(ttmp.Result));
                else
                    instance.reciveds2(ptmp.Result, DateTime.FromBinary(ttmp.Result));

            }

            #endregion
        }
        Random R = new Random();
        byte[] randomBytes()
        {
            byte[] b = new Byte[(int)numericUpDown5.Value];
            for (int i = 0; i < numericUpDown5.Value; i++)
                b[i] = 0;
            return b;
        }

        //Side 1
        private void button7_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = -10;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 10;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Side1 == null)
                return;

            ByteWriter bw = new ByteWriter();
            bw.Write(Convert.ToInt32(numericUpDown1.Value));
            bw.Write(DateTime.Now.ToBinary());
            bw.Write(randomBytes());
            for (int i = 0; i < numericUpDown3.Value; i++)
                Side1.Write_ModulePacket(bw.GetArray(), 101, Convert.ToInt32(numericUpDown1.Value));
        }

        protected void reciveds1(int priority, DateTime Time)
        {
            Invoke((Action)(() =>
            {
               // listBox1.Items.Add(string.Format("Received a packet with {0} priority, sent at {1}", priority, Time));
              //  listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        //Side 2

        private void button8_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = -10;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 0;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 10;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Side2 == null)
                return;

            ByteWriter bw = new ByteWriter();
            bw.Write(Convert.ToInt32(numericUpDown2.Value));
            bw.Write(DateTime.Now.ToBinary());
            bw.Write(randomBytes());
            for (int i = 0; i < numericUpDown4.Value; i++)
                Side2.Write_ModulePacket(bw.GetArray(), 101, Convert.ToInt32(numericUpDown2.Value));
        }


        protected void reciveds2(int priority, DateTime Time)
        {
            Invoke((Action)(() =>
             {
               //  listBox2.Items.Add(string.Format("Received a packet with {0} priority, sent at {1}", priority, Time));
               //  listBox2.SelectedIndex = listBox2.Items.Count - 1;
             }));
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            label3.Text = "Current Time: " + DateTime.Now.ToString();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }



    }
}
