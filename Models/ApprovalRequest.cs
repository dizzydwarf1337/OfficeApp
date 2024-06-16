using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartItApp.Models;

public partial class ApprovalRequest
{
    public int Id { get; set; }

    public int Approver { get; set; }

    public int LeaveRequest { get; set; }
    [ForeignKey(nameof(employee))]
    public int EmployeeId { get; set; }
    public string Status { get; set; } = null!;
    [NotMapped]
    [ForeignKey(nameof(EmployeeId))]
    public Models.Employee employee { get; set; }

}
