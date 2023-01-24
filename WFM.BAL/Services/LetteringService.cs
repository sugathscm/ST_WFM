using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class LetteringService
    {
        public List<Lettering> GetLetteringList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Letterings.OrderBy(d => d.Id).ToList();
            }
        }

        public Lettering GetLetteringById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Letterings.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Lettering lettering)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (lettering.Id == 0)
                {
                    entities.Letterings.Add(lettering);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(lettering).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }
}
