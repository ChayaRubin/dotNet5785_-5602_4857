using System;
using static BO.Enums;

namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; init; }
        public CallTypeEnum CallType { get; set; }
        public string? Description { get; set; }
        public string FullAddress { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime? MaxCloseTime { get; set; }
        public double DistanceFromVolunteer { get; set; }

        /// <summary>
        /// Provides a string representation of the object for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the OpenCallInList object.</returns>
        public override string ToString()
        {
            return $"OpenCallInList:\n" +
                   $"  Id: {Id}\n" +
                   $"  CallType: {CallType}\n" +
                   $"  Description: {(string.IsNullOrEmpty(Description) ? "None" : Description)}\n" +
                   $"  FullAddress: {FullAddress}\n" +
                   $"  OpenTime: {OpenTime}\n" +
                   $"  MaxCloseTime: {(MaxCloseTime.HasValue ? MaxCloseTime.ToString() : "None")}\n" +
                   $"  DistanceFromVolunteer: {DistanceFromVolunteer:F2} km";
        }
    }
}
