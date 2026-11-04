using Microsoft.AspNetCore.Mvc;
using OrgFlow.Dto;
using OrgFlow.Services;

namespace OrgFlow.Controllers
{
    [ApiController]              // Model doğrulama hatalarını otomatik 400 olarak döner
    [Route("api/[controller]")]  // URL: api/employee
    public class EmployeeController : ControllerBase // API için View desteğine gerek yok
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET api/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees); // 200 (bu metot exception fırlatmaz, try/catch gerekmez)
        }

        // GET api/employee/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                return Ok(employee); // 200
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message); // 400 - ID geçersiz
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 - çalışan yok
            }
        }

        // GET api/employee/hierarchy  ("hierarchy" sabit bir yol, değişken değil)
        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetEmployeeHierarchy()
        {
            var hierarchy = await _employeeService.GetHierarchyAsync();
            return Ok(hierarchy); // 200
        }

        // POST api/employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto employeeDto)
        {
            try
            {
                var employee = await _employeeService.CreateAsync(employeeDto);

                // 201 Created + Location header: api/employee/{yeniId}
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 - belirtilen yönetici yok
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 - sicil numarası zaten kayıtlı
            }
        }

        // PUT api/employee/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto employeeDto)
        {
            try
            {
                var employee = await _employeeService.UpdateAsync(id, employeeDto);
                return Ok(employee); // 200
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message); // 400 - ID geçersiz
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 - çalışan veya yönetici yok
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 - sicil numarası çakışması / kendi yöneticisi olamaz
            }
        }

        // DELETE api/employee/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message); // 400 - ID geçersiz
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 - çalışan yok
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 - astları var, silinemez
            }
        }
    }
}