using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartItApp.Models;

namespace SmartItApp.Pages.Lists.Employees
{
    [Authorize(Roles = "Supervisor,Admin,HR")]
    public class EditModel : PageModel
    {
        private readonly SmartItApp.Models.SmartItAppContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<Employee> _userManager;
        public EditModel(SmartItApp.Models.SmartItAppContext context, IWebHostEnvironment hostingEnvironment,UserManager<Employee> UserManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = UserManager;
        }

        [BindProperty]
        public Models.Employee Employee { get; set; } = default!;
        public List<Employee> HRs { get; set; } = default!;
        public IFormFile? ImageFile { get; set; }
        public string ImgDel;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee =  await _context.Employees.FirstOrDefaultAsync(m => m.Id == id);
            var hrs = await _context.Employees.Where(e => e.Position == "HR").ToListAsync();
            if (employee == null)
            {
                return NotFound();
            }
            Employee = employee;
            HRs = hrs;
            TempData["empId"]= Employee.Id;
            TempData["ImgDel"] = Employee.Photo;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? ImageFile)
        {
            ImgDel = TempData["ImgDel"] as string;
            var employee = await _context.Employees.FindAsync(Employee.Id);
            if (employee == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            employee.FullName = Employee.FullName;
            employee.Subdivision = Employee.Subdivision;
            employee.Position = Employee.Position;
            employee.Status = Employee.Status;
            employee.PeoplePartner = Employee.PeoplePartner;
            employee.DaysOff = Employee.DaysOff;

            try
            {
                if (ImageFile != null)
                {
                    await DeleteImage(ImgDel);
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    employee.Photo = "/images/" + uniqueFileName;
                }
                else
                {
                    employee.Photo = ImgDel;
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.Id))
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

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        public async Task<bool> DeleteImage(string fileName)
        {
            try
            {
                var user = await _context.Employees.FindAsync(Convert.ToInt32(TempData["empId"]));
                var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, fileName.TrimStart('/'));
                if(user.Photo == "/images/user_blank.jpg")
                {
                    return true;
                }
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No existing file {ex.Message}");
                return false;
            }
        }
    }
}
