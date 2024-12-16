namespace BO;

[Serializable]
public class BLDoesNotExistException : Exception
{
    public BLDoesNotExistException(string? message) : base(message) { }

    public BLDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

[Serializable]
public class BLAlreadyExistException : Exception
{
    public BLAlreadyExistException(string? message) : base(message) { }
    public BLAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

[Serializable]
public class BLIncorrectPassword : Exception
{
    public BLIncorrectPassword(string? message) : base(message) { }
    
}

[Serializable]
public class BLFormatException : Exception
{
    public BLFormatException(string? message) : base(message) { }
}

[Serializable]
public class BLInvalidOperationException : Exception
{
    public BLInvalidOperationException(string? message) : base(message) { }
}

[Serializable]
public class BLXMLFileLoadCreateException : Exception
{
    public BLXMLFileLoadCreateException(string? message) : base(message) { }
    public BLXMLFileLoadCreateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}