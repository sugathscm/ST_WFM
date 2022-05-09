using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFM.BAL.Enums
{
    public enum OrderStatus
    {
        Not_Started = 1,
        In_Progress,
        Ready_To_Invoice,
        Completed
    }
    public enum Visibility
    {
        [Display(Name = "Single Sided")]
        SingleSided = 1,
        [Display(Name = "Double Sided")]
        DoubleSided
    }


}
