using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class CategoryService
    {
        public List<Category> GetCategoryList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Categories.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Category GetCategoryById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Categories.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Category category)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (category.Id == 0)
                {
                    entities.Categories.Add(category);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
