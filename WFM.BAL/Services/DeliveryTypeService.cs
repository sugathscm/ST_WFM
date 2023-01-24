using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class DeliveryTypeService
    {

        public List<DeliveryType> GetDeliveryTypeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.DeliveryTypes.OrderBy(d => d.Id).ToList();
            }
        }

        public DeliveryType GetDeliveryTypeById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.DeliveryTypes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(DeliveryType deliveryType)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (deliveryType.Id == 0)
                {
                    entities.DeliveryTypes.Add(deliveryType);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(deliveryType).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
