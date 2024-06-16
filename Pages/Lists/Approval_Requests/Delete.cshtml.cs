using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Aproval_Requests
{
    [Authorize(Roles ="Supervisor")]
    public class DeleteModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public DeleteModel(SmartItApp.Models.SmartItAppContext context)
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

            var approvalrequest = await _context.ApprovalRequests.FirstOrDefaultAsync(m => m.Id == id);

            if (approvalrequest == null)
            {
                return NotFound();
            }
            else
            {
                ApprovalRequest = approvalrequest;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalrequest = await _context.ApprovalRequests.FindAsync(id);
            if (approvalrequest != null)
            {
                ApprovalRequest = approvalrequest;
                _context.ApprovalRequests.Remove(ApprovalRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
