using System;
using System.Linq;
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
    UserListItems GetListItems(string userId);
}

public class ListItemService : IListItemService
{
    private readonly CosmosClient _cosmosClient;

    public ListItemService(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public UserListItems GetListItems(string userId)
    {
        var container = _cosmosClient.GetContainer("ToDoList", "Items");
        var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{userId}'";
        var queryDefinition = new QueryDefinition(sqlQueryText);
        var queryResultSetIterator = container.GetItemQueryIterator<UserListItems>(queryDefinition);

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<UserListItems> response = queryResultSetIterator.ReadNextAsync().Result;
            UserListItems dbItem = response.First();
            return dbItem;
        }

        throw new Exception("No items found.");
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
