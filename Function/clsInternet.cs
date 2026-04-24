using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace MIS
{
    public class clsInternet
    {
        private string m_IPAdress = clsGlobalVariables.strAPIServerIPAddress;
        private int kPort = int.Parse(clsGlobalVariables.strAPIServerPort);

        private string m_FTPIPAdress = clsGlobalVariables.strFTPURL;
        private int kFTPPort = int.Parse(clsGlobalVariables.strFTPPORT);
        
        public int bytes;
        private Socket m_Socket;
        public byte[] bytesReceived;
        public string receivedData = "";

        public string GetLocalIPAddress()
        {
            string sIP = "";
            string sComputerName = "";

            sComputerName = Dns.GetHostName();
            IPAddress[] ipaddress = Dns.GetHostAddresses(sComputerName);

            foreach (IPAddress ip in ipaddress)
            {
                sIP = ip.ToString();
            }

            return sIP;
        }        

        public string GetComputerName()
        {
            string sComputerName = "";

            sComputerName = Dns.GetHostName();

            return sComputerName;
        }

        public bool CheckInternetConnection(string sIPAddress)
        {
            int iCtr = 0;
            bool isConnected = false;

            Ping pingSender = new Ping();

            try
            {
                do
                {
                    PingReply reply = pingSender.Send(sIPAddress);

                    if (reply.Status == IPStatus.Success)
                        isConnected = true;
                    else
                        isConnected = false;

                    iCtr++;
                }
                while (iCtr < 1);
                
            }
            catch (Exception)
            {
                isConnected = false;
            }

            return isConnected;
        }

        public bool CheckServerSocket()
        {
            bool isAlive = false;

            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Try to resolve as IP or domain name
                IPAddress[] addresses = Dns.GetHostAddresses(m_IPAdress);
                if (addresses.Length == 0)
                {
                    return false; // Cannot resolve host
                }

                IPEndPoint remoteEndPoint = new IPEndPoint(addresses[0], kPort);

                m_Socket.Connect(remoteEndPoint);
                isAlive = true;
            }
            catch
            {
                isAlive = false;
            }
            finally
            {
                CloseServerSocket();
            }

            return isAlive;
        }


        public bool CheckFTPServerSocket()
        {
            bool isAlive = false;

            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(m_FTPIPAdress);
            System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, kFTPPort);

            try
            {
                m_Socket.Connect(remoteEndPoint);
                isAlive = true;
            }
            catch
            {
                isAlive = false;
            }

            CloseServerSocket();

            return isAlive;
        }
        
        public void CloseServerSocket()
        {
            m_Socket.Close();
            m_Socket = null;
        }
    }
}
