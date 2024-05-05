using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WFM.BAL.Enums;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class InvoiceService
    {

        public InvoiceService() { }

        public void SaveOrUpdate(Invoice invoice)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                try
                {

                    if (invoice.Id == 0)
                    {
                        entities.Invoices.Add(invoice);
                        entities.SaveChanges();
                    }
                    else
                    {
                        entities.Entry(invoice).State = System.Data.Entity.EntityState.Modified;
                        entities.SaveChanges();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        public List<Invoice> GetInvoiceList()
        {
            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    return entities.Invoices.Include("Order")
                       .Include("Order.Client")
                       .Include("Order.Employee").OrderByDescending(q=>q.CreatedDate).ToList();

                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public bool InvoiceExists(int orderId)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                var AprilInvoices = entities.Invoices.Where(q => q.OrderId == orderId).OrderBy(q => q.Id).ToList();
                return AprilInvoices.Any();
            }

        }


        public Invoice GetInvoiceById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Invoices
                    .Include("Order")
                    .Include("Order.AdditionalCharges")
                    .Include("Order.Client")
                    .Include("Order.OrderType")
                    .Include("Order.OrderItems")
                    .Include("Order.OrderItems.Category").Where(s => s.Id == id).SingleOrDefault();
            }
        }
    }
}
