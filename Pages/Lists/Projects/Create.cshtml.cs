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

namespace SmartItApp.Pages.Lists.Projects
{
    [Authorize(Roles ="Supervisor,Admin,PM")]
    public class CreateModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly SignInManager<Employee> _signInManager;

        public CreateModel(SmartItApp.Models.SmartItAppContext context, SignInManager<Employee> SignInManager)
        {
            _context = context;
            _signInManager=SignInManager;
        }

        public IActionResult OnGet()
        {
        ViewData["ProjectManager"] = new SelectList(_context.Employees, "Id", "FullName");
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ProjectEmployee projectEmployee = new ProjectEmployee();
            projectEmployee.ProjectId = Project.Id;
            projectEmployee.Project=Project;
            projectEmployee.Employee = (await _signInManager.UserManager.GetUserAsync(User));
            projectEmployee.EmployeeId = (await _signInManager.UserManager.GetUserAsync(User)).Id;
            _context.ProjectEmployees.Add(projectEmployee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
