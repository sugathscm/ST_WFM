using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class SupplierTypeService
    {
        public List<SupplierType> GetSupplierTypeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.SupplierTypes.OrderBy(d => d.Type).ToList();
            }
        }

        public SupplierType GetSupplierTypeById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.SupplierTypes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(SupplierType supplierType)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (supplierType.Id == 0)
                {
                    entities.SupplierTypes.Add(supplierType);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(supplierType).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}