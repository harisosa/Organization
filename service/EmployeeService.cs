using organization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
public class EmployeeService : IEmployeeService
{
    public List<Employee> Employees { get; private set; }
    private Dictionary<int, Employee> _employeeById;
    public EmployeeService()
    {
        string? path = Path.Combine(AppContext.BaseDirectory, "assets", "organization-tree.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Could not find file at path: {path}");
        }

        string? data = File.ReadAllText(path);
        if (data == "")
        {
            throw new InvalidDataException($"Failed to deserialize data from file at path: {path}");
        }

        List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(data) ?? new List<Employee>();
        _employeeById = employees.ToDictionary(e => e.EmployeeId);
        foreach (Employee employee in employees)
        {
            if (employee.ManagerId.HasValue)
            {
                Employee manager = _employeeById[employee.ManagerId.Value];
                manager.DirectReports.Add(employee);
            }
        }

        Employees = employees;
    }

    public List<Employee> GetTopLevelManagers()
    {
        return Employees.Where(e => e.ManagerId == null).ToList();
    }
    public Employee GetById(int id, bool reportingTree)
    {
        _employeeById.TryGetValue(id, out Employee temp);
        if(temp== null){
            return null;
        }
        Employee employee = new Employee()
        {
            EmployeeId = temp.EmployeeId,
            ManagerId = temp.ManagerId,
            Name = temp.Name,
            DirectReports = temp.DirectReports
        };
        if (!reportingTree)
        {

            employee.DirectReports = new List<Employee>();
        }
        return employee;
    }

    public Employee? Update(int id, string name, int? managerId)
    {
        if (_employeeById.TryGetValue(id, out Employee employee))
        {
            employee.Name = name;
            employee.ManagerId = managerId;
            if (managerId != null && _employeeById.TryGetValue(managerId.Value, out Employee newManager))
            {
                Employee oldManager = Employees.FirstOrDefault(e => e.DirectReports.Contains(employee));
                oldManager?.DirectReports.Remove(employee);
                newManager.DirectReports.Add(employee);
            }
            return employee;
        }

        return null;
    }
    public Employee Add(Employee newEmployee)
    {
        var newEmployeeId = Employees.Max(e => e.EmployeeId) + 1;

        var employee = new Employee
        {
            EmployeeId = newEmployeeId,
            Name = newEmployee.Name,
            ManagerId = newEmployee.ManagerId,
            DirectReports = new List<Employee>()
        };

        if (newEmployee.ManagerId.HasValue && _employeeById.TryGetValue(newEmployee.ManagerId.Value, out Employee manager))
        {
            manager.DirectReports.Add(employee);
        }

        _employeeById[newEmployeeId] = employee;
        Employees.Add(employee);

        return employee;
    }
    public (string, bool) DeleteEmployee(int employeeId)
    {
        if (!_employeeById.ContainsKey(employeeId))
        {
            return ($"No employee found with ID {employeeId}", false);
        }

        var employeeToDelete = _employeeById[employeeId];
        if (employeeToDelete.DirectReports != null && employeeToDelete.DirectReports.Any())
        {
            return ("Cannot delete an employee who has direct reports.", false);
        }
        if (employeeToDelete.ManagerId.HasValue && _employeeById.TryGetValue(employeeToDelete.ManagerId.Value, out var manager))
            {
                manager.DirectReports.Remove(employeeToDelete);
            }
            foreach (var employee in _employeeById.Values)
            {
                if (employee.ManagerId == employeeId)
                {
                    employee.ManagerId = null;
                }
            }
            _employeeById.Remove(employeeId);

        return ("Delete Sucessfull", true);
    }
}