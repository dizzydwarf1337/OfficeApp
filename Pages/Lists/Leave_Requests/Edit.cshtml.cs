using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Leave_Requests
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public EditModel(SmartItApp.Models.SmartItAppContext context)
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

            var leaverequest =  await _context.LeaveRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (leaverequest == null)
            {
                return NotFound();
            }
            LeaveRequest = leaverequest;
           ViewData["Employee"] = new SelectList(_context.Employees, "Id", "FullName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LeaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(LeaveRequest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.Id == id);
        }
    }
}
