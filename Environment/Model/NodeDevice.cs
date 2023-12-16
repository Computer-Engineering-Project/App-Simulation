using Environment.Model.Module;
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
    public class NodeDevice
    {
        public const int MODE_NORMAL = 0;
        public const int MODE_WAKEUP = 1;
        public const int MODE_POWERSAVING = 2;
        public const int MODE_SLEEP = 3;
        public const int MODE_SETUP = 3;

        public SerialPort serialport { get; set; }
        public object lockObjecCollision = new object();
        public object lockObjectChangeMode = new object();
        public int mode { get; set; }
        public int flagDataIn { get; set; }
        public bool existReadThread { get; set; }
        public Thread readDataFromHardware { get; set; }
        public Thread transferDataIn { get; set; }
        public Thread transferDataOut { get; set; }
        public ModuleObject moduleObject { get; set; }
        public ConcurrentQueue<ErrorPacket> errorPackets = new ConcurrentQueue<ErrorPacket>();

        public ConcurrentQueue<DataProcessed> packetQueueIn = new ConcurrentQueue<DataProcessed>();

        public ConcurrentQueue<InternalPacket> packetQueueOut = new ConcurrentQueue<InternalPacket>();

    }

}
