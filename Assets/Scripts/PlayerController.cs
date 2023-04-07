using System.Collections;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody rigidbodyPers;
    Vector3 startGamePosition;
    Quaternion startGameRotation;
    Coroutine movingCorutine;
    bool isMoving = false;
    bool isJumping = false;
    [SerializeField] float laneChangeSpeed = 15;
    [SerializeField] float jumpPover = 10;
    [SerializeField] float jumpGravity = -40;
    float laneOffset = 2f;
    float realGravity = -9.8f;
    float pointStart;
    float pointFinish;
    float lastVectorX;
    public static event Action _resetLevel;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbodyPers = GetComponent<Rigidbody>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
        SwipeManager.instance.MoveEvent += MovePlayer;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && pointFinish > -laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.D) && pointFinish < laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W) && isJumping == false)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.S) )
        {
            Roll();
        }
    }

    void MovePlayer(bool[] swipes)
    {
        if (swipes[(int)SwipeManager.Direction.Left] && pointFinish > -laneOffset)
        {
            JumpLow();
            MoveHorizontal(-laneChangeSpeed);
            
        }
        if (swipes[(int)SwipeManager.Direction.Right] && pointFinish < laneOffset)
        {
            JumpLow();
            MoveHorizontal(laneChangeSpeed);
            
        }
        if (swipes[(int)SwipeManager.Direction.Up] && isJumping == false)
        {
            Jump();
        }
        if (swipes[(int)SwipeManager.Direction.Down] && isJumping == false)
        {
            Roll();
        }
    }
    void Jump()
    {
        isJumping = true;
        animator.SetTrigger("Jump");
        rigidbodyPers.AddForce(Vector3.up * jumpPover, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCorutine());

    }
    void JumpLow()
    {
        animator.SetTrigger("JumpLow");
    }
    void Roll()
    {
        animator.SetTrigger("Roll");
    }
    IEnumerator StopJumpCorutine()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        }
        while (rigidbodyPers.velocity.y != 0);
        isJumping = false;
        Physics.gravity = new Vector3(0, realGravity, 0);
    }
    void MoveHorizontal(float speed)
    {
        animator.applyRootMotion = false; 
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;
        if (isMoving)
        {
            StopCoroutine(movingCorutine);
            isMoving = false;

        }
        movingCorutine = StartCoroutine(MoveCorutine(speed));


    }

    IEnumerator MoveCorutine(float vectorX)
    {
        isMoving = true;
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();
            rigidbodyPers.velocity = new Vector3(vectorX, rigidbodyPers.velocity.y, 0);
            lastVectorX = vectorX;
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rigidbodyPers.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        if (transform.position.y > 0.5)
        {
            rigidbodyPers.velocity = new Vector3(rigidbodyPers.velocity.x, -10, rigidbodyPers.velocity.z);
            
        }
        
        isMoving = false;
    }

    public void StartLevel()
    {
        RoadGenerator.Instance.StartLevel();

    }
    public void StartGame()
    {
        animator.SetTrigger("Run");

    }

    public void ResetGame()
    {
        rigidbodyPers.velocity = Vector3.zero;
        pointStart = 0;
        pointFinish = 0;
        animator.SetTrigger("Idel");
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
        RoadGenerator.Instance.ResetLevel();
        _resetLevel?.Invoke();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rump")
        {
            rigidbodyPers.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if (other.gameObject.tag == "Loose")
        {
            ResetGame();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Rump")
        {
            rigidbodyPers.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (rigidbodyPers.velocity.x == 0)
            {
                rigidbodyPers.velocity = new Vector3(rigidbodyPers.velocity.x, 0, rigidbodyPers.velocity.z);
            }
        }
        if (collision.gameObject.tag == "NotLose")
        {
            MoveHorizontal(-lastVectorX);
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "RumpPlaine")
        {
            if (rigidbodyPers.velocity.x == 0 && isJumping == false)
            {
                rigidbodyPers.velocity = new Vector3(rigidbodyPers.velocity.x, -10, rigidbodyPers.velocity.z);
            }
        }

    }
}
