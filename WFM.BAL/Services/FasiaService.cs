using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class FasiaService
    {
        public List<Fasia> GetFasiaList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Fasias.OrderBy(d => d.Id).ToList();
            }
        }

        public Fasia GetFasiaById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Fasias.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Fasia fasia)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (fasia.Id == 0)
                {
                    entities.Fasias.Add(fasia);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(fasia).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
