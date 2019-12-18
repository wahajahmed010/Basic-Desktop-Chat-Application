using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint eplocal, epremote;
        byte[] buffer;
         
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set up Socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // get user IP

            textLocalIp.Text = GetLocalIP();
            textRemoteIp.Text = GetLocalIP();
        }

        private String GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            }
            return "127.0.0.1";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //binding socket
            eplocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(eplocal);

            //Connecting to remote IP
            epremote = new IPEndPoint(IPAddress.Parse(textRemoteIp.Text), Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epremote);

            //Listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,ref epremote, new AsyncCallback(MessageCallBack), buffer);



        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;
                //Converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string RecievedMessage = aEncoding.GetString(receivedData);

                //Adding this message to Listbox
                listMessage.Items.Add("Friend: " + RecievedMessage);
                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epremote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            // Convert string message to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMessage.Text);

            // Sending the Encoded Message
            sck.Send(sendingMessage);
            
            // Adding to list box
            listMessage.Items.Add("Me: " + textMessage);
            textMessage.Text = "";


        }

    }
}
