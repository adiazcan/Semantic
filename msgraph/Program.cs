using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .AddJsonFile("appsettings.development.json", false)
    .Build();

var scopes = new[] { "https://graph.microsoft.com/.default" };

var clientId = configuration["ClientId"];;
var tenantId = configuration["TenantId"];;
var clientSecret = configuration["ClientSecret"];;

var groups = await GetUserGroupsAsync("5ce3c08a-e0f1-4883-af6d-a1693f7546ad");

foreach (var group in groups)
{
    Console.WriteLine(group.Id);
}

async Task<List<DirectoryObject>> GetUserGroupsAsync(string userId)
{
    var options = new ClientSecretCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    };

    var clientSecretCredential = new ClientSecretCredential(
        tenantId, clientId, clientSecret, options);

    var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

    var groups = await graphClient.Users[userId].TransitiveMemberOf.GetAsync();

    if (groups == null)
    {
        return null;
    }

    var groupList = new List<DirectoryObject>();
    var pageIterator = PageIterator<DirectoryObject, DirectoryObjectCollectionResponse>
                    .CreatePageIterator(graphClient, groups, (group) => 
                    {
                        groupList.Add(group);
                        return true;
                    });

    await pageIterator.IterateAsync();

    while (pageIterator.State != PagingState.Complete)
    {
        await pageIterator.ResumeAsync();
    }

    return groupList;
}
