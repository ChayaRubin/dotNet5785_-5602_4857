namespace BO;

/// <summary>
/// Exception thrown when a required property is null or missing.
/// </summary>
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid time unit is encountered.
/// </summary>
[Serializable]
public class BlInvalidTimeUnitException : Exception
{
    public BlInvalidTimeUnitException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a user attempts an unauthorized action.
/// </summary>
[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a requested entity does not exist in the system.
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an invalid format is encountered in input data.
/// </summary>
[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when a general database error occurs.
/// </summary>
[Serializable]
public class BlGeneralDatabaseException : Exception
{
    public BlGeneralDatabaseException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when an entity that is being created already exists.
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
}
