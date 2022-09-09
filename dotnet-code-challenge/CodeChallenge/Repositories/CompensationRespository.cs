using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            var existingCompensation = _employeeContext.Compensations.Include(c => c.Employee).SingleOrDefault(c => c.Employee.EmployeeId == compensation.Employee.EmployeeId);
            if (existingCompensation is not null) return existingCompensation;
            var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == compensation.Employee.EmployeeId);
            if (employee is not null) compensation.Employee = employee;
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {

            return _employeeContext.Compensations.Include(c => c.Employee).SingleOrDefault(c => c.Employee.EmployeeId == employeeId);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
