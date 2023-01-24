using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;


namespace WFM.BAL.Services
{
    public class FrameworkService
    {
        public List<Framework> GetFrameworkList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Frameworks.OrderBy(d => d.Id).ToList();
            }
        }

        public Framework GetFrameworkById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Frameworks.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Framework framework)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (framework.Id == 0)
                {
                    entities.Frameworks.Add(framework);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(framework).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
