using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    
    public float rotationSpeed = 300f;

    
    public Vector3 axis = new Vector3(0,0,-10f);


    
    void Update()
    {
        transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }
}
