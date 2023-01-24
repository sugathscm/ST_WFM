using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class SizeService
    {

        public List<Size> GetSizeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Sizes.OrderBy(d => d.Id).ToList();
            }
        }

        public Size GetSizeById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Sizes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Size size)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (size.Id == 0)
                {
                    entities.Sizes.Add(size);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(size).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
