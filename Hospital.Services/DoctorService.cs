using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public class DoctorService : IDoctorService
    {
        private IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task AddTiming(TimingViewModel timing)
        {
            var model = new TimingViewModel().ConvertViewModel(timing);
            _unitOfWork.GenericRepository<Timing>().Add(model);
            _unitOfWork.Save();
            return Task.CompletedTask;
        }

        public void DeleteTiming(int TimingId)
        {
            var model = _unitOfWork.GenericRepository<Timing>().GetById(TimingId);
            _unitOfWork.GenericRepository<Timing>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<TimingViewModel> GetAll(int pageNumber, int pageSize)
        {
            var vm = new TimingViewModel();
            int totalCount;
            List<TimingViewModel> vmlist = new List<TimingViewModel>();
            try
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;

                var modelList = _unitOfWork.GenericRepository<Timing>().GetAll()
                    .Skip(ExcludeRecords).Take(pageSize).ToList();

                totalCount = _unitOfWork.GenericRepository<Timing>().GetAll().ToList().Count;

                vmlist = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<TimingViewModel>
            {
                Data = vmlist,
                TotalItems = vmlist.Count,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public IEnumerable<TimingViewModel> GetAllList()
        {
            var timingList = _unitOfWork.GenericRepository<Timing>().GetAll().ToList();
            var vmList = ConvertModelToViewModelList(timingList);
            return vmList;
        }

        public TimingViewModel GetTimingById(int TimingId)
        {
            var model = _unitOfWork.GenericRepository<Timing>().GetById(TimingId);
            var vm = new TimingViewModel(model);
            return vm;
        }

        public Task UpdateTiming(TimingViewModel timing)
        {
            var model = new TimingViewModel().ConvertViewModel(timing);
            var ModelById = _unitOfWork.GenericRepository<Timing>().GetById(model.Id);
            ModelById.Id = timing.Id;
            ModelById.Doctor = timing.Doctor;
            ModelById.Status = timing.Status;
            ModelById.Duration = timing.Duration;
            ModelById.MorningShiftStartTime = timing.MorningShiftStartTime;
            ModelById.MorningShiftEndTime = timing.MorningShiftEndTime;
            ModelById.AfternoonShiftStartTime = timing.AfternoonShiftStartTime;
            ModelById.AfternoonShiftEndTime = timing.AfternoonShiftEndTime;

            _unitOfWork.GenericRepository<Timing>().Update(ModelById);
            _unitOfWork.Save();
            return Task.CompletedTask;
        }

        private List<TimingViewModel> ConvertModelToViewModelList(List<Timing> modelList)
        {
            return modelList.Select(x => new TimingViewModel(x)).ToList();
        }
    }
}
