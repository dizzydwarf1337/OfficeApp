using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartItApp.Models
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
    }
}
