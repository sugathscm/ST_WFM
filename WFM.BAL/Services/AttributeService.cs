using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;
using Attribute = WFM.DAL.Attribute;

namespace WFM.BAL.Services
{
    public class AttributeService
    {

        public List<Attribute> GetAttributeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Attributes.OrderBy(d => d.Attribute1).ToList();
            }
        }

        public Attribute GetAttributeId(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Attributes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Attribute attribute)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (attribute.Id == 0)
                {
                    entities.Attributes.Add(attribute);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(attribute).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
