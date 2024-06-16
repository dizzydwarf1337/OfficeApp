using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Aproval_Requests
{
    [Authorize(Roles="Supervisor")]
    public class CreateModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public CreateModel(SmartItApp.Models.SmartItAppContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["Approver"] = new SelectList(_context.Employees, "Id", "FullName");
        ViewData["LeaveRequest"] = new SelectList(_context.LeaveRequests, "Id", "AbsenceReason");
            return Page();
        }

        [BindProperty]
        public ApprovalRequest ApprovalRequest { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ApprovalRequests.Add(ApprovalRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
