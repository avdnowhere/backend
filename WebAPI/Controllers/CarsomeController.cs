using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using MySql.Data.MySqlClient;
using static WebAPI.Models.Booking;

namespace WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CarsomeController : ApiController
    {
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
                query.CommandText = "SELECT COUNT(*) as Count FROM carsome_bookdetailstbl WHERE Date = '" + date.ToString("yyyy") + "-" + date.ToString("MM") + "-" + date.ToString("dd") + "' AND BookTimeId = " + booktimeid + " AND BookStatusId != (SELECT BookStatusId FROM carsome_bookstatustbl WHERE Name = 'Cancelled') LIMIT 1";
                MySqlDataReader idr = query.ExecuteReader();

                while (idr.Read())
                {
                    CheckBookingDetailsData _checkBookingDetailsData = new CheckBookingDetailsData();

                    _checkBookingDetailsData.ApiStatus = "Not Available";
                    _checkBookingDetailsData.ApiMessage = "The selected slot is not available";
                    _checkBookingDetailsData.Id = 0;

                    _checkBookingDetailsData.Count = Int32.Parse(idr["Count"].ToString());

                    int dayOfWeek = (int)date.DayOfWeek;

                    if (((dayOfWeek >= 1 && dayOfWeek <= 5) && _checkBookingDetailsData.Count < 2) || (dayOfWeek == 6 && _checkBookingDetailsData.Count < 4))
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

            MySqlConnection conn1 = WebApiConfig.conn();

            try
            {
                conn1.Open();
            }
            catch (MySqlException ex)
            {
                throw;
            }

            AddNewBookingDetailsData addNewBookingDetails = new AddNewBookingDetailsData();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v" && !string.IsNullOrEmpty(item.CarRegistrationNo))
            {
                MySqlCommand query1 = conn1.CreateCommand();
                query1.CommandText = "SELECT Name FROM carsome_booktimetbl WHERE BookTimeId = " + item.BookTimeId;
                MySqlDataReader idr1 = query1.ExecuteReader();
                bool isValid = true;

                while (idr1.Read())
                {
                    AddNewBookingDetailsData _addNewBookingDetailsData = new AddNewBookingDetailsData();

                    _addNewBookingDetailsData.ApiStatus = "Not Valid";
                    _addNewBookingDetailsData.ApiMessage = "The selected slot is not valid";
                    _addNewBookingDetailsData.Id = 0;

                    string Name = idr1["Name"].ToString();
                    string[] intName = Name.Split('.');
                    int Hour = int.Parse(intName[0]);

                    DateTime date = DateTime.Now;
                    string bookingDate = item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd");
                    string currentDate = date.Date.ToString("yyyy") + "-" + date.Date.ToString("MM") + "-" + date.Date.ToString("dd");

                    if (bookingDate == currentDate)
                    {
                        if (Hour <= date.Hour)
                        {
                            isValid = false;
                        }
                    }

                    if (isValid)
                    {
                        DateTime newDate = DateTime.Now.AddDays(21).Date;
                        DateTime newBookingDate = DateTime.ParseExact(item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                        if (newBookingDate > newDate)
                        {
                            isValid = false;
                        }
                    }

                    addNewBookingDetails = _addNewBookingDetailsData;
                }

                idr1.Close();
                idr1.Dispose();

                if (isValid)
                {
                    MySqlConnection conn2 = WebApiConfig.conn();

                    try
                    {
                        conn2.Open();
                    }
                    catch (MySqlException ex)
                    {
                        throw;
                    }

                    MySqlCommand query2 = conn2.CreateCommand();
                    query2.CommandText = "SELECT COUNT(*) as Count FROM carsome_bookdetailstbl WHERE Date = '" + item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd") + "' AND BookTimeId = " + item.BookTimeId + " AND BookStatusId != (SELECT BookStatusId FROM carsome_bookstatustbl WHERE Name = 'Cancelled') LIMIT 1";
                    MySqlDataReader idr2 = query2.ExecuteReader();

                    while (idr2.Read())
                    {
                        AddNewBookingDetailsData _addNewBookingDetailsData = new AddNewBookingDetailsData();

                        _addNewBookingDetailsData.ApiStatus = "Not Available";
                        _addNewBookingDetailsData.ApiMessage = "The selected slot is not available";
                        _addNewBookingDetailsData.Id = 0;

                        _addNewBookingDetailsData.Count = Int32.Parse(idr2["Count"].ToString());

                        int dayOfWeek = (int)item.Date.DayOfWeek;

                        if (((dayOfWeek >= 1 && dayOfWeek <= 5) && _addNewBookingDetailsData.Count < 2) || (dayOfWeek == 6 && _addNewBookingDetailsData.Count < 4))
                        {
                            MySqlConnection conn3 = WebApiConfig.conn();

                            try
                            {
                                conn3.Open();
                            }
                            catch (MySqlException ex)
                            {
                                throw;
                            }

                            MySqlCommand query3 = conn3.CreateCommand();
                            query3.CommandText = "SELECT COUNT(*) as Count FROM carsome_bookdetailstbl WHERE CarRegistrationNo = '" + Scan(item.CarRegistrationNo) + "' AND BookStatusId != (SELECT BookStatusId FROM carsome_bookstatustbl WHERE Name = 'Cancelled') AND Date >= CURDATE() LIMIT 1";
                            MySqlDataReader idr3 = query3.ExecuteReader();

                            while (idr3.Read())
                            {
                                _addNewBookingDetailsData.ApiStatus = "Already Exist";
                                _addNewBookingDetailsData.ApiMessage = "The car registration no already exist";
                                _addNewBookingDetailsData.Id = 0;

                                _addNewBookingDetailsData.Count = Int32.Parse(idr3["Count"].ToString());

                                if (_addNewBookingDetailsData.Count <= 0)
                                {
                                    try
                                    {
                                        MySqlConnection conn4 = WebApiConfig.conn();

                                        try
                                        {
                                            conn4.Open();
                                        }
                                        catch (MySqlException ex)
                                        {
                                            throw;
                                        }

                                        string bookingno = GenerateBookingNumber("CS", item.CarRegistrationNo + DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm"));

                                        MySqlCommand query4 = conn4.CreateCommand();
                                        query4.CommandText = "INSERT INTO carsome_bookdetailstbl (date, booktimeid, name, email, mobileno, carregistrationno, bookingno, bookstatusid, createddate, lastupdateddate) " +
                                                         "VALUES('" + item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd") + "', " + item.BookTimeId + ", '" + item.Name + "', '" + item.Email + "', '" + item.MobileNo + "', '" + item.CarRegistrationNo + "', '" + bookingno + "', (SELECT BookStatusId FROM (SELECT * FROM carsome_bookstatustbl) AS t WHERE Name = 'Confirmed'), '" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "', '" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "')";
                                        query4.ExecuteNonQuery();

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

                            idr3.Close();
                            idr3.Dispose();
                        }

                        addNewBookingDetails = _addNewBookingDetailsData;
                    }

                    idr2.Close();
                    idr2.Dispose();
                }
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
                query.CommandText = "SELECT COUNT(*) AS Count, Date, BookTimeId, Name, Email, MobileNo, CarRegistrationNo, BookingNo, (SELECT b.Name FROM carsome_bookstatustbl b WHERE b.BookStatusId = a.BookStatusId) AS BookingStatus FROM carsome_bookdetailstbl a WHERE BookingNo = '" + Scan(bookingno) + "' LIMIT 1";
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
                        _addNewBookingDetailsData.BookingStatus = idr["BookingStatus"].ToString();
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

        [HttpPut]
        [Route("api/updatebookingdetails/{bookingno}")]
        public UpdateBookingDetails PutUpdateBookingDetails(string bookingno, [FromBody]UpdateBookingDetailsData item)
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

            UpdateBookingDetailsData updateBookingDetails = new UpdateBookingDetailsData();

            if (!string.IsNullOrEmpty(apikey) && apikey == "SuFH7x5V2v")
            {
                UpdateBookingDetailsData _updateBookingDetailsData = new UpdateBookingDetailsData();

                try
                {
                    MySqlCommand query = conn.CreateCommand();
                    query.CommandText = "UPDATE carsome_bookdetailstbl SET BookStatusId = (SELECT BookStatusId FROM carsome_bookstatustbl WHERE Name = '" + item.BookingStatus + "'), Date = '" + item.Date.ToString("yyyy") + "-" + item.Date.ToString("MM") + "-" + item.Date.ToString("dd") + "', BookTimeId = " + item.BookTimeId + ", LastUpdatedDate = '" + DateTime.Now.ToString("yyyy-MM-dd H:mm:ss") + "' WHERE BookingNo = '" + bookingno + "'";
                    query.ExecuteNonQuery();

                    _updateBookingDetailsData.ApiStatus = "Success";
                    _updateBookingDetailsData.ApiMessage = "The booking details has been updated successfully";
                    _updateBookingDetailsData.Id = 1;
                }
                catch (Exception ex)
                {
                    _updateBookingDetailsData.ApiStatus = "Failed";
                    _updateBookingDetailsData.ApiMessage = ex.Message;
                    _updateBookingDetailsData.Id = 0;
                }

                updateBookingDetails = _updateBookingDetailsData;
            }

            UpdateBookingDetails bookingDetails = new UpdateBookingDetails
            {
                BookingDetailsData = updateBookingDetails
            };

            return bookingDetails;
        }

        #endregion
    }
}
