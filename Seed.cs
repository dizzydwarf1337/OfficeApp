using SmartItApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Data.Common;
using SQLitePCL;

namespace SmartItApp
{
    public class Seed
    {
        public static class DbInitializer
        {
            public static async void Initialize(SmartItAppContext context)
            {
                if (!context.Employees.Any(e => e.UserName == "Supervisor"))
                {
                    var supervisor = new Employee()
                    {
                        Id=4,
                        FullName = "Supervisor",
                        Position = "Supervisor",
                        Subdivision = "Supervisor",
                        Status = "Active",
                        PeoplePartner = 4,
                        DaysOff = 0,
                        Photo = "/images/user_blank.jpg",
                        UserName = "Supervisor",
                        NormalizedUserName = "SUPERVISOR",
                        Email = "noneed@gmail.com",
                        NormalizedEmail = "NONEED@GMAIL.COM",
                        EmailConfirmed = true,
                        PasswordHash = "AQAAAAIAAYagAAAAEElk8U/FcVK+RFYsfEUjPWHg0uTeMg7qu4CAedBqDJR5+O5o7Ld+MubsUQ3/hnUOEw==",
                        SecurityStamp = "LWLQQXAXI3KUM2Q5PBL4UAKYTYH2HMAH",
                        ConcurrencyStamp = "6ef1ed88-fe81-431d-aa48-b7ecfc112530",
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.AspNetUsers ON");
                            context.Employees.Add(supervisor);
                            context.SaveChanges();
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.AspNetUsers OFF");
                            transaction.Commit();
                        }
                        catch (DbException)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                    string insertRolesSql = $@"
                    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId) VALUES (4, 1);
                    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId) VALUES (4, 2);
                    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId) VALUES (4, 4);
                    INSERT INTO dbo.AspNetUserRoles (UserId, RoleId) VALUES (4, 5);
                    ";

                    context.Database.ExecuteSqlRaw(insertRolesSql);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}