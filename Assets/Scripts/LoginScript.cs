using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{

    public GameObject username;
    public GameObject password;
    private restClient restClient;
    public static string CurrUserName ="";
    //private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckInput()
    {
        //gameManager = FindObjectOfType<GameManager>();
        restClient = FindObjectOfType<restClient>();
        string name = username.GetComponent<TMP_InputField>().text;
        string pass = password.GetComponent<TMP_InputField>().text;

        if(name != "" && pass != "")
        {
            restClient.LoginInput(name, pass); 
            CurrUserName = name;
            Debug.LogAssertion(CurrUserName);
        }
    }
}
