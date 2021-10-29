using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float force,speed;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] GameObject waterSurfaceSplash;
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject landMark;
    GameObject landMarkObj;
    RaycastHit hit;


    public Transform cam;
    [SerializeField] Vector3 nextPlatform;
    bool onGround;
    float waitToLookAt;
    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        SwipeMovement();
        if (Physics.Raycast(transform.position,Vector3.down*100,out hit))
        {
            line.SetPosition(0, transform.position + Vector3.down * 0.2f);
            line.SetPosition(1, hit.point);
            if (hit.collider.tag=="Ground"|| hit.collider.tag=="Finish")
            {
                onGround = true;
                line.material.color = Color.green;
                if (!landMarkObj)
                {
                    landMarkObj = Instantiate(landMark, hit.point, Quaternion.identity);
                }
                else
                {
                    landMarkObj.transform.position = new Vector3(transform.position.x, landMarkObj.transform.position.y, transform.position.z);
                }
            }
            else
            {
                onGround = false;
                Destroy(landMarkObj);
            }
        }
    }
    [SerializeField] Vector3 Direction;
    Vector3 initialPos, mouseDir;
    public void SwipeMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPos = Input.mousePosition;
            isMoving = true;
        }
        else if (Input.GetMouseButton(0))
        {

            mouseDir = Input.mousePosition - initialPos;
            initialPos = Input.mousePosition;
            
            transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y+Input.GetAxis("Mouse X")*2.5f, 0);          
        }
        else if(onGround)
        {
            waitToLookAt -= Time.deltaTime;
            if (waitToLookAt<=0 && nextPlatform.magnitude!=0)
            {
                Direction = nextPlatform - transform.position;
                Vector3 direction = new Vector3(Direction.x, 0, Direction.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 1 * Time.deltaTime);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
            waitToLookAt = 1.5f;
        }
    }
    public bool isMoving;
    private void FixedUpdate()
    {
        if (isMoving)
        {
            // rb.AddForce(new Vector3(transform.forward.x, 0, transform.forward.z) * speed * Time.fixedDeltaTime);
            rb.velocity = new Vector3(transform.forward.x * speed, rb.velocity.y, transform.forward.z * speed );
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            Platform p = collision.collider.GetComponent<Platform>();
            nextPlatform = p.nextPlatformCenter;
            Jump(1);
            if (!p.isBreakable)
            {
                collision.gameObject.GetComponent<Animator>().Play("Jump");
            }           
        }
        else if (collision.collider.tag == "Water Platform")
        {
            Jump(2.5f);
        }
        else if (collision.collider.tag == "Finish")
        {
            GameManager.Instance.SetPlayer(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Water")
        {
            Instantiate(waterSurfaceSplash, transform.position+Vector3.up*0.5f, Quaternion.identity);
            Destroy(gameObject);
            GameManager.Instance.SetPlayer(true,true);
        }
    }
    private void Jump(float multiplier)
    {
        rb.AddForce(Vector3.up * force* multiplier * Time.fixedDeltaTime, ForceMode.Impulse);
        int r = Random.Range(0, 3);
        if (r==0)
        {
            anim.Play("Jump 1");
        }
        else if (r==1)
        {
            anim.Play("Jump 2");
        }
        else
        {
            anim.Play("Jump 3");
        }
        
    }

}
