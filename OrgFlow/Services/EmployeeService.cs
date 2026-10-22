using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OrgFlow.Data;
using OrgFlow.Dto;
using OrgFlow.Entities;



namespace OrgFlow.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EmployeeCreateDto> CreateAsync(EmployeeCreateDto employeeCreateDto)
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
                    throw new InvalidOperationException(" Belirtilen Yönetici Bulunamadı");

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
            return new EmployeeCreateDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                RegistrationNumber = employee.RegistrationNumber,
                ManagerId = employee.ManagerId
            };

        }







        public async Task<List<EmployeeResponseDto>> GetAllAsync()
        {
            //Veritabanındaki çalışanları getir
            return await _context.Employees.AsNoTracking().Select(e => new EmployeeResponseDto {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                RegistrationNumber = e.RegistrationNumber,
                ManagerId = e.ManagerId


            }).ToListAsync();
        }


        public async Task<EmployeeResponseDto> GetByIdAsync(int id)
        {
            if (id < 0) // id 0'dan küçük olamaz
                throw new ArgumentOutOfRangeException("id geçersiz");
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            return new EmployeeResponseDto
            {
                Id = employee.Id,        //EmployeeResponseDto propertyleri employee bilgilerini aktarılıp ResponseDto ile döner         
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                RegistrationNumber = employee.RegistrationNumber

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

            foreach (var employee in employees)
            {
                if (employee.ManagerId.HasValue)// İşçinin yönetici var mı yok mu kontrol ediyoruz
                {
                    int managerId = employee.ManagerId.Value;// işçinin yöneticisinin Id'sini alıyoruz
                    var manager = dictionary.TryGetValue(managerId, out var m) ? m : null; //Yönetici varsa onu alıyoruz yoksa null döndürüyoruz  
                    if (manager != null) // Yönetici varsa işçiyi onun altına ekliyoruz
                    {
                        manager.Subordinates.Add(employee);
                    }
                }

                roots.Add(employee); // Yönetici olmayan işçileri kök listeye ekliyoruz
            }



            _context.SaveChangesAsync();

            return roots;

        }

        public async Task<EmployeeUpdateDto> UpdateAsync(int employeId, EmployeeUpdateDto employeUpdateDto)
        {
            if (employeId < 0) // id 0'dan küçük mü diye kontrol ediyoruz
                throw new InvalidOperationException("ID geçersiz"); // ID geçersizse hata fırlatıyoruz
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeId); // ID'ye göre işçiyi buluyoruz

            if (employee == null) // işçi bulunamazsa hata fırlatıyoruz
                throw new InvalidOperationException("İşçi");

            employee.FirstName = employeUpdateDto.FirstName; // işçinin adını güncelliyoruz
            employee.LastName = employeUpdateDto.LastName;// işçinin soyadını güncelliyoruz
            employee.RegistrationNumber = employeUpdateDto.RegistrationNumber; // işçinin sicil numarasını güncelliyoruz

            _context.Employees.Update(employee);
            // Entity'yi EF Core üzerinde Modified olarak işaretler.
            // Veritabanına henüz UPDATE sorgusu göndermez.
            // Sadece bellekte işlem yaptığı için asenkron değildir, await kullanılmaz.

            await _context.SaveChangesAsync();
            //Veritabanına değişiklikleri kaydediyoruz
            //İşlem sürerken metot askıya alınabilir,thread serbest kalabilir.
            //İşlem sürerken metot askıya alınır ve diğer işlemler devam eder. await ile metot tamamlanana kadar bekleriz.


            return new EmployeeUpdateDto
            {
                FirstName = employeUpdateDto.FirstName, // işçinin adını güncelledikten sonra geri döndürüyoruz
                LastName = employeUpdateDto.LastName, // işçinin soyadını güncelledikten sonra geri döndürüyoruz
                RegistrationNumber = employeUpdateDto.RegistrationNumber // işçinin sicil numarasını güncelledikten sonra geri döndürüyoruz


            };


        }
    } }
