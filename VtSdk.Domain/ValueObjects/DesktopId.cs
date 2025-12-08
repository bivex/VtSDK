using System;

namespace VtSdk.Domain.ValueObjects;

/// <summary>
/// Represents a unique identifier for a virtual desktop.
/// </summary>
public sealed class DesktopId : IEquatable<DesktopId>
{
    /// <summary>
    /// Gets the underlying GUID value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopId"/> class with a new GUID.
    /// </summary>
    public DesktopId() : this(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopId"/> class with the specified GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    public DesktopId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a <see cref="DesktopId"/> from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <returns>A new <see cref="DesktopId"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static DesktopId FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException("Invalid GUID format", nameof(value));
        }
        return new DesktopId(guid);
    }

    /// <summary>
    /// Returns a string representation of the desktop ID.
    /// </summary>
    /// <returns>The string representation of the GUID.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is DesktopId other && Equals(other);

    /// <summary>
    /// Determines whether the specified <see cref="DesktopId"/> is equal to the current <see cref="DesktopId"/>.
    /// </summary>
    /// <param name="other">The <see cref="DesktopId"/> to compare with the current <see cref="DesktopId"/>.</param>
    /// <returns>true if the specified <see cref="DesktopId"/> is equal to the current <see cref="DesktopId"/>; otherwise, false.</returns>
    public bool Equals(DesktopId? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Determines whether two specified <see cref="DesktopId"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="DesktopId"/> to compare.</param>
    /// <param name="right">The second <see cref="DesktopId"/> to compare.</param>
    /// <returns>true if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(DesktopId? left, DesktopId? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two specified <see cref="DesktopId"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="DesktopId"/> to compare.</param>
    /// <param name="right">The second <see cref="DesktopId"/> to compare.</param>
    /// <returns>true if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(DesktopId? left, DesktopId? right) => !Equals(left, right);
}