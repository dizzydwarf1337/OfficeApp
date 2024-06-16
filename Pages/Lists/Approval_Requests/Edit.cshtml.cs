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

namespace SmartItApp.Pages.Lists.Aproval_Requests
{
    [Authorize(Roles = "Supervisor")]
    public class EditModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public EditModel(SmartItApp.Models.SmartItAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ApprovalRequest ApprovalRequest { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalrequest =  await _context.ApprovalRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (approvalrequest == null)
            {
                return NotFound();
            }
            ApprovalRequest = approvalrequest;
           ViewData["Approver"] = new SelectList(_context.Employees, "Id", "FullName");
           ViewData["LeaveRequest"] = new SelectList(_context.LeaveRequests, "Id", "AbsenceReason");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ApprovalRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalRequestExists(ApprovalRequest.Id))
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

        private bool ApprovalRequestExists(int id)
        {
            return _context.ApprovalRequests.Any(e => e.Id == id);
        }
    }
}
