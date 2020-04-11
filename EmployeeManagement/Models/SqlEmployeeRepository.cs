using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _MyDbContext;

        public SqlEmployeeRepository(AppDbContext MyDbContext)
        {
            _MyDbContext = MyDbContext; 
        }   
        
        public Employee AddEmployee(Employee employee)
        {
            _MyDbContext.Employees.Add(employee);
            _MyDbContext.SaveChanges(); 

           return employee;
        }

        public Employee DeleteEmployee(int id)
        {
           var employee = _MyDbContext.Employees.Find(id);
            if (employee != null)
            _MyDbContext.Employees.Remove(employee);
            _MyDbContext.SaveChanges();
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _MyDbContext.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            return _MyDbContext.Employees.Find(Id);
           
        }

        public Employee UpdateEmployee(Employee employeeChanges)
        {
            //here we attach the change coming from the user to the var 
            var employee = _MyDbContext.Employees.Attach(employeeChanges);
            if (employee != null)
            {
                //and we tell entity farmewrk that the sate of the object coming is modified
                employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _MyDbContext.SaveChanges();
            return employeeChanges;
        }
    }
}
