using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CreateMessage : MonoBehaviour
{
    public GameObject inputfield;
    public GameObject writing;
    private GameManager gameManager;
    private restClient restClient;
    public void UpdateText()
    {
        writing.GetComponent<TextMeshProUGUI>().text =
        inputfield.GetComponentInChildren<TMP_InputField>().text;

    }
    public void SubmitMSG()
    {
        StartCoroutine(PostMSG());
    }
    private IEnumerator PostMSG()
    {
        string baseurl = "http://localhost/bb/api";
        string msgTitle = "Kommentti";
        string message = inputfield.GetComponentInChildren<TMP_InputField>().text;
        int authorID = AuthorId();
        int threadID = FindObjectOfType<Messages>().ThreadID; // Get id from current messages threadID

        // Create a JSON object     
        string json = "{\"thread\": " + threadID + ", \"title\": \"" + msgTitle + "\", " +
                      "\"content\": \"" + message + "\", \"author\": " + authorID + "}";

        UnityWebRequest www = new UnityWebRequest(baseurl + "/msgs", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        // Set request headers
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");


        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/msgs");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else Debug.LogAssertion("Posted a message with content: " + message);

    }

    private int AuthorId()
    {
        int id = 0;
        string currUser = LoginScript.CurrUserName;
        Debug.Log(currUser);
        restClient = FindObjectOfType<restClient>();
        Debug.Log("Users array lenght: " + restClient.users.Length);
        foreach(var item in restClient.users) 
        { 
            if(item.username.ToLower() == currUser.ToLower())
            {
                id = item.id;
                Debug.Log("Found MATCH with id: " + id);
                return id;
            }
            else Debug.Log(currUser + " | " + item.username);
        }
        Debug.LogAssertion("No matches were found, return with zero");
        return id;
    }
}
