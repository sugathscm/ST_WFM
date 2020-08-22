using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class QuoteView : Quote
    {
        public string ClientName { get; set; }
        public string CreatedDateString { get; set; }

        public string CodeFormatted { get; set; }
    }
}