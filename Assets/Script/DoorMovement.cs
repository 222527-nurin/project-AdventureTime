using System.Collections;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 3f;

    [Header("Interaction")]
    public GameObject interactText;

    private bool playerNearby = false;
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Coroutine currentCoroutine;

    void Start()
    {
        closedRotation = transform.rotation;

        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(ToggleDoor());
        }
    }

    IEnumerator ToggleDoor()
    {
        Quaternion targetRotation;

        if (isOpen)
        {
            // Close the door
            targetRotation = closedRotation;
        }
        else
        {
            // Find the player
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            // Determine which side of the door the player is on
            Vector3 direction = player.position - transform.position;

            float dot = Vector3.Dot(transform.right, direction);

            float angle = (dot > 0) ? openAngle : -openAngle;

            targetRotation = Quaternion.Euler(
                closedRotation.eulerAngles + new Vector3(0, angle, 0));
        }

        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed);

            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (interactText != null)
                interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (interactText != null)
                interactText.SetActive(false);
        }
    }
}