using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        public AppDbContext MyDbContext { get; }

        private readonly List<Employee> employess;
        public MockEmployeeRepository(AppDbContext appDbContext)
        {
            MyDbContext = appDbContext;
            employess = new List<Employee>()
            {
                new Employee { Id= 1, Name = "Mary", Deparment = Dept.HR, Email = "Mary@gmail.com" },
                new Employee { Id= 2, Name = "John", Deparment = Dept.IT, Email = "john@gmail.com" },
                new Employee { Id= 3, Name = "Foluke", Deparment = Dept.Payroll, Email = "foluke@gmail.com"}
            };
        }
        //Adds a new employee
        public Employee AddEmployee(Employee employee)
        {
            employee.Id = employess.Max(e => e.Id) + 1;
            employess.Add(employee);
            return employee;
        }

        //returns employee with a specific ID
        public Employee GetEmployee(int Id) 
        {
            return employess.FirstOrDefault(e => e.Id==Id);
        }

        //returns all the employee
        IEnumerable<Employee> IEmployeeRepository.GetAllEmployee()
        {
            return from e in employess 
                   orderby e.Id
                   select e;
        }

        public Employee UpdateEmployee(Employee employeeChanges)
        {
            var employee = employess.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Deparment = employeeChanges.Deparment;
                employee.Email = employeeChanges.Email;
            }
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
          var employeeResponse = employess.FirstOrDefault(e => e.Id == id);
            if(employeeResponse != null)
            {
                employess.Remove(employeeResponse);
            }
            return employeeResponse;
        }
    }
}
