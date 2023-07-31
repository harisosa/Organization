namespace organization;

public interface IEmployeeService{
    public List<Employee> GetTopLevelManagers();
    public Employee GetById(int id,bool reportingTree);
     public Employee? Update(int id, string name, int? managerId);
     public Employee Add(Employee newEmployee);
     public (string,bool) DeleteEmployee(int employeeId);
}