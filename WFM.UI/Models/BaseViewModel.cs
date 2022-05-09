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

    public class DocumentViewModel
    {
        public string DocumentId { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }

        public string DocumentPath { get; set; }
        public string Name { get; set; }
    }


}