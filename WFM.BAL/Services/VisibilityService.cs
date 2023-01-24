using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class VisibilityService
    {

        public List<Visibility> GetVisibilityList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Visibilities.OrderBy(d => d.Name).ToList();
            }
        }

        public Visibility GetVisibilityId(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Visibilities.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Visibility visibility)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (visibility.Id == 0)
                {
                    entities.Visibilities.Add(visibility);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(visibility).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
