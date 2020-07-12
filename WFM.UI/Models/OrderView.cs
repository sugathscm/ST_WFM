using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class OrderView : Order
    {
        public string ClientName { get; set; }
        public string StatusName { get; set; }
    }
}