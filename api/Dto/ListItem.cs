namespace Api.Dto;

/// <summary>
/// Represents an item in a list.
/// </summary>
/// <param name="Id">A value uniquely identifying the item.</param>
/// <param name="Description">The description of the list item.</param>
/// <param name="IsComplete">Indicates whether the list item has been completed.</param>
public record class ListItem(int Id, string Description, bool IsComplete) { }
