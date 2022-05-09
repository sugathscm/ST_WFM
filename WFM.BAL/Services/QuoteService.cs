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

        public List<Quote> GetQuoteActiveList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsConverted == false || o.IsConverted == null).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteConvertedList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsConverted == true).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Quote> GetQuoteApprovedList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Quotes.Include("Client").Where(o => o.IsApproved == true).OrderBy(d => d.Id).ToList();
            }
        }

        public Quote GetQuoteById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
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
            using (STWFMEntities entities = new STWFMEntities())
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
            using (STWFMEntities entities = new STWFMEntities())
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
            using (STWFMEntities entities = new STWFMEntities())
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
            using (STWFMEntities entities = new STWFMEntities())
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
            using (STWFMEntities entities = new STWFMEntities())
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
