using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using TMPro;

public class ServerControl : MonoBehaviour
{
    [Header("Server Settings")]
    public string baseUrl = "http://localhost:3000";
    public string playerId = "1";

    [Header("Player Settings")]
    public GameObject playerPrefab;
    private GameObject activePlayer;
    public GameObject potionPrefab;

    [Header("UI Elements")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI inventoryText;

    void Update()
    {
        // P. fetch player data and spawn player
        if (Input.GetKeyUp(KeyCode.P))
        {
            StartCoroutine(GetRequest(baseUrl + "/getPlayerData/" + playerId, "player"));
        }

        // I: fetch inventory data
        if (Input.GetKeyUp(KeyCode.I))
        {
            bool isOpening = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isOpening);

            // toggle cursor based on if inventory is open
            Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpening;

            if (isOpening)
            {
                StartCoroutine(GetRequest(baseUrl + "/getInventory/" + playerId, "inventory"));
            }
        }

        // O: open chest and add item to database
        if (Input.GetKeyUp(KeyCode.O))
        {
            GameObject chest = GameObject.FindWithTag("Chest");
            if (chest != null && activePlayer != null)
            {
                float dist = Vector3.Distance(activePlayer.transform.position, chest.transform.position);
                if (dist < 2.5f)
                {
                    Debug.Log("Chest Opened!");
                    Instantiate(potionPrefab, chest.transform.position + Vector3.right, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Too far from chest!");
                }
            }
        }
    }

    // generic GET request handler
    IEnumerator GetRequest(string uri, string requestType)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Server Error: " + webRequest.error);
            }
            else
            {
                string rawResponse = webRequest.downloadHandler.text;
                Debug.Log("Response Received: " + rawResponse);
                HandleResponse(rawResponse, requestType);
            }
        }
    }

    void HandleResponse(string jsonRaw, string type)
    {
        JSONNode data = JSON.Parse(jsonRaw);

        if (type == "player")
        {
            if (activePlayer == null)
            {
                Vector3 spawnPos = new Vector3(0, 0.5f, 0);
                activePlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            }
        }
        else if (type == "inventory")
        {
            // Update UI text
            inventoryText.text = "Items in Bag:\n";
            JSONArray items = data.AsArray;
            if (items.Count == 0) inventoryText.text += "Empty";
            for (int i = 0; i < items.Count; i++)
            {
                inventoryText.text += items[i]["item_name"] + " x " + items[i]["quantity"] + "\n";
            }
        }
        else if (type == "chest" || type == "use" || type == "discard")
        {
            Debug.Log("Action Success: " + data["message"]);
            // tell Unity to auto fetch the updated list
            StartCoroutine(GetRequest(baseUrl + "/getInventory/" + playerId, "inventory"));
        }
    }

    // potion prefab related
    public void AddPotionToServer()
    {
        StartCoroutine(GetRequest(baseUrl + "/openChest/" + playerId, "chest"));
    }


    // for the buttons

    public void UsePotion()
    {
        Debug.Log("Using potion...");
        StartCoroutine(GetRequest(baseUrl + "/useItem/" + playerId, "use"));
    }

    public void DiscardItem()
    {
        Debug.Log("Discarding item...");
        StartCoroutine(GetRequest(baseUrl + "/discardItem/" + playerId, "discard"));
    }
}