using System;

namespace BO
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? CurrentAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public PositionEnum Role { get; set; }
        public bool IsActive { get; set; }
        public double? MaxDistance { get; set; }
        public DistanceType TypeOfDistance { get; set; }
        public int TotalCallsHandled { get; set; }
        public int TotalCallsCanceled { get; set; }
        public int TotalExpiredCalls { get; set; }
        public CallInProgress CurrentCall { get; set; }

        // Override ToString method
        public override string ToString()
        {
            return $"Volunteer ID: {Id}\n" +
                   $"Full Name: {FullName}\n" +
                   $"Phone Number: {PhoneNumber}\n" +
                   $"Email: {Email}\n" +
                   $"Active: {IsActive}\n" +
                   $"Total Calls Handled: {TotalCallsHandled}\n" +
                   $"Total Calls Canceled: {TotalCallsCanceled}\n" +
                   $"Total Calls Expired: {TotalExpiredCalls}\n" +
                   $"Current Address: {CurrentAddress}\n" +
                   $"Latitude: {Latitude}\n" +
                   $"Longitude: {Longitude}\n" +
                   $"Role: {Role}\n" +
                   $"Max Distance: {MaxDistance}\n" +
                   $"Distance Type: {TypeOfDistance}\n" +
                   $"Current Call: {CurrentCall?.ToString() ?? "None"}";
        }
    }
}
