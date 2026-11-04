using OrgFlow.Dto;

namespace OrgFlow.Services
{
    public interface IEmployeeService
    {
        //Tüm çalışanları listeler
        Task<List<EmployeeResponseDto>> GetAllAsync();

        //ID değerine göre çalışan getirir
        Task<EmployeeResponseDto> GetByIdAsync(int id);

        Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto employeeCreateDto);
        Task<EmployeeResponseDto> UpdateAsync(int employeeId, EmployeeUpdateDto employeeUpdateDto);

        Task<List<EmployeeHierarchyDto>> GetHierarchyAsync();

        Task DeleteAsync(int employeeId);



    }
}
