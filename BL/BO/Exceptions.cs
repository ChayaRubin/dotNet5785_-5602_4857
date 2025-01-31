namespace BO;


    [Serializable]
    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
    }


    [Serializable]
public class BlInvalidTimeUnitException : Exception
{
	public BlInvalidTimeUnitException(string? message) : base(message) { }
}


[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
}

[Serializable]
public class BlGeneralDatabaseException : Exception
{
    public BlGeneralDatabaseException(string? message) : base(message) { }
}