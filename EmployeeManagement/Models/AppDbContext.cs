using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //dbset of our model which creates the table in the DB
        public DbSet<Employee> Employees { get; set; }

        /*
         * 1. The line of code below overrides the method "OnModelCreating" in order to seed 
         * the database so we can have something to work with
         * then an extension method is created inside of a seperate class called
         * "SeedingExtension" and the Seed mehtod is called which contained the "Dummy data's".
         */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //the line of code below enables DELETEBehaviour To be restricted such 
            //that Beofre A role is deleted the Users under it must be deleted first ...
            //After this you add a new migration, then u update database..

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
            //SeedingExtenstionClass.Seed();
            modelBuilder.Seed();
        }
    }
}
