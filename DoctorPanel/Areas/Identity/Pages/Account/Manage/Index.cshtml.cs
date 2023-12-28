// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoctorPanel.Models;

namespace DoctorPanel.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly AppDbContext _context;

        public IndexModel(
            UserManager<AdminUser> userManager,
            SignInManager<AdminUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Фамилия")]
            public string SurName { get; set; }
            [Display(Name = "Отчество")]
            public string MiddleName { get; set; }
            [Display(Name = "Специализация")]
            public string Specialization { get; set; }
            [Display(Name = "Категория")]
            public string Category{ get; set; }
            [Display(Name = "Адрес")]
            public string Address { get; set; }
        }

        private async Task LoadAsync(AdminUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            Username = userName;


            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                SurName = user.SurName,
                MiddleName = user.MiddleName,
                Specialization = user.Specialization,
                Category = user.Category,
                Address = user.Address
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            List<Category> categories = _context.Categories.ToList();
            List<Specialization> specializations = _context.Specializations.ToList();
            List<LPU> LPUs = _context.LPUs.ToList();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name"); 
            ViewData["Specializations"] = new SelectList(specializations, "Id", "Name");
            ViewData["LPUs"] = new SelectList(LPUs, "Id", "Name");
            AdminUser user = (AdminUser)await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            List<Category> categories = _context.Categories.ToList();
            List<Specialization> specializations = _context.Specializations.ToList();
            List<LPU> LPUs= _context.LPUs.ToList();
            ViewData["Categories"] = new SelectList(categories, "Id", "Name");
            ViewData["Specializations"] = new SelectList(specializations, "Id", "Name");
            ViewData["LPUs"] = new SelectList(LPUs, "Id", "Name");
            AdminUser user = (AdminUser)await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.SurName!= user.SurName)
            {
                user.SurName = Input.SurName;
            }

            if (Input.MiddleName != user.MiddleName)
            {
                user.MiddleName = Input.MiddleName;
            }

            if (Input.Category != user.Category)
            {
                user.Category = Input.Category;
            }

            if (Input.Address != user.Address)
            {
                user.Address = Input.Address;
            }

            if (Input.Specialization != user.Specialization)
            {
                user.Specialization = Input.Specialization;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
