using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;

namespace WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CarsomeController : ApiController
    {
        #region CARSOME CLASS

        public class BookingTime
        {
            public List<BookingTimeData> BookingTimeData { get; set; }
        }

        public class BookingTimeData
        {
            public int key { get; set; }
            public int value { get; set; }
            public string label { get; set; }
        }

        public class CheckBookingDetails
        {
            public CheckBookingDetailsData BookingDetailsData { get; set; }
        }

        public class CheckBookingDetailsData
        {
            public int Count { get; set; }
            public string ApiStatus { get; set; }
            public string ApiMessage { get; set; }
            public int Id { get; set; }
        }

        public class AddNewBookingDetails
        {
            public AddNewBookingDetailsData BookingDetailsData { get; set; }
        }

        public class AddNewBookingDetailsData
        {
            public int Count { get; set; }
            public DateTime Date { get; set; }
            public long BookTimeId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string CarRegistrationNo { get; set; }
            public string BookingNo { get; set; }
            public string ApiStatus { get; set; }
            public string ApiMessage { get; set; }
            public int Id { get; set; }
        }

        #endregion


        #region CARSOME FUNCTION

        public static string Scan(string str)
        {
            StringBuilder sb = new StringBuilder(str);
            try
            {
                sb.Replace("'", "''");
                sb.Replace("\\", "\\\\");

                return sb.ToString();
            }
            catch
            {
                return sb.ToString();
            }
        }

        public static string GenerateBookingNumber(string prefix, string suffix)
        {
            MySqlConnection conn = WebApiConfig.conn();

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                throw;
            }

            StringBuilder rn = new StringBuilder();
            string temp = string.Empty;

            MySqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT RunningNo FROM carsome_bookrunningtbl WHERE Code = 'BookingNo' LIMIT 1";
            MySqlDataReader idr = query.ExecuteReader();

            while (idr.Read())
            {
                temp = idr["RunningNo"].ToString();
            }

            idr.Close();
            idr.Dispose();

            if (!string.IsNullOrEmpty(temp))
            {
                #region Update Current Running No

                try
                {
                    MySqlCommand newquery = conn.CreateCommand();
                    newquery.CommandText = "UPDATE carsome_bookrunningtbl SET RunningNo = RunningNo + 1 WHERE Code = 'BookingNo'";
                    newquery.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                #endregion


                while (temp.Length < 5)
                {
                    rn = new StringBuilder("0" + temp);
                    temp = rn.ToString();
                }

                #region Prefix

                if (!string.IsNullOrEmpty(prefix))
                {
                    rn = new StringBuilder(prefix);
                }
                else
                {
                    rn = new StringBuilder(string.Empty);
                }

                #endregion

                rn.Append(temp);

                #region Suffix

                if (!string.IsNullOrEmpty(suffix))
                {
                    rn.Append(suffix);
                }
                else
                {
                    rn.Append(string.Empty);
                }

                #endregion
            }

            return rn.ToString();
        }

        #endregion


        #region CARSOME API

        [HttpGet]
        [Route("api/allbookingtime")]
        public BookingTime GetAllBookingTime()
        {
            #region ApiKey
            string apikey = string.Empty;
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("apikey"))
            {
                apikey = headers.GetValues("apikey").First();
            }
            #endregion

            MySqlConnection conn = WebApiConfig.conn();

            try
            {
                conn.Open();
            }
            catch(MySqlException ex)
            {
                throw;
            }

            List<BookingTimeData> bookingTimeData = new List<BookingTimeData>();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v")
            {
                MySqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT BookTimeId, Name FROM carsome_booktimetbl";
                MySqlDataReader idr = query.ExecuteReader();

                while (idr.Read())
                {
                    BookingTimeData _bookingTimeData = new BookingTimeData();

                    _bookingTimeData.key = Int32.Parse(idr["BookTimeId"].ToString());
                    _bookingTimeData.value = Int32.Parse(idr["BookTimeId"].ToString());
                    _bookingTimeData.label = idr["Name"].ToString();

                    bookingTimeData.Add(_bookingTimeData);
                }

                idr.Close();
                idr.Dispose();
            }

            BookingTime bookingTime = new BookingTime
            {
                BookingTimeData = new List<BookingTimeData>(bookingTimeData)
            };

            return bookingTime;
        }

        [HttpGet]
        [Route("api/checkbookingdetails/{date}/{booktimeid}")]
        public CheckBookingDetails GetCheckBookingDetails(DateTime date, long booktimeid)
        {
            #region ApiKey
            string apikey = string.Empty;
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("apikey"))
            {
                apikey = headers.GetValues("apikey").First();
            }
            #endregion

            MySqlConnection conn = WebApiConfig.conn();

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                throw;
            }

            CheckBookingDetailsData checkBookingDetails = new CheckBookingDetailsData();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v")
            {
                MySqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT COUNT(*) as Count FROM carsome_bookdetailstbl WHERE Date = '" + date.ToString("yyyy") + "-" + date.ToString("MM") + "-" + date.ToString("dd") + "' AND BookTimeId = " + booktimeid + " LIMIT 1";
                MySqlDataReader idr = query.ExecuteReader();

                while (idr.Read())
                {
                    CheckBookingDetailsData _checkBookingDetailsData = new CheckBookingDetailsData();

                    _checkBookingDetailsData.ApiStatus = "Not Available";
                    _checkBookingDetailsData.ApiMessage = "The selected slot is not available";
                    _checkBookingDetailsData.Id = 0;

                    _checkBookingDetailsData.Count = Int32.Parse(idr["Count"].ToString());

                    int dayOfWeek = (int)date.DayOfWeek;

                    if ((dayOfWeek < 6 && _checkBookingDetailsData.Count < 2) || (dayOfWeek >= 6 && _checkBookingDetailsData.Count < 4))
                    {
                        _checkBookingDetailsData.ApiStatus = "Available";
                        _checkBookingDetailsData.ApiMessage = "The selected slot is available";
                        _checkBookingDetailsData.Id = 1;
                    }

                    checkBookingDetails = _checkBookingDetailsData;
                }

                idr.Close();
                idr.Dispose();
            }

            CheckBookingDetails bookingDetails = new CheckBookingDetails
            {
                BookingDetailsData = checkBookingDetails
            };

            return bookingDetails;
        }

        [HttpPost]
        [Route("api/addnewbookingdetails")]
        public AddNewBookingDetails PostAddNewBookingDetails([FromBody]AddNewBookingDetailsData item)
        {
            #region ApiKey
            string apikey = string.Empty;
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("apikey"))
            {
                apikey = headers.GetValues("apikey").First();
            }
            #endregion

            MySqlConnection conn = WebApiConfig.conn();

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                throw;
            }

            AddNewBookingDetailsData addNewBookingDetails = new AddNewBookingDetailsData();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v")
            {
                MySqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT COUNT(*) as Count FROM carsome_bookdetailstbl WHERE CarRegistrationNo = '" + Scan(item.CarRegistrationNo) + "' AND Date >= NOW() LIMIT 1";
                MySqlDataReader idr = query.ExecuteReader();

                while (idr.Read())
                {
                    AddNewBookingDetailsData _addNewBookingDetailsData = new AddNewBookingDetailsData();

                    _addNewBookingDetailsData.ApiStatus = "Already Exist";
                    _addNewBookingDetailsData.ApiMessage = "The car registration no already exist";
                    _addNewBookingDetailsData.Id = 0;

                    _addNewBookingDetailsData.Count = Int32.Parse(idr["Count"].ToString());

                    if (_addNewBookingDetailsData.Count <= 0)
                    {
                        try
                        {
                            MySqlConnection newconn = WebApiConfig.conn();

                            try
                            {
                                newconn.Open();
                            }
                            catch (MySqlException ex)
                            {
                                throw;
                            }

                            string bookingno = GenerateBookingNumber("CS", item.CarRegistrationNo + DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm"));

                            MySqlCommand newquery = newconn.CreateCommand();
                            newquery.CommandText = "INSERT INTO carsome_bookdetailstbl (date, booktimeid, name, email, mobileno, carregistrationno, bookingno, createddate, lastupdateddate) " +
                                             "VALUES('" + item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd") + "', " + item.BookTimeId + ", '" + item.Name + "', '" + item.Email + "', '" + item.MobileNo + "', '" + item.CarRegistrationNo + "', '" + bookingno + "', '" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "', '" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "')";
                            newquery.ExecuteNonQuery();

                            _addNewBookingDetailsData.Name = item.Name;
                            _addNewBookingDetailsData.BookingNo = bookingno;
                            _addNewBookingDetailsData.ApiStatus = "Success";
                            _addNewBookingDetailsData.ApiMessage = "The booking details has been created successfully";
                            _addNewBookingDetailsData.Id = 1;
                        }
                        catch (Exception ex)
                        {
                            _addNewBookingDetailsData.ApiStatus = "Failed";
                            _addNewBookingDetailsData.ApiMessage = ex.Message;
                            _addNewBookingDetailsData.Id = 0;
                        }
                    }

                    addNewBookingDetails = _addNewBookingDetailsData;
                }

                idr.Close();
                idr.Dispose();
            }

            AddNewBookingDetails bookingDetails = new AddNewBookingDetails
            {
                BookingDetailsData = addNewBookingDetails
            };

            return bookingDetails;
        }

        [HttpGet]
        [Route("api/searchbookingdetails/{bookingno}")]
        public AddNewBookingDetails GetSearchBookingDetails(string bookingno)
        {
            #region ApiKey
            string apikey = string.Empty;
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("apikey"))
            {
                apikey = headers.GetValues("apikey").First();
            }
            #endregion

            MySqlConnection conn = WebApiConfig.conn();

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                throw;
            }

            AddNewBookingDetailsData addNewBookingDetails = new AddNewBookingDetailsData();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v")
            {
                MySqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT COUNT(*) AS Count, Date, BookTimeId, Name, Email, MobileNo, CarRegistrationNo, BookingNo FROM carsome_bookdetailstbl WHERE BookingNo = '" + Scan(bookingno) + "' LIMIT 1";
                MySqlDataReader idr = query.ExecuteReader();

                while (idr.Read())
                {
                    AddNewBookingDetailsData _addNewBookingDetailsData = new AddNewBookingDetailsData();

                    _addNewBookingDetailsData.ApiStatus = "Not Exist";
                    _addNewBookingDetailsData.ApiMessage = "The booking no is not exist";
                    _addNewBookingDetailsData.Id = 0;

                    _addNewBookingDetailsData.Count = Int32.Parse(idr["Count"].ToString());

                    if (_addNewBookingDetailsData.Count > 0)
                    {
                        _addNewBookingDetailsData.Date = DateTime.Parse(idr["Date"].ToString());
                        _addNewBookingDetailsData.BookTimeId = Int32.Parse(idr["BookTimeId"].ToString());
                        _addNewBookingDetailsData.Name = idr["Name"].ToString();
                        _addNewBookingDetailsData.Email = idr["Email"].ToString();
                        _addNewBookingDetailsData.MobileNo = idr["MobileNo"].ToString();
                        _addNewBookingDetailsData.CarRegistrationNo = idr["CarRegistrationNo"].ToString();
                        _addNewBookingDetailsData.BookingNo = idr["BookingNo"].ToString();
                        _addNewBookingDetailsData.ApiStatus = "Exist";
                        _addNewBookingDetailsData.ApiMessage = "The booking no is exist";
                        _addNewBookingDetailsData.Id = 1;
                    }

                    addNewBookingDetails = _addNewBookingDetailsData;
                }

                idr.Close();
                idr.Dispose();
            }

            AddNewBookingDetails bookingDetails = new AddNewBookingDetails
            {
                BookingDetailsData = addNewBookingDetails
            };

            return bookingDetails;
        }

        #endregion
    }
}
