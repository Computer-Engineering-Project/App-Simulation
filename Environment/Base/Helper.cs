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
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Diagnostics.Metrics;
using System.Diagnostics;

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
            /*read data from hardware until end byte, timeout 1s, if after 5s no response, send again. if send again 3 times, return false*/
            while (count < 2)
            {
                serialPort.Write(packetTransmit.getPacket(), 0, packetTransmit.getPacket().Length);
                byte[] bytes = new byte[7];

                bool check = ExecuteWithTimeout(() =>
                {
                    bytes = GetDataFromHardware(serialPort);
                }, TimeSpan.FromSeconds(3));

                if (check)
                {
                    if (bytes.Length > 0)
                    {
                        PacketTransmit packetTransmit1 = HandleMessFromHardware(bytes);
                        if (packetTransmit1.cmdWord == PacketTransmit.READCONFIG)
                        {
                            return packetTransmit1;
                        }
                    }
                }
                count++;

            }

            return null;

        }
        public static bool SendCmdConfigToHardware(SerialPort serialPort, byte module, byte[] data)
        {
            bool success = false;
            byte cmdWord = PacketTransmit.CONFIG;
            byte[] dataLength = { 0x00, 0x00 };
            int dataLengthRaw = data.Length -1;
            dataLength[0] = (byte)(dataLengthRaw >> 8);
            dataLength[1] = (byte)(dataLengthRaw & 0xFF);

            PacketTransmit packetTransmit = new PacketTransmit(module, cmdWord, dataLength, data);

            int count = 0;
            /*read data from hardware until end byte, timeout 1s, if after 5s no response, send again. if send again 3 times, return false*/

            while(count < 2)
            { 
                serialPort.Write(packetTransmit.getPacket(), 0, packetTransmit.getPacket().Length);
                byte[] bytes = new byte[7];

                bool check = ExecuteWithTimeout(() =>
                {
                    bytes = GetDataFromHardware(serialPort);
                }, TimeSpan.FromSeconds(1));

                if (check)
                {
                    if (bytes.Length > 0)
                    {
                        PacketTransmit packetTransmit1 = HandleMessFromHardware(bytes);
                        if (packetTransmit1.cmdWord == PacketTransmit.CONFIG)
                        {
                            return true;
                        }
                    }
                }   
                count++;

            }

            return success;

        }

        public static byte[] GetDataFromHardware(SerialPort serialPort)
        {
            // read data from hardware until end byte
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
            byte[] dataLength = { data[3], data[2] };
            byte[] dataRaw;
            if(cmdWord == PacketTransmit.SENDDATA)
            {
               dataRaw  = new byte[dataLength[0] * 256 + dataLength[1]];
            }
            else
            {
                dataRaw = new byte[dataLength[1] * 256 + dataLength[0]];
            }
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

        public static byte ConvertSpeedrate(string baudrate)
        {
              switch (baudrate)
            {
                case "300":
                    return 0x00;
                case "1200":
                    return 0x01;
                case "2400":
                    return 0x02;
                case "4800":
                    return 0x03;
                case "9600":
                    return 0x04;
                case "19200":
                    return 0x05;
                case "38400":
                    return 0x06;
                case "57600":
                    return 0x07;
                case "115200":
                    return 0x08;
                default:
                    return 0x00;
            }
        }

        static bool ExecuteWithTimeout(Action action, TimeSpan timeout)
        {
            var task = Task.Run(() =>
            {
                // Sử dụng ManualResetEventSlim để đồng bộ hóa giữa luồng chính và luồng thực thi hàm B
                var completedEvent = new ManualResetEventSlim();

                // Thực thi hàm B trong một luồng mới
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    finally
                    {
                        completedEvent.Set();
                    }
                });

                // Chờ đến khi hàm B hoàn thành hoặc đã quá thời gian chờ
                return completedEvent.Wait(timeout);
            });

            return task.Result;
        }

    }
}
