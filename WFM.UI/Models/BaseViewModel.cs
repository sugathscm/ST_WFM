using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class BaseViewModel
    {
        public int Id { get; set; }

        //[RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "White space found in Name field.")]
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }

    public class CategoryViewModel : BaseViewModel
    {
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Category Type")]
        public string CategoryType { get; set; }
    }
    public class SizeViewModel : BaseViewModel
    {
        [Display(Name = "Size")]
        public string Size { get; set; }
      
    }
    public class SupplierViewModel : BaseViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "ContactNo")]
        public string ContactNo { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        public string SupplierType { get; set; }

    }
    public class FasiaViewModel : BaseViewModel
    {
        [Display(Name = "Fasia")]
        public string Fasia { get; set; }

    }
    public class SupplierTypeViewModel : BaseViewModel
    {
        [Display(Name = "Supplier Type")]
        public string Type { get; set; }

    }
    public class MaterialViewModel : BaseViewModel
    {
        [Display(Name = "Material")]
        public string Material { get; set; }

    }
    public class FrameworkViewModel : BaseViewModel
    {
        [Display(Name = "Framework")]
        public string Framework { get; set; }

    }
    public class LetteringViewModel : BaseViewModel
    {
        [Display(Name = "Lettering")]
        public string Name { get; set; }
    }
    public class VisibilityViewModel : BaseViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
    public class IlluminationViewModel : BaseViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
    public class WarrantyPeriodViewModel : BaseViewModel
    {
        [Display(Name = "Duration")]
        public string Duration { get; set; }
    }
    public class DeliveryTypeViewModel : BaseViewModel
    {
        [Display(Name = "Type")]
        public string Type { get; set; }
    }

    public class DocumentViewModel
    {
        public string DocumentId { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }

        public string DocumentPath { get; set; }
        public string Name { get; set; }
    }


}