using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Leave_Requests
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public DeleteModel(SmartItApp.Models.SmartItAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LeaveRequest LeaveRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaverequest = await _context.LeaveRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (leaverequest == null)
            {
                return NotFound();
            }
            else
            {
                LeaveRequest = leaverequest;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaverequest = await _context.LeaveRequests.FindAsync(id);
            if (leaverequest.Status != Enums.RequestStatus.New.ToString() && User.IsInRole("Employee")) return Forbid();
            if (leaverequest != null)
            {
                LeaveRequest = leaverequest;
                _context.LeaveRequests.Remove(LeaveRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
