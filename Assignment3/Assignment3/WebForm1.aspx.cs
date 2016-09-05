using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Xml;
using System.Threading;

namespace Assignment3
{
    public class PostCodeInfo
    {
        public string PostCode { get; set; }
        public string Locality { get; set; }
        public string State { get; set; }

        public PostCodeInfo(string postCode, string locality, string state)
        {
            PostCode = postCode;
            Locality = locality;
            State = state;
        }
    }

    public class States
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string State1 { get; set; }
        public string State2 { get; set; }

        public States(string origin, string destination, string state1, string state2)
        {
            Origin = origin;
            Destination = destination;
            State1 = state1;
            State2 = state2;
        }
    }

    public class HourlyRates
    {
        public string State { get; set; }
        public int WD_Stime { get; set; }
        public int WD_Etime { get; set; }
        public int WD_Rate { get; set; }
        public int WE_Stime { get; set; }
        public int WE_Etime { get; set; }
        public int WE_Rate { get; set; }
        public int PH_Stime { get; set; }
        public int PH_Etime { get; set; }
        public int PH_Rate { get; set; }

        public HourlyRates(string state, int wd_stime, int wd_etime, int wd_rate, int we_stime, int we_etime, int we_rate, int ph_stime, int ph_etime, int ph_rate)
        {
            State = state;
            WD_Stime = wd_stime;
            WD_Etime = wd_etime;
            WD_Rate = wd_rate;
            WE_Stime = we_stime;
            WE_Etime = we_etime;
            WE_Rate = we_rate;
            PH_Stime = ph_stime;
            PH_Etime = ph_etime;
            PH_Rate = ph_rate;
        }
    }

    public class PostCodeLocation
    {
        public string PostCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public PostCodeLocation(string postCode, double lat, double lon)
        {
            PostCode = postCode;
            Latitude = lat;
            Longitude = lon;
        }
    }

    public class PublicHolidays
    {
        public string State { get; set; }
        public string Holiday { get; set; }
        public DateTime Date { get; set; }
        public bool Value { get; set; }

        public PublicHolidays(string state, string holiday, DateTime date, bool value)
        {
            State = state;
            Holiday = holiday;
            Date = date;
            Value = value;
        }
    }
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected Rebate_Processer Proc;
        protected IConnectionFactory ConFac;
        protected IConnection Con;
        protected ISession Ses;
        protected Publisher Pub;
        SqlConnection conn = new SqlConnection(
                 @"Data Source=(LocalDB)\v11.0;AttachDbFilename='|DataDirectory|\Database1.mdf';Integrated Security=True");
        protected static List<PostCodeInfo> PostCodeList;
        protected static List<PostCodeLocation> PostCodeLocList;
        protected static List<States> StateList;
        protected static List<HourlyRates> RateList;
        protected static List<PublicHolidays> HolidayList;
        static protected string InvoiceFilePath;
        double credit = 0.0;
        double discount = 0.0;
        double credit_claim = 0.0;

        protected void WriteInvoiceNumber(string invoiceFilePath, string invoiceNumber)
        { //This code was getting repeated in GenerateNextInvoiceNumber(). It's always better to throw repeated code in a method instead of copy-pasting your code!
            StreamWriter Writer = new StreamWriter(invoiceFilePath);
            Writer.WriteLine(invoiceNumber);
            Writer.Close();
        }

        protected string GenerateNextInvoiceNumber(string invoiceFilePath)
        {
            if (File.Exists(invoiceFilePath) == false) //The first time your app is run the file probably won't exist yet, so we need to check if it does and create it if not.
            {
                WriteInvoiceNumber(invoiceFilePath, "0");
                return "0";
            }
            string CurrNumber = GetCurrentInvoiceNumber(invoiceFilePath);
            string NextNumber = (int.Parse(CurrNumber) + 1).ToString(); //Add 1 to whatever our invoice number is. We have to convert to an integer and back to do arithmatic on a string.
            WriteInvoiceNumber(invoiceFilePath, NextNumber);
            return NextNumber;
        }

        protected string GetCurrentInvoiceNumber(string invoiceFilePath)
        { //The invoice number file should be just one line of text in a plain text file, so we only need to call ReadLine() once and we should have the invoice number.
            StreamReader Reader = new StreamReader(invoiceFilePath);
            string CurrLine = Reader.ReadLine();
            Reader.Close();
            return CurrLine;
        }


        protected bool Contains(List<PostCodeInfo> listToCheck, string postCode)
        { //This is a very slow search that you normally wouldn't use, but to keep things simple we'll just iterate through every list item and check for our post code
            foreach (PostCodeInfo entry in listToCheck)
                if (entry.PostCode == postCode)
                    return true;
            return false;
        }
        protected bool Contains(List<PostCodeLocation> listToCheck, string postCode)
        { //This is a very slow search that you normally wouldn't use, but to keep things simple we'll just iterate through every list item and check for our post code
            foreach (PostCodeLocation entry in listToCheck)
                if (entry.PostCode == postCode)
                    return true;
            return false;
        }
        protected bool LocContains(List<PostCodeLocation> listToCheck, string postCode)
        { //This is a very slow search that you normally wouldn't use, but to keep things simple we'll just iterate through every list item and check for our post code
            foreach (PostCodeLocation entry in listToCheck)
                if (entry.PostCode == postCode)
                    return true;
            return false;
        }

        protected string FindState(List<PostCodeInfo> listToCheck, string postCode)
        { //Again, the search here is not what you would normally use because of how slow it is but we use it to keep things simple
            //Those of you who want a better solution can look into the SortedDictionary to use instead of a List (MSDN is very useful!)
            foreach (PostCodeInfo entry in listToCheck)
            {
                if (entry.PostCode == postCode)
                    return entry.State;
            }
            return null; //Return nothing if it's not in our list
        }

        protected States FindState(List<States> listToCheck, string origin, string destination)
        { //Again, the search here is not what you would normally use because of how slow it is but we use it to keep things simple
            //Those of you who want a better solution can look into the SortedDictionary to use instead of a List (MSDN is very useful!)
            foreach (States entry in listToCheck)
            {
                if (entry.Origin == origin && entry.Destination == destination)
                    return entry;
            }
            return null; //Return nothing if it's not in our list
        }

        protected PublicHolidays MatchDate(List<PublicHolidays> listToCheck, DateTime date, string state)
        { //Again, the search here is not what you would normally use because of how slow it is but we use it to keep things simple
            //Those of you who want a better solution can look into the SortedDictionary to use instead of a List (MSDN is very useful!)
            foreach (PublicHolidays entry in listToCheck)
            {
                if (entry.Date == date && entry.State == state)
                    return entry;
            }
            return null; //Return nothing if it's not in our list
        }

        protected PostCodeLocation Find(List<PostCodeLocation> listToCheck, string postCode)
        { //Again, the search here is not what you would normally use because of how slow it is but we use it to keep things simple
            //Those of you who want a better solution can look into the SortedDictionary to use instead of a List (MSDN is very useful!)
            foreach (PostCodeLocation entry in listToCheck)
            {
                if (entry.PostCode == postCode)
                    return entry;
            }
            return null; //Return nothing if it's not in our list
        }

        protected HourlyRates[] FindState(List<HourlyRates> listToCheck, string state)
        { //Again, the search here is not what you would normally use because of how slow it is but we use it to keep things simple
            //Those of you who want a better solution can look into the SortedDictionary to use instead of a List (MSDN is very useful!)
            HourlyRates[] entries = new HourlyRates[2];
            int i = 0;
            foreach (HourlyRates entry in listToCheck)
            {
                if (entry.State == state)
                {
                    entries[i] = entry;
                    i++;
                }
            }
            return entries; //Return nothing if it's not in our list
        }

        protected double HourlyRateCal(DateTime startTime, DateTime endTime, string state)
        {
            HourlyRates[] entries = FindState(RateList, state);
            DateTime temp = startTime;
            double price1 = 0.0;
            while (temp <= endTime)
            {
                PublicHolidays match = MatchDate(HolidayList, temp.Date, state);
                if (match != null && match.Value == true)
                {

                    if (temp.Hour >= entries[0].PH_Stime && temp.Hour <= entries[0].PH_Etime)
                    {
                        price1 = price1 + entries[0].PH_Rate;

                    }
                    else// if (temp.Hour >= entries[1].PH_Stime && temp.Hour <= entries[1].PH_Etime)
                    {
                        price1 = price1 + entries[1].PH_Rate;
                    }



                }

                else if (!(temp.DayOfWeek == DayOfWeek.Saturday || temp.DayOfWeek == DayOfWeek.Sunday)) //if day is weekday
                {
                    int day = (int)temp.DayOfWeek;
                    if (temp.Hour >= entries[0].WD_Stime && temp.Hour <= entries[0].WD_Etime)
                    {
                        price1 = price1 + entries[0].WD_Rate;

                    }
                    else// if (temp.Hour >= entries[1].WD_Stime && temp.Hour <= entries[1].WD_Etime)
                    {
                        price1 = price1 + entries[1].WD_Rate;
                    }

                }
                else
                {
                    if (temp.Hour >= entries[0].WE_Stime && temp.Hour <= entries[0].WE_Etime)
                    {
                        price1 = price1 + entries[0].WE_Rate;

                    }
                    else// if (temp.Hour >= entries[1].WE_Stime && temp.Hour <= entries[1].WE_Etime)
                    {
                        price1 = price1 + entries[1].WE_Rate;
                    }
                }

                temp = temp.AddHours(1);
                // Console.WriteLine(temp.ToString());
            }



            return price1;
        }


        protected double Hourly_Rates(string state1, string state2, string state3, string state4, double Distance, double TIME, DateTime DoD)
        {
            double ferry_price = 1000;
            double TotalHourlyPrice = 0;

            if (state1 == state4) // vic,vic
            {
                double hoursPerState = TIME;
                DateTime State1_StartTime = DoD.AddHours(-TIME);
                DateTime State1_EndTime = DoD;
                double state1_hrlyRate = HourlyRateCal(State1_StartTime, State1_EndTime, state1);
                TotalHourlyPrice = state1_hrlyRate;
                return TotalHourlyPrice;
            }

            else if (state2 == "") //vic,qld
            {

                double hoursPerState = TIME / 2;
                DateTime State1_StartTime = DoD.AddHours(-TIME);
                DateTime State1_EndTime = State1_StartTime.AddHours(hoursPerState);
                double state1_hrlyRate = HourlyRateCal(State1_StartTime, State1_EndTime, state1);
                TimeSpan State1_Timespan = State1_EndTime - State1_StartTime;

                DateTime State2_StartTime = State1_EndTime;
                DateTime State2_EndTime = DoD;
                double state2_hrlyRate = HourlyRateCal(State2_StartTime, State2_EndTime, state4);
                if (state1 == "TAS" || state4 == "TAS")
                {
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate + ferry_price;
                }
                else
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate;
                return TotalHourlyPrice;
                //Distance = Distance / 2;
            }
            else if (state2 != "" && state3 == "") //vic,qld,act
            {
                double hoursPerState = TIME / 3;
                DateTime State1_StartTime = DoD.AddHours(-TIME);
                DateTime State1_EndTime = State1_StartTime.AddHours(hoursPerState);
                double state1_hrlyRate = HourlyRateCal(State1_StartTime, State1_EndTime, state1);
                DateTime State2_StartTime = State1_EndTime;
                DateTime State2_EndTime = State2_StartTime.AddHours(hoursPerState);
                double state2_hrlyRate = HourlyRateCal(State2_StartTime, State2_EndTime, state2);
                DateTime State3_StartTime = State2_EndTime;
                DateTime State3_EndTime = DoD;
                double state3_hrlyRate = HourlyRateCal(State3_StartTime, State3_EndTime, state4);
                if (state1 == "TAS" || state2 == "TAS" || state4 == "TAS")
                {
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate + state3_hrlyRate + ferry_price;
                }
                else
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate + state3_hrlyRate;
                return TotalHourlyPrice;

            }
            else //vic,qld,act,tas
            {
                double hoursPerState = TIME / 4;
                DateTime State1_StartTime = DoD.AddHours(-TIME);
                DateTime State1_EndTime = State1_StartTime.AddHours(hoursPerState);
                double state1_hrlyRate = HourlyRateCal(State1_StartTime, State1_EndTime, state1);
                DateTime State2_StartTime = State1_EndTime;
                DateTime State2_EndTime = State2_StartTime.AddHours(hoursPerState);
                double state2_hrlyRate = HourlyRateCal(State2_StartTime, State2_EndTime, state2);
                DateTime State3_StartTime = State2_EndTime;
                DateTime State3_EndTime = State3_StartTime.AddHours(hoursPerState);
                double state3_hrlyRate = HourlyRateCal(State3_StartTime, State3_EndTime, state3);
                DateTime State4_StartTime = State3_EndTime;
                DateTime State4_EndTime = DoD;
                double state4_hrlyRate = HourlyRateCal(State4_StartTime, State4_EndTime, state4);
                if (state1 == "TAS" || state2 == "TAS" || state3 == "TAS" || state4 == "TAS")
                {
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate + state3_hrlyRate + state4_hrlyRate + ferry_price;
                }
                else
                    TotalHourlyPrice = state1_hrlyRate + state2_hrlyRate + state3_hrlyRate + state4_hrlyRate;
                return TotalHourlyPrice;
            }

        }

        protected static Double rad2deg(Double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        protected static Double deg2rad(Double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        protected double distance(double lat1, double lon1, double lat2, double lon2)
        {

            double theta = lon1 - lon2;

            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));

            dist = Math.Acos(dist);

            dist = rad2deg(dist);

            dist = dist * 60 * 1.1515;

            dist = dist * 1.609344;


            return (dist);

        }

        protected void Load_Postcodes(string CSVFileName)
        {
            PostCodeList = new List<PostCodeInfo>();

            StreamReader Reader = null;
            try
            {
                Reader = new StreamReader(CSVFileName);
            }
            catch (FileNotFoundException ex)
            {
                lblOutput.Text = string.Format("Error: Check your file path and file name for the post code CSV are correct! Current setting is: '{0}'", CSVFileName);
                return;
            }
            Reader.ReadLine(); //Get rid of the first line of the file which contains the names of the columns (e.g. "Pcode", "Locality" , etc.)
            while (Reader.EndOfStream == false)
            {
                string CurrentLine = Reader.ReadLine();
                string[] Columns = CurrentLine.Split(new char[] { ',' });
                for (int i = 0; i < Columns.Length; i++)
                { //Remove the redundant double quotes included in the CSV file
                    Columns[i] = Columns[i].TrimStart(new char[] { '"' });
                    Columns[i] = Columns[i].TrimEnd(new char[] { '"' });
                }
                PostCodeInfo NewPostCode = new PostCodeInfo(Columns[0], Columns[1], Columns[2]); //Consider error checking each column in your assignments
                if (Contains(PostCodeList, NewPostCode.PostCode) == false)
                { //Don't add the PostCodeInfo if we already encountered this post code
                    PostCodeList.Add(NewPostCode);
                }
            }
        }

        protected void Load_Location(string CSVFileName)
        {
            PostCodeLocList = new List<PostCodeLocation>();

            StreamReader Reader = null;
            try
            {
                Reader = new StreamReader(CSVFileName);
            }
            catch (FileNotFoundException ex)
            {
                lblOutput.Text = string.Format("Error: Check your file path and file name for the post code CSV are correct! Current setting is: '{0}'", CSVFileName);
                return;
            }
            Reader.ReadLine(); //Get rid of the first line of the file which contains the names of the columns (e.g. "Pcode", "Locality" , etc.)
            while (Reader.EndOfStream == false)
            {
                string CurrentLine = Reader.ReadLine();
                string[] Columns = CurrentLine.Split(new char[] { ',' });
                for (int i = 0; i < Columns.Length; i++)
                { //Remove the redundant double quotes included in the CSV file
                    Columns[i] = Columns[i].TrimStart(new char[] { '"' });
                    Columns[i] = Columns[i].TrimEnd(new char[] { '"' });
                }
                PostCodeLocation NewPostCodeLoc = new PostCodeLocation(Columns[0], double.Parse(Columns[5]), double.Parse(Columns[6])); //Consider error checking each column in your assignments
                if (LocContains(PostCodeLocList, NewPostCodeLoc.PostCode) == false)
                { //Don't add the PostCodeInfo if we already encountered this post code
                    PostCodeLocList.Add(NewPostCodeLoc);
                }
            }
        }

        protected void Load_States(string CSVFileName)
        {
            StateList = new List<States>();

            StreamReader Reader = null;
            try
            {
                Reader = new StreamReader(CSVFileName);
            }
            catch (FileNotFoundException ex)
            {
                lblOutput.Text = string.Format("Error: Check your file path and file name for the post code CSV are correct! Current setting is: '{0}'", CSVFileName);
                return;
            }
            Reader.ReadLine(); //Get rid of the first line of the file which contains the names of the columns (e.g. "Pcode", "Locality" , etc.)
            while (Reader.EndOfStream == false)
            {
                string CurrentLine = Reader.ReadLine();
                string[] Columns = CurrentLine.Split(new char[] { ',' });
                for (int i = 0; i < Columns.Length; i++)
                { //Remove the redundant double quotes included in the CSV file
                    Columns[i] = Columns[i].TrimStart(new char[] { '"' });
                    Columns[i] = Columns[i].TrimEnd(new char[] { '"' });
                }

                States NewStates = new States(Columns[0], Columns[1], Columns[2], Columns[3]); //Consider error checking each column in your assignments

                {
                    StateList.Add(NewStates);
                }
            }
        }

        protected void Load_Rates(string CSVFileName)
        {
            RateList = new List<HourlyRates>();

            StreamReader Reader = null;
            try
            {
                Reader = new StreamReader(CSVFileName);
            }
            catch (FileNotFoundException ex)
            {
                lblOutput.Text = string.Format("Error: Check your file path and file name for the post code CSV are correct! Current setting is: '{0}'", CSVFileName);
                return;
            }
            Reader.ReadLine(); //Get rid of the first line of the file which contains the names of the columns (e.g. "Pcode", "Locality" , etc.)
            while (Reader.EndOfStream == false)
            {
                string CurrentLine = Reader.ReadLine();
                string[] Columns = CurrentLine.Split(new char[] { ',' });
                for (int i = 0; i < Columns.Length; i++)
                { //Remove the redundant double quotes included in the CSV file
                    Columns[i] = Columns[i].TrimStart(new char[] { '"' });
                    Columns[i] = Columns[i].TrimEnd(new char[] { '"' });
                }
                HourlyRates NewRate = new HourlyRates(Columns[0], Convert.ToInt32((Columns[1])), Convert.ToInt32((Columns[2])), Convert.ToInt32((Columns[3])), Convert.ToInt32((Columns[4])), Convert.ToInt32((Columns[5])), Convert.ToInt32((Columns[6])), Convert.ToInt32((Columns[7])), Convert.ToInt32((Columns[8])), Convert.ToInt32((Columns[9]))); //Consider error checking each column in your assignments

                { //Don't add the PostCodeInfo if we already encountered this post code
                    RateList.Add(NewRate);
                }
            }
        }

        protected void Load_Holidays(string CSVFileName)
        {
            HolidayList = new List<PublicHolidays>();

            StreamReader Reader = null;
            try
            {
                Reader = new StreamReader(CSVFileName);
            }
            catch (FileNotFoundException ex)
            {
                lblOutput.Text = string.Format("Error: Check your file path and file name for the post code CSV are correct! Current setting is: '{0}'", CSVFileName);
                return;
            }
            Reader.ReadLine(); //Get rid of the first line of the file which contains the names of the columns (e.g. "Pcode", "Locality" , etc.)
            while (Reader.EndOfStream == false)
            {
                string CurrentLine = Reader.ReadLine();
                string[] Columns = CurrentLine.Split(new char[] { ',' });
                for (int i = 0; i < Columns.Length; i++)
                { //Remove the redundant double quotes included in the CSV file
                    Columns[i] = Columns[i].TrimStart(new char[] { '"' });
                    Columns[i] = Columns[i].TrimEnd(new char[] { '"' });
                }
                PublicHolidays NewHoliday = new PublicHolidays(Columns[0], Columns[1],
                    Convert.ToDateTime((Columns[2])),
                    Convert.ToBoolean((Columns[3]).ToLower())); //Consider error checking each column in your assignments

                { //Don't add the PostCodeInfo if we already encountered this post code
                    HolidayList.Add(NewHoliday);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Proc == null) //Only run the first time a page is loaded
            {
                Proc = new Rebate_Processer(this);

                const string TOPIC_NAME = "Assignment3";
                ConFac = new ConnectionFactory(@"tcp://localhost:61616");
                Con = ConFac.CreateConnection();
                Con.Start();
                Ses = Con.CreateSession();
                Pub = new Publisher(Ses, TOPIC_NAME);
            }
            // dateTimePicker.MinDate = DateTime.Now;

            if (InvoiceFilePath == null)
            {
                InvoiceFilePath = Server.MapPath("PDFs") + @"\InvoiceNumber.txt";
            }
            // if (PostCodeList == null && PostCodeLocList == null) //This if statement isn't necessary, but if we don't have it we have to re-read our CSV file every time someone refreshs our web form! That's extremely slow.
            {
                Load_Postcodes(@"\SIT322\PostCode-Full_20130228.csv"); //Put in your CSV file location here! Warning: Relative file paths go from the server executable file path, NOT your web app's "bin" file path. Use a full file path if you have problems
                //                                                       The '@' denotes a string literal ('\' characters are treated as normal characters, i.e. as literal characters)
                Load_Location(@"\SIT322\Australian_Post_Codes_Lat_Lon.csv");
                Load_States(@"\SIT322\Path_new.csv");
                Load_Rates(@"\SIT322\Rate2.csv");
                Load_Holidays(@"\SIT322\PH.csv");

            }

        }

        protected bool CheckDate(DateTime date)
        {
            if (date < DateTime.Now)
            {
                msgError.Text = "Error : Date cannot be of past";
                msgError.Visible = true;
                return false;
            }
            else if (date < DateTime.Now.AddDays(4))
            {
                msgError.Text = "AtLeast 4 Days should be given for delivery";
                msgError.Visible = true;
                return false;
            }
            else return true;
        }

        protected double DistanceTravelled()
        {
            if (Page.IsValid == true)
            {
                string OriginCode = Convert.ToString((txtOriginCode.Text));
                //string OState = FindState(PostCodeList, OriginCode);
                PostCodeLocation OLoc = Find(PostCodeLocList, OriginCode);
                double lat1 = OLoc.Latitude;
                double lon1 = OLoc.Longitude;
                string DestCode = Convert.ToString((txtDstCode.Text));
                // string DstState = FindState(PostCodeList, DestCode);
                PostCodeLocation DstLoc = Find(PostCodeLocList, DestCode);
                double lat2 = DstLoc.Latitude;
                double lon2 = DstLoc.Longitude;
                double Distance = distance(lat1, lon1, lat2, lon2);
                // Hourly_Rates(OState, DstState, Distance);
                return Distance;
            }
            else return 0;

        }

        protected void RebateProcessor(string InvoiceNumber, string mem_class, double price)
        {
            if (mem_class.Contains("gold")) credit = 300;
            if (mem_class.Contains("regular")) credit = 200;

            if (price > 5000)
            {
                if (mem_class.Contains("silver"))
                {
                    discount = price * 0.15;
                }
                if (mem_class.Contains("gold"))
                {
                    discount = price * 0.20;
                }
            }

            
            conn.Open();
            SqlDataReader Reader = null;
            SqlCommand cmd = new SqlCommand(@"INSERT INTO Rebate(Membership_ID, Credit, Booking_No) VALUES(@mem_id, @credit, @booking)", conn);
            cmd.Parameters.AddWithValue("@mem_id", txtMemID.Text);
            cmd.Parameters.AddWithValue("@credit", credit);
            cmd.Parameters.AddWithValue("@booking", InvoiceNumber);
            cmd.ExecuteNonQuery();
            SqlCommand cmd2 = new SqlCommand(@"SELECT Credit, Booking_No from Rebate WHERE Membership_ID = @mem_id AND Booking_No < @bookNo",conn);
            cmd2.Parameters.AddWithValue("@mem_id", txtMemID.Text);
            int bookNo = Convert.ToInt32(InvoiceNumber);
            cmd2.Parameters.AddWithValue("@bookNo", bookNo);
            Reader = cmd2.ExecuteReader();
            //if (credit_object != null)
            //{
            //    credit_claim = Convert.ToDouble(credit_object);
            //}
            if (Reader.HasRows == true)
            {
                int Retrieved_Bookno = 0;
                while (Reader.Read())
                {
                    credit_claim = Convert.ToDouble(Reader[0]);
                    Retrieved_Bookno = Convert.ToInt32(Reader[1]);
                }
                Reader.Close();
                SqlCommand cmd3 = new SqlCommand(@"DELETE FROM Rebate WHERE Booking_No = @booking", conn);
                cmd3.Parameters.AddWithValue("@booking", Retrieved_Bookno);
                cmd3.ExecuteNonQuery();
            }
            Reader = null;
            conn.Close();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //if (!Page.IsValid)
            //{
            //    lblOutput.Text = "Errors found, cannot submit form.";
            //    return;
            //}
            //else
            //{
            //    lblOutput.Text = "";
            //}
            string NewInvoiceNumber = GenerateNextInvoiceNumber(InvoiceFilePath);
            string firstname = "";
            string lastname = "";
            string mem_class = "";
            //double discount = 0.0;
            //double credit = 0.0;
            SqlDataReader Reader = null;
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"SELECT first_name, last_name, m_group from member WHERE Id = @memID", conn);
            cmd.Parameters.AddWithValue("@memID", txtMemID.Text);
            Reader = cmd.ExecuteReader();
            if (Reader.HasRows == true)
            {
                while (Reader.Read())
                {
                    firstname = Convert.ToString(Reader[0]);
                    lastname = Convert.ToString(Reader[1]);
                    mem_class = Convert.ToString(Reader[2]);
                }
                txtFName.Text = firstname; txtFName.ReadOnly = true;
                txtLName.Text = lastname; txtLName.ReadOnly = true;

            }
            else
            {
                Reader.Close(); Reader = null;
                SqlCommand cmd2 = new SqlCommand(@"SET IDENTITY_INSERT member ON INSERT INTO member (Id, first_name,last_name,dob,m_group) VALUES (@id,@fname,@lname,@dob,@memGroup)", conn);
                cmd2.Parameters.AddWithValue("@id", Convert.ToInt32(txtMemID.Text));
                cmd2.Parameters.AddWithValue("@fname", txtFName.Text);
                cmd2.Parameters.AddWithValue("@lname", txtLName.Text);
                cmd2.Parameters.AddWithValue("@dob", txtDOB.Text);
                cmd2.Parameters.AddWithValue("@memGroup", "regular");
                cmd2.ExecuteNonQuery();
                
                mem_class = "regular";
            }
            conn.Close();
            string date = txtDelDate.Text;
            int hours = Convert.ToInt32(txtDelTimeHours.Text);
            double minutes = Convert.ToDouble(txtDelTimeMinutes.Text);
            string datestr = date + " " + hours + ":" + minutes + ":" + "00";
            DateTime dod = new DateTime();
            if (!(DateTime.TryParse(datestr, out dod)))
            {
                msgError.Text = "Error : Wrong date";
                msgError.Visible = true;
                return;
            }

            //DateTime dod = DateTime.Parse(Request.Form[txtDelDate.UniqueID]);
            bool DateCheck = CheckDate(dod);
            if (DateCheck == false) return;

            double Distance = DistanceTravelled();

            //SPEED cONSTANT = 60KM/HR
            double time = Distance / 60; //hrs
            double Insurance_Cost = Distance * 0.10;

            string OriginCode = Convert.ToString((txtOriginCode.Text));
            string OState = FindState(PostCodeList, OriginCode);
            string DestCode = Convert.ToString((txtDstCode.Text));
            string DstState = FindState(PostCodeList, DestCode);
            States path = FindState(StateList, OState, DstState);
            string state_1 = OState;
            string state_2 = path.State1;
            string state_3 = path.State2;
            string state_4 = DstState;

            double HourlyRate = Hourly_Rates(state_1, state_2, state_3, state_4, Distance, time, dod);
            double DelPrice = double.Parse(txtTrucks.Text) * 1000 + HourlyRate;
            double price = DelPrice + Insurance_Cost;
            //if (price > 5000 && mem_class != "regular")
            //{
            //    discount = getDiscount(price, mem_class);
            //}
            RebateProcessor(NewInvoiceNumber,mem_class, price);
            price = price - discount - credit_claim;
            //Adding GST To price
            price = price + 0.10 * price;
            lblPrice.Text = string.Format("Price {0:c:2f}", price);
            lblOutput.Text = string.Format("Distance {0}", Distance);

            StringWriter StringWriter = new StringWriter();
            XmlTextWriter Writer = new XmlTextWriter(StringWriter);
            Writer.WriteStartDocument();
            Writer.WriteStartElement("WebFormData");
            Writer.WriteElementString("ID", txtMemID.Text);
            Writer.WriteElementString("First_Name", txtFName.Text);
            Writer.WriteElementString("Class", mem_class);
            Writer.WriteElementString("Credit", credit.ToString());
            Writer.WriteEndDocument();
            string XmlDoc = StringWriter.ToString();

            Pub.SendMessage(XmlDoc);
            System.Threading.Thread.Sleep(10000);

            Document MyDoc = new Document();
            string PDFPath = Server.MapPath("PDFs");
            //Name each of our PDF files uniquely using the invoice number so we don't overwrite them.
            FileStream LocalStream = new FileStream(string.Format(@"{0}\Receipt-{1}.pdf", PDFPath, NewInvoiceNumber), FileMode.Create);
            PdfWriter.GetInstance(MyDoc, LocalStream);

            //**Also store the document in memory such that we can send it to the client as an array of bytes**
            MemoryStream HTTPStream = new MemoryStream();
            PdfWriter.GetInstance(MyDoc, HTTPStream);

            //**Create our PDF**
            MyDoc.Open();
            //Create the custom fonts we'll use in the PDF
            Font DarkBlue = new Font(Font.FontFamily.TIMES_ROMAN, 32f, Font.BOLD, new BaseColor(System.Drawing.Color.DarkBlue));
            Font LightBlue = new Font(Font.FontFamily.TIMES_ROMAN, 26f, Font.BOLD, new BaseColor(System.Drawing.Color.LightBlue));
            Font Grey = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray));

            //We create our Title paragraph seperately because we need to specify the text alignment in order to put it in the center.
            Paragraph Title = new Paragraph("Truck delivery receipt\n", DarkBlue);
            Title.Alignment = Element.ALIGN_CENTER;
            MyDoc.Add(Title);
            MyDoc.Add(new Paragraph("\nProcessed on " + DateTime.Now, Grey));
            MyDoc.Add(new Paragraph("\nOrder invoice number: " + NewInvoiceNumber, Grey));

            MyDoc.Add(new Paragraph("\nThank you for doing business with Truck Deliveries Co., " + txtFName.Text + " " + txtLName.Text + ". Your Invoice number is " + NewInvoiceNumber + ". Please quote this number if you contact support."));
            Paragraph Title2 = new Paragraph("\nDelivery details\n", LightBlue);
            Title2.Alignment = Element.ALIGN_LEFT;
            MyDoc.Add(Title2);
            MyDoc.Add(new Paragraph("\nYour delivery will arrive at " + dod + ", after leaving for delivery at " + (dod.AddHours(-time)) + "\n"));
            MyDoc.Add(new Paragraph(txtTrucks.Text + " truck delivery from " + txtOriginAdd.Text + ", " + txtOriginCode.Text + " to " + txtDstAdd.Text + ", " + txtDstCode.Text));
            MyDoc.Add(new Paragraph("Cost of Delivery: " + DelPrice.ToString("C2")));
            MyDoc.Add(new Paragraph("Cost of Insurace: " + Insurance_Cost.ToString("C2")));
            MyDoc.Add(new Paragraph("Cost Charged (incl. GST): " + price.ToString("C2")));
            MyDoc.Add(new Paragraph("\n Any querues regarding your delivery can be sent to TruckDeliver@Trucks.com, or alternatively you can call 0398765432. \n"));
            MyDoc.Add(new Paragraph("\nOur business is located at 72 Kaolin Street, VIC, AU, The fees from this truck delivery will be charged to 52 Enigma Crescent. We will contact you via phone number 03 9876 1234 if we experience any issues with your order."));
            MyDoc.Close();

            //**Send PDF to client via a HTTP response**
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", string.Format(@"attachment; filename=Receipt-{0}.pdf", NewInvoiceNumber));
            Response.BinaryWrite(HTTPStream.ToArray());



           
        }

        private double getDiscount(double price, string m_class)
        {
            double discount = 0.0;
            if (m_class == "silver")
            {
                discount = price * 0.15;
            }
            if (m_class == "gold")
            {
                discount = price * 0.20;
            }
            return discount;
        }
        protected void cusDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator CusSender = (CustomValidator)source;
            TextBox OrignCode = (TextBox)CusSender.FindControl(CusSender.ControlToValidate);
            string Input = OrignCode.Text;

            if (Contains(PostCodeLocList, Input))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }

        }

        protected void btnMem_Click(object sender, EventArgs e)
        {
            SqlDataReader Reader = null;
            conn.Open();
            SqlCommand cmd = new SqlCommand(@"SELECT first_name, last_name, dob from member WHERE Id = @memID", conn);
            cmd.Parameters.AddWithValue("@memID", txtMemID.Text);
            Reader = cmd.ExecuteReader();
            if (Reader.HasRows == true)
            {
                while (Reader.Read())
                {
                    txtFName.Text = Convert.ToString(Reader[0]); txtFName.ReadOnly = true;
                    txtLName.Text = Convert.ToString(Reader[1]); txtLName.ReadOnly = true;
                    DateTime dob = Convert.ToDateTime(Reader[2]);
                    txtDOB.Text = (dob).ToString("d"); txtDOB.ReadOnly = true;
                }

            }
            else
                lblMemOutput.Text = "Member not in Database";
        }
    }
    }
