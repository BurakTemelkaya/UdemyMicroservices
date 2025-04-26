using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog;

public class CourseCreateInput
{
    [Display(Name ="Kurs İsmi")]
    public string Name { get; set; }

    [Display(Name = "Kurs Açıklaması")]
    public string Description { get; set; }

    [Display(Name = "Kurs Fiyatı")]
    public decimal Price { get; set; }
    public string? Picture { get; set; }
    public string? UserId { get; set; }
    public FeatureViewModel Feature { get; set; }

    [Display(Name = "Kurs Kategorisi")]
    public string CategoryId { get; set; }

    [Display(Name = "Resim")]
    public IFormFile PhotoFormFile { get; set; }
}
