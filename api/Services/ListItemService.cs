using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Dto;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
    /// <returns>The full list of items as they exist in the data store immediately after the addition.</returns>
    Task<UserListItems> AddListItem(string userId, ListItemAdd itemToAdd);

    /// <summary>
    /// Updates the specified item in the list, adding it if it doesn't already exist, and returns the full list.
    /// If an item is being marked as complete, it is moved to the top of the complete items.
    /// If an item is being marked as NOT complete, it is moved to the top of the list.
    /// </summary>
    /// <param name="userId">A unique identifier for the user. Ex: myname@outlook.com</param>
    /// <param name="itemToUpdate">The item to update</param>
    /// <returns>The full list of items as they exist in the data store immediately after the update.</returns>
    Task<UserListItems> UpdateListItem(string userId, ListItemUpdate itemToUpdate);

    /// <summary>
    /// Permanently remove the item belonging to the specified user from their list.
    /// </summary>
    /// <param name="userId">A unique identifier for the user. Ex: myname@outlook.com</param>
    /// <param name="itemDescription">The name of the item to delete.</param>
    /// <returns>The full list of items as they exist in the data store immediately after the deletion.</returns>
    Task<UserListItems> DeleteListItem(string userId, string itemDescription);

}

/// <inheritdoc />
public class ListItemService : IListItemService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private readonly IConfiguration _configuration;

    public ListItemService(CosmosClient cosmosClient, IConfiguration configuration)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer("ToDoList", "Items");
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Bing Search subscription key. This should match one of the keys on the Resource Management => Keys and Endpoint tab in the Bing Search area of the Azure Portal.
    /// Ex: 1234567890abcdef1234567890abcdef
    /// </summary>
    public string BingSearchSubscriptionKey
    {
        get
        {
            var configValue = _configuration["BingSearch:SubscriptionKey"];

            if (string.IsNullOrEmpty(configValue) || configValue.StartsWith("Put actual key in secrets.json"))
            {
                throw new ConfigurationErrorsException("Missing setting 'BingSearch:SubscriptionKey'. Please specify a valid subscription key in the appSettings.json file or your Azure Functions Settings.");
            }
            return configValue;
        }
    }

    /// <summary>
    /// Gets the Bing Search endpoint URL. This should match the endpoint on the Resource Management => Endpoint tab in the Bing Search area of the Azure Portal.
    /// Ex: https://api.bing.microsoft.com/v7.0/images/search
    /// </summary>
    public string BingSearchEndpointUrl
    {
        get
        {
            var configValue = _configuration["BingSearch:EndpointUrl"];

            if (string.IsNullOrEmpty(configValue) || configValue.StartsWith("Put actual key in secrets.json"))
            {
                throw new ConfigurationErrorsException("Missing setting 'BingSearch:EndpointUrl'. Please specify a valid subscription key in the appSettings.json file or your Azure Functions Settings.");
            }
            return configValue;
        }
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

    public async Task<UserListItems> AddListItem(string userId, ListItemAdd itemToAdd)
    {
        var userListItems = await GetListItems(userId);

        // If the item already exists, use that instead of creating a duplicate.
        var listItem = userListItems.ListItems.FirstOrDefault(i => i.Description.Equals(itemToAdd.Description, StringComparison.OrdinalIgnoreCase));

        if (listItem == null)
        {
            listItem = new ListItem(itemToAdd.Description, await GetThumbnailImageUrl(itemToAdd.Description), false);
        }
        else
        {
            // Update the description (useful if the user has changed the capitalization) and mark as NOT complete
            // (useful when user is adding an item that has been previously completed).
            listItem = listItem with { Description = itemToAdd.Description, IsComplete = false };
        }

        // Add the item to the beginning of the list, excluding any that have the same name that already exist.
        userListItems = userListItems with { ListItems = userListItems.ListItems.Where(li => !li.Description.Equals(itemToAdd.Description, StringComparison.OrdinalIgnoreCase)).Prepend(listItem).ToArray() };

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    public async Task<UserListItems> UpdateListItem(string userId, ListItemUpdate itemToUpdate)
    {
        var userListItems = await GetListItems(userId);

        var dbItemToUpdate = userListItems.ListItems.FirstOrDefault(i => i.Description.Equals(itemToUpdate.Description, StringComparison.OrdinalIgnoreCase));

        if (dbItemToUpdate == null)
        {
            return await this.AddListItem(userId, new ListItemAdd(itemToUpdate.Description));
        }

        ListItem[] updatedListItems;
        if (dbItemToUpdate.IsComplete && !itemToUpdate.IsComplete)
        {
            // Item is being unchecked. Move it to the top of the list.
            updatedListItems = userListItems.ListItems
                .Where(listItem => listItem != dbItemToUpdate)
                .Prepend(dbItemToUpdate with { IsComplete = itemToUpdate.IsComplete })
                .ToArray();
        }
        else if (!dbItemToUpdate.IsComplete && itemToUpdate.IsComplete)
        {
            // Item is being checked. Move it to the beginning of the complete items.
            updatedListItems = userListItems.ListItems
                .Where(li => !li.IsComplete && li != dbItemToUpdate)
                .Append(dbItemToUpdate with { IsComplete = itemToUpdate.IsComplete })
                .Concat(userListItems.ListItems.Where(li => li.IsComplete))
                .ToArray();
        }
        else
        {
            // Item is being updated, but not checked/unchecked. Keep it in the same place in the list and update the description & image URL.
            var thumbnailUrl = await GetThumbnailImageUrl(itemToUpdate.NewDescription);
            updatedListItems = userListItems.ListItems
                .Select(listItem => listItem == dbItemToUpdate ? dbItemToUpdate with { Description = itemToUpdate.NewDescription, ImageUrl = thumbnailUrl } : listItem) // Update the description and image URL of the item we're updating.
                .GroupBy(listItem => new { Item1 = listItem.Description.ToUpperInvariant(), Item2 = listItem.ImageUrl.ToUpperInvariant() })
                .Select(g => g.First()) // This .GroupBy and .Select removes any duplicates (e.g. when renaming an item to match an existing one).
                .OrderBy(listItem => listItem.IsComplete)
                .ToArray();
        }

        userListItems = userListItems with { ListItems = updatedListItems };

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    public async Task<UserListItems> DeleteListItem(string userId, string itemDescription)
    {
        var userListItems = await GetListItems(userId);

        userListItems = userListItems with
        {
            ListItems = userListItems.ListItems.Where(listItem => !listItem.Description.Equals(itemDescription, StringComparison.OrdinalIgnoreCase)).ToArray()
        };

        ItemResponse<UserListItems> response = await _container.UpsertItemAsync(
            item: userListItems
        );

        return response.Resource;
    }

    private async Task<string> GetThumbnailImageUrl(string itemDescription)
    {
        // More info: https://learn.microsoft.com/en-us/bing/search-apis/bing-image-search/reference/query-parameters
        var uriQuery = $"{BingSearchEndpointUrl}?q={Uri.EscapeDataString(itemDescription)}&count=1";

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", BingSearchSubscriptionKey);
        HttpResponseMessage response = await client.GetAsync(uriQuery);
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        dynamic parsedJson = JsonConvert.DeserializeObject(json)!;
        var thumbnailUrl = parsedJson.value[0].thumbnailUrl;

        return thumbnailUrl;
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
