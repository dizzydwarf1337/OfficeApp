using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Leave_Requests
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly SignInManager <Employee> _signInManager;

        public CreateModel(SmartItApp.Models.SmartItAppContext context, SignInManager<Employee> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
        
        ViewData["Employee"] = new SelectList(_context.Employees, "Id", "FullName");
            return Page();
        }

        [BindProperty]
        public LeaveRequest LeaveRequest { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            Employee employee = await _signInManager.UserManager.GetUserAsync(User);
            LeaveRequest.Employee = employee.Id;
            TimeSpan difference = LeaveRequest.EndDate.ToDateTime(TimeOnly.Parse("10:00 PM")) - LeaveRequest.StartDate.ToDateTime(TimeOnly.Parse("10:00 PM"));
            if(DateTime.Compare(LeaveRequest.StartDate.ToDateTime(TimeOnly.Parse("10:00 PM")), DateTime.Now)<0)
            {
                ModelState.AddModelError("LeaveRequest.StartDate", "Start Date is earlier than now");
            }
            if (difference.Days < 0)
            {
                ModelState.AddModelError("LeaveRequest.EndDate", "You chose wrong Start date End date interval");
            }
            int DaysDifference = difference.Days;
            if (DaysDifference > employee.DaysOff)
            {
               ModelState.AddModelError("LeaveRequest.EndDate", "You do not have enough days off or ");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.LeaveRequests.Add(LeaveRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
