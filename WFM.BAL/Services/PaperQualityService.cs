using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class PaperQualityService
    {
        public List<PaperQuality> GetPaperQualityList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.PaperQualities.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public PaperQuality GetPaperQualityById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.PaperQualities.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(PaperQuality paperQuality)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (paperQuality.Id == 0)
                {
                    entities.PaperQualities.Add(paperQuality);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(paperQuality).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}