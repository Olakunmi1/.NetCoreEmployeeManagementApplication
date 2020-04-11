using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    /*
     * This is an extension class created that inherits from "ModelBuilder" 
     * in which theh Seed prarameter is decorated as "Void"
     * which is then called in "AppDbContext" inside of ModelBuilder method
     */
    public static class SeedingExtenstionClass
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            /*
        * 1. The line of code below overrides the method "OnModelCreating" in order to seed 
        * the database so we can have something to work with
        * 2. then we call ".HasData" which is the actuall method that seeds the DB 
        * Basiclly the code below seeds d database with dummy data..
        */

            modelBuilder.Entity<Employee>()
                        .HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Mark__James",
                    Deparment = Dept.Payroll,
                    Email = "James_M@gmail.com",
                    
                },

                   new Employee
                   {
                       Id = 2,
                       Name = "John__M",
                       Deparment = Dept.IT,
                       Email = "john@gmail.com",
                    
                   },

                      new Employee
                      {
                          Id = 3,
                          Name = "Regina",
                          Deparment = Dept.HR,
                          Email = "regina_a@gmail.com",
                         
                      }
                );
        }
    }
}
