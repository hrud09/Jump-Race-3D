using UnityEngine;


public class AIPlayer : MonoBehaviour
{
    public float RotationSpeed, force;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;

    Vector3 Direction;
    Vector3 nextPlatform;
    public float wait=6;
    bool launch;
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }


    void FixedUpdate()
    {
        if (nextPlatform.magnitude==0 || !gameManager.gameStarted)
        {
            return;
        }

        if (wait <= 0)
        {
            Direction = nextPlatform - transform.position;
            Vector3 direction = new Vector3(Direction.x, 0, Direction.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
            if (Vector3.Angle(transform.forward, direction) <= 0.1f)
            {
                launch = true;
            }

        }
        else
        {
            wait -= Time.deltaTime;
        }


    }
    private Vector3 Velocity(Vector3 destination)
    {
        Vector3 dir = destination - transform.position; 
        float height = dir.y;
        dir.y = 0;
        float dist = dir.magnitude;
        float a = 60 * Mathf.Deg2Rad; 
        dir.y = dist * Mathf.Tan(a); 
        dist += height / Mathf.Tan(a); 


        float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized; 
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag == "Ground")
        {
            if (collision.collider.GetComponent<Platform>())
            {
                nextPlatform = collision.collider.GetComponent<Platform>().nextPlatformCenter;
                if (!collision.collider.GetComponent<Platform>().isBreakable)
                {
                    collision.gameObject.GetComponent<Animator>().Play("Jump");
                }
              
            }
           
           
            if (launch)
            {
                rb.velocity = Velocity(nextPlatform);
                wait = Random.Range(3f, 6f);
                launch = false;
            }
            else
            {
                Jump();
            }

        }
        else if (collision.collider.tag == "Finish")
        {
            anim.Play("Winner");
            gameManager.SetPlayer(false);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        { 
            Destroy(gameObject);
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * force * Time.fixedDeltaTime, ForceMode.Impulse);
        anim.Play("Jump 1");
    }

}
