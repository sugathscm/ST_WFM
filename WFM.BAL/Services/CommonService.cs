using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public static class CommonService
    {
        public static int SaveLoginAudit(LoginAudit loginAudit)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                entities.LoginAudits.Add(loginAudit);
                entities.SaveChanges();
            }
            return 1;
        }

        public static int SaveDataAudit(DataAudit dataAudit)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                entities.DataAudits.Add(dataAudit);
                entities.SaveChanges();
            }
            return 1;
        }

        public static string GenerateQuoteCode(string year, string month, bool isVAT)
        {
            var number = 1;

            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                var quotesInMonth = entities.Quotes.Where(q => q.Month == month && q.Year == year).OrderBy(q => q.Id).ToList();
                if (quotesInMonth.Count > 0)
                {
                    number = quotesInMonth.Last().CodeNumber.Value + 1;
                }
            }

            return string.Format("{0}-{1}-{2}-{3}", (isVAT) ? "ST" : "STA", year.Substring(2, 2), month, number.ToString("000"));
        }

        public static string GenerateOrderCode(string year, string month, bool isVAT, string QuoteCode)
        {
            var number = 1;



            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                var OrdetypeId = entities.OrderTypes.Where(q => q.Name == QuoteCode.Trim()).OrderBy(q => q.Id).ToList().FirstOrDefault();

                var qordersInMonth = entities.Orders.Where(q => q.Month == month && q.Year == year && q.OrderTypeId == OrdetypeId.Id).OrderBy(q => q.Id).ToList();
                if (qordersInMonth.Count > 0)
                {
                    number = qordersInMonth.Last().CodeNumber.Value + 1;
                }
            }

            return string.Format("{0}-{1}-{2}-{3}", QuoteCode.Split('-')[0], year.Substring(2, 2), month, number.ToString("000"));
        }

        public static string GenerateOrderCodeSave(string year, string month, int OrdetypeId)
        {
            var number = 1;
            string codestr = "";


            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                codestr = entities.OrderTypes.Where(q => q.Id == OrdetypeId).OrderBy(q => q.Id).ToList().FirstOrDefault().Name;

                var qordersInMonth = entities.Orders.Where(q => q.Month == month && q.Year == year && q.OrderTypeId == OrdetypeId).OrderBy(q => q.Id).ToList();
                if (qordersInMonth.Count > 0)
                {
                    number = qordersInMonth.Last().CodeNumber.Value + 1;
                }
            }

            return string.Format("{0}-{1}-{2}-{3}", codestr.Split('-')[0], year.Substring(2, 2), month, number.ToString("000"));
        }
        public static string GenerateInvoiceCodeSave(Order order)
        {
            var number = 1;
            string Typestr = "";
            var utcTime = DateTime.Now.ToUniversalTime();
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));
            DateTime april1st = new DateTime(DateTime.Now.Year, 4, 1);
            bool isApril1st = today.Date == april1st;
            var Year = (today.Month < 4) ? (today.Year - 1) : today.Year;

            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {

                Typestr = entities.OrderTypes.Where(q => q.Id == order.OrderTypeId).OrderBy(q => q.Id).ToList().FirstOrDefault().Name;
                

            
                if (isApril1st)
                {
                    var AprilInvoices = entities.Invoices
                        .Where(q => q.CreatedDate.HasValue && q.CreatedDate.Value.Date == april1st && q.Order.OrderTypeId == order.OrderTypeId)
                        .OrderBy(q => q.Id).ToList();


                    if (!(AprilInvoices.Count > 0))
                    {
                        return string.Format("{0}-{1}-{2}", Typestr.Split('-')[0], Year.ToString().Substring(2, 2), number.ToString("000"));
                    }

                    number = AprilInvoices.Last().CodeNumber.Value + 1;
                    return string.Format("{0}-{1}-{2}", Typestr.Split('-')[0], Year.ToString().Substring(2, 2), number.ToString("000"));
                }
                var Invoices = entities.Invoices.Where(q => q.Order.OrderTypeId == order.OrderTypeId).OrderBy(q => q.Id).ToList();

                if (Invoices.Count > 0)
                {
                    number = Invoices.Last().CodeNumber.Value + 1;
                }




            }

            return string.Format("{0}-{1}-{2}", Typestr.Split('-')[0], Year.ToString().Substring(2, 2), number.ToString("000"));
        }

        //public static List<GetDataAuditByUser_Result> GetDataAuditByUser(Guid userId)
        //{
        //    using (WorkFlowEntities entities = new WorkFlowEntities())
        //    {
        //        return entities.GetDataAuditByUser(userId).OrderByDescending(o => o.UpdatedOn).ToList();
        //    }
        //}

        //public static List<GetLoginAuditByUser_Result> GetLoginAuditByUser(Guid userId)
        //{
        //    using (WorkFlowEntities entities = new WorkFlowEntities())
        //    {
        //        return entities.GetLoginAuditByUser(userId).OrderByDescending(o => o.DateLogged).ToList();
        //    }
        //}
    }
}