﻿using System;
using System.Collections.Generic;
using System.Linq;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class DivisionService
    {
        public List<Division> GetDivisionList()
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Divisions.Where(d => d.IsActive == true).OrderBy(d => d.Name).ToList();
            }
        }

        public Division GetDivisionById(int? id)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                return entities.Divisions.Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Division division)
        {
            using (STWFMEntities entities = new STWFMEntities())
            {
                if (division.Id == 0)
                {
                    entities.Divisions.Add(division);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(division).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
