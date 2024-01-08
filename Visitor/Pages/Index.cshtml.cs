using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Visitor.Pages
{
    public class IndexModel : PageModel
    {
        public VisitorInfo visitorInfo = new VisitorInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public int ID = 0;

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            visitorInfo.Name = Request.Form["name"];
            visitorInfo.ICNo = Request.Form["icno"];
            visitorInfo.PhoneNo = Request.Form["phoneno"];
            visitorInfo.Company = Request.Form["company"];
            visitorInfo.NumOfPerson = Request.Form["numofperson"];
            visitorInfo.Purpose = Request.Form["purpose"];
          
            if (visitorInfo.Name.Length == 0 || visitorInfo.ICNo.Length == 0|| visitorInfo.PhoneNo.Length == 0 ||
                visitorInfo.Company.Length == 0 || visitorInfo.NumOfPerson.Length == 0 || visitorInfo.Purpose.Length == 0)
            {
                errorMessage = "All the fields are required.";
                return;
            }

            //save the visitor into the database
            try
            {
                String connectionString = "Data Source=DESKTOP-R4FGAIN\\MSSQL2014;Initial Catalog=Visitor;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Step 1: Insert into checkin
                    String checkinSql = "INSERT INTO checkin " +
                                        "(Name, ICNo, PhoneNo, Company, NumOfPerson, Purpose, TimeIn) OUTPUT INSERTED.ID " +
                                        "VALUES (@name, @icno, @phoneno, @company, @numofperson, @purpose, @timein);";

                    int checkinId;
                    using (SqlCommand checkinCommand = new SqlCommand(checkinSql, connection))
                    {
                        checkinCommand.Parameters.AddWithValue("@name", visitorInfo.Name);
                        checkinCommand.Parameters.AddWithValue("@icno", visitorInfo.ICNo);
                        checkinCommand.Parameters.AddWithValue("@phoneno", visitorInfo.PhoneNo);
                        checkinCommand.Parameters.AddWithValue("@company", visitorInfo.Company);
                        checkinCommand.Parameters.AddWithValue("@numofperson", visitorInfo.NumOfPerson);
                        checkinCommand.Parameters.AddWithValue("@purpose", visitorInfo.Purpose);
                        checkinCommand.Parameters.AddWithValue("@timein", DateTime.Now);

                        // Execute the SQL command and get the last inserted ID from the checkin table
                        checkinId = Convert.ToInt32(checkinCommand.ExecuteScalar());
                        ID = checkinId;
                    }

                    // Step 2: Insert into record using the checkinId
                    String recordSql = "INSERT INTO record " +
                                        "(CheckinId, Name, ICNo, PhoneNo, Company, NumOfPerson, Purpose, TimeIn) VALUES " +
                                        "(@checkinId, @name, @icno, @phoneno, @company, @numofperson, @purpose, @timein);";

                    using (SqlCommand recordCommand = new SqlCommand(recordSql, connection))
                    {
                        recordCommand.Parameters.AddWithValue("@checkinId", checkinId);
                        recordCommand.Parameters.AddWithValue("@name", visitorInfo.Name);
                        recordCommand.Parameters.AddWithValue("@icno", visitorInfo.ICNo);
                        recordCommand.Parameters.AddWithValue("@phoneno", visitorInfo.PhoneNo);
                        recordCommand.Parameters.AddWithValue("@company", visitorInfo.Company);
                        recordCommand.Parameters.AddWithValue("@numofperson", visitorInfo.NumOfPerson);
                        recordCommand.Parameters.AddWithValue("@purpose", visitorInfo.Purpose);
                        recordCommand.Parameters.AddWithValue("@timein", DateTime.Now);

                        // Execute the SQL command
                        recordCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log or show an error message)
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (ID == 0)
            {
                errorMessage = "Some error occur. Please check in again.";
                return;
            }

            visitorInfo.Name = "";
            visitorInfo.ICNo = "";
            visitorInfo.PhoneNo = "";
            visitorInfo.Company = "";
            visitorInfo.NumOfPerson = "";
            visitorInfo.Purpose = "";
            visitorInfo.TimeOut = "";

            successMessage = "Visitor check in successful.";
        }
    }

    public class VisitorInfo
    {
        public string Id { get; set; }
        public string CheckinId { get; set; }
        public string Name { get; set; }
        public string ICNo { get; set; }
        public string PhoneNo { get; set; }
        public string Company { get; set; }
        public string NumOfPerson { get; set; }
        public string Purpose { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
    }
}
