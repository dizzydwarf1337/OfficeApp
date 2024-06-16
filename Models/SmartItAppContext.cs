using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartItApp.Models;

public partial class SmartItAppContext : IdentityDbContext<Employee,IdentityRole<int>,int>
{
    public SmartItAppContext()
    {
    }

    public SmartItAppContext(DbContextOptions<SmartItAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApprovalRequest> ApprovalRequests { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Aboba10;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Approval__3214EC27AE2FE38E");

            entity.ToTable("Approval Request");

            entity.HasIndex(e => e.Approver, "IDX_ApprovalRequest_Approver");

            entity.HasIndex(e => e.LeaveRequest, "IDX_ApprovalRequest_LeaveRequest");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LeaveRequest).HasColumnName("Leave Request");
            entity.Property(e => e.Status).HasMaxLength(200);

            entity.HasOne<ApprovalRequest>().WithMany()
                .HasForeignKey(d => d.Approver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1_ApprovalRequest");

            entity.HasOne<LeaveRequest>().WithMany()
                .HasForeignKey(d => d.LeaveRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK2_ApprovalRequest");
        });
        modelBuilder.Entity<ProjectEmployee>()
                .HasKey(pe => new { pe.ProjectId, pe.EmployeeId });
        modelBuilder
              .HasAnnotation("ProductVersion", "8.0.6")
              .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .HasColumnType("nvarchar(max)");

            b.Property<string>("Name")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("NormalizedName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.HasKey("Id");

            b.HasIndex("NormalizedName")
                .IsUnique()
                .HasDatabaseName("RoleNameIndex")
                .HasFilter("[NormalizedName] IS NOT NULL");

            b.ToTable("AspNetRoles", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("ClaimType")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("ClaimValue")
                .HasColumnType("nvarchar(max)");

            b.Property<int>("RoleId")
                .HasColumnType("int");

            b.HasKey("Id");

            b.HasIndex("RoleId");

            b.ToTable("AspNetRoleClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("ClaimType")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("ClaimValue")
                .HasColumnType("nvarchar(max)");

            b.Property<int>("UserId")
                .HasColumnType("int");

            b.HasKey("Id");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
        {
            b.Property<string>("LoginProvider")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ProviderKey")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ProviderDisplayName")
                .HasColumnType("nvarchar(max)");

            b.Property<int>("UserId")
                .HasColumnType("int");

            b.HasKey("LoginProvider", "ProviderKey");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserLogins", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
        {
            b.Property<int>("UserId")
                .HasColumnType("int");

            b.Property<int>("RoleId")
                .HasColumnType("int");

            b.HasKey("UserId", "RoleId");

            b.HasIndex("RoleId");

            b.ToTable("AspNetUserRoles", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
        {
            b.Property<int>("UserId")
                .HasColumnType("int");

            b.Property<string>("LoginProvider")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("Name")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("Value")
                .HasColumnType("nvarchar(max)");

            b.HasKey("UserId", "LoginProvider", "Name");

            b.ToTable("AspNetUserTokens", (string)null);
        });

        modelBuilder.Entity("SmartItApp.Models.ApprovalRequest", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .HasColumnName("ID");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<int>("Approver")
                .HasColumnType("int");

            b.Property<int>("LeaveRequest")
                .HasColumnType("int")
                .HasColumnName("Leave Request");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.HasKey("Id")
                .HasName("PK__Approval__3214EC27AE2FE38E");

            b.HasIndex(new[] { "Approver" }, "IDX_ApprovalRequest_Approver");

            b.HasIndex(new[] { "LeaveRequest" }, "IDX_ApprovalRequest_LeaveRequest");

            b.ToTable("Approval Request", (string)null);
        });

        modelBuilder.Entity("SmartItApp.Models.Employee", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .HasColumnName("ID");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<int>("AccessFailedCount")
                .HasColumnType("int");

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .HasColumnType("nvarchar(max)");

            b.Property<int>("DaysOff")
                .HasColumnType("int");

            b.Property<string>("Email")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<bool>("EmailConfirmed")
                .HasColumnType("bit");

            b.Property<string>("FullName")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)")
                .HasColumnName("Full Name");

            b.Property<bool>("LockoutEnabled")
                .HasColumnType("bit");

            b.Property<DateTimeOffset?>("LockoutEnd")
                .HasColumnType("datetimeoffset");

            b.Property<string>("NormalizedEmail")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("NormalizedUserName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("PasswordHash")
                .HasColumnType("nvarchar(max)");

            b.Property<int>("PeoplePartner")
                .HasColumnType("int")
                .HasColumnName("People Partner");

            b.Property<string>("PhoneNumber")
                .HasColumnType("nvarchar(max)");

            b.Property<bool>("PhoneNumberConfirmed")
                .HasColumnType("bit");

            b.Property<string>("Photo")
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            b.Property<string>("Position")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("SecurityStamp")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<string>("Subdivision")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.Property<bool>("TwoFactorEnabled")
                .HasColumnType("bit");

            b.Property<string>("UserName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.HasKey("Id")
                .HasName("PK__Employee__3214EC270CA31F57");

            b.HasIndex("NormalizedEmail")
                .HasDatabaseName("EmailIndex");

            b.HasIndex("NormalizedUserName")
                .IsUnique()
                .HasDatabaseName("UserNameIndex")
                .HasFilter("[NormalizedUserName] IS NOT NULL");

            b.HasIndex("PeoplePartner");

            b.HasIndex(new[] { "Position" }, "IDX_Employee_Position");

            b.HasIndex(new[] { "Subdivision" }, "IDX_Employee_Subdivision");

            b.ToTable("AspNetUsers", (string)null);
        });

        modelBuilder.Entity("SmartItApp.Models.LeaveRequest", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .HasColumnName("ID");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("AbsenceReason")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)")
                .HasColumnName("Absence Reason");

            b.Property<string>("Comment")
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            b.Property<int>("Employee")
                .HasColumnType("int");

            b.Property<DateOnly>("EndDate")
                .HasColumnType("date")
                .HasColumnName("End Date");

            b.Property<DateOnly>("StartDate")
                .HasColumnType("date")
                .HasColumnName("Start Date");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.HasKey("Id")
                .HasName("PK__Leave Re__3214EC27F5BD9FEC");

            b.HasIndex(new[] { "Employee" }, "IDX_LeaveRequest_Employee");

            b.HasIndex(new[] { "StartDate" }, "IDX_LeaveRequest_StartDate");

            b.ToTable("Leave Request", (string)null);
        });

        modelBuilder.Entity("SmartItApp.Models.Project", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .HasColumnName("ID");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("Comment")
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            b.Property<DateOnly>("EndDate")
                .HasColumnType("date")
                .HasColumnName("End Date");

            b.Property<int>("ProjectManager")
                .HasColumnType("int")
                .HasColumnName("Project Manager");

            b.Property<string>("ProjectType")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)")
                .HasColumnName("Project Type");

            b.Property<DateOnly>("StartDate")
                .HasColumnType("date")
                .HasColumnName("Start Date");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            b.HasKey("Id")
                .HasName("PK__Project__3214EC271F4B943D");

            b.HasIndex(new[] { "ProjectManager" }, "IDX_Project_ProjectManager");

            b.HasIndex(new[] { "ProjectType" }, "IDX_Project_ProjectType");

            b.ToTable("Project", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
        {
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
        {
            b.HasOne("SmartItApp.Models.Employee", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
        {
            b.HasOne("SmartItApp.Models.Employee", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
        {
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("SmartItApp.Models.Employee", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
        {
            b.HasOne("SmartItApp.Models.Employee", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });


        modelBuilder.Entity("SmartItApp.Models.Employee", b =>
        {
            b.HasOne("SmartItApp.Models.Employee", "PeoplePartnerNavigation")
                .WithMany("InversePeoplePartnerNavigation")
                .HasForeignKey("PeoplePartner")
                .IsRequired()
                .HasConstraintName("FK_Employee");

            b.Navigation("PeoplePartnerNavigation");
        });
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC270CA31F57");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.Position, "IDX_Employee_Position");

            entity.HasIndex(e => e.Subdivision, "IDX_Employee_Subdivision");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .HasColumnName("Full Name");
            entity.Property(e => e.PeoplePartner).HasColumnName("People Partner");
            entity.Property(e => e.Photo).HasMaxLength(1000);
            entity.Property(e => e.Position).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(200);
            entity.Property(e => e.Subdivision).HasMaxLength(200);

            entity.HasOne(d => d.PeoplePartnerNavigation).WithMany(p => p.InversePeoplePartnerNavigation)
                .HasForeignKey(d => d.PeoplePartner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Leave Re__3214EC27F5BD9FEC");

            entity.ToTable("Leave Request");

            entity.HasIndex(e => e.Employee, "IDX_LeaveRequest_Employee");

            entity.HasIndex(e => e.StartDate, "IDX_LeaveRequest_StartDate");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AbsenceReason)
                .HasMaxLength(200)
                .HasColumnName("Absence Reason");
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.EndDate).HasColumnName("End Date");
            entity.Property(e => e.StartDate).HasColumnName("Start Date");
            entity.Property(e => e.Status).HasMaxLength(200);

            entity.HasOne<Employee>().WithMany()
                .HasForeignKey(d => d.Employee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequest");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Project__3214EC271F4B943D");

            entity.ToTable("Project");

            entity.HasIndex(e => e.ProjectManager, "IDX_Project_ProjectManager");

            entity.HasIndex(e => e.ProjectType, "IDX_Project_ProjectType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.EndDate).HasColumnName("End Date");
            entity.Property(e => e.ProjectManager).HasColumnName("Project Manager");
            entity.Property(e => e.ProjectType)
                .HasMaxLength(200)
                .HasColumnName("Project Type");
            entity.Property(e => e.StartDate).HasColumnName("Start Date");
            entity.Property(e => e.Status).HasMaxLength(200);

            entity.HasOne<Project>().WithMany()
                .HasForeignKey(d => d.ProjectManager)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project");
        });
        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
