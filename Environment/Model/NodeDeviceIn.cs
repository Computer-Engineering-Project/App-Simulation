using Environment.Model.Packet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model
{
    public class NodeDeviceIn
    {
        public SerialPort serialport { get; set; }
        public object lockObject { get; set; }
        public string mode { get; set; }
        public Thread transferData { get; set; }
        public ConcurrentQueue<DataProcessed> packetQueue = new ConcurrentQueue<DataProcessed>(); 
    }
    public class NodeDeviceOut
    {
        public SerialPort serialport { get; set; }
        public object lockObject { get; set; }
        public string mode { get; set; }
        public Thread transferData { get; set; }
        public ConcurrentQueue<InternalPacket> packetQueue = new ConcurrentQueue<InternalPacket>();
    }
}
