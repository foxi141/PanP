using System.ComponentModel.DataAnnotations;
namespace KalkulatorPaliwka.Models;
public class User
{
    [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
    public string username { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
    public string email { get; set; }

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
    public string passwordhash { get; set; }

    public string userid { get; set; }
    public DateTime createdat { get; set; }

    public string avatarpath { get; set; } = "/images/avatar.png";
}
