using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class DesignationService
    {
        public List<Designation> GetDesignationList()
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Designations.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Designation GetDesignationById(int? id)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Designations.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Designation designation)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                if (designation.Id == 0)
                {
                    entities.Designations.Add(designation);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(designation).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}