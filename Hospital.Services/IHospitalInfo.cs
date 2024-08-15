using Hospital.Models;
using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IHospitalInfo
    {
        PagedResult<HospitalInfoViewModel> GetAll(int pageNumber, int pageSize);

        HospitalInfoViewModel GetHospitalById(int HospitalId);

        Task UpdateHospital(HospitalInfoViewModel hospitalInfo);

        Task InsertHospitalInfo(HospitalInfoViewModel hospitalInfo);

        void DeleteHospitalInfo(int id);
    }
}
