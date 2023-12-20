using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ThreadClick : MonoBehaviour
{
    private Messages messages;
    private GameManager gameManager;
    private void Awake()
    {
        messages = FindObjectOfType<Messages>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public void Clickered()
    {
        string header = this.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        gameManager.ChangeView(1);
        messages.SetupMessages(this.gameObject, header);       
    }
}
