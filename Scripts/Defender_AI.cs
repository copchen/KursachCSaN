

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender_AI : MonoBehaviour
{
   
    public Transform gunHead;

    
    public float dampingSpeed = 10f;

    
    public string targetTag = "Enemy";

    
    public float shootingDistance = 30f;

    [Header("Seek Animation")]
    
    public bool playAnimationClip;
    public float seekSpeed = 50f;
    public float rotateAngle = 70f;

    
    Vector3 originalRotation;
    bool isActive;
    Transform target;

    IEnumerator Start()
    {
        
        originalRotation = gunHead.localRotation.eulerAngles;

        while (true)
        {
            
            target = FindClosestEnemy();

            if(target)
            {
                
                if (Vector3.Distance(transform.position, target.position) <= shootingDistance)
                {
                    
                    GetComponent<Weapon>().canShoot = true;
                    isActive = true;
                }
                else
                {
                   
                    GetComponent<Weapon>().canShoot = false;
                    isActive = false;
                }
            }
            else
            {
               
                GetComponent<Weapon>().canShoot = false;
                isActive = false;
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    
    void Update()
    {
        if (isActive)
        {
            if (target)
            {
               
                Vector3 lookPos = target.position - gunHead.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                gunHead.rotation = Quaternion.Slerp(gunHead.rotation, rotation, Time.deltaTime * dampingSpeed);
            }
        }
        else
        {
            
            if(playAnimationClip)
            {
                if (GetComponent<AnimationList>().actor)
                {
                       GetComponent<AnimationList>().actor.CrossFade(GetComponent<AnimationList>().seekClip);
                }
            }
            else
            {
                
                gunHead.localRotation = Quaternion.Euler(originalRotation.x, Mathf.PingPong(Time.time * seekSpeed, rotateAngle * 2) - rotateAngle, 1f);
            }
        }
    }

 
    GameObject closest;
    Transform FindClosestEnemy()
    {
        
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(targetTag);
        if (gos.Length == 0)
            return null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        
        return closest.transform;
    }


}
