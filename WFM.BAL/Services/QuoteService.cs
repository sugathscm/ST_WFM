using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class QuoteService
    {
        public List<Quote> GetQuoteList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Include("Client").Where(d => d.IsActive == true).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteFullList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Include("Client").OrderBy(d => d.Id).ToList();
            }
        }

        public Quote GetQuoteById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Quote employee)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (employee.Id == 0)
                {
                    entities.Quotes.Add(employee);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
