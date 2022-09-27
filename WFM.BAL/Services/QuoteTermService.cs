using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;


namespace WFM.BAL.Services
{
    public class QuoteTermService
    {
        public List<QuoteTerm> GetQuoteTermList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.QuoteTerms.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public QuoteTerm GetQuoteTermById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.QuoteTerms.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(QuoteTerm QuoteTerms)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (QuoteTerms.Id == 0)
                {
                    entities.QuoteTerms.Add(QuoteTerms);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(QuoteTerms).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public List<QuoteTermDetail> GetQuoteTermsByQuoteId(int? quoteId)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.QuoteTermDetails.Include("QuoteTerm").Where(qt => qt.QuoteId == quoteId).ToList();
            }
        }
    }
}