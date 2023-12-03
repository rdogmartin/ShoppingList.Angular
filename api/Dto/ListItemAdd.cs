namespace Api.Dto;

/// <summary>
/// Represents an item to be added to a list.
/// </summary>
/// <param name="Description">The description of the list item.</param>
public record class ListItemAdd(string Description) { }
