using System.Collections.Generic;
using Api.Dto;

namespace Api.Services;

/// <summary>
/// Contains functionality for CRUD operations on list items.
/// </summary>
public interface IListItemService
{
    /// <summary>
    /// Retrieves all current list items
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>A list of items.</returns>
    IList<ListItem> GetListItems(string userId);
}

public class ListItemService : IListItemService
{
    public IList<ListItem> GetListItems(string userId)
    {
        List<ListItem> items = new()
        {
            new ListItem(1, "Bread", false),
            new ListItem(2, "Carrots", false),
            new ListItem(3, "Shampoo", false),
            new ListItem(4, $"Shoes for {userId}", false),
        };

        return items;
    }
}
