using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class ClientService
    {
        public List<Client> GetClientList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Clients.Include("Designation").Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public List<Client> GetClientFullList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Clients.Include("Designation").OrderBy(d => d.Name).ToList();
            }
        }

        public Client GetClientById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Clients.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Client employee)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (employee.Id == 0)
                {
                    entities.Clients.Add(employee);
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
