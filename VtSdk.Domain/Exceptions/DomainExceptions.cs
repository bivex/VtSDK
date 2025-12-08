using System.Runtime.Serialization;
using VtSdk.Domain.ValueObjects;

namespace VtSdk.Domain.Exceptions;

/// <summary>
/// Base class for all domain-related exceptions.
/// </summary>
[Serializable]
public abstract class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    protected DomainException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// Exception thrown when a virtual desktop operation fails.
/// </summary>
[Serializable]
public class DesktopOperationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopOperationException"/> class.
    /// </summary>
    public DesktopOperationException() : base("Virtual desktop operation failed.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopOperationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DesktopOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopOperationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DesktopOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopOperationException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected DesktopOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// Exception thrown when a requested virtual desktop is not found.
/// </summary>
[Serializable]
public class DesktopNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the desktop that was not found.
    /// </summary>
    public DesktopId DesktopId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopNotFoundException"/> class.
    /// </summary>
    /// <param name="desktopId">The ID of the desktop that was not found.</param>
    public DesktopNotFoundException(DesktopId desktopId)
        : base($"Virtual desktop with ID '{desktopId}' was not found.")
    {
        DesktopId = desktopId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="desktopId">The ID of the desktop that was not found.</param>
    /// <param name="message">The message that describes the error.</param>
    public DesktopNotFoundException(DesktopId desktopId, string message) : base(message)
    {
        DesktopId = desktopId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopNotFoundException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected DesktopNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// Exception thrown when attempting to perform an invalid operation on a virtual desktop.
/// </summary>
[Serializable]
public class InvalidDesktopOperationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDesktopOperationException"/> class.
    /// </summary>
    public InvalidDesktopOperationException() : base("Invalid virtual desktop operation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDesktopOperationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidDesktopOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDesktopOperationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public InvalidDesktopOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidDesktopOperationException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected InvalidDesktopOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

/// <summary>
/// Exception thrown when a requested window is not found.
/// </summary>
[Serializable]
public class WindowNotFoundException : DomainException
{
    /// <summary>
    /// Gets the handle of the window that was not found.
    /// </summary>
    public WindowHandle WindowHandle { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowNotFoundException"/> class.
    /// </summary>
    /// <param name="windowHandle">The handle of the window that was not found.</param>
    public WindowNotFoundException(WindowHandle windowHandle)
        : base($"Window with handle '{windowHandle}' was not found.")
    {
        WindowHandle = windowHandle;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="windowHandle">The handle of the window that was not found.</param>
    /// <param name="message">The message that describes the error.</param>
    public WindowNotFoundException(WindowHandle windowHandle, string message) : base(message)
    {
        WindowHandle = windowHandle;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowNotFoundException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected WindowNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}