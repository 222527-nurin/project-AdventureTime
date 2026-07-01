using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectGem : MonoBehaviour
{
    public string popupMessage = "+1 Diamond";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddGem(popupMessage);
            Destroy(gameObject);
        }
    }
}