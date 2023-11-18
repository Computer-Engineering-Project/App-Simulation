using Environment.Model.Packet;

namespace Environment.Service.Interface
{
    public interface ICommunication
    {
        public void showQueueReceivedFromHardware(PacketTransferToView listTransferedPacket);
    }
}
