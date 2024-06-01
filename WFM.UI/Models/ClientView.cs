using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class ClientView
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string CPTitle { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Contact Person")]
        public string CPName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Mobile")]
        public string CPMobile { get; set; }

        [Display(Name="E-mail")]
        public string Email { get; set; }

        public string FixedLine { get; set; }

        public string LandLine { get; set; }

        public string DesignationName { get; set; }

        public string VATNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public int CPDesignationId { get; set; }

        public Designation Designation { get; set; }
    }
}