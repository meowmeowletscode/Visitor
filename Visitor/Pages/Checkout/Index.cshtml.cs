using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Visitor.Pages.Visitors
{
    public class IndexModel : PageModel
    {

        public VisitorInfo visitorInfo = new VisitorInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            visitorInfo.Id = Request.Form["id"];

            if (visitorInfo.Id.Length == 0 )
            {
                errorMessage = "Visitor Number is required.";
                return;
            }
           
            try
            {
                String connectionString = "Data Source=DESKTOP-R4FGAIN\\MSSQL2014;Initial Catalog=Visitor;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //remove the visitor from checkin list
                    String checkinSql = "DELETE FROM checkin WHERE Id=@id;";

                    using (SqlCommand command = new SqlCommand(checkinSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", visitorInfo.Id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            successMessage = "Visitor " + visitorInfo.Id + " check out successful.";
                        }
                        else
                        {
                            errorMessage = "Visitor " + visitorInfo.Id + " does not exist. Please check and try again.";
                            return;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }                      

            try
            {
                String connectionString = "Data Source=DESKTOP-R4FGAIN\\MSSQL2014;Initial Catalog=Visitor;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String checkinSql = "UPDATE record SET TimeOut=@timeout " +
                        "WHERE CheckinId=@id AND ID = (SELECT TOP 1 ID FROM record WHERE CheckinId=@id ORDER BY ID DESC);";

                    using (SqlCommand command = new SqlCommand(checkinSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", visitorInfo.Id);
                        command.Parameters.AddWithValue("@timeout", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            visitorInfo.Id = "";
        }
    }
    
}
