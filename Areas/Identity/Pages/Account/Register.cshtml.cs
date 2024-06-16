﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SmartItApp.Models;

namespace SmartItApp.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Supervisor,Admin,HR")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Employee> _signInManager;
        private readonly UserManager<Employee> _userManager;
        private readonly IUserStore<Employee> _userStore;
        private readonly IUserEmailStore<Employee> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly SmartItAppContext _context;
        public RegisterModel(
            UserManager<Employee> userManager,
            IUserStore<Employee> userStore,
            SignInManager<Employee> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment hostingEnvironment,
            SmartItAppContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<Employee> HRs{ get; set; } = new List<Employee>();
        public string ReturnUrl { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            public string FullName { get; set; } = null!;
            [Required]
            public string Subdivision { get; set; } = null!;
            
            [Required]
            public string Position { get; set; } = null!;
            
            [Required]
            public string Status { get; set; } = null!;
            public int? PeoplePartner { get; set; }
            [Required]
            public int DaysOff { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Role { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            HRs = _context.Employees.Where(x=>x.Position=="HR" || x.Position=="Supervisor").ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile ImageFile,string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                if (ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    user.Photo = "/images/" + uniqueFileName;
                }
                else
                {
                    user.Photo = "/images/user_blank.jpg";
                }

                await _userStore.SetUserNameAsync(user, Input.FullName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, "noneed@gmail.com", CancellationToken.None);

                await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
                user.DaysOff = Input.DaysOff;
                user.FullName=Input.FullName;
                user.Subdivision = Input.Subdivision;
                user.Position = Input.Position;
                user.Status= Input.Status;
                if (Input.PeoplePartner == null)
                {
                    user.PeoplePartner = Convert.ToInt32(_signInManager.UserManager.GetUserId(User));
                }
                else
                {
                    user.PeoplePartner = (int)Input.PeoplePartner;
                }
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _signInManager.UserManager.AddToRoleAsync(user, Input.Role);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        return LocalRedirect(returnUrl);
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        private Employee CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Employee>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Employee)}'. " +
                    $"Ensure that '{nameof(Employee)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Employee> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Employee>)_userStore;
        }
    }
}
