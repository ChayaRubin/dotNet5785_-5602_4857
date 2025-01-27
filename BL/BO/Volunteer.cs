using System;
using System.Collections.Generic;
using static BO.Enums;

namespace BO
{
    public class Volunteer
    {
        public int Id { get; init; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string CurrentAddress { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public PositionEnum Role { get; set; }

        public bool IsActive { get; set; }

        public double? MaxDistance { get; set; }

        public DistanceType TypeOfDistance { get; set; }

        public int TotalCallsHandled { get; set; }

        public int TotalCallsCanceled { get; set; }

        public int TotalCallsExpired { get; set; }

        public CallInProgress CurrentCall { get; set; }

    }
}