using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog;

public class FeatureViewModel
{
    [Display(Name ="Kurs Süresi")]
    [Required]
    public int Duration { get; set; }
}
