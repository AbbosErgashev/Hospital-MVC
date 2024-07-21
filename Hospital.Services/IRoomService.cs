using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IRoomService
    {
        PagedResult<RoomViewModel> GetAll(int pageNumber, int pageSize);
        RoomViewModel GetRoomById(int roomId);
        void UpdateRoom(RoomViewModel room);
        void InsertRoom(RoomViewModel room);
        void DeleteRoom(int id);
    }
}
