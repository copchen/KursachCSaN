


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    public float destroyDelay = 5f;

    IEnumerator Start()
    {        
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
