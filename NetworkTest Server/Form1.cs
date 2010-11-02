using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tortoise.Server.Connections;
using Tortoise.Shared.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.IO;

namespace NetworkTest_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = string.Format("Connections: {0} - {1} Packets/sec", Tortoise.Server.Connections.ClientHandle._instance.ConnectedClients, Tortoise.Server.Connections.ClientHandle._instance.LastPacketSpeed);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientHandle.CreateInstance();
            timer1.Start();
            Connection.AddModuleHandle(101, new ModuleRecive());

        }

        private class ModuleRecive : IComModule
        {
            #region IComModule Members

            public void Communication(Connection Sender, ByteReader data)
            {

                //do nothing atm...

            }

            #endregion
        }
    }
}
