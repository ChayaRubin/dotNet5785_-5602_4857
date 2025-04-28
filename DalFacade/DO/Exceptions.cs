
namespace DO;

[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}

[Serializable]
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}

/// <summary>
///exception to handle cases where data format issues occur, 
///such as incorrect or missing data being processed, 
///ensuring the program can handle these errors gracefully instead of crashing.
/// </summary>
[Serializable]
public class DalFormatException : Exception
{
    public DalFormatException(string? message) : base(message) { }
}

/// <summary>
/// handle cases where the user selects an option that does not exist within the valid set of choices, 
/// ensuring the program can gracefully handle invalid inputs and provide clear feedback.
/// </summary>
[Serializable]
public class DalInvalidOptionException : Exception
{
    public DalInvalidOptionException(string message) : base(message) { }
}

/// <summary>
/// Use this exception to handle and report issues specific to XML file operations in the DAL.
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string message) : base(message) { }
}


/// <summary>
/// Use this exception to handle and report issues specific to DAL configuration.
/// </summary>
[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
}

/// <summary>
/// Dal Unauthorized Access Exception
/// </summary>
[Serializable]
public class DalUnauthorizedAccessException : Exception
{
    public DalUnauthorizedAccessException(string msg) : base(msg) { }
}

/// <summary>
/// Dal Argument Exception
/// </summary>
[Serializable]
public class DalArgumentException : Exception
{
    public DalArgumentException(string msg) : base(msg) { }
}

/// <summary>
/// Dal Null Property Exception
/// </summary>
[Serializable]
public class DalNullPropertyException : Exception
{
    public DalNullPropertyException(string? message) : base(message) { }
}

/// <summary>
/// Dal No Permition Exception
/// </summary>
[Serializable]
public class DalNoPermitionException : Exception
{
    public DalNoPermitionException(string? message) : base(message) { }
}

/// <summary>
/// Dal General Database Exception
/// </summary>
[Serializable]
public class DalGeneralDatabaseException : Exception
{
    public DalGeneralDatabaseException(string? message) : base(message) { }
}

/// <summary>
/// Dal Coordination Exceprion
/// </summary>
[Serializable]
public class DalCoordinationExceprion : Exception
{
    public DalCoordinationExceprion(string? message) : base(message) { }
}

/// <summary>
/// Dal Invalid Time Unit Exceprion
/// </summary>
[Serializable]
public class DalInvalidTimeUnitException : Exception
{
    public DalInvalidTimeUnitException(string? message) : base(message) { }
}