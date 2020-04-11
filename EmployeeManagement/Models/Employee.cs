using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models

{
    public class Employee
    {
        public int Id { get; set; }
        [Required, MaxLength(50, ErrorMessage = "Name cannot Exceed 50 Characters")]
        public string Name { get; set; }
        [Required]
        [Display(Name ="Office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Deparment { get; set; }

        //Just Adding comment to this property 
        public string StringPath { get; set; }



    }
}
