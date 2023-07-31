namespace organization;
public class Employee
{
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public int? ManagerId { get; set; }
    public List<Employee> DirectReports { get; set; } = new List<Employee>();
}



