using VtSdk.Domain.ValueObjects;

namespace VtSdk.Domain.Entities;

/// <summary>
/// Represents a virtual desktop in the Windows virtual desktop system.
/// </summary>
public class VirtualDesktop
{
    /// <summary>
    /// Gets the unique identifier of the virtual desktop.
    /// </summary>
    public DesktopId Id { get; }

    /// <summary>
    /// Gets the display name of the virtual desktop.
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// Gets the index of the virtual desktop in the system's desktop list.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this desktop is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualDesktop"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the desktop.</param>
    /// <param name="name">The display name of the desktop.</param>
    /// <param name="index">The index of the desktop.</param>
    /// <param name="isActive">Whether the desktop is currently active.</param>
    public VirtualDesktop(DesktopId id, string? name, int index, bool isActive)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name;
        Index = index;
        IsActive = isActive;
    }

    /// <summary>
    /// Updates the display name of the virtual desktop.
    /// </summary>
    /// <param name="name">The new display name.</param>
    public void UpdateName(string? name)
    {
        Name = name;
    }

    /// <summary>
    /// Updates the index of the virtual desktop.
    /// </summary>
    /// <param name="index">The new index.</param>
    public void UpdateIndex(int index)
    {
        Index = index;
    }

    /// <summary>
    /// Marks this desktop as active or inactive.
    /// </summary>
    /// <param name="isActive">Whether the desktop should be marked as active.</param>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    /// <summary>
    /// Returns a string representation of the virtual desktop.
    /// </summary>
    /// <returns>A string containing the desktop's ID, name, and index.</returns>
    public override string ToString()
    {
        var name = string.IsNullOrEmpty(Name) ? $"Desktop {Index + 1}" : Name;
        return $"{name} (ID: {Id}, Index: {Index}, Active: {IsActive})";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is VirtualDesktop other && Id.Equals(other.Id);

    /// <summary>
    /// Returns a hash code for the current object.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => Id.GetHashCode();
}