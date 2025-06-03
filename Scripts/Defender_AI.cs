using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender_AI : MonoBehaviour
{
    public Transform gunHead;
    public float dampingSpeed = 10f;
    public float shootingDistance = 30f;
    public bool playAnimationClip;
    public float seekSpeed = 50f;
    public float rotateAngle = 70f;
    public string mySide;
    public bool isPlaced = false;


    private Vector3 originalRotation;
    private bool isActive;
    private Transform target;


    IEnumerator Start()
    {
        originalRotation = gunHead.localRotation.eulerAngles;

        while (!isPlaced)
            yield return null;

        while (true)
        {
            target = FindClosestEnemy();

            if (target != null && Vector3.Distance(transform.position, target.position) <= shootingDistance)
            {
                GetComponent<Weapon>().canShoot = true;
                isActive = true;
            }
            else
            {
                GetComponent<Weapon>().canShoot = false;
                isActive = false;
                target = null;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }


    void Update()
    {
        if (isActive && target)
        {
            Vector3 lookPos = target.position - gunHead.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            gunHead.rotation = Quaternion.Slerp(gunHead.rotation, rotation, Time.deltaTime * dampingSpeed);
        }
        else
        {
            if (playAnimationClip && TryGetComponent(out AnimationList anim) && anim.actor)
                anim.actor.CrossFade(anim.seekClip);
            else
                gunHead.localRotation = Quaternion.Euler(originalRotation.x, Mathf.PingPong(Time.time * seekSpeed, rotateAngle * 2) - rotateAngle, 1f);
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        float distance = Mathf.Infinity;
        GameObject closest = null;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            if (!go.TryGetComponent(out Health hp)) continue;


            if (hp.side == mySide) continue;

            float curDistance = (go.transform.position - position).sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest ? closest.transform : null;
    }


}




