using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Platform : MonoBehaviour
{
    [SerializeField] Transform nextPlatform;
    public Vector3 nextPlatformCenter;
    GameObject nextPerfectionEffect, ownPerfectionEffect;
    public bool isTriggered;
    public bool isBreakable, isMovable;
    Rigidbody[] brokenParts;
    Collider[] colliders;
    int random;
    void Start()
    {
        if (isBreakable)
        {
            brokenParts = GetComponentsInChildren<Rigidbody>();
        }

        random = Random.Range(-1, 2);
        colliders = GetComponents<Collider>();
        nextPlatformCenter = nextPlatform.position;
        nextPerfectionEffect = nextPlatform.GetChild(0).gameObject;
        ownPerfectionEffect = transform.GetChild(0).gameObject;
       
     
    }

    private void Update()
    {
        if (isMovable)
        {
            transform.position = transform.position + new Vector3(Mathf.Sin(Time.time)* random * 0.015f, 0.0f, 0.0f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag=="Player")
        {
             float dis = Vector3.Distance(collision.collider.transform.position, transform.position);
             if (dis<=0.5f)
                {
                    GameManager.Instance.ShowReward(0);
                    StartCoroutine(SlowMotion());
                }
                else if (dis>0.5f && dis<=1f)
                {
                    GameManager.Instance.ShowReward(1);
                }
                if (isBreakable)
                {
                    Break();
                }
           
          
            nextPerfectionEffect.SetActive(true);
            ownPerfectionEffect.SetActive(false);
        }
    }

    private void Break()
    {
        foreach (Rigidbody r in brokenParts)
        {
            r.isKinematic = false;
            r.AddForce(transform.position);         
        }
        foreach (Collider item in colliders)
        {
            item.isTrigger = true;
        }
    }

    IEnumerator SlowMotion()
    {
        Time.timeScale = 0.6f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
    }
    
}
