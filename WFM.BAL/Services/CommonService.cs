using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public static class CommonService
    {
        public static int SaveLoginAudit(LoginAudit loginAudit)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                entities.LoginAudits.Add(loginAudit);
                entities.SaveChanges();
            }
            return 1;
        }

        public static int SaveDataAudit(DataAudit dataAudit)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                entities.DataAudits.Add(dataAudit);
                entities.SaveChanges();
            }
            return 1;
        }

        public static string GenerateQuoteCode(string year, string month, bool isVAT)
        {
            var number = 1;

            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
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

            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                var qordersInMonth = entities.Orders.Where(q => q.Month == month && q.Year == year).OrderBy(q => q.Id).ToList();
                if (qordersInMonth.Count > 0)
                {
                    number = qordersInMonth.Last().CodeNumber.Value + 1;
                }
            }

            return string.Format("{0}/{1}/{2}/{3}", QuoteCode.Split('/')[0], year.Substring(2, 2), month, number.ToString("000"));
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