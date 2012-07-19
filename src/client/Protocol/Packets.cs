using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HeroWars.Protocol
{
    //      C#   |   C++
    // ----------------------
    //    byte   |   uint8
    //  ushort   |   uint16
    //    uint   |   uint32
    //   ulong   |   uint64
    //
    //   sbyte   |   int8
    //   short   |   int16
    //     int   |   int32
    //    long   |   int64


    public enum PacketType : byte
    {
    	PKT_HandshakeReq		=	0x01,
	    PKT_HandshakeRes		=	0x02,
	    PKT_AuthReq				=	0x03,
	    PKT_AuthRes				=	0x04,
    
	    PKT_Logout				=	0xFF
    };

    public enum AuthResult : byte
    {
	    AUR_OK					=	0x00,
	    AUR_AuthFailed			=	0xFF,
    };
    
    public enum HandshakeResult : byte
    {
	    HSK_OK					=	0x00,
	    HSK_WrongVersion		=	0x01,
	    HSK_Unknown				=	0xFF,
    };

    //Packet header
    public struct PacketHeader
    {
	    public PacketType packetType;
        public uint sessionHash;	
    };

    public struct HandshakeReq
    {
        public HandshakeReq(string version)
        {
            header = new PacketHeader {packetType = PacketType.PKT_HandshakeReq};
            this.version = version;
        }
        public PacketHeader header;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string version;
    };

    public struct HandshakeRes
    {
        public PacketHeader header;
        public HandshakeResult handshakeResult; // 0x00 when ok, 0xFF when failed
    };

    struct AuthReq
    {
        public AuthReq(string username, string password)
        {
            header = new PacketHeader {packetType = PacketType.PKT_AuthReq};
            this.username = username;
            this.password = password;
        }
        public PacketHeader header;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string username;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string password; 
    }

    public struct AuthRes
    {
        public PacketHeader header;
        public AuthResult result;
    }

    public struct Logout
    {
        public PacketHeader header;
    }
}
