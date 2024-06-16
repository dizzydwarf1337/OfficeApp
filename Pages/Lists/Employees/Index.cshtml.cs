using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Employees
{
    [Authorize(Roles = "Supervisor,Admin,HR,PM")]
    public class IndexModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly UserManager<Employee> _userManager;
        public IndexModel(SmartItApp.Models.SmartItAppContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Employee> Employee { get;set; } = default!;
        [BindProperty]
        public string SearchString { get; set;  }
        [BindProperty]
        public int SearchStringId { get; set; }
        public string CurrentSortOrder { get; set; }
        public async Task OnGetAsync(string sortOrder)
        {
            CurrentSortOrder = sortOrder;
            IQueryable<Employee> employeesIQ = from e in _context.Employees
                                               select e;

            employeesIQ = sortOrder switch
            {
                "Id"=> employeesIQ.OrderBy(e => e.Id),
                "Id_desc" => employeesIQ.OrderByDescending(e => e.Id),
                "FullName" => employeesIQ.OrderBy(e => e.FullName),
                "FullName_desc" => employeesIQ.OrderByDescending(e => e.FullName),
                "Subdivision" => employeesIQ.OrderBy(e => e.Subdivision),
                "Subdivision_desc" => employeesIQ.OrderByDescending(e => e.Subdivision),
                "Position" => employeesIQ.OrderBy(e => e.Position),
                "Position_desc" => employeesIQ.OrderByDescending(e => e.Position),
                "Status" => employeesIQ.OrderBy(e => e.Status),
                "Status_desc" => employeesIQ.OrderByDescending(e => e.Status),
                "DaysOff" => employeesIQ.OrderBy(e => e.DaysOff),
                "DaysOff_desc" => employeesIQ.OrderByDescending(e => e.DaysOff),
                _ => employeesIQ.OrderBy(e => e.FullName),
            };
            Employee = await employeesIQ.AsNoTracking().ToListAsync();
        }

        public async Task OnPostFindAsync()
        {
            Employee = await _context.Employees.Where(x => x.FullName.Contains(SearchString) && x.Position!="Supervisor").ToListAsync();
        }
        public async Task OnPostFindIdAsync()
        {
            Employee = await _context.Employees.Where(x => x.Id == (SearchStringId)).ToListAsync();
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
        public async Task<IActionResult> OnPostChangeStatusAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            if(employee.Status == SmartItApp.Enums.Statuses.Active.ToString())
            {
                employee.Status = SmartItApp.Enums.Statuses.Inactive.ToString();
            }
            else
            {
                employee.Status = SmartItApp.Enums.Statuses.Active.ToString();
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
