using DepartmentsEmployeesConsole.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DepartmentsEmployeesConsole.Data
{   //This class is for retreiving data from our database
   public class EmployeeRepository
    {
        public SqlConnection Connection
        {
            get
            {
                string _connectionString = "Data Source=localhost\\SQLEXPRESS; Initial Catalog=DepartmentsEmployeesJW; Integrated Security=True";
                return new SqlConnection(_connectionString);
            }
        }

        public List<Employee> GetAllEmployees()
        {
            // 1. Open a connection to the database
            // 2. Create a SQL SELECT  statement as a C# string
            // 3. Execute that SQL statement against the database
            // 4. From the database, we get "raw data" back. We need to parse this as a C# object
            // 5. Close the connection to the database
            // 6. Return the Employee object

            // This opens the connection. SQLConnection is the TUNNEL
            using (SqlConnection conn = Connection)
            {
                // This opens the GATES on either side of the TUNNEL
                conn.Open();

                // SQLCommand is the list of instructions to give to a truck driver when they get to the other side of the TUNNEL
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here is the sql command that we want to be run when the driver gets to the database
                    cmd.CommandText = @"
                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.Id, d.DeptName
                        FROM Employee e
                        LEFT JOIN Department d
                        ON e.DepartmentId = d.Id";

                    // ExecuteReader actually has the driver go to the database and executes that command. The driver then comes back with a bunch of data from the database. This is held in the this variable called "reader"
                    SqlDataReader reader = cmd.ExecuteReader();

                    // This is just us initializing the list that we'll eventually return
                    List<Employee> allEmployees = new List<Employee>();

                    // The reader will read the returned data from the database one row at a time. This is why we put it in a while loop
                    while (reader.Read())
                    {
                        // Get ordinal returns us what "position" the Id column is in
                        int idColumn = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumn);

                        // The reader isn't smart enough to know automatically what TYPE of data it's reading.
                        // For that reason we have to tell it, by saying `GetInt32`, `GetString`, GetDate`, etc
                        int firstNameColumn = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumn);

                        int lastNameColumn = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumn);

                        int departmentIdColumn = reader.GetOrdinal("DepartmentId");
                        int departmentValue = reader.GetInt32(departmentIdColumn);

                        int departmentNameColumn = reader.GetOrdinal("DeptName");
                        string departmentNameValue = reader.GetString(departmentNameColumn);

                        // Now that all the data is parsed, we create a new C# object
                        var employee = new Employee()
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            DepartmentId = departmentValue,
                            Department = new Department()
                            {
                                Id = departmentValue,
                                DeptName = departmentNameValue
                            }
                        };

                        // Now that we have a parsed C# object, we can add it to the list and continue with the while loop
                        allEmployees.Add(employee);
                    }

                    // Now we can close the connection
                    reader.Close();

                    // and return all employees
                    return allEmployees;
                }
            }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            // This opens the connection. SQLConnection is the TUNNEL
            using (SqlConnection conn = Connection)
            {
                // This opens the GATES on either side of the TUNNEL
                conn.Open();

                // SQLCommand is the list of instructions to give to a truck driver when they get to the other side of the TUNNEL
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here is the sql command that we want to be run when the driver gets to the database
                    cmd.CommandText = @"
                        SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, d.Id, d.DeptName
                        FROM Employee e
                        LEFT JOIN Department d
                        ON e.DepartmentId = d.Id
                        WHERE e.Id = @id";

                    // This is us telling the truck driver that there is a VARIABLE in the sql statement. When you get to the database, replace the string "@id" with employeeId
                    cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                    // ExecuteReader actually has the driver go to the database and executes that command. The driver then comes back with a bunch of data from the database. This is held in the this variable called "reader"
                    SqlDataReader reader = cmd.ExecuteReader();


                    // The reader will read the returned data from the database if it finds the single row we're looking for. If it doesn't find the employee with the given Id, reader.Read() will return false
                    if (reader.Read())
                    {
                        // Get ordinal returns us what "position" the Id column is in
                        int idColumn = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumn);

                        // The reader isn't smart enough to know automatically what TYPE of data it's reading.
                        // For that reason we have to tell it, by saying `GetInt32`, `GetString`, GetDate`, etc
                        int firstNameColumn = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumn);

                        int lastNameColumn = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumn);

                        int departmentIdColumn = reader.GetOrdinal("DepartmentId");
                        int departmentValue = reader.GetInt32(departmentIdColumn);

                        int departmentNameColumn = reader.GetOrdinal("DeptName");
                        string departmentNameValue = reader.GetString(departmentNameColumn);

                        // Now that all the data is parsed, we create a new C# object
                        var employee = new Employee()
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            DepartmentId = departmentValue,
                            Department = new Department()
                            {
                                Id = departmentValue,
                                DeptName = departmentNameValue
                            }
                        };

                        // Now we can close the reader
                        reader.Close();

                        return employee;
                    }
                    else
                    {
                        // We didn't find the employee with that ID in the database. return null
                        return null;
                    }
                }
            }
        }

        public Employee CreateNewEmployee(Employee employeeToAdd)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Employee (FirstName, LastName, DepartmentId)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName, @departmentId)";

                    cmd.Parameters.Add(new SqlParameter("@firstName", employeeToAdd.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employeeToAdd.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", employeeToAdd.DepartmentId));

                    int id = (int)cmd.ExecuteScalar();

                    employeeToAdd.Id = id;

                    return employeeToAdd;
                }
            }
        }

        // Update Employee
        public void UpdateEmployee(int employeeId, Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Employee
                        SET FirstName = @firstName, LastName = @lastName, DepartmentId = @departmentId
                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                    // We don't expect anything back from the database (it's not really a "query", so we can say Execute NonQuery
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Delete an employee
        public void DeleteEmployee(int employeeId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Employee WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
