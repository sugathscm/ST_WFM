using System.ComponentModel.DataAnnotations;
using System.Web;

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
    public class FasiaViewModel : BaseViewModel
    {
        [Display(Name = "Fasia")]
        public string Fasia { get; set; }

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