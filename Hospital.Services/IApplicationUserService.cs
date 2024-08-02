using Hospital.Models;
using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IApplicationUserService
    {
        public PagedResult<ApplicationUserViewModel> GetAll(int pageNumber, int pageSize);
        public PagedResult<ApplicationUserViewModel> GetAllDoctor(int pageNumber, int pageSize);
        public PagedResult<ApplicationUserViewModel> GetAllPatient(int pageNumber, int pageSize);
        public PagedResult<ApplicationUserViewModel> SearchDoctor(int pageNumber, int pageSize, string Spici___);
    }
}
