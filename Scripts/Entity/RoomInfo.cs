using Unity.Netcode;
public struct RoomInfo:INetworkSerializable
{
    public string RoomID;
    public string HostPlayerID;
    public string ClientPlayerID;
    public bool IsMatch;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref RoomID);
        serializer.SerializeValue(ref HostPlayerID);
        serializer.SerializeValue(ref ClientPlayerID);
        serializer.SerializeValue(ref IsMatch);
    }
}