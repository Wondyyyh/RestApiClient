using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public string password;
    public string firstname;
    public string lastname;
    public string created;
    public string lastseen;
    public int banned;
    public int isadmin;
    public int score;


    public static User CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<User>(jsonString);
    }

}
[System.Serializable]
public class Thread
{
    public int id;
    public string title;
    public int author;
    public string datetime;
    public int hidden;

    public static Thread CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Thread>(jsonString);
    }

}
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
public class restClient : MonoBehaviour
{
    private string baseurl = "http://localhost/bb/api";
    private bool loggedIn = false;
    //private bool objsInited = false;
    public bool ThreadView = false;

    public User[] users; // array representing copy of the database (as the backend server provides)
    public Thread[] threads; // array representing copy of the database (as the backend server provides)

    [Header("Threads")]
    public GameObject ThreadsPanel;
    public GameObject ThreadCellPrefab;
    public Dictionary<GameObject, int> ThreadID_BTN = new Dictionary<GameObject, int>();
    private GameManager gameManager;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.ViewChanged += PollByCurrView;
        PollByCurrView(gameManager.StartingView);
    }
    public void LoginInput(string name, string pass)
    {
        StartCoroutine(Login(name, pass));
    }
    private string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }
    // perform one-time login at the beginning of a connection
    IEnumerator Login(string name, string pass)
    {
        UnityWebRequest www = UnityWebRequest.Post(
            baseurl + "/login",
            "{ \"username\": \"" + name + "\", \"password\": \"" + pass + "\" }",
            "application/json"
            );
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/login");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            var text = www.downloadHandler.text;
            if (text.Contains("fail"))
            {
                Debug.Log("Login failed. Invalid credentials!");
                loggedIn = false;
            }
            else
            {
                Debug.Log("Login success!");
                loggedIn = true;
                gameManager.ChangeView(5);
            }
        }
    }
    private void PollByCurrView(int currViewIndex)
    {
        switch (currViewIndex)
        {
            case 0: //Threads                
                StartCoroutine(PollThreads());                
                break;
            case 1: //Messages                
                break;
            case 2: //Login
                break;
            case 3: //Game
                break;
            case 4: //CreateThread
                break;
            case 5: //MainMenu
                StartCoroutine(PollUsers());
                break;
            case 6: //CreateMSGView
                break;
            default:
                break;
        }
    }
    // perform asynchronnous polling of threads information every X seconds after login succesfull
    IEnumerator PollThreads()
    {
        while (!loggedIn) yield return new WaitForSeconds(1); // wait for login to happen        
        UnityWebRequest www = UnityWebRequest.Get(baseurl + "/threads");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/threads");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            var text = www.downloadHandler.text;
            Debug.Log("threads download complete: " + text);
            loggedIn = true;

            // TODO: handle messages JSON somwhow
            string jsonString = fixJson(text); // add Items: in front of the json array
                                               //Debug.Log(jsonString);
            threads = JsonHelper.FromJson<Thread>(jsonString);
            //Debug.Log(threads.Length);


            // -- if theres thread cells -> destroy them before updating threads -- //
            if (ThreadsPanel.transform.childCount != 0)
            {
                foreach (Transform obj in ThreadsPanel.transform)
                {
                    Destroy(obj.gameObject);
                }
            }

            ThreadID_BTN.Clear();
            // Create threads
            for (int i = 0; i < threads.Length; i++)
            {
                GameObject cell = Instantiate(ThreadCellPrefab, ThreadsPanel.transform);
                cell.GetComponentInChildren<TextMeshProUGUI>().text = threads[i].title;
                ThreadID_BTN.Add(cell, threads[i].id);
                //Debug.Log("Gameobject: " + cell + " | threadID: " + threads[i].id + " | threadTitle: " + threads[i].title);
            }
        }
    }

    // perform asynchronnous polling of users information every X seconds after login succesfull
    IEnumerator PollUsers()
    {
        while (!loggedIn) yield return new WaitForSeconds(1); // wait for login to happen        
        //yield return new WaitForSeconds(2); // wait for login to happen        
        UnityWebRequest www = UnityWebRequest.Get(baseurl + "/users");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            var text = www.downloadHandler.text;
            Debug.Log(baseurl + "/users");
            Debug.Log(www.error);
            Debug.Log(text);
        }
        else
        {
            var text = www.downloadHandler.text;
            Debug.Log("users download complete: " + text);
            loggedIn = true;
            string jsonString = fixJson(text); // add Items: in front of the json array               
            users = JsonHelper.FromJson<User>(jsonString); // convert json to User-array (public users) // overwrite data each update!
                                                           // SEE :
                                                           // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111
                                                           //if (!objsInited)
                                                           //{
                                                           //    GameObject userSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                                           //    Vector3 position = new Vector3(2.0f, 2.0f, 0.0f);
                                                           //    float gap = 2;
                                                           //    for (int i = 0; i < users.Length; i++)
                                                           //    {
                                                           //        GameObject newObject = (GameObject)Instantiate(userSphere, position, Quaternion.identity);
                                                           //        position.x += gap;
                                                           //        newObject.name = users[i].username;

            //    }
            //    objsInited = true;
            //}
            //else
            //{
            //    // TODO: only update users, e.g. add new user or update changed properties of existing one, need to compare existing ones
            //}
        }
    }
}
