using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class MaterialService
    {

        public List<Material> GetMaterialList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Materials.OrderBy(d => d.Name).ToList();
            }
        }

        public Material GetMaterialId(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Materials.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Material material)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (material.Id == 0)
                {
                    entities.Materials.Add(material);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(material).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
