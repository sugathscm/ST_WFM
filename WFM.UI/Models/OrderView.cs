using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class OrderView : Order
    {
        public string ClientName { get; set; }
        public string StatusName { get; set; }
        public string CreatedDateString { get; set; }
        public DateTime Delivery { get; set; }
        public string Month { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryDateString { get; set; }
        public List<OrderItem> Items { get; set; }
        public string BaseQoute { get; set; }
        public double  VAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double OrderTotal { get; set; }
        public int OrderCount { get; set; } 
        public int OrderTypeId { get; set; }
        public string ChanneledBy{ get; set; }

        public double BalancePayment { get; set; }



    }
  
}