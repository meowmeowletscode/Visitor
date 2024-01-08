using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Net;
using System.Numerics;
using System.Xml.Linq;

namespace Visitor.Pages.Visitors
{
    public class ListsModel : PageModel
    {
        public List<VisitorInfo> listVisitors { get; set; }

        public ListsModel()
        {
            listVisitors = new List<VisitorInfo>();
        }

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-R4FGAIN\\MSSQL2014;Initial Catalog=Visitor;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM record ORDER BY Id DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VisitorInfo visitorInfo = new VisitorInfo
                                {
                                    Id = reader.GetInt32(0).ToString(),
                                    CheckinId = reader.GetInt32(1).ToString(),
                                    Name = reader.GetString(2),
                                    ICNo = reader.GetString(3),
                                    PhoneNo = reader.GetString(4),
                                    Company = reader.GetString(5),
                                    NumOfPerson = reader.GetInt32(6).ToString(),
                                    Purpose = reader.GetString(7),
                                    TimeIn = reader.GetDateTime(8).ToString(),
                                    TimeOut = reader.IsDBNull(9) ? null : reader.GetDateTime(9).ToString()
                                };

                                listVisitors.Add(visitorInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}
