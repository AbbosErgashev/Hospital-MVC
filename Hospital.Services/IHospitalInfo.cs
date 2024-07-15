using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IHospitalInfo
    {
        PagedResult<HospitalInfoViewModel> GetAll(int pageNumber, int pageSize);

        HospitalInfoViewModel GetHospitalById(int HospitalId);

        void UpdateHospital(HospitalInfoViewModel hospitalInfo);

        void InsertHospitalInfo(HospitalInfoViewModel hospitalInfo);

        void DeleteHospitalInfo(int id);
    }
}
