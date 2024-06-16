using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Leave_Requests
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly SignInManager<Employee> _signInManager;

        public IndexModel(SmartItApp.Models.SmartItAppContext context, SignInManager<Employee> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        public Employee employee { get; set; } = default!;
        [BindProperty]
        public int SearchString { get; set; }
        public string CurrentSortOrder { get; set; }
        public IList<LeaveRequest> LeaveRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string sortOrder)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }

            CurrentSortOrder = sortOrder ?? "Id_asc";
            IQueryable<LeaveRequest> LeaveRequestsIQ = _context.LeaveRequests;
            LeaveRequestsIQ = sortOrder switch
            {
                "Id_desc" => LeaveRequestsIQ.OrderByDescending(e => e.Id),
                "Employee_asc" => LeaveRequestsIQ.OrderBy(e => e.Employee),
                "Employee_desc" => LeaveRequestsIQ.OrderByDescending(e => e.Employee),
                "AbsenceReason_asc" => LeaveRequestsIQ.OrderBy(e => e.AbsenceReason),
                "AbsenceReason_desc" => LeaveRequestsIQ.OrderByDescending(e => e.AbsenceReason),
                "StartDate_asc" => LeaveRequestsIQ.OrderBy(e => e.StartDate),
                "StartDate_desc" => LeaveRequestsIQ.OrderByDescending(e => e.StartDate),
                "EndDate_asc" => LeaveRequestsIQ.OrderBy(e => e.EndDate),
                "EndDate_desc" => LeaveRequestsIQ.OrderByDescending(e => e.EndDate),
                "Comment_asc" => LeaveRequestsIQ.OrderBy(e => e.Comment),
                "Comment_desc" => LeaveRequestsIQ.OrderByDescending(e => e.Comment),
                "Status_asc" => LeaveRequestsIQ.OrderBy(e => e.Status),
                "Status_desc" => LeaveRequestsIQ.OrderByDescending(e => e.Status),
                _ => LeaveRequestsIQ.OrderBy(e => e.Id) 
            };

            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (User.IsInRole("Admin") || User.IsInRole("Supervisor"))
            {
                LeaveRequest = await LeaveRequestsIQ.ToListAsync();
            }
            else if (User.IsInRole("HR") || User.IsInRole("PM"))
            {
                var currentUser = await _signInManager.UserManager.GetUserAsync(User);
                var subdivision = currentUser.Subdivision;
                LeaveRequest = await _context.LeaveRequests
                    .Where(lr => _context.Employees.Any(e => e.Id == lr.Employee && e.Subdivision == subdivision))
                    .ToListAsync();
            }
            else if (User.IsInRole("Employee"))
            {
                LeaveRequest = await LeaveRequestsIQ.Where(x => x.Employee == user.Id).ToListAsync();
                employee = user;
            }

            return Page();
        }

        public async Task OnPostFindAsync()
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            if (User.IsInRole("Employee"))
            {
                LeaveRequest = await _context.LeaveRequests.Where(x => x.Id == SearchString && x.Employee==user.Id).ToListAsync();
            }
            else
            {
                LeaveRequest = await _context.LeaveRequests.Where(x => x.Id == SearchString).ToListAsync();
            }
        }

        public string GetSortOrder(string columnName, string currentSortOrder)
        {
            if (string.IsNullOrEmpty(currentSortOrder) || !currentSortOrder.StartsWith(columnName))
            {
                return columnName + "_asc";
            }
            else if (currentSortOrder.EndsWith("_asc"))
            {
                return columnName + "_desc";
            }
            else
            {
                return columnName + "_asc";
            }
        }
        public async Task<IActionResult> OnPostSubmitAsync(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            LeaveRequest leaveRequest = await _context.LeaveRequests.FindAsync(id);
            ApprovalRequest approvalRequest = new ApprovalRequest();
            approvalRequest.LeaveRequest = leaveRequest.Id;
            approvalRequest.Approver = 4;
            leaveRequest.Status=Enums.RequestStatus.Submitted.ToString();
            approvalRequest.Status=Enums.RequestStatus.Submitted.ToString();
            approvalRequest.EmployeeId = leaveRequest.Employee;
            await _context.ApprovalRequests.AddAsync(approvalRequest);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Submitted");

        }
    }
}

