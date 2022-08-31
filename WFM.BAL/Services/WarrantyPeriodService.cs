using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class WarrantyPeriodService
    {
        public List<WarrantyPeriod> GetWarrantyPeriodList()
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.WarrantyPeriods.OrderBy(d => d.Name).ToList();
            }
        }

        public WarrantyPeriod GetWarrantyPeriodById(int? id)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.WarrantyPeriods.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(WarrantyPeriod WarrantyPeriods)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                if (WarrantyPeriods.Id == 0)
                {
                    entities.WarrantyPeriods.Add(WarrantyPeriods);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(WarrantyPeriods).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}