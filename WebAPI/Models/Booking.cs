using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Booking
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
            public string BookingStatus { get; set; }
            public string ApiStatus { get; set; }
            public string ApiMessage { get; set; }
            public int Id { get; set; }
        }

        public class UpdateBookingDetails
        {
            public UpdateBookingDetailsData BookingDetailsData { get; set; }
        }

        public class UpdateBookingDetailsData
        {
            public DateTime Date { get; set; }
            public long BookTimeId { get; set; }
            public string BookingStatus { get; set; }
            public string ApiStatus { get; set; }
            public string ApiMessage { get; set; }
            public int Id { get; set; }
        }

        #endregion
    }
}