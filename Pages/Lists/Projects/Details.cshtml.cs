using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Projects
{
    [Authorize(Roles = "Admin,Supervisor,PM")]
    public class DetailsModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly SignInManager<Employee> _signInManager;

        public DetailsModel(SmartItApp.Models.SmartItAppContext context, SignInManager<Employee> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public Models.Project Project { get; set; } = default!;
        [BindProperty]
        public List<Employee> ProjectEmployees { get; set; } = default!;
        public List<Employee> PMEmployees { get; set; } = default!;
        [BindProperty]
        public Employee EmployeeToAdd { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
            if (Project == null)
            {
                return NotFound();
            }


            ProjectEmployees = await (
                from pe in _context.ProjectEmployees
                where pe.ProjectId == id
                join e in _context.Employees on pe.EmployeeId equals e.Id
                select e
            ).ToListAsync();

            if (User.IsInRole("Admin") || User.IsInRole("Supervisor"))
            {
                //PMEmployees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
                PMEmployees = await _context.Employees.ToListAsync();
            }
            else if (User.IsInRole("PM"))
            {
                var user = await _signInManager.UserManager.GetUserAsync(User);
                var subdivision = user.Subdivision;
                //PMEmployees = new SelectList(await _context.Employees.Where(e => e.Subdivision == subdivision).ToListAsync(), "Id", "FullName");
                PMEmployees = await _context.Employees.Where(e => e.Subdivision == subdivision).ToListAsync();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAddEmployeeAsync(int id, int employeeId)
        {
            var employeeToAdd = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employeeToAdd == null)
            {
                return NotFound("Selected employee not found.");
            }
            if (await _context.ProjectEmployees.AnyAsync(pe => pe.ProjectId == id && pe.EmployeeId == employeeId))
            {
                return NotFound("Selected employee is already in the project.");
            }
            var projectEmployee = new ProjectEmployee
            {
                ProjectId = id,
                EmployeeId = employeeToAdd.Id
            };

            _context.ProjectEmployees.Add(projectEmployee);
            await _context.SaveChangesAsync();
            return await OnGetAsync(id);
        }
        public async Task<IActionResult> OnPostDeleteFromProjectAsync(int EmployeeId,int ProjectId)
        {
            ProjectEmployee projectEmployee = await _context.ProjectEmployees.FirstOrDefaultAsync(pe => pe.EmployeeId == EmployeeId && pe.ProjectId == ProjectId);
            if (projectEmployee == null)
            {
                return NotFound("Selected employee not found.");
            }
            _context.ProjectEmployees.Remove(projectEmployee);
            await _context.SaveChangesAsync();
            return await OnGetAsync(ProjectId);
        }
    }
}
