namespace BO;

[Serializable]
public class BLDoesNotExistException : Exception
{
    public BLDoesNotExistException(string? message) : base(message) { }
}

[Serializable]
public class BLAlreadyExistException : Exception
{
    public BLAlreadyExistException(string? message) : base(message) { }
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