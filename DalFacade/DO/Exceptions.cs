
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
public class DalInvalidOptionException : Exception
{
    public DalInvalidOptionException(string message) : base(message) { }
}

/// <summary>

/// </summary>
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string message) : base(message) { }
}