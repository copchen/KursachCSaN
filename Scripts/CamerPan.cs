
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanMulty : MonoBehaviour
{
    [Space(5)]
    [Header("Camera Pan System")]
    public float mouseSensitivity = 1.0f;
    Vector3 lastPosition;

    public float limitLeft, limitRight;
    GameManagerMulty gameManager;

    private void Start()
    {

        gameManager = GameObject.FindObjectOfType<GameManagerMulty>();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastPosition = Input.mousePosition;
        }


        if (Input.GetKey(KeyCode.Mouse0))
        {

            if (gameManager.dragOnViewSpace)
            {
                Vector3 delta = Input.mousePosition - lastPosition;
                Vector3 CameralastLocation = gameObject.transform.position;


                transform.Translate(delta.x * mouseSensitivity, 0, 0);


                if (transform.position.x > limitRight)
                    transform.position = new Vector3(CameralastLocation.x, transform.position.y, transform.position.z);
                if (transform.position.x < limitLeft)
                    transform.position = new Vector3(CameralastLocation.x, transform.position.y, transform.position.z);


                lastPosition = Input.mousePosition;
            }
        }
    }
}
