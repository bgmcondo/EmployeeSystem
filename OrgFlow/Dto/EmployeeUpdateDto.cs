using System.ComponentModel.DataAnnotations;

namespace OrgFlow.Dto
{
    public class EmployeeUpdateDto
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Sicil numarası zorunludur.")]
        [RegularExpression(
            @"^[a-zA-Z0-9]{11}$",
            ErrorMessage = "Sicil numarası 11 karakter ve alfanümerik olmalıdır.")]
        public string RegistrationNumber { get; set; } = null!;

        public int? ManagerId { get; set; } //Employee en üst manager da olabilir 

    }
}
