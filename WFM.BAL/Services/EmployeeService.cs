using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class EmployeeService
    {
        public List<Employee> GetEmployeeList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Employees.Include("Designation").Include("Division").Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public List<Employee> GetEmployeeFullList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Employees.Include("Designation").Include("Division").OrderBy(d => d.Name).ToList();
            }
        }

        public Employee GetEmployeeById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Employees.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Employee employee)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (employee.Id == 0)
                {
                    entities.Employees.Add(employee);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}