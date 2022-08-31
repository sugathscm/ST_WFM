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
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes.Include("Client").Where(d => d.IsActive == true).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteActiveList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsConverted == false || o.IsConverted == null).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteConvertedList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsConverted == true).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteApprovedList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsApproved == true).OrderBy(d => d.Id).ToList();
            }
        }

        public Quote GetQuoteById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Quotes
                    .Include("QuoteItems")
                    .Include("Client")
                    .Include("QuoteTermDetails")
                    .Include("QuoteTermDetails.QuoteTerm")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Quote quote)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (quote.Id == 0)
                {
                    entities.Quotes.Add(quote);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(quote).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(QuoteItem quoteItem)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (quoteItem.QuoteId != 0)
                {
                    entities.QuoteItems.Add(quoteItem);
                    entities.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(QuoteTermDetail quoteTermDetail)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (quoteTermDetail.QuoteId != 0)
                {
                    entities.QuoteTermDetails.Add(quoteTermDetail);
                    entities.SaveChanges();
                }
            }
        }

        public void RemoveItems(Quote quote)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                bool validateOnSaveEnabled = entities.Configuration.ValidateOnSaveEnabled;
                try
                {
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    foreach (var item in quote.QuoteItems.ToList())
                    {
                        entities.QuoteItems.Attach(item);
                        entities.QuoteItems.Remove(item);
                        entities.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                    throw;
                }
                finally
                {
                    entities.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
                }
            }
        }

        public void RemoveTerms(Quote quote)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                bool validateOnSaveEnabled = entities.Configuration.ValidateOnSaveEnabled;
                try
                {
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    foreach (var item in quote.QuoteTermDetails.ToList())
                    {
                        entities.QuoteTermDetails.Attach(item);
                        entities.QuoteTermDetails.Remove(item);
                        entities.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                    throw;
                }
                finally
                {
                    entities.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
                }
            }
        }
    }
}