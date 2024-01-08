using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Visitor.Pages.Login
{
    public class IndexModel : PageModel
    {
        public UserInfo userInfo = new UserInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            userInfo.UserName = Request.Form["username"];
            userInfo.Password = Request.Form["password"];

            if (userInfo.UserName.Length == 0 || userInfo.Password.Length == 0 )
            {
                errorMessage = "All the fields are required.";
                return;
            }

            //save the visitor into the database
            try
            {
                string connectionString = "Data Source=DESKTOP-R4FGAIN\\MSSQL2014;Initial Catalog=Visitor;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check user credentials
                    string checkinSql = "SELECT * FROM [user] WHERE " +
                                "UserName=@username AND Password=@password;";

                    using (SqlCommand command = new SqlCommand(checkinSql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userInfo.UserName);
                        command.Parameters.AddWithValue("@password", userInfo.Password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Retrieve data from the reader
                                string usernameFromDatabase = reader["UserName"].ToString();
                                // Add other fields as needed

                                // Close the reader before executing another command
                                reader.Close();

                                // Update LastLogin
                                string dateSql = "UPDATE [user] SET LastLogin=@date WHERE UserName=@username AND Password=@password;";

                                using (SqlCommand date = new SqlCommand(dateSql, connection))
                                {
                                    date.Parameters.AddWithValue("@date", DateTime.Now);
                                    date.Parameters.AddWithValue("@username", userInfo.UserName);
                                    date.Parameters.AddWithValue("@password", userInfo.Password);

                                    date.ExecuteNonQuery();

                                    // Redirect to the desired page after successful login
                                    Response.Redirect("/Visitors/Lists");
                                }
                            }
                            else
                            {
                                errorMessage = "User " + userInfo.UserName + " does not exist or incorrect password. Please check and try again.";
                                // Handle the error here, show a message, log, etc.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log or show an error message)
                Console.WriteLine($"Error: {ex.Message}");
            }


            userInfo.UserName = "";
            userInfo.Password = "";

            successMessage = "User login successful.";
        }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public  string LastLogin { get; set; }
    }
}
