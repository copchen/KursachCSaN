using System.Collections;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public Transform gunHead;
    public float dampingSpeed = 1f;
    public string mySide = "A";
    public float shootingDistance = 30f;
    public bool shootOnlyTower = true;

    Quaternion originalRotation;
    bool isActive;
    Transform target;

    void Start()
    {
        originalRotation = gunHead.rotation;
        StartCoroutine(AIUpdate());
    }

    IEnumerator AIUpdate()
    {
        while (true)
        {
            target = FindClosestEnemy();

            if (target != null && Vector3.Distance(transform.position, target.position) <= shootingDistance)
            {
                if (shootOnlyTower)
                {
                    if (GetComponent<NavMover>().reachedToEnd)
                    {
                        GetComponent<Weapon>().canShoot = true;
                        isActive = true;
                    }
                }
                else
                {
                    GetComponent<Weapon>().canShoot = true;
                    isActive = true;
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
        if (isActive && target != null)
        {
            Vector3 lookPos = target.position - gunHead.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            gunHead.rotation = Quaternion.Slerp(gunHead.rotation, rotation, Time.deltaTime * dampingSpeed);
        }
    }

    GameObject closest;
    Transform FindClosestEnemy()
    {
        string tagToSearch = mySide == "A" ? "Tower_B" : "Tower_A";
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tagToSearch);
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
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
