using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Aproval_Requests
{
    public class DetailsModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public DetailsModel(SmartItApp.Models.SmartItAppContext context)
        {
            _context = context;
        }

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
    }
}
