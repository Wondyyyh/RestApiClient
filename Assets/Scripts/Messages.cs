using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Messages : MonoBehaviour
{
    [SerializeField] private restClient restClient;
    public TextMeshProUGUI Header;
    public MSG[] Msgs;
    public GameObject MSG_cell;
    public GameObject MessagesPanel;
    public int ThreadID;

    public void SetupMessages(GameObject obj, string header)
    {
        ThreadID = restClient.ThreadID_BTN[obj];
        Header.text = header;
        StopAllCoroutines();
        StartCoroutine(PollThreadMessages(ThreadID));
    }
    [System.Serializable]
    public class MSG
    {
        public int id;
        public string thrTit;
        public string datetime;
        public string content;
        public string username;
        public int userID;
        public int thrAuth;
        public static MSG CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<MSG>(jsonString);
        }
    }
    private string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    IEnumerator PollThreadMessages(int ThreadID)
    {
        string baseurl = "http://localhost/bb/api";    
        UnityWebRequest www = UnityWebRequest.Get(baseurl + "/thread/" + ThreadID);
        yield return www.SendWebRequest();
        RemoveOldCells(); // ClearMessageView

        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/thread/" + ThreadID);
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            var text = www.downloadHandler.text;
            Debug.Log("MSGs download complete: " + text);
            string jsonString = fixJson(text); // add Items: in front of the json array                                  
            Msgs = JsonHelper.FromJson<MSG>(jsonString);
            Debug.Log(Msgs.Length);
            // Create new cells
            for (int i = 0; i < Msgs.Length; i++)
            {
                GameObject cell = Instantiate(MSG_cell, MessagesPanel.transform);
                cell.GetComponentInChildren<TextMeshProUGUI>().text = Msgs[i].username + " : " + Msgs[i].content;
            }
        }
    }
    private void RemoveOldCells() // << -- Destroy curr cells if existing
    {        
        if (MessagesPanel.transform.childCount != 0)
        {
            foreach (Transform obj in MessagesPanel.transform)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
