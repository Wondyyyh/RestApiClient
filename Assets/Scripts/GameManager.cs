using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] views;
    public int StartingView = 0;
    public event Action<int> ViewChanged;

    // 0 = Threads
    // 1 = Messages
    // 2 = Login
    // 3 = Game
    // 4 = Create Thread
    // 3 = Main Menu

    private void Awake()
    {      
        foreach (var gameObject in views)
        {
            gameObject.SetActive(false);
           
        } 
        views[StartingView].SetActive(true);        
    }

    public void ChangeView(int viewIndex)
    {
        // Deactivate all views
        foreach (GameObject view in views)
        {
            view.SetActive(false);
        }

        // Activate the view based on the index
        if (viewIndex >= 0 && viewIndex < views.Length)
        {
            views[viewIndex].SetActive(true);
        }
        else
        {
            Debug.LogWarning("View index out of range");
        }
        ViewChanged?.Invoke(viewIndex);
    }
}
