using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

/// <summary>
/// Student Entity represents a student with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the student (as in national id card)</param>
/// <param name="Name">Private Name of the Volunteer</param>
///  <param name="Phone">Personal Phone number of the Volunteer</param>
///  <param name="Email">Personal Email of the Volunteer</param>
///  <param name="Password">Personal unique password of the Volunteer(nullable).</param>
///  <param name="Address">Personal address of the Volunteer</param>
///  <param name="Latitude">Geographic latitude of the volunteer (nullable)</param>
///  <param name="Longitude">Geographic longitude of the volunteer (nullable)</param>
///  <param name="Position">Enum indicating rather it's a volunteer or head.</param>
///  <param name="Active">Indicates if the volunteer is active</param>
///  <param name="MaxResponseDistance">Maximum distance (in meters, for example) the volunteer is willing to travel to respond (nullable)</param>
///  <param name="TypeOfDistance">Enum representing the unit or type of distance (e.g., by bus, planw...)</param>
///  

public record Volunteer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public PositionEnum Position { get; set; }
    public bool Active { get; set; }
    public double? MaxResponseDistance { get; set; }
    public DistanceTypeEnum TypeOfDistance { get; set; }


    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", null, null, null, null, PositionEnum.Volunteer, false, null, DistanceTypeEnum.AirDistance) { }

    /// <summary>
    /// Custom constructor with specific values for all fields.
    /// </summary>
    public Volunteer(int MyId, string MyName, string MyPhone, string MyEmail, string? MyPassword, string? MyAddress,
    double? MyLatitude, double? MyLongitude, PositionEnum MyPosition, bool MyActive, double? MyMaxResponseDistance, DistanceTypeEnum MyTypeOfDistance/* = DistanceTypeEnum.AirDistance*/)
    {
        //if (!IsValidPassword(MyPassword))
        //{
        //    return;
        //}

        Id = MyId;
        Name = MyName;
        Phone = MyPhone;
        Email = MyEmail;
        Password = MyPassword;
        Address = MyAddress;
        Latitude = MyLatitude;
        Longitude = MyLongitude;
        Position = MyPosition;
        Active = MyActive;
        MaxResponseDistance = MyMaxResponseDistance;
        TypeOfDistance = MyTypeOfDistance;
    }

    /*public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Phone: {Phone}, Email: {Email}, Password:{Password},Active: {Active}, MaxResponseDistance: {MaxResponseDistance}";
    }*/


    //private bool IsValidPassword(string? password)
    //{
    //    if (password == null)
    //        return false;

    //    return password.Length >= 8 && password.Any(char.IsDigit) && password.Any(char.IsLetter);
    //}
}



