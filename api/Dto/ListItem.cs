namespace Api.Dto;

/// <summary>
/// Represents an item in a list.
/// </summary>
/// <param name="Description">The description of the list item.</param>
/// <param name="ImageUrl">A fully qualified URL to an image of the item. Ex: https://tse3.mm.bing.net/th?id=OIP.DzzBtp9wRuY1VocmOurZ7gHaJE&pid=Api</param>
/// <param name="IsComplete">Indicates whether the list item has been completed.</param>
public record class ListItem(string Description, string ImageUrl, bool IsComplete) { }
