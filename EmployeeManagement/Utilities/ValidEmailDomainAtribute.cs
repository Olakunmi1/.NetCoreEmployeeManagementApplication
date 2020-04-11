using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities
{
    //Implementing a Custom Validation ..the class inherits from VAlidationAttribute
    //THEn we override the IsValid method
    public class ValidEmailDomainAtribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }
    }
}
