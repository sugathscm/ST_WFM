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
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Designations.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Designation GetDesignationById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Designations.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Designation designation)
        {
            using (STWFMEntities entities = new STWFMEntities())
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
