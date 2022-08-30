using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class PrinterService
    {
        public List<Printer> GetPrinterList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Printers.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Printer GetPrinterById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Printers.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Printer printer)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (printer.Id == 0)
                {
                    entities.Printers.Add(printer);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(printer).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}