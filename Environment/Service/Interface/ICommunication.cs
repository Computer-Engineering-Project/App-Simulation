using Environment.Model.Packet;

namespace Environment.Service.Interface
{
    public interface ICommunication
    {
        public void sendMessageIsRunning();
        public void sendMessageIsIdle();
        public void sendMessageIsStop();
        public void showQueueReceivedFromHardware(PacketSendTransferToView transferedPacket, string portClicked);
        public void showQueueReceivedFromOtherDevice(PacketReceivedTransferToView transferedPacket, string portClicked);
        public void deviceChangeMode(int mode, string port);
    }
}
