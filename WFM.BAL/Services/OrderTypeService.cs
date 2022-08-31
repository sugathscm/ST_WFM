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
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.OrderTypes.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public OrderType GetOrderTypeById(int? id)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.OrderTypes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(OrderType orderType)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
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