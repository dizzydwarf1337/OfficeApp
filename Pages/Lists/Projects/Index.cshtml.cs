using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Projects
{
    [Authorize]
    public class IndexModel : PageModel
    {
        
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly UserManager<Employee> _userManager;
        public IndexModel(SmartItApp.Models.SmartItAppContext context,UserManager<Employee> userManager)
        {
            _context = context;
            _userManager=userManager;
        }

        public IList<Project> Project { get;set; } = default!;
        [BindProperty]
        public int SearchStringId { get; set; }
        public string CurrentSortOrder { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            CurrentSortOrder = sortOrder;
            IQueryable<Project> projectsIQ = from e in _context.Projects
                                               select e;
            projectsIQ = sortOrder switch
            {
                "Id" => projectsIQ.OrderBy(e => e.Id),
                "Id_desc" => projectsIQ.OrderByDescending(e => e.Id),
                "ProjectManager" => projectsIQ.OrderBy(e => e.ProjectManager),
                "ProjectManager_desc" => projectsIQ.OrderByDescending(e => e.ProjectManager),
                "ProjectType" => projectsIQ.OrderBy(e => e.ProjectType),
                "ProjectType_desc" => projectsIQ.OrderByDescending(e => e.ProjectType),
                "StartDate" => projectsIQ.OrderBy(e => e.StartDate),
                "StartDate_desc" => projectsIQ.OrderByDescending(e => e.StartDate),
                "EndDate" => projectsIQ.OrderBy(e => e.EndDate),
                "EndDate_desc" => projectsIQ.OrderByDescending(e => e.EndDate),
                "Status" => projectsIQ.OrderBy(e => e.Status),
                "Status_desc" => projectsIQ.OrderByDescending(e => e.Status),
                _ => projectsIQ.OrderBy(e => e.ProjectManager),
            };
            if (User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("HR"))
            {
                Project = await projectsIQ.AsNoTracking().ToListAsync();
            }
            else if (User.IsInRole("Employee"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projectEmployees = await _context.ProjectEmployees
                    .Where(pe => pe.EmployeeId == user.Id)
                    .Select(pe => pe.Project)
                    .ToListAsync();
                Project = projectEmployees;
            }
            else if (User.IsInRole("PM"))
            {
                var user = await _userManager.GetUserAsync(User);
                Project = await projectsIQ
                    .Where(p => p.ProjectManager == user.Id)
                    .ToListAsync();
            }
            
        }
        public string GetSortOrder(string columnName, string currentSortOrder)
        {
            if (string.IsNullOrEmpty(currentSortOrder) || !currentSortOrder.StartsWith(columnName))
            {
                return columnName;
            }
            else if (currentSortOrder.EndsWith("_desc"))
            {
                return columnName;
            }
            else
            {
                return columnName + "_desc";
            }
        }
        public async Task OnPostFindIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin") || User.IsInRole("Supervisor") || User.IsInRole("HR")) {
                Project = await _context.Projects.Where(e => e.Id == SearchStringId).ToListAsync();
                return;
            }
            else 
            {
                var projectEmployees = await _context.ProjectEmployees
                .Include(pe => pe.Project)
                .Where(pe => pe.ProjectId == SearchStringId && pe.EmployeeId==user.Id)
                .ToListAsync();
                Project = projectEmployees.Select(pe => pe.Project).ToList();
                return;

            }
        }
    }
}
