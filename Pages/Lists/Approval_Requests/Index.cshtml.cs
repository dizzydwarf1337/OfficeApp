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

namespace SmartItApp.Pages.Lists.Aproval_Requests
{
    [Authorize(Roles = "Supervisor,Admin,HR,PM")]
    public class IndexModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly SignInManager<Employee> _signInManager;

        public IndexModel(SmartItApp.Models.SmartItAppContext context, SignInManager<Employee> signInManager)
        {
            _context = context;
            _signInManager= signInManager;
        }

        public IList<ApprovalRequest> ApprovalRequest { get; set; } = default!;
        [BindProperty]
        public int SearchString { get; set; }
        public string CurrentSortOrder { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            CurrentSortOrder = sortOrder ?? "Id_asc";
            IQueryable<ApprovalRequest> approvalRequestsIQ = from s in _context.ApprovalRequests
                                                             select s;

            approvalRequestsIQ = sortOrder switch
            {
                "Approver_asc" => approvalRequestsIQ.OrderBy(s => s.Approver),
                "Approver_desc" => approvalRequestsIQ.OrderByDescending(s => s.Approver),
                "Id_desc" => approvalRequestsIQ.OrderByDescending(s => s.Id),
                "LeaveRequest_asc" => approvalRequestsIQ.OrderBy(s => s.LeaveRequest),
                "LeaveRequest_desc" => approvalRequestsIQ.OrderByDescending(s => s.LeaveRequest),
                "EmployeeId_asc" => approvalRequestsIQ.OrderBy(s => s.EmployeeId),
                "EmployeeId_desc" => approvalRequestsIQ.OrderByDescending(s => s.EmployeeId),
                "Status_asc" => approvalRequestsIQ.OrderBy(s => s.Status),
                "Status_desc" => approvalRequestsIQ.OrderByDescending(s => s.Status),
                _ => approvalRequestsIQ.OrderBy(s => s.Id),
            };

            ApprovalRequest = await approvalRequestsIQ.AsNoTracking().ToListAsync();
        }

        public async Task OnPostFindAsync()
        {
            ApprovalRequest = await _context.ApprovalRequests.Where(x => x.Id == SearchString).ToListAsync();
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
        public async Task<IActionResult> OnPostApproveRequestAsync(int? id)
        {
            var approvalRequest= await _context.ApprovalRequests.FindAsync(id);
            if (approvalRequest == null)
            {
                return NotFound();
            }
            var leaveRequest = await _context.LeaveRequests.FindAsync(approvalRequest.LeaveRequest);
            var employee = await _context.Employees.FindAsync(leaveRequest.Employee);
            approvalRequest.Status = SmartItApp.Enums.RequestStatus.Approved.ToString();
            leaveRequest.Status = SmartItApp.Enums.RequestStatus.Approved.ToString();
            TimeSpan difference = leaveRequest.EndDate.ToDateTime(TimeOnly.Parse("10:00 PM")) - leaveRequest.StartDate.ToDateTime(TimeOnly.Parse("10:00 PM"));
            employee.DaysOff-=difference.Days;
            leaveRequest.Comment = "Approved";
            approvalRequest.Approver = (await _signInManager.UserManager.GetUserAsync(User)).Id;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Approved");
        }
        public async Task<IActionResult> OnPostRejectRequestAsync(int? id,string rejectReason)
        {
            var approvalRequest = await _context.ApprovalRequests.FindAsync(id);
            if (approvalRequest == null)
            {
                return NotFound();
            }
            var leaveRequest = await _context.LeaveRequests.FindAsync(approvalRequest.LeaveRequest);
            approvalRequest.Status = SmartItApp.Enums.RequestStatus.Rejected.ToString();
            leaveRequest.Status = SmartItApp.Enums.RequestStatus.Rejected.ToString();
            leaveRequest.Comment = rejectReason;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Rejected");
        }
    }
}
