namespace Scenes.Ingame.Stage
{
    public struct RoomData
    {
        private RoomType _roomType;
        private int _roomId;
        private string _roomName;

        public RoomType RoomType { get => _roomType; }
        public int RoomId { get => _roomId; }
        public string roomName { get => _roomName; }

        public void RoomDataSet(RoomType roomType,int roomId)
        {
            _roomType = roomType;
            _roomId = roomId;
        }
        public void SetRoomName(string name)
        {
            _roomName = name;
        }
    }
}