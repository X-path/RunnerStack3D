using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        Dance,
        Die
    }
    public static PlayerController instance = null;

    public PlayerState playerState;

    [SerializeField] Animator animator;
    const string ANIM_Speed = "Speed";

    public float speed = 5f;

    Rigidbody rb;

    float dieWait = .15f;
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
    }

    void PlayerMove()
    {
        if (playerState != PlayerState.Run)
            return;

        if (SphereRaycast() == false)
            return;

        if (animator.GetBool("Running") == false)
        {
            animator.SetBool("Dance", false);
            animator.SetBool("Running", true);
        }

        rb.velocity = Vector3.forward * speed;
        Vector3 _rbPos = rb.transform.position;
        _rbPos = Vector3.Lerp(_rbPos, new Vector3(LevelCreator.instance.lastCube.transform.position.x, _rbPos.y, _rbPos.z), .7f * Time.deltaTime);
        rb.transform.position = _rbPos;
        animator.SetFloat(ANIM_Speed, speed);


    }
    void PlayerDance()
    {
        if (playerState != PlayerState.Dance)
            playerState = PlayerState.Dance;

        rb.velocity = Vector3.zero;
        animator.SetBool("Running", false);

        if (animator.GetBool("Dance") == false)
            animator.SetBool("Dance", true);

        StartCoroutine(CameraController.instance.FinishCam());

    }
    void PlayerDie()
    {
        if (dieWait > 0)
        {
            dieWait -= Time.deltaTime;
            return;
        }


        playerState = PlayerState.Die;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(Lose());

    }
    public void PlayerReset(float _posZ)
    {
        playerState = PlayerState.Idle;
        transform.position = new Vector3(0, 0, _posZ - 1.10f);
        animator.SetBool("Dance", false);

    }
    IEnumerator Lose()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool SphereRaycast()
    {
        List<Collider> hitColliders = new List<Collider>();
        hitColliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z - .1f), .25f).ToList();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == 7)//GroundCheck
            {
                dieWait = 0.15f;
                return true;
            }
            else if (hitCollider.gameObject.layer == 8&&hitCollider.transform.position.z>transform.position.z)//Finish
            {
                
                PlayerDance();
                return false;
            }
        }

        PlayerDie();
        return false;

    }
    public void OnDrawGizmosSelected()
    {
        Vector3 _pos = new Vector3(transform.position.x, transform.position.y, transform.position.z - .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pos == Vector3.zero ? _pos : _pos, 0.25f);
    }

}
