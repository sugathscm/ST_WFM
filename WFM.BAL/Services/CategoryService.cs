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
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Categories.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Category GetCategoryById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Categories.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public string GetCategoryDescription(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (id != null & id != 0)
                    return entities.Categories.Where(s => s.Id == id).SingleOrDefault().Description;
                else
                    return string.Empty;
            }
        }

        public void SaveOrUpdate(Category category)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
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