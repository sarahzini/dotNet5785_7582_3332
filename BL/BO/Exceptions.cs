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
    public BLIncorrectPassword(string? message, Exception? innerException) : base(message, innerException)
    {
    }
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
    public BLInvalidOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
[Serializable]
public class BLAlreadyCompleted : Exception
{
    public BLAlreadyCompleted(string? message) : base(message) { }
    public BLAlreadyCompleted(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}