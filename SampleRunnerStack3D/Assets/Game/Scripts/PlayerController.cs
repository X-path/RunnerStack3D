using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        Dance
    }
    public static PlayerController instance = null;

    public PlayerState playerState;

    [SerializeField] Animator animator;
    const string ANIM_Speed = "Speed";

    public float speed = 5f;

    Rigidbody rb;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    void Start()
    {
        FindAll();
    }
    void FindAll()
    {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        PlayerMove();
        PlayerDance();
    }

    void PlayerMove()
    {
        if (playerState != PlayerState.Run)
            return;

        if (animator.GetBool("Running") == false)
        {
            animator.SetBool("Dance", false);
            animator.SetBool("Running", true);
        }

        rb.velocity = Vector3.forward * speed;
       
        animator.SetFloat(ANIM_Speed, speed);


    }
    void PlayerDance()
    {
        if (playerState != PlayerState.Dance)
            return;

        rb.velocity = Vector3.zero;
        animator.SetBool("Running", false);

        if (animator.GetBool("Dance") == false)
            animator.SetBool("Dance", true);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

}
