// entity ve DTO 

using System.ComponentModel.DataAnnotations;

namespace OrgFlow.Dto
{
    public class EmployeeCreateDto
    {
        [Required(ErrorMessage = "Ad zorunludur.")] //Bu alanlara attribute deniliyor
        public string FirstName { get; set; } = null!;

        
        [Required(ErrorMessage ="Soyadı zorunludur")]

        public string LastName { get; set; } = null!;

        [Required(ErrorMessage ="Sicil numarası zorunludur")]
        [RegularExpression(@"^[a-zA-Z0-9]{11}$", ErrorMessage = "Sicil numarası 11 karakter ve alfanümerik olmalıdır.")]

        public string RegistrationNumber { get; set; } = null!;

        public int? ManagerId { get; set; }
    }
}
