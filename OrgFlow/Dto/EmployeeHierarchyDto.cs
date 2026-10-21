namespace OrgFlow.Dto
{
    public class EmployeeHierarchyDto
    {
        // Verileri biz oluşturmadığımız için required attribute a ihtiyacımız yok

        public int Id { get; set; }
<<<<<<< HEAD
        public int? ManagerId { get; set; }

        public  int ManagerName {  get; set; }
=======
>>>>>>> 0c0aa762bd907aab000b4074eec05cfcdaf4b6a2

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public List<EmployeeHierarchyDto> Subordinates { get; set; } = new();

    }
}
