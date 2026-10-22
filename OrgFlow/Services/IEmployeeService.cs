using OrgFlow.Dto;

namespace OrgFlow.Services
{
    public interface IEmployeeService
    {
        //Tüm çalışanları listeler
        Task<List<EmployeeResponseDto>> GetAllAsync();

        //ID değerine göre çalışan getirir
        Task<EmployeeResponseDto> GetByIdAsync(int id);

        Task<EmployeeCreateDto> CreateAsync(EmployeeCreateDto employeeCreateDto);
        Task<EmployeeUpdateDto> UpdateAsync(int employeId,EmployeeUpdateDto employeUpdateDto);

        Task<List<EmployeeHierarchyDto>> GetHierarchyAsync();


    }
}
