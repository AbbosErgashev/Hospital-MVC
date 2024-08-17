using Hospital.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Hospital.Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_doctorService.GetAll(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult AddTiming()
        {
            Timing timing = new Timing();
            List<SelectListItem> morningShiftStartTime = new List<SelectListItem>();
            List<SelectListItem> morningShiftEndTime = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftStartTime = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftEndTime = new List<SelectListItem>();

            for (int i = 1; i <= 11; i++)
            {
                morningShiftStartTime.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            for (int i = 1; i <= 13; i++)
            {
                morningShiftEndTime.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            for (int i = 1; i <= 16; i++)
            {
                afternoonShiftStartTime.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            for (int i = 1; i <= 18; i++)
            {
                afternoonShiftEndTime.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            ViewBag.morningStart = new SelectList(morningShiftStartTime, "Value", "Text");
            ViewBag.morningEnd = new SelectList(morningShiftEndTime, "Value", "Text");
            ViewBag.evenStart = new SelectList(afternoonShiftStartTime, "Value", "Text");
            ViewBag.evenEnd = new SelectList(afternoonShiftEndTime, "Value", "Text");

            TimingViewModel vm = new TimingViewModel();
            vm.ScheduleDate = DateTime.UtcNow;
            vm.ScheduleDate = vm.ScheduleDate.AddDays(1);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTiming(TimingViewModel vm)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (Claims is not null)
            {
                vm.Doctor.Id = Claims.Value;
                _doctorService.AddTiming(vm);
            }

            await _doctorService.AddTiming(vm);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = _doctorService.GetTimingById(id);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TimingViewModel vm)
        {
            await _doctorService.UpdateTiming(vm);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _doctorService.DeleteTiming(id);
            return RedirectToAction("Index");
        }
    }
}
