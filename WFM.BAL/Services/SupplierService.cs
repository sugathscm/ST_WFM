using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class SupplierService
    {

        public List<Supplier> GetSupplierList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Suppliers.Include("SupplierType").OrderBy(d => d.Name).ToList();
            }
        }

        public Supplier GetSupplierById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Suppliers.Include("SupplierType").Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Supplier supplier)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (supplier.Id == 0)
                {
                    entities.Suppliers.Add(supplier);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(supplier).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }
}
