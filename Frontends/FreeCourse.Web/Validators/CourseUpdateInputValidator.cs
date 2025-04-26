using FluentValidation;
using FreeCourse.Web.Models.Catalog;

namespace FreeCourse.Web.Validators;

public class CourseUpdateInputValidator:AbstractValidator<CourseUpdateInput>
{
    public CourseUpdateInputValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kategori alanı seçiniz.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Kurs ismi boş olamaz.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Kurs açıklaması boş olamaz.");
        RuleFor(x => x.Feature.Duration).InclusiveBetween(1, int.MaxValue).WithMessage("Süre alanı boş olamaz.");
        RuleFor(x => x.Price).NotEmpty().WithMessage("Kurs fiyatı boş olamaz.").PrecisionScale(6, 2, true).WithMessage("Hatalı para formatı.");
    }
}
