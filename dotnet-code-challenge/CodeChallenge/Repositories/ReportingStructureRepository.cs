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
    public class ReportingStructureRepository : IReportingStructureRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IReportingStructureRepository> _logger;

        public ReportingStructureRepository(ILogger<IReportingStructureRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }
        public ReportingStructure GetByEmployeeId(string id)
        {
            var employee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            var reportingstructure = new ReportingStructure{Employee = employee};
            if (employee is null) return reportingstructure;
            reportingstructure.NumberOfReports = CalculateNumberOfReports(employee);

            return reportingstructure;
        }

        private int CalculateNumberOfReports (Employee employee) {
            if (employee.DirectReports is null) return 0;
            var numReports = employee.DirectReports.Count;
            foreach (var report in employee?.DirectReports) {
                var fullReport = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == report.EmployeeId);
                numReports += CalculateNumberOfReports(fullReport);
            }
            
            return numReports;
        }
    }
}
