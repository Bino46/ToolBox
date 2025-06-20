using UnityEngine;
using PlayFab;
using PlayFab.Multiplayer;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class HelloMultiplayerLogic : MonoBehaviour
{
    void Start()
    {
        // // Log into playfab
        // var request = new LoginWithCustomIDRequest { CustomId = UnityEngine.Random.value.ToString(), CreateAccount = true };
        // PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        string entityId = "000"; // PlayFab user's entity Id
        string entityType = "title_player_account"; // PlayFab user's entity type

        PlayFabMultiplayer.OnLobbyCreateAndJoinCompleted += this.PlayFabMultiplayer_OnLobbyCreateAndJoinCompleted;
        PlayFabMultiplayer.OnLobbyDisconnected += this.PlayFabMultiplayer_OnLobbyDisconnected;
        PlayFabMultiplayer.OnMatchmakingTicketCompleted += PlayFabMultiplayer_OnMatchmakingTicketCompleted;
        PlayFabMultiplayer.OnLobbyJoinCompleted += this.PlayFabMultiplayer_OnLobbyJoinCompleted;
        PlayFabMultiplayer.OnLobbyFindLobbiesCompleted += this.PlayFabMultiplayer_OnLobbyFindLobbiesCompleted;
        PlayFabMultiplayer.OnMatchmakingTicketStatusChanged += PlayFabMultiplayer_OnMatchmakingTicketStatusChanged;
        PlayFabMultiplayer.OnMatchmakingTicketCompleted += PlayFabMultiplayer_OnMatchmakingTicketCompleted;

        PFEntityKey entityKey = new PFEntityKey(entityId, entityType); // PlayFab user's entity key
        PlayFabMultiplayer.SetEntityToken(entityKey, result.AuthenticationContext.EntityToken);

        var createConfig = new LobbyCreateConfiguration()
        {
            MaxMemberCount = 10,
            OwnerMigrationPolicy = LobbyOwnerMigrationPolicy.Automatic,
            AccessPolicy = LobbyAccessPolicy.Public
        };

        createConfig.LobbyProperties["Prop1"] = "Value1";
        createConfig.LobbyProperties["Prop2"] = "Value2";

        var joinConfig = new LobbyJoinConfiguration();
        joinConfig.MemberProperties["MemberProp1"] = "MemberValue1";
        joinConfig.MemberProperties["MemberProp2"] = "MemberValue2";

        PlayFabMultiplayer.CreateAndJoinLobby(
            entityKey,
            createConfig,
            joinConfig);

        LobbySearchConfiguration config = new LobbySearchConfiguration();
        PlayFabMultiplayer.FindLobbies(entityKey, config);

        string connectionString = "<your lobby connection string>";

        PlayFabMultiplayer.JoinLobby(
                entityKey,
                connectionString,
                null);

        PFEntityKey remoteEntityKey = new PFEntityKey(entityId, entityType); // another PlayFab user's entity key
        string remoteUserAttributesJson = "001"; // JSON string with another PlayFab user's attributes for matchmaking

        List<MatchUser> localUsers = new List<MatchUser>();
        localUsers.Add(new MatchUser(entityKey, remoteUserAttributesJson));

        List<PFEntityKey> membersToMatchWith = new List<PFEntityKey>();
        membersToMatchWith.Add(remoteEntityKey);

        PlayFabMultiplayer.CreateMatchmakingTicket(
            localUsers,
            "QuickMatchQueueName",
            membersToMatchWith);

        string ticketId = "0"; // Matchmaking ticket obtained from the client that created the ticket

        // Create JSON string with PlayFab user's attributes for matchmaking. This will need to be shared with other clients taking part in matchmaking
        string uniqueId = System.Guid.NewGuid().ToString();
        string userAttributesJson = "{\"MatchIdentifier\": \"" + uniqueId + "\"}";

        PlayFabMultiplayer.JoinMatchmakingTicketFromId(
            new MatchUser(entityKey, userAttributesJson),
            ticketId,
            "QuickMatchQueueName",
            new List<PFEntityKey>());

    }

    private void OnLoginFailure(PlayFabError error)
    {
    }

    private void PlayFabMultiplayer_OnLobbyCreateAndJoinCompleted(Lobby lobby, int result)
    {
        if (LobbyError.SUCCEEDED(result))
        {
            // Lobby was successfully created
            Debug.Log(lobby.ConnectionString);
        }
        else
        {
            // Error creating a lobby
            Debug.Log("Error creating a lobby");
        }
    }

    private void PlayFabMultiplayer_OnLobbyDisconnected(Lobby lobby)
    {
        // Disconnected from lobby
        Debug.Log("Disconnected from lobby!");
    }

    private void PlayFabMultiplayer_OnLobbyJoinCompleted(Lobby lobby, PFEntityKey newMember, int reason)
    {
        if (LobbyError.SUCCEEDED(reason))
        {
            // Successfully joined a lobby
            Debug.Log("Joined a lobby");
        }
        else
        {
            // Error joining a lobby
            Debug.Log("Error joining a lobby");
        }
    }

    private void PlayFabMultiplayer_OnLobbyFindLobbiesCompleted(
        IList<LobbySearchResult> searchResults,
        PFEntityKey newMember,
        int reason)
    {
        if (LobbyError.SUCCEEDED(reason))
        {
            // Successfully found lobbies
            Debug.Log("Found lobbies");

            // Iterate through lobby search results
            foreach (LobbySearchResult result in searchResults)
            {
                // Examine a search result
            }
        }
        else
        {
            // Error finding lobbies
            Debug.Log("Error finding lobbies");
        }
    }

    private void PlayFabMultiplayer_OnMatchmakingTicketStatusChanged(MatchmakingTicket ticket)
    {
        // Store and print matchmaking ticket
        Debug.Log(ticket.TicketId);

        // Examine matchmaking ticket status
        Debug.Log(ticket.Status);

        // Share matchmaking ticket with other clients taking part in matchmaking

        // Examine ticket
    }

    private void PlayFabMultiplayer_OnMatchmakingTicketCompleted(MatchmakingTicket ticket, int result)
{
    if (LobbyError.SUCCEEDED(result))
    {
        // Successfully completed matchmaking ticket
        Debug.Log("Completed matchmaking ticket");

        // Examine matchmaking details
        MatchmakingMatchDetails details = ticket.GetMatchDetails();
    }
    else
    {
        // Error completing a matchmaking ticket
        Debug.Log("Error completing a matchmaking ticket");
    }
}

}
