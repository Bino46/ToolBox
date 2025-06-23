// using UnityEngine;
// using PlayFab;
// using PlayFab.Multiplayer;
// using PlayFab.ClientModels;
// using System.Collections.Generic;
// using PlayFab.Internal;

// public class PlayfabManager : MonoBehaviour
// {
//     [SerializeField] LobbyInterface lobbyInterface;
//     string deviceInfo;
//     PFEntityKey entityKey;
//     Lobby currLobby;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         deviceInfo = "3";
//         // Log into playfab
//         var request = new LoginWithCustomIDRequest { CustomId = deviceInfo, CreateAccount = true };
//         PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
//     }

//     private void OnLoginSuccess(LoginResult result)
//     {
//         string entityType = "title_player_account"; // PlayFab user's entity type

//         PFEntityKey newKey = new PFEntityKey(result.EntityToken.Entity.Id, entityType);

//         entityKey = newKey;

//         PlayFabMultiplayer.SetEntityToken(entityKey, result.EntityToken.EntityToken);

//         var createConfig = new LobbyCreateConfiguration()
//         {
//             MaxMemberCount = 10,
//             OwnerMigrationPolicy = LobbyOwnerMigrationPolicy.Automatic,
//             AccessPolicy = LobbyAccessPolicy.Public
//         };
//         PlayFabMultiplayer.OnLobbyDisconnected += PlayFabMultiplayer_OnLobbyDisconnected;

//         createConfig.LobbyProperties["Prop1"] = "Value1";
//         createConfig.LobbyProperties["Prop2"] = "Value2";

//         var joinConfig = new LobbyJoinConfiguration();
//         joinConfig.MemberProperties["MemberProp1"] = "MemberValue1";
//         joinConfig.MemberProperties["MemberProp2"] = "MemberValue2";

//         PlayFabMultiplayer.OnLobbyCreateAndJoinCompleted += PlayFabMultiplayer_OnLobbyCreateAndJoinCompleted;
//         PlayFabMultiplayer.CreateAndJoinLobby(entityKey, createConfig, joinConfig);

//         Debug.Log("make lobby " + entityKey.Id);

//         PlayFabMultiplayer.OnLobbyFindLobbiesCompleted += PlayFabMultiplayer_OnLobbyFindLobbiesCompleted;
//         LobbySearchConfiguration config = new LobbySearchConfiguration();
//         PlayFabMultiplayer.FindLobbies(entityKey, config);
//     }

//     private void PlayFabMultiplayer_OnLobbyCreateAndJoinCompleted(Lobby lobby, int result)
//     {
//         Debug.Log("here");
//         if (LobbyError.SUCCEEDED(result))
//         {
//             Debug.Log("lobby created");
//             currLobby = lobby;
//         }
//         else
//         {
//             // Error creating a lobby
//             Debug.Log("Error creating a lobby");
//         }
//     }

//     private void PlayFabMultiplayer_OnLobbyDisconnected(Lobby lobby)
//     {
//         // Disconnected from lobby
//         Debug.Log("Disconnected from lobby!");
//     }

//     private void OnLoginFailure(PlayFabError error)
//     {
//     }

//     private void PlayFabMultiplayer_OnLobbyFindLobbiesCompleted(IList<LobbySearchResult> searchResults, PFEntityKey newMember, int reason)
//     {
//         if (LobbyError.SUCCEEDED(reason))
//         {
//             // Successfully found lobbies
//             Debug.Log("Found lobbies " + reason);

//             if (searchResults.Count > 0)
//                 lobbyInterface.DestroyBefore();

//             // Iterate through lobby search results
//             foreach (LobbySearchResult result in searchResults)
//             {
//                 lobbyInterface.ShowLobby(result.LobbyId, result.CurrentMemberCount.ToString());
//             }
//         }
//         else
//         {
//             // Error finding lobbies
//             Debug.Log("Error finding lobbies");
//         }
//     }

//     private void PlayFabMultiplayer_OnLobbyJoinCompleted(Lobby lobby, PFEntityKey newMember, int reason)
//     {
//         if (LobbyError.SUCCEEDED(reason))
//         {
//             // Successfully joined a lobby
//             Debug.Log("Joined a lobby");
//         }
//         else
//         {
//             // Error joining a lobby
//             Debug.Log("Error joining a lobby");
//         }
//     }

//     //public
//     public void MakeNewLobby()
//     {
//         var createConfig = new LobbyCreateConfiguration()
//         {
//             MaxMemberCount = 10,
//             OwnerMigrationPolicy = LobbyOwnerMigrationPolicy.Automatic,
//             AccessPolicy = LobbyAccessPolicy.Public
//         };

//         createConfig.LobbyProperties["Prop1"] = "Value1";
//         createConfig.LobbyProperties["Prop2"] = "Value2";

//         var joinConfig = new LobbyJoinConfiguration();
//         joinConfig.MemberProperties["MemberProp1"] = "MemberValue1";
//         joinConfig.MemberProperties["MemberProp2"] = "MemberValue2";

//         PlayFabMultiplayer.OnLobbyDisconnected += PlayFabMultiplayer_OnLobbyDisconnected;
//         PlayFabMultiplayer.OnLobbyCreateAndJoinCompleted += PlayFabMultiplayer_OnLobbyCreateAndJoinCompleted;
//         PlayFabMultiplayer.CreateAndJoinLobby(entityKey, createConfig, joinConfig);

//         Debug.Log("make lobby " + entityKey.Id);
//     }

//     public void RefreshLobbies()
//     {
//         Debug.Log("refresh");

//         LobbySearchConfiguration config = new LobbySearchConfiguration();
//         PlayFabMultiplayer.FindLobbies(entityKey, config);
//     }

//     public void JoinLobby()
//     {
//         Debug.Log("Join " + currLobby.ConnectionString);
//         PlayFabMultiplayer.OnLobbyJoinCompleted += PlayFabMultiplayer_OnLobbyJoinCompleted;
//         PlayFabMultiplayer.JoinLobby(
//                 entityKey,
//                 currLobby.ConnectionString,
//                 null);
//     }

//     public void QuitLobby()
//     {
//         Debug.Log("Leave " + entityKey);
//         currLobby.Leave(entityKey);
//     }
// }
