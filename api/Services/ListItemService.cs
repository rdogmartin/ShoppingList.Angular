using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Dto;
using Microsoft.Azure.Cosmos;

namespace Api.Services;

/// <summary>
/// Contains functionality for CRUD operations on list items.
/// </summary>
public interface IListItemService
{
    /// <summary>
    /// Retrieves all current list items for the specified user. If the user does not exist, a new user will be created with an empty list.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>A list of items belonging to the user.</returns>
    Task<UserListItems> GetListItems(string userId);

    /// <summary>
    /// Insert the specified item at the beginning of the list and returns the full list. If an item with the same description exists, then
    /// we'll just move that one to the top of the list and mark it as NOT complete.
    /// </summary>
    /// <param name="userId">A unique identifier for the user. Ex: myname@outlook.com</param>
    /// <param name="itemToAdd">The item to add</param>
    /// <returns>The full list of items, including the newly added one.</returns>
    Task<UserListItems> AddListItem(string userId, ListItem itemToAdd);

    /// <summary>
    /// Updates the specified item in the list, adding it if it doesn't already exist, and returns the full list.
    /// If an item is being marked as complete, it is moved to the top of the complete items.
    /// If an item is being marked as NOT complete, it is moved to the top of the list.
    /// </summary>
    /// <param name="userId">A unique identifier for the user. Ex: myname@outlook.com</param>
    /// <param name="itemToUpdate">The item to update</param>
    /// <returns>The full list of items, including the newly added one.</returns>
    Task<UserListItems> UpdateListItem(string userId, ListItem itemToUpdate);
}

/// <inheritdoc />
public class ListItemService : IListItemService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public ListItemService(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer("ToDoList", "Items");
    }

    public async Task<UserListItems> GetListItems(string userId)
    {
        var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{userId}'";
        var queryDefinition = new QueryDefinition(sqlQueryText);
        var queryResultSetIterator = _container.GetItemQueryIterator<UserListItems>(queryDefinition);

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<UserListItems> response = queryResultSetIterator.ReadNextAsync().Result;
            return response.FirstOrDefault() ?? await InsertNewUser(userId);
        }

        throw new Exception("No items found.");
    }

    public async Task<UserListItems> AddListItem(string userId, ListItem itemToAdd)
    {
        var userListItems = await GetListItems(userId);

        // If the item already exists, use that instead of creating a duplicate.
        itemToAdd = userListItems.ListItems.FirstOrDefault(i => i.Description.Equals(itemToAdd.Description, StringComparison.OrdinalIgnoreCase)) ?? itemToAdd;

        // Mark as NOT complete (mostly relevant when user is adding an item that has been previously completed).
        itemToAdd = itemToAdd with { IsComplete = false };

        // Add the item to the beginning of the list, excluding any that have the same name that already exist.
        userListItems = userListItems with { ListItems = userListItems.ListItems.Where(li => !li.Description.Equals(itemToAdd.Description, StringComparison.OrdinalIgnoreCase)).Prepend(itemToAdd).ToArray() };

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    public async Task<UserListItems> UpdateListItem(string userId, ListItem itemToUpdate)
    {
        var userListItems = await GetListItems(userId);

        var dbItemToUpdate = userListItems.ListItems.FirstOrDefault(i => i.Description == itemToUpdate.Description);

        if (dbItemToUpdate == null)
        {
            return await this.AddListItem(userId, itemToUpdate);
        }

        ListItem[] updatedListItems;
        if (dbItemToUpdate.IsComplete && !itemToUpdate.IsComplete)
        {
            // Item is being unchecked. Move it to the top of the list.
            updatedListItems = userListItems.ListItems.Where(listItem => listItem != dbItemToUpdate).Prepend(itemToUpdate).ToArray();
        }
        else if (!dbItemToUpdate.IsComplete && itemToUpdate.IsComplete)
        {
            // Item is being checked. Move it to the beginning of the complete items.
            updatedListItems = userListItems.ListItems.Where(li => !li.IsComplete && li != dbItemToUpdate).Append(itemToUpdate).Concat(userListItems.ListItems.Where(li => li.IsComplete)).ToArray();
        }
        else
        {
            // Item is being updated, but not checked/unchecked. Keep it in the same place in the list.
            updatedListItems = userListItems.ListItems.Select(listItem => listItem == dbItemToUpdate ? itemToUpdate : listItem).OrderBy(listItem => listItem.IsComplete).ToArray();
        }

        userListItems = userListItems with { ListItems = updatedListItems };

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    private async Task<UserListItems> InsertNewUser(string userId)
    {
        var userListItems = new UserListItems(userId, Array.Empty<ListItem>());

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }
}
