using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using Environment.Model.Packet;

namespace Environment.Base
{
    public static class Helper
    {
        public static byte[] CmdActiveHardware()
        {
            byte module = 0xff;
            byte cmdWord = PacketTransmit.ACTIVE;
            byte[] dataLength = { 0x00, 0x00 };
            byte[] data = { PacketTransmit.ENDBYTE};

            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, data);

            return packetTransmit.getPacket();
        }
        public static PacketTransmit SendCmdGetConfigFromHardware(SerialPort serialPort)
        {
            byte module = 0x01;
            byte cmdWord = PacketTransmit.READCONFIG;
            byte[] dataLength = { 0x00, 0x00 };
            byte[] data = { PacketTransmit.ENDBYTE };

            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, data);
            // send cmd to hardware
            int count = 0;
            while (true)
            {
                count++;
                serialPort.Write(packetTransmit.getPacket(), 0, packetTransmit.getPacket().Length);
                // read data from hardware until end byte, if after 1s no response, send again
                byte[] dataFromHardware = GetDataFromHardware(serialPort);
                if (dataFromHardware.Length > 0)
                {
                    packetTransmit = HandleMessFromHardware(dataFromHardware);
                    return packetTransmit;
                }

                if (count == 10000)
                {
                    break;
                }
            }

            return packetTransmit;

        }
        public static bool SendCmdConfigToHardware(SerialPort serialPort, byte module, byte[] data)
        {
            byte cmdWord = PacketTransmit.CONFIG;
            byte[] dataLength = { 0x00, 0x00 };
            dataLength[0] = (byte)(data.Length >> 8);
            dataLength[1] = (byte)(data.Length & 0xFF);

            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, data);

            // send cmd to hardware and read data from hardware until end byte, if after 1s no response, send again

            int count = 0;
            while (true)
            {
                count++;
                serialPort.Write(packetTransmit.getPacket(), 0, packetTransmit.getPacket().Length);
                byte[] dataFromHardware = GetDataFromHardware(serialPort);
                if (dataFromHardware.Length > 0)
                {
                    packetTransmit = HandleMessFromHardware(dataFromHardware);
                    if (packetTransmit.cmdWord == PacketTransmit.CONFIG)
                    {
                        return true;
                    }
                }

                if (count == 10000)
                {
                    break;
                }
            }

            return false;
        }

        public static byte[] GetDataFromHardware(SerialPort serialPort)
        {
            byte[] data = new byte[1024];
            // read data from hardware until end byte
            int i = 0;
            while (true)
            {
                byte[] temp = new byte[1];
                serialPort.Read(temp, 0, 1);
                if (temp[0] == PacketTransmit.ENDBYTE)
                {
                    data[i] = temp[0];
                    break;
                }
                data[i] = temp[0];
                i++;
            }
            return data;
        }

        public static PacketTransmit HandleMessFromHardware(byte[] data)
        {
            byte module = data[0];
            byte cmdWord = data[1];
            byte[] dataLength = { data[2], data[3] };
            byte[] dataRaw = new byte[dataLength[0] * 256 + dataLength[1]];
            for (int i = 0; i < dataRaw.Length; i++)
            {
                dataRaw[i] = data[4 + i];
            }
            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, dataRaw);
            return packetTransmit;
        }
        public static string DecodeMessage(byte[] input)
        {
            return "";
        }
        public static string generatePreamble(int length)
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=<>? ";
            Random random = new Random();

            char[] preambleArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                preambleArray[i] = characters[random.Next(characters.Length)];
            }

            return new string(preambleArray);
        }
        public static PacketTransmit formatDataFollowProtocol(byte protocol, string data)
        {
            byte module = 0x01;
            byte cmdWord = protocol;
            byte[] dataLength = { 0x00, 0x00 };
            byte[] dataRaw = new byte[data.Length];
            int dataLengthRaw = data.Length;
            for (int i = 0; i < dataLengthRaw; i++)
            {
                dataRaw[i] = (byte)data[i];
            }

            dataLength[0] = (byte)(data.Length >> 8);
            dataLength[1] = (byte)(data.Length & 0xFF);
            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, dataRaw);
            return packetTransmit;
        }


        
    }
}
