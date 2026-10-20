
namespace OrgFlow.Entities
{
    /// <summary>
    /// Sistemdeki çalışanları ve organizasyon hiyerarşisini temsil eder.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Çalışanın benzersiz kimlik numarasıdır (Primary Key).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Çalışanın adını belirtir. Zorunlu alandır.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Çalışanın soyadını belirtir. Zorunlu alandır.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Çalışana ait benzersiz sicil numarasıdır.
        /// Tam 11 karakter uzunluğunda ve alfanümerik olmalıdır.
        /// </summary>
        public string RegistrationNumber { get; set; } = null!;

        /// <summary>
        /// Çalışanın doğrudan yöneticisinin kimlik numarasıdır (Foreign Key).
        /// Nullable tanımlanmıştır; çünkü en üst düzey yöneticinin
        /// herhangi bir yöneticisi bulunmayabilir.
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// Çalışanın bağlı olduğu yöneticiyi temsil eden
        /// navigation property'dir.
        /// Employee tablosuyla öz-referanslı ilişki kurar.
        /// </summary>
        public Employee? Manager { get; set; }

        /// <summary>
        /// Çalışana doğrudan bağlı olan ast çalışanların koleksiyonudur.
        /// Bir yöneticinin birden fazla astı olabilir.
        /// One-to-Many (bire-çok) ilişkisinin çok tarafını temsil eder.
        /// </summary>
        public ICollection<Employee> Subordinates { get; set; }
            = new List<Employee>();
    }
}
