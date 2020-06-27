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
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Printers.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Printer GetPrinterById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Printers.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Printer printer)
        {
            using (STWFMEntities entities = new STWFMEntities())
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
