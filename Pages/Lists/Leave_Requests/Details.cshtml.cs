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
    public class DetailsModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;

        public DetailsModel(SmartItApp.Models.SmartItAppContext context)
        {
            _context = context;
        }

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
    }
}
