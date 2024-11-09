namespace DO;


/// <summary>
/// Student Entity represents a student with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the student (as in national id card)</param>
/// <param name="Name">Private Name of the student</param>

public record Volunteer
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string? Password,
    string? Address,
    double? Latitude,
    double? Longitude,
    Enum Position,
    bool Active,
    double? MaxResponseDistance,
   Enum TypeOfDistince
);

