namespace OrgFlow.Dto
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string RegistrationNumber { get; set; } = null!;

        public int? ManagerId { get; set; } //En üst yöneticinin bir yöneticisi olamaz o yüzden nullable yapılır

        public string? ManagerName { get; set; } //En üst yöneticinin bir yöneticisi olamaz o yüzden nullable yapılır
    }
}
