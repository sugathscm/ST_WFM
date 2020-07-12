using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WFM.DAL;

namespace WFM.UI.Models
{
    public class EmployeeView
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Mobile { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string FixedLine { get; set; }
        public string DesignationName { get; set; }
        public string DivisionName { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        public int DesignationId { get; set; }
        public Designation Designation { get; set; }
    }
}