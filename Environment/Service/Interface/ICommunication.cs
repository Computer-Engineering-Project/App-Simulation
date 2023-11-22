using Environment.Model.Packet;

namespace Environment.Service.Interface
{
    public interface ICommunication
    {
        public void sendMessageIsRunning();
        public void showQueueReceivedFromHardware(PacketTransferToView listTransferedPacket, string portClicked);
        public void deviceChangeMode(int mode, string port);
    }
}
