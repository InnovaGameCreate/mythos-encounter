namespace Scenes.Ingame.Stage
{
    public struct RoomData
    {
       private RoomType _roomType;
       private int _roomId;

        public RoomType RoomType { get => _roomType; }
        public int RoomId { get => _roomId; }

        public void RoomDataSet(RoomType roomType,int roomId)
        {
            _roomType = roomType;
            _roomId = roomId;
        }
    }
}