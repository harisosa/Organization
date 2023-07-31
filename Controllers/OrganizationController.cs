using organization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public OrganizationController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public ActionResult<ResponseBody<List<Employee>>> Get()
    {
        var response = new ResponseBody<List<Employee>>();
        response.Data = _employeeService.GetTopLevelManagers();
        response.Code=StatusCodes.Status200OK;
        response.Message="";
                    
        return Ok(response);
    }
        [HttpGet("{id}")]
    public ActionResult<ResponseBody<Employee>> Get(int id)
    {
        Employee employee = _employeeService.GetById(id,false);
        var response = new ResponseBody<Employee>();
        if (employee == null)
        {
            response.Code=StatusCodes.Status400BadRequest;
            response.Message = "Employee Id not found";
            return BadRequest(response);
        }
        return Ok(employee);
    }

    [HttpGet("{id}/includeReportingTree={includeReportingTree}")]
    public ActionResult<ResponseBody<Employee>> GetIncludeReportTree(int id, bool includeReportingTree)
    {
        Employee employee = _employeeService.GetById(id,includeReportingTree);
        var response = new ResponseBody<Employee>();
        if (employee == null)
        {
            response.Code=StatusCodes.Status400BadRequest;
            response.Message = "Employee Id not found";
            return BadRequest(response);
        }
        response.Data = employee;
        response.Code=StatusCodes.Status200OK;
        response.Message="";
        return Ok(response);
    }
    [HttpPut("{id}")]
    public ActionResult<ResponseBody<Employee>> Put(int id, [FromBody] RequestBody updatedEmployee)
    {
        var existingEmployee = _employeeService.GetById(id,true);
        var response = new ResponseBody<Employee>();
        if (existingEmployee == null)
        {
            response.Code=StatusCodes.Status400BadRequest;
            response.Message = "Employee Id not found";
            return NotFound(response);
        }

        response.Data = _employeeService.Update(id, updatedEmployee.Name, updatedEmployee.ManagerId);
        response.Code=StatusCodes.Status200OK;
        return Ok(response);
    }
    [HttpPost]
    public ActionResult<ResponseBody<Employee>> Post([FromBody] RequestBody newEmployee)
    {
         var response = new ResponseBody<Employee>();
        if (string.IsNullOrEmpty(newEmployee.Name))
        {
            response.Code=StatusCodes.Status400BadRequest;
            response.Message = "Name is required";
            return BadRequest(response);
        }
        Employee newEmp = new Employee();
        newEmp.Name = newEmployee.Name;
        newEmp.ManagerId = newEmployee.ManagerId;

        var result = _employeeService.Add(newEmp);
        response.Message = "Sucessfully add Employee!";
        response.Code=StatusCodes.Status200OK;
        return Ok(response);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
       
        var (message,isSucess) = _employeeService.DeleteEmployee(id);
        var response = new ResponseBody<Employee>();
        response.Message = message;
        if(isSucess){
            response.Code=StatusCodes.Status200OK;
           return Ok(response); 
        }else{
            response.Code=StatusCodes.Status400BadRequest;
           return BadRequest(response); 
        }
        
    }
}