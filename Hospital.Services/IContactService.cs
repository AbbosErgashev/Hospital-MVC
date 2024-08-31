using Hospital.Utilities;
using Hospital.ViewModels;

namespace Hospital.Services
{
    public interface IContactService
    {
        PagedResult<ContactViewModel> GetAll(int pageNumber, int pageSize);
        ContactViewModel GetContactById(int ContactId);
        Task UpdateContact(ContactViewModel contact);
        Task InsertContact(ContactViewModel contact);
        void DeleteContact(int id);
    }
}
