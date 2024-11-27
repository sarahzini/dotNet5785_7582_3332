﻿namespace DO;

// This exception is thrown when a Data Access Layer (DAL) entity does not exist.
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

// This exception is thrown when a Data Access Layer (DAL) entity already exists.
[Serializable]
public class DalAlreadyExistException : Exception
{
    public DalAlreadyExistException(string? message) : base(message) { }
}


// This exception is thrown when there is an issue loading or creating an XML file in the Data Access Layer (DAL).
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
