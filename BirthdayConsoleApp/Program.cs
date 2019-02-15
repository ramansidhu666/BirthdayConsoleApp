using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            SqlConnection con = new SqlConnection("Data Source=74.208.69.145;Initial Catalog=kahlonteam_Realtors;User ID=sa;Password=!nd!@123");
            SqlDataAdapter adp = new SqlDataAdapter("select * from AdminClient", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    if (row["DOB"] != System.DBNull.Value && row["DOB"] != "")
                    {

                        var Email = row["EmailId"].ToString();
                        //var Photopath = row["PhotoPath"].ToString();
                        var Name = row["Name"].ToString();
                        var PhotoPath = row["Photopath"].ToString();

                        //user date of birth
                        var dateofbirth = row["DOB"].ToString();
                        var splitdate = dateofbirth.Split('/');
                        if(splitdate.Length==3)
                        {
                            if (Convert.ToInt32(splitdate[0]) > 12)
                            {
                                dateofbirth = splitdate[1] + "/" + splitdate[0] + "/" + splitdate[2];
                            }
                            DateTime Dt = Convert.ToDateTime(dateofbirth);
                            string output = Dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime Dob = DateTime.ParseExact(output, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var dob_day = Dob.Month;
                            var dob_month = Dob.Day;
                            //End

                            //Today date
                            var currentdate = DateTime.Now;
                            var current_day = currentdate.Day;
                            var current_month = currentdate.Month;
                            //End

                            if (current_month == dob_month)
                            {
                                if (current_day == dob_day)
                                {

                                    BirthdayEmail(Email, Name, dateofbirth, PhotoPath);

                                }

                            }

                        }


                    }
                }
                catch
                {

                }
                

            }

        }


        public static void BirthdayEmail(string Email,string Name,string Dob,string PhotoPath)
        {
            //Email = "only4agentss@gmail.com";
            //Send mail
            MailMessage mail = new MailMessage();
           
            string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"];
            string FromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];

            SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
            int _Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
            Boolean _UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
            Boolean _EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
            mail.To.Add(new MailAddress(Email));
            mail.From = new MailAddress(FromEmailID);
            mail.Subject = "Happy B'day";
            string msgbody = "";

            using (StreamReader reader = new StreamReader(@"C:\sites\BirthdayConsoleApp\BirthdayConsoleApp\Templates\FirstPostCard.html"))
            {
                msgbody = reader.ReadToEnd();               
                msgbody = msgbody.Replace("{Name}", Name);
                msgbody = msgbody.Replace("{DOB}", Dob);
                msgbody = msgbody.Replace("{PhotoPath}", PhotoPath);
            }

            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(msgbody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
            System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(msgbody, null, "text/html");

            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);
            // mail.Body = msgbody;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = _Port;
            smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
            smtp.EnableSsl = _EnableSsl;
            smtp.Send(mail);

        }


    }
}
