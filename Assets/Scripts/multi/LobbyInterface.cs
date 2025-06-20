using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


public class LobbyInterface : MonoBehaviour
{
    [SerializeField] GameObject lobbyTitlePrefab;
    [SerializeField] Transform uiList;
    private TextMeshProUGUI lobbyName;
    private TextMeshProUGUI playerCount;
    private List<GameObject> currLobbies;

    public void ShowLobby(string newLobbyName, string count)
    {
        Debug.Log("new lobby made");
        GameObject newLobby = Instantiate(lobbyTitlePrefab, uiList);
        lobbyName = newLobby.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        playerCount = newLobby.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        lobbyName.text = newLobbyName;
        playerCount.text = count;
    }

    public void DestroyBefore()
    {
        foreach (GameObject obj in currLobbies)
        {
            Destroy(obj);
        }
    }
}
