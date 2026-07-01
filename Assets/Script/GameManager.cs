using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int totalGems = 0;

    [Header("Popup UI")]
    public GameObject popupTextPrefab;
    public Transform uiParent;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void AddGem(string message)
    {
        totalGems++;

        ShowPopup(message);
    }

    private void ShowPopup(string message)
    {
        Debug.Log("Popup triggered: " + message);

        GameObject popup = Instantiate(popupTextPrefab, uiParent);

        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();

        if (text == null)
            Debug.LogError("TMP_Text not found in prefab!");

        text.text = message;
    }
}