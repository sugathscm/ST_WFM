using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class IlluminationService
    {
        public List<Illumination> GetIlluminationList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Illuminations.OrderBy(d => d.Name).ToList();
            }
        }

        public Illumination GetIlluminationId(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Illuminations.Where(s => s.Id == id).SingleOrDefault();
            }
        }
        public void SaveOrUpdate(Illumination illumination)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (illumination.Id == 0)
                {
                    entities.Illuminations.Add(illumination);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(illumination).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
