namespace Api.Dto;

/// <summary>
/// Represents an item in a list.
/// </summary>
/// <param name="Id">A value uniquely identifying the item. Ex: ***REMOVED***</param>
/// <param name="ListItems">The list of items belonging to the user.</param>
public record class UserListItems(string Id, ListItem[] ListItems) { }
