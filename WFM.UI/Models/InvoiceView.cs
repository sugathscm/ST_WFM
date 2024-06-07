using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WFM.BAL.Services;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class InvoiceView : Invoice
    {

        private InvoiceService invoiceService;

        public InvoiceView(InvoiceService _invoiceService)
        {
            invoiceService = _invoiceService;
        }
        public string ClientName { get; set; }
        public string Channelledby { get; set; }
        public string ClientAddress { get; set; }
        public string ClientAddress2 { get; set; }
        public string CreatedDateString { get; set; }

        public string OrderCode { get; set; }
        public string OrderType { get; set; }
        public string Location { get; set; }

        public string VatNo { get; set; }
        public string ClientSVatNo { get; set; }
        public string ClientPoNo { get; set; }
        public string ClientVatNo { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<AdditionalCharge> additionalCharges { get; set; }

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double vatAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double orderTotal { get; set;}
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double TotalWithVat { get; set;}
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double AdvancePayment { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public double BalancePayment { get; set; }

        public bool isVat { get; set; }





    }
}