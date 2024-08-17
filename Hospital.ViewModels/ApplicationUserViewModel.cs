using Hospital.Models;

namespace Hospital.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }
        public Gender Gender { get; set; }
        public bool IsDoctor { get; set; }
        public string Specialist { get; set; }
        public List<ApplicationUser> Doctors { get; set; } = new List<ApplicationUser>();

        public ApplicationUserViewModel()
        {
            
        }

        public ApplicationUserViewModel(ApplicationUser user)
        {
            Id = user.Id.ToString();
            Name = user.Name;
            City = user.City;
            Gender = user.Gender;
            IsDoctor = user.IsDoctor;
            Specialist = user.Specialist;
            Email = user.Email;
            UserName = user.UserName;
        }

        public ApplicationUser ConvertViewModelToModel(ApplicationUserViewModel user)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                Name = user.Name,
                City = user.City,
                Gender = user.Gender,
                IsDoctor = user.IsDoctor,
                Specialist = user.Specialist,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
