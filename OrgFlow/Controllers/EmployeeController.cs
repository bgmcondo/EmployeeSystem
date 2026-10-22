using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrgFlow.Dto;
using OrgFlow.Services;

namespace OrgFlow.Controllers

{
    [ApiController] //Bu sınıfın API controller davranışını etkinleştirmek için kullanılır. Bu attribute, model doğrulama hatalarını otomatik olarak döndürür ve HTTP yanıtlarını uygun şekilde yönetir.

    [Route("api/[controller]")] // Bu attribute, controller'ın hangi URL yoluna yanıt vereceğini belirtir. [controller] yer tutucu, controller sınıfının adını (Employee) temsil eder ve bu sayede URL yolu dinamik olarak oluşturulur. Örneğin, bu controller için URL yolu "api/employee" olacaktır.
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService; //IEmployeeService interface'ini kullanarak EmployeeService sınıfına erişim sağlanır.

        public EmployeeController(IEmployeeService employeeService) //Dependency Injection sistemine service sınıfı enjekte edilir. Bu sayede controller sınıfı, service sınıfının metodlarını kullanabilir.
        {
            _employeeService = employeeService;
        }

        [HttpGet] //Get isteklerini işlemek için kullanılır. Bu attribute, bu metodun HTTP GET isteğine yanıt vereceğini belirtir.

        public async Task<IActionResult> getAllEmployees() //IActionResult, HTTP yanıtlarını temsil eden bir türdür. Bu sayede metod, farklı HTTP durum kodları ve yanıt türleri döndürebilir. Task<IActionResult> ise metodun asenkron olduğunu ve IActionResult türünde bir sonuç döndüreceğini belirtir.
        {
            var employees = await _employeeService.GetAllAsync(); //EmployeeService sınıfının GetAllAsync metodunu çağırarak tüm çalışanları alır.
            return Ok(employees); //HTTP 200 OK yanıtı ile birlikte çalışanları döndürür.   
        }

        [HttpGet("{id}")] //Get isteklerini işlemek için kullanılır. Bu attribute, bu metodun HTTP GET isteğine yanıt vereceğini belirtir. {id} parametresi, URL yolunda bir değişken olarak tanımlanır ve metodun parametresi olarak kullanılır.

        public async Task<IActionResult> GetEmployeeById(int id) //IActionResult, HTTP yanıtlarını temsil eden bir türdür. Bu sayede metod, farklı HTTP durum kodları ve yanıt türleri döndürebilir. Task<IActionResult> ise metodun asenkron olduğunu ve IActionResult türünde bir sonuç döndüreceğini belirtir.
        {
            var employee = await _employeeService.GetByIdAsync(id); //EmployeeService sınıfının GetByIdAsync metodunu çağırarak id parametresine göre çalışanı alır.
            if (employee == null) //Eğer çalışan bulunamazsa
                return NotFound(); //HTTP 404 Not Found yanıtı döndürür.
            return Ok(employee); //HTTP 200 OK yanıtı ile birlikte çalışanı döndürür
        }

        [HttpGet("hierarchy")] //Get isteklerini işlemek için kullanılır. Bu attribute, bu metodun HTTP GET isteğine yanıt vereceğini belirtir. "hierarchy" parametresi, URL yolunda bir değişken olarak tanımlanır ve metodun parametresi olarak kullanılır.)]

        public async Task<IActionResult> GetEmployeeHierarchy() //IActionResult, HTTP yanıtlarını temsil eden bir türdür. Bu sayede metod, farklı HTTP durum kodları ve yanıt türleri döndürebilir. Task<IActionResult> ise metodun asenkron olduğunu ve IActionResult türünde bir sonuç döndüreceğini belirtir.
        {
            var hierarchy = await _employeeService.GetHierarchyAsync(); //EmployeeService sınıfının GetHierarchyAsync metodunu çağırarak çalışanların hiyerarşisini alır.
            return Ok(hierarchy); //HTTP 200 OK yanıtı ile birlikte çalışanların hiyerarşisini döndürür
        }

        [HttpPost] //Post isteklerini işlemek için kullanılır. Bu attribute, bu metodun HTTP POST isteğine yanıt vereceğini belirtir.
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto employeeDto)
        {

           var employee = await _employeeService.CreateAsync(employeeDto); //EmployeeService sınıfının CreateAsync metodunu çağırarak yeni bir çalışan oluşturur.
            return Ok(employee);
        }

        [HttpPut("{id}")] //Put isteklerini işlemek için kullanılır. Bu attribute, bu metodun HTTP PUT isteğine yanıt vereceğini belirtir. {id} parametresi, URL yolunda bir değişken olarak tanımlanır ve metodun parametresi olarak kullanılır.
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto employeeDto) //IActionResult, HTTP yanıtlarını temsil eden bir türdür. Bu sayede metod, farklı HTTP durum kodları ve yanıt türleri döndürebilir. Task<IActionResult> ise metodun asenkron olduğunu ve IActionResult türünde bir sonuç döndüreceğini belirtir.
        {
            var employee = await _employeeService.UpdateAsync(id, employeeDto); //EmployeeService sınıfının UpdateAsync metodunu çağırarak id parametresine göre çalışanı günceller.
            return Ok(employee);
        }
    }
}