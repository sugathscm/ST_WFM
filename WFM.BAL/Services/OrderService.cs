using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class OrderService
    {
        public List<Order> GetOrderList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Orders.Include("Client").OrderBy(d => d.Id).ToList();
            }
        }

        public List<Order> GetOrderFullList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Orders.Include("Client").OrderBy(d => d.Id).ToList();
            }
        }

        public Order GetOrderById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Orders.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Order employee)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (employee.Id == 0)
                {
                    entities.Orders.Add(employee);
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
