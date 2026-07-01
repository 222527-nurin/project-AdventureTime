using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        //move
        rb.MovePosition(transform.position + direction.normalized * speed * Time.deltaTime);

        //rotate to face movement direction
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        //animation
        animator.SetFloat("Speed", direction.magnitude);

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
    }
}
