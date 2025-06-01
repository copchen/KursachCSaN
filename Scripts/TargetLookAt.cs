
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLookAt : MonoBehaviour
{

    
    public float updateInterval = 0.1f;

    IEnumerator Start()
    {
       
        while(true)
        {
            transform.LookAt(Camera.main.transform, Vector3.up);
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
