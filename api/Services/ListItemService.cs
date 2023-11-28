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
    /// Retrieves all current list items
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>A list of items.</returns>
    Task<UserListItems> GetListItems(string userId);
}

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
            return response.FirstOrDefault() ?? await InsertNewUserAsync(userId);
        }

        throw new Exception("No items found.");
    }

    private async Task<UserListItems> InsertNewUserAsync(string userId)
    {
        var userListItems = new UserListItems(userId, Array.Empty<ListItem>());

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    //public UserListItems GetListItems(string userId)
    //{
    //    List<ListItem> items = new()
    //    {
    //        new ListItem(1, "Bread", false),
    //        new ListItem(2, "Carrots", false),
    //        new ListItem(3, "Shampoo", false),
    //        new ListItem(4, $"Shoes for {userId}", false),
    //    };

    //    return items;
    //}

    //private void InsertListItemIntoCosmos(string id)
    //{
    //    var userListItems = new UserListItems(id,
    //        new ListItem[]
    //        {
    //            new(1, "Carrots", false),
    //            new(3, "Shampoo", false),
    //            new(4, $"Shoes for Unknown", false),
    //        });

    //    var container = _cosmosClient.GetContainer("ToDoList", "Items");
    //    var response = container.CreateItemAsync(userListItems).Result;
    //}
}
