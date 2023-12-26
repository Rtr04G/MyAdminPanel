using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DoctorPanel.Models;
using System.Web.WebPages;
using Microsoft.AspNetCore.Identity;

namespace DoctorPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AdminUser> _userManager;

        public HomeController(UserManager<AdminUser> userManager, IWebHostEnvironment environment, AppDbContext context, ILogger<HomeController> logger)
        {
            _environment = environment;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            IQueryable <Record> records = _context.Records.AsQueryable();
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            records = records.Where(r=>r.DoctorId==userId);
            List<AdminUser> usersDB = _context.AdminUsers.AsQueryable().ToList();
            List<RecordIndexViewModel> recordsOfThisUser = new List<RecordIndexViewModel>();
            RecordIndexViewModel myrecord = new RecordIndexViewModel();
            foreach (var record in records)
            {
                myrecord.SurName = usersDB.FirstOrDefault(u => u.Id == record.PatientId).SurName;
                myrecord.MiddleName = usersDB.FirstOrDefault(u => u.Id == record.PatientId).MiddleName;
                myrecord.Adderss = usersDB.FirstOrDefault(u => u.Id == record.PatientId).Address;
                myrecord.Date = record.DateX;
                recordsOfThisUser.Add(myrecord);
            };
            return View(recordsOfThisUser);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}