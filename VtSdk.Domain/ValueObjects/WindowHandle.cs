using System;

namespace VtSdk.Domain.ValueObjects;

/// <summary>
/// Represents a Windows window handle (HWND).
/// </summary>
public sealed class WindowHandle : IEquatable<WindowHandle>
{
    /// <summary>
    /// Gets the underlying IntPtr value representing the window handle.
    /// </summary>
    public IntPtr Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHandle"/> class.
    /// </summary>
    /// <param name="value">The IntPtr value representing the window handle.</param>
    public WindowHandle(IntPtr value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a <see cref="WindowHandle"/> from an integer value.
    /// </summary>
    /// <param name="value">The integer representation of the window handle.</param>
    /// <returns>A new <see cref="WindowHandle"/> instance.</returns>
    public static WindowHandle FromIntPtr(IntPtr value) => new(value);

    /// <summary>
    /// Gets whether this window handle represents a valid window.
    /// </summary>
    /// <returns>true if the handle is not IntPtr.Zero; otherwise, false.</returns>
    public bool IsValid => Value != IntPtr.Zero;

    /// <summary>
    /// Returns a string representation of the window handle.
    /// </summary>
    /// <returns>The string representation of the IntPtr value.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is WindowHandle other && Equals(other);

    /// <summary>
    /// Determines whether the specified <see cref="WindowHandle"/> is equal to the current <see cref="WindowHandle"/>.
    /// </summary>
    /// <param name="other">The <see cref="WindowHandle"/> to compare with the current <see cref="WindowHandle"/>.</param>
    /// <returns>true if the specified <see cref="WindowHandle"/> is equal to the current <see cref="WindowHandle"/>; otherwise, false.</returns>
    public bool Equals(WindowHandle? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Determines whether two specified <see cref="WindowHandle"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="WindowHandle"/> to compare.</param>
    /// <param name="right">The second <see cref="WindowHandle"/> to compare.</param>
    /// <returns>true if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(WindowHandle? left, WindowHandle? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two specified <see cref="WindowHandle"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="WindowHandle"/> to compare.</param>
    /// <param name="right">The second <see cref="WindowHandle"/> to compare.</param>
    /// <returns>true if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(WindowHandle? left, WindowHandle? right) => !Equals(left, right);
}