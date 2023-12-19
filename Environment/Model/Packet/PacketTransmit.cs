using Environment.Model.Module;
using System.Text;

namespace Environment.Model.Packet
{
    public class PacketTransmit
    {
        //module type
        public const byte LORA = 0x01;
        public const byte ZIGBEE = 0x02;
        public const byte UNK = 0xff;
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
            if (cmdWord == PacketTransmit.SENDDATA)
            {
                packet[2] = dataLength[1];
                packet[3] = dataLength[0];
            }
            else
            {
                packet[2] = dataLength[0];
                packet[3] = dataLength[1];
            }
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

        public DataProcessed(byte[] data)
        {
            this.address = "";
            this.channel = "";
            this.data = Encoding.ASCII.GetString(data);
        }
        public DataProcessed(string fixedMode, byte[] data)
        {
            if (fixedMode == FixedMode.FIXED)
            {
                this.address = data[1].ToString("X2") + data[0].ToString("X2");
                this.channel = data[2].ToString("X2");
                this.data = Encoding.ASCII.GetString(data.Skip(3).ToArray());
            }
            else
            {
                this.address = "";
                this.channel = "";
                this.data = Encoding.ASCII.GetString(data);
            }
        }

    }
    public class PacketSendTransferToView
    {
        public string type { get; set; }
        public string portName { get; set; }
        public DataProcessed packet { get; set; }
    }
    public class PacketReceivedTransferToView
    {
        public string type { get; set; }
        public InternalPacket packet { get; set; }
    }
    public class InternalPacket
    {
        public DataProcessed packet { get; set; }
        public double DelayTime { get; set; }
        public string? PreambleCode { get; set; }
        public string RSSI { get; set; }
        public string PathLoss { get; set; }
        public string SNR = "20";
        public string Distance { get; set; }
        public int typeError { get; set; }
        public ModuleObject sourceModule { get; set; }
        public ModuleObject receivedModule { get; set; }
        public string timeUTC { get; set; }
        public string timeMilisecond { get; set; }
    }
}
