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
           return await _context.Employees.AsNoTracking().Select(e=>new EmployeeResponseDto {
           Id= e.Id,
           FirstName = e.FirstName,
           LastName= e.LastName,
           RegistrationNumber= e.RegistrationNumber,
           ManagerId= e.ManagerId
           
           
           }).ToListAsync();
        }

        public Task<EmployeeResponseDto> GetByIdAsync(int id)
        {
           
        }

        public Task<List<EmployeeHierarchyDto>> GetHierarchyAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmployeUpdateDto> UpdateAsync(int employeId, EmployeUpdateDto employeUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
