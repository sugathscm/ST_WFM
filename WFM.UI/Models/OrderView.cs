using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WFM.BAL.Services;
using WFM.DAL;
using WFM.UI.Models;

namespace WFM.UI.Models
{
    public class OrderView : Order
    {
        private OrderService orderService;

        public OrderView(OrderService _orderService)
        {
            orderService = _orderService;
        }

        public string ClientName { get; set; }
        public string StatusName { get; set; }
        public string CreatedDateString { get; set; }
        public DateTime Delivery { get; set; }
        public string Month { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryDateString { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<OrderAttachment> Attachments { get; set; }
        public string BaseQoute { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double VAT { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double OrderTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double TotalWithVat { get; set; }
        public int OrderCount { get; set; }
        public int OrderTypeId { get; set; }
        public string ChanneledBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}")]
        public double BalancePayment { get; set; }

        public string Illumination { get; set; }

        public string GetIllumination(int? Id = 7)
        {
            return orderService.GetIllumination(Id);
        }
        public string GetWarranty(int? Id = 4)
        {
            return orderService.GetWarranty(Id);
        }
        public string Getvisibility(int? Id = 3)
        {
            return orderService.Getvisibility(Id);
        }
        public string GetDelivery(int Id)
        {
            return orderService.GetDeliveryType(Id);
        }
      



    }

}