namespace OrgFlow.Dto
{
    public class EmployeeHierarchyDto
    {
        // Verileri biz oluşturmadığımız için required attribute a ihtiyacımız yok

        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public List<EmployeeHierarchyDto> Subordinates { get; set; } = new();

    }
}
