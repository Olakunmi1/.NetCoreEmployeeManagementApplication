using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
   public interface IEmployeeRepository
    {
       // IEnumerable<Employee> GetAllEmployess();
        Employee GetEmployee(int Id); 
        IEnumerable<Employee> GetAllEmployee(); 
        Employee AddEmployee(Employee employee);
        Employee UpdateEmployee(Employee employeeChanges);
        Employee DeleteEmployee(int id);


    }
}
