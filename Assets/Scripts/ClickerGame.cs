using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClickerGame : MonoBehaviour
{
    [SerializeField]private GameObject ScoreOBJ;
    public int Score = 0;
    private restClient restClient;
    private int UserID = 0;
    private string UserName = "";
    private GameManager gameManager;

    private void Start()
    {
        restClient = FindObjectOfType<restClient>();
        gameManager = FindObjectOfType<GameManager>();
        Debug.Log("Userscount: " + restClient.users.Length);
        foreach (var user in restClient.users)
        {
            if (user.username.ToLower() == LoginScript.CurrUserName.ToLower())
            {
                Debug.Log("Found score for the player: " + user.username + " -> " + user.score);
                Score = user.score;
                UserID = user.id;
                UserName = user.username;
                ScoreOBJ.GetComponent<TextMeshProUGUI>().text = Score.ToString();
                return;
            }            
        }
        Debug.Log("No MATCH were found");
    }
    
    public void CookieClicked()
    {
        Score += 10;
        ScoreOBJ.GetComponent<TextMeshProUGUI>().text = Score.ToString();
    }

    public void ReturnToMenu()
    {
        StartCoroutine(UpdateScore());
        gameManager.ChangeView(5); //MainMenu
    }

    private IEnumerator UpdateScore()
    {
        string baseurl = "http://localhost/bb/api";  

        // Create a JSON object     
        //string json = "{\"username\": " + UserName + ", \"score\": \"" + Score + "\"}";
        string json = "{\"score\": \"" + Score + "\"}";
        Debug.Log(json);

        UnityWebRequest www = new UnityWebRequest(baseurl + "/user/" + UserID, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        Debug.Log(bodyRaw);
        // Set request headers
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");


        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/user/" + UserID);
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            var text = www.downloadHandler.text;
            Debug.Log("users score update complete: " + text);
        }

        Debug.LogAssertion("Done DEAL");
    }
}
