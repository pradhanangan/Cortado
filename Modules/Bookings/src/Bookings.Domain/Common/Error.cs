namespace Bookings.Domain.Common;

public record Error(string Code, string Name)
{
    public static Error None = new(string.Empty, string.Empty);

    public static Error NullValue = new("Error.NullValue", "Null value was provided");

    /// <summary>
    /// Only for internal use, not returned to client
    /// </summary>
    public Exception? Exception { get; set; }
}