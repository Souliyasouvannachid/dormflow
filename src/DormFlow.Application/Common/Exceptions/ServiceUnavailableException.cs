namespace DormFlow.Application.Common.Exceptions;

/// <summary>A required feature is temporarily unavailable; mapped to HTTP 503.</summary>
public class ServiceUnavailableException : Exception
{
    /// <summary>Creates an unavailable-service error.</summary>
    public ServiceUnavailableException()
        : base("บริการนี้ยังไม่พร้อมใช้งาน")
    {
    }

    /// <summary>Creates an error with a public explanation.</summary>
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>Creates an error with an underlying cause.</summary>
    public ServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
