namespace Api.Dto;

/// <summary>
/// Represents an item in a list to be updated in the data store.
/// </summary>
/// <param name="Description">The description of the list item as it exists in the data store. This is needed to identify the item we are going to update.</param>
/// <param name="NewDescription">The updated description to store.</param>
/// <param name="IsComplete">Indicates whether the list item has been completed.</param>
public record class ListItemUpdate(string Description, string NewDescription, bool IsComplete) { }
