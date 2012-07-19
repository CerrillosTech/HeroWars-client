using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeroWars.GUI;

namespace HeroWars.Protocol
{
    class PacketHandler
    {
        private readonly TCPClient _tcpClient;
        private readonly LoginWindow _gui;

        public PacketHandler(TCPClient tcpClient, LoginWindow loginWindow)
        {
            _tcpClient = tcpClient;
            _gui = loginWindow;
        }

        public bool HandlePacket(byte[] packet, int size)
        {
            switch ((PacketType)packet[0])
            {
                case PacketType.PKT_HandshakeRes:
                    return HandleHandshake(packet);
                case PacketType.PKT_AuthRes:
                    return HandleAuthentification(packet);
            }
            return false;
        }

        private bool HandleHandshake(byte[] packet)
        {
            HandshakeRes res = new HandshakeRes();
            _tcpClient.ToStructure(packet, ref res);
            if(res.handshakeResult != HandshakeResult.HSK_OK) _gui.HandleHandshakeError();
            return true;
        }

        private bool HandleAuthentification(byte[] packet)
        {
            AuthRes res = new AuthRes();
            _tcpClient.ToStructure(packet, ref res);
            _gui.Login(res.result == AuthResult.AUR_OK);
            if(res.result == AuthResult.AUR_OK)
                _gui.MainWindow.SetSessionId(Convert.ToString(res.header.sessionHash));
            return true;
        }
    }
}
