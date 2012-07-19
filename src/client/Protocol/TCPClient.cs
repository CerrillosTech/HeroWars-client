using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using HeroWars.GUI;
using HeroWars.Properties;
using HeroWars.Tools;

namespace HeroWars.Protocol
{
    class TCPClient
    {
        public TcpClient Client { get; private set; }
        private readonly PacketHandler _packetHandler;
        private Stream tcpStream;
        private readonly LoginWindow _gui;
        public uint SessionHash { get; set; }

        public TCPClient(LoginWindow loginWindow)
        {
            _gui = loginWindow;
            _packetHandler = new PacketHandler(this, _gui);
            SessionHash = 0;
        }

        public bool Connect(String server, int port)
        {
            try
            {
                Client = new TcpClient();
                Client.Connect(server, port);
                tcpStream = Client.GetStream();

                SendHandshake();
                byte[] buffer = new byte[1024];
                int receiveSize = tcpStream.Read(buffer, 0, 1024);
                if (!_packetHandler.HandlePacket(buffer, receiveSize))
                    throw new Exception();
                return true;
            }
            catch(Exception e)
            {
                Log.WriteError("Unable to connect to server '" + server + "': " + e.Message);
                return false;
            }
        }

        public void Run()
        {
            Threading.RunAsThread(ListenThread);
        }

        public bool SendHandshake()
        {
            HandshakeReq req = new HandshakeReq(Crypt.MD5(Settings.Default.version));
            byte[] buffer = GetBytes(req);
            return SendPacket(buffer, 0, buffer.Length);
        }

        public void SendAuth(string username, string password)
        {
            string cPassword = Crypt.MD5(password + Crypt.MD5(username.ToLower()).ToLower());

            AuthReq req = new AuthReq(username, cPassword) {header = {packetType = PacketType.PKT_AuthReq}};
            byte[] buffer = GetBytes(req);
            SendPacket(buffer, 0, buffer.Length);
        }

        private void ListenThread(int threadId, object args)
        {
            try
            {
                byte[] buffer = new byte[1024];

                while(Client.Connected)
                {
                    int receiveSize = tcpStream.Read(buffer, 0, 1024);
                    if(!_packetHandler.HandlePacket(buffer, receiveSize))
                    {
                        Log.WriteError("Unable to handle packet " + buffer[0]);
                    }
                }
                _gui.Dispatcher.Invoke(new Action(() =>
                                                 {
                                                     MessageBox.Show("Lost connection to server.", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                                                     Application.Current.Shutdown(-3);
                                                 }));

            }
            catch (Exception e) { Log.WriteError(e.Message); return; }
        }

        public bool SendPacket(byte[] packet, int offset, int size)
        {
            try
            {
                if(!Client.Connected)
                {
                    _gui.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBox.Show("Lost connection to server.", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown(-3);
                    }));
                }
                tcpStream.Write(packet, offset, size);
                return true;
            }
            catch (Exception e)
            {
                Log.WriteError("Unable to send packet to server: " + e.Message);
                return false;
            }
        }

        public byte[] GetBytes<T>(T input)
        {
            int size = Marshal.SizeOf(input);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(input, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public void ToStructure<T>(byte[] arr, ref T structure)
        {
            try
            {
                int size = Marshal.SizeOf(structure);
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(arr, 0, ptr, size);

                structure = (T)Marshal.PtrToStructure(ptr, structure.GetType());
                Marshal.FreeHGlobal(ptr);
            }
            catch (Exception e)
            {
                Log.WriteError("Unable to convert byte array to structure '" + structure.GetType().Name + "': " + e.Message);
            }
        }
    }
}
