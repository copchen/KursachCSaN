

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPoints : MonoBehaviour
{
	[Header("Face spawned defenders to the center of road")]
	[Space(5)]
	public bool isActive = true;

     void Start()
    {
		isActive = false;
	}

    void OnDrawGizmos()
	{
		if (isActive)
		{
			Transform[] tem = GetComponentsInChildren<Transform>();

			if (tem.Length > 0)
			{
				Gizmos.color = Color.blue;
				for (int a = 0; a < tem.Length; a++)
				{
					if (a != 0)
					{
						Gizmos.DrawSphere(tem[a].position, 1f);
						tem[a].gameObject.name = a.ToString();
						tem[a].tag = "Center Point";
					}
				}
			}
		}
	}
}
