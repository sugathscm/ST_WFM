using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class OrderTypeService
    {
        public List<OrderType> GetOrderTypeList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.OrderTypes.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public OrderType GetOrderTypeById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.OrderTypes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(OrderType orderType)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (orderType.Id == 0)
                {
                    entities.OrderTypes.Add(orderType);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(orderType).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}

