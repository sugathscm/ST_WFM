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
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.WarrantyPeriods.OrderBy(d => d.Name).ToList();
            }
        }

        public WarrantyPeriod GetWarrantyPeriodById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.WarrantyPeriods.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(WarrantyPeriod WarrantyPeriods)
        {
            using (STWFMEntities entities = new STWFMEntities())
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

