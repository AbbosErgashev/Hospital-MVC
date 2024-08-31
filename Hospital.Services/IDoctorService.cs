using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IDoctorService
    {
        PagedResult<TimingViewModel> GetAll(int pageNumber, int pageSize);
        IEnumerable<TimingViewModel> GetAllList();
        TimingViewModel GetTimingById(int TimingId);
        Task UpdateTiming(TimingViewModel timing);
        Task AddTiming(TimingViewModel timing);
        void DeleteTiming(int TimingId);
    }
}
