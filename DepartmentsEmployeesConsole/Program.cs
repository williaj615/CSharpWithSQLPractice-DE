using System;
using System.Collections.Generic;
using DepartmentsEmployeesConsole.Data;
using DepartmentsEmployeesConsole.Models;

namespace DepartmentsEmployeesConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new EmployeeRepository();
            var employees = repo.GetAllEmployees();

            foreach (var employee in employees)
            {
                Console.WriteLine($"{employee.FirstName} {employee.LastName} is in {employee.Department.DeptName}");
            }

            Console.WriteLine("Let's get an employee with the ID 2");

            var employeeWithId2 = repo.GetEmployeeById(2);

            Console.WriteLine($"Employee with Id 2 is {employeeWithId2.FirstName} {employeeWithId2.LastName}");

            DepartmentRepository departmentRepo = new DepartmentRepository();

            Console.WriteLine("Getting All Departments:");
            Console.WriteLine();

            List<Department> allDepartments = departmentRepo.GetAllDepartments();

            foreach (Department dept in allDepartments)
            {
                Console.WriteLine($"{dept.Id} {dept.DeptName}");
            }

            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting Department with Id 1");

            Department singleDepartment = departmentRepo.GetDepartmentById(1);

            Console.WriteLine($"{singleDepartment.Id} {singleDepartment.DeptName}");


            Department legalDept = new Department
            {
                DeptName = "Legal"
            };

            departmentRepo.AddDepartment(legalDept);

            Console.WriteLine("-------------------------------");
            Console.WriteLine("Added the new Legal Department!");
        }
    }
}
