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


