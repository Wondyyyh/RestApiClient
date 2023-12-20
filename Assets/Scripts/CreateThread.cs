using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CreateThread : MonoBehaviour
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
    public void SubmitThread()
    {
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(PostThread());
        gameManager.ChangeView(0); // Change back to threads
    }

    private IEnumerator PostThread()
    {
        string baseurl = "http://localhost/bb/api";
        string threadTitle = inputfield.GetComponentInChildren<TMP_InputField>().text;        

        // Create a JSON object with title and author ID
        string json = "{\"title\": \"" + threadTitle + "\", \"author\": " + AuthorId() + "}"; //<<<---- AND HERE

        UnityWebRequest www = new UnityWebRequest(baseurl + "/threads", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        // Set request headers
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");


        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/threads");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else Debug.Log("Posted a thread with title: " + threadTitle);

    }
    private int AuthorId()
    {
        int id = 0;
        string currUser = LoginScript.CurrUserName;
        restClient = FindObjectOfType<restClient>();
        foreach (var item in restClient.users)
        {
            if (item.username.ToLower() == currUser.ToLower())
            {
                id = item.id;
                return id;
            }
            else Debug.Log(currUser + " | " + item.username);
        }
        return id;
    }
}
