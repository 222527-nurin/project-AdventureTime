using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorControl01 : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            if (openTrigger)
            {
                myDoor.Play("OpenDoor",0,0.0f);
                myDoor.Play("OpenDoorRight", 0, 0.0f);

            }
            else if (closeTrigger)
            {
                myDoor.Play("CloseDoor",0,0.0f);
                myDoor.Play("CloseDoorRight", 0, 0.0f);

            }
        }
    }
}
