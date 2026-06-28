using Unity.Netcode;

public struct PlayerInfo:INetworkSerializable
{
    public string PlayerID;
    public string RoomID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerID);
        serializer.SerializeValue(ref RoomID);
    }
}