using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SmartItApp.Models;

public partial class Employee : IdentityUser<int>
{
    override public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Subdivision { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int PeoplePartner { get; set; }

    public int DaysOff { get; set; }

    public string? Photo { get; set; }

    [NotMapped]
    public virtual ICollection<ApprovalRequest>? ApprovalRequests { get; set; } = new List<ApprovalRequest>();
    [NotMapped]
    public virtual ICollection<Employee>? InversePeoplePartnerNavigation { get; set; } = new List<Employee>();
    [NotMapped]
    public virtual ICollection<LeaveRequest>? LeaveRequests { get; set; } = new List<LeaveRequest>();
    [NotMapped]
    public virtual Employee? PeoplePartnerNavigation { get; set; } = null!;
    [NotMapped]
    public virtual ICollection<Project>? Projects { get; set; } = new List<Project>();
}
