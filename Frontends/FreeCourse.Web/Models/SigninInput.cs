using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models;

public class SigninInput
{
    [Required(ErrorMessage = "Email Adresiniz Boş Geçilemez")]
    [Display(Name ="Email Adresiniz")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Parolanız Boş Geçilemez")]
    [Display(Name = "Parolanız")]
    public string Password { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool IsRemember { get; set; }
}
