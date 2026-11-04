using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OrgFlow.Data;
using OrgFlow.Dto;
using OrgFlow.Entities;
using static System.Net.WebRequestMethods;



namespace OrgFlow.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto employeeCreateDto)
        {
            //Sicil numarası daha önce kullanılmış mı?
            bool exists = await _context.Employees.AnyAsync(e => e.RegistrationNumber == employeeCreateDto.RegistrationNumber);
            if (exists)
                throw new InvalidOperationException("Bu sicil numarası zaten kayıtlı");

            bool managerExists = false;
            if (employeeCreateDto.ManagerId.HasValue)
            {
                // değişkeni burada tanımladığımmız için if yapısını parantez içine almamız gerekir

                managerExists = await _context.Employees.AnyAsync(e => e.Id == employeeCreateDto.ManagerId.Value);  //bu linq sorgusuyla ID'nin ManagerID'ye eşit olup olmadığı ölçülür 

                if (!managerExists)
                    throw new KeyNotFoundException(" Belirtilen Yönetici Bulunamadı");

            }
            var employee = new Employee
            {
                FirstName = employeeCreateDto.FirstName,
                LastName = employeeCreateDto.LastName,
                RegistrationNumber = employeeCreateDto.RegistrationNumber,
                ManagerId = employeeCreateDto.ManagerId
            };
            //Çalışanı EF CORE'A ekle
            await _context.Employees.AddAsync(employee);
            //Değişiklikleri veritabanına kaydet.
            await _context.SaveChangesAsync();
            return new EmployeeResponseDto
            {
                Id = employee.Id, //EmployeeResponseDto propertyleri employee bilgilerini aktarılıp ResponseDto ile döner
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                RegistrationNumber = employee.RegistrationNumber,
                ManagerId = employee.ManagerId
            };

        }







        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            //Veritabanındaki çalışanları getir
            return await _context.Employees.AsNoTracking().Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                RegistrationNumber = e.RegistrationNumber,
                ManagerId = e.ManagerId


            }).ToListAsync();
        }


        public async Task<EmployeeResponseDto> GetByIdAsync(int id)
        {
            if (id <= 0) // id 0'dan küçük olamaz
                throw new ArgumentOutOfRangeException(nameof(id), "ID geçersiz");
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                throw new KeyNotFoundException("Çalışan bulunamadı");
            return new EmployeeResponseDto
            {
                Id = employee.Id,        //EmployeeResponseDto propertyleri employee bilgilerini aktarılıp ResponseDto ile döner         
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                RegistrationNumber = employee.RegistrationNumber,
                ManagerId = employee.ManagerId

            };
        }

        public async Task<List<EmployeeHierarchyDto>> GetHierarchyAsync() //Bu kısım henüz kafamda oturmadı yarın incelenilecek
        {
            var employees = await _context.Employees.AsNoTracking().Select(e
                => new EmployeeHierarchyDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    ManagerId = e.ManagerId,





                }

                ).ToListAsync();

            var dictionary = employees.ToDictionary(e => e.Id); //İşçileri anahtara dönüştürüyoruz

            var roots = new List<EmployeeHierarchyDto>(); //Ana yöneticileri tutacak liste

            // Çalışanın bir yöneticisi tanımlı mı(ManagerId null değil mi) diye bakıyoruz.
            // Tanımlıysa, bu Id'ye sahip yöneticinin dictionary'de bulunup bulunmadığını TryGetValue ile kontrol ediyoruz.
            // TryGetValue anahtar varsa true döner ve bulunan yöneticiyi "manager" değişkenine atar;
            // anahtar yoksa hata fırlatmaz, false döner ve manager null olur.
            // && kısa devre yapar: ManagerId null ise ikinci koşul hiç çalışmaz, bu yüzden .Value güvenle kullanılır.
            // Kısacası: "Yöneticisi var VE listede bulundu" ise if bloğuna girilir, çalışan yöneticinin altına eklenir.
            // Aksi halde (yöneticisi yok ya da bulunamadı) else bloğuna düşülür ve çalışan köke eklenir.

            foreach (var employee in employees)
            {


                if (employee.ManagerId.HasValue && dictionary.TryGetValue(employee.ManagerId.Value, out var manager))

                {

                    manager.Subordinates.Add(employee);
                }

                else
                    roots.Add(employee); // Yönetici olmayan işçileri kök listeye ekliyoruz    // Yöneticisi olmayan veya yöneticisi bulunamayan çalışanları köke ekliyoruz

            }





            return roots;

        }

        public async Task<EmployeeResponseDto> UpdateAsync(int employeeId, EmployeeUpdateDto employeeUpdateDto)
        {
            if (employeeId <= 0) // id 0'dan küçük mü diye kontrol ediyoruz
                throw new ArgumentOutOfRangeException(nameof(employeeId), "ID geçersiz"); // ID geçersizse hata fırlatıyoruz
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId); // ID'ye göre işçiyi buluyoruz

            if (employee == null) // işçi bulunamazsa hata fırlatıyoruz
                throw new KeyNotFoundException("İşçi bulunamadı");

            if(await _context.Employees.AnyAsync(e => e.RegistrationNumber == employeeUpdateDto.RegistrationNumber && e.Id != employeeId))  
                throw new InvalidOperationException("Sicil numarası zaten kullanılıyor");

            // ---- YENİ: Yönetici kontrolleri ----
            if (employeeUpdateDto.ManagerId.HasValue) // yönetici belirtildiyse kontrol et, null ise "yöneticisi yok" demektir
            {
                // 1) Çalışan kendi kendisinin yöneticisi olamaz
                if (employeeUpdateDto.ManagerId.Value == employeeId)
                    throw new InvalidOperationException("Çalışan kendi yöneticisi olamaz");

                // 2) Belirtilen yönetici veritabanında var mı?
                bool managerExists = await _context.Employees.AnyAsync(e => e.Id == employeeUpdateDto.ManagerId.Value);
                if (!managerExists)
                    throw new KeyNotFoundException("Belirtilen yönetici bulunamadı");
            }
            // ------------------------------------

            employee.FirstName = employeeUpdateDto.FirstName; // işçinin adını güncelliyoruz
            employee.LastName = employeeUpdateDto.LastName;// işçinin soyadını güncelliyoruz
            employee.RegistrationNumber = employeeUpdateDto.RegistrationNumber; // işçinin sicil numarasını güncelliyoruz
            employee.ManagerId = employeeUpdateDto.ManagerId; // işçinin yöneticisinin Id'sini güncelliyoruz    
                                                              // AsNoTracking() kullanmadığımız için EF Core, employee nesnesini izliyor ve değişiklikleri fark ediyor. Bu yüzden Update metodunu çağırmamıza gerek yok.
                                                              // 
            await _context.SaveChangesAsync(); // değişiklikleri veritabanına kaydediyoruz



            return new EmployeeResponseDto
            {
                Id = employee.Id,
                FirstName = employeeUpdateDto.FirstName, // işçinin adını güncelledikten sonra geri döndürüyoruz
                LastName = employeeUpdateDto.LastName, // işçinin soyadını güncelledikten sonra geri döndürüyoruz
                RegistrationNumber = employeeUpdateDto.RegistrationNumber, // işçinin sicil numarasını güncelledikten sonra geri döndürüyoruz
                ManagerId = employeeUpdateDto.ManagerId // işçinin yöneticisinin Id'sini güncelledikten sonra geri döndürüyoruz 
            };


        }


        public async Task DeleteAsync(int employeeId)
        {
            if (employeeId <= 0) // id 0'dan küçük mü diye kontrol ediyoruz
                throw new  ArgumentOutOfRangeException(nameof(employeeId), "ID geçersiz"); // ID geçersizse hata fırlatıyoruz
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId); // ID'ye göre işçiyi buluyoruz
            if (employee == null) // işçi bulunamazsa hata fırlatıyoruz
                throw new KeyNotFoundException("İşçi bulunamadı");
            if (await _context.Employees.AnyAsync(e => e.ManagerId == employeeId))
                throw new InvalidOperationException("Bu çalışanın astları var, önce onları başka yöneticiye atayın");

            _context.Employees.Remove(employee); // işçiyi silmek için Remove metodunu kullanıyoruz
            await _context.SaveChangesAsync(); // değişiklikleri veritabanına kaydediyoruz
        }
    }


}
