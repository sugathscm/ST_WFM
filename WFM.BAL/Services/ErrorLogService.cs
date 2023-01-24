using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class ErrorLogService
    {
        public void SaveOrUpdate(ErrorLog erorlog)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                
                    entities.ErrorLogs.Add(erorlog);
                    entities.SaveChanges();
                
               
            }
        }
    }
}
