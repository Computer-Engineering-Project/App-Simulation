using Environment.Model.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Packet
{
    public class PacketTransmit
    {
        //module type
        public const byte LORA = 0x01;
        public const byte ZIGBEE = 0x02;
        //cmd word
        public const byte ACTIVE = 0x00;
        public const byte READCONFIG = 0x01;
        public const byte CONFIG = 0x02;
        public const byte CHANGEMODE = 0x03;
        public const byte SENDDATA = 0x04;
        //define end byte
        public const byte ENDBYTE = 0x23;
        public byte module;
        public byte cmdWord;
        public byte[] dataLength;
        public byte[] data;

        public PacketTransmit(byte module, byte cmdWord, byte[] dataLength, byte[] data)
        {
            this.module = module;
            this.cmdWord = cmdWord;
            this.dataLength = dataLength;
            this.data = data;
        }

        public byte[] getPacket()
        {
            byte[] packet = new byte[4 + data.Length];
            packet[0] = module;
            packet[1] = cmdWord;
            packet[2] = dataLength[0];
            packet[3] = dataLength[1];
            for (int i = 0; i < data.Length; i++)
            {
                packet[4 + i] = data[i];
            }
            return packet;
        }
    }

    public class DataProcessed
    {
        public string address; // 2 byte
        public string channel; // 1 byte
        public string data; // data.Length - 3 byte

        public DataProcessed( byte[] data )
        {
            this.address = data[0].ToString("X2") + data[1].ToString("X2");
            this.channel = data[2].ToString("X2");
            this.data = "";
            for (int i = 3; i < data.Length; i++)
            {
                this.data += data[i].ToString("X2");
            }
        }
        public DataProcessed(string fixedMode, byte[] data)
        {
            if(fixedMode == FixedMode.FIXED)
            {
                this.address = "";
                this.channel = "";
                this.data = "";
                for (int i = 0; i < data.Length; i++)
                {
                    this.data += data[i].ToString("X2");
                }
            }
            else
            {
                this.address = data[0].ToString("X2") + data[1].ToString("X2");
                this.channel = data[2].ToString("X2");
                this.data = "";
                for (int i = 3; i < data.Length; i++)
                {
                    this.data += data[i].ToString("X2");
                }
            }
        }

    }
    public class PacketTransferToView
    {
        public string type { get; set; }
        public string portName { get; set; }
        public DataProcessed packet { get; set; }
    }
    public class InternalPacket
    {
        public DataProcessed packet { get; set;}
        public double DelayTime { get; set;}
        public string? PreambleCode { get; set;}
    }
}
