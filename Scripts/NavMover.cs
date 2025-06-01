using UnityEngine;
using System.Collections.Generic;

public class NavMover : MonoBehaviour
{
    public string waypointName;
    public float moveSpeed = 2.5f;
    public float reachDistance = 0.1f;

    private List<Transform> points = new List<Transform>();
    private int currentPoint = 0;
    public bool reachedToEnd = false;

    void Start()
    {
        var pathObject = GameObject.Find(waypointName);
        if (pathObject == null)
        {
            Debug.LogError($"[NavMover] ❌ Не найден путь: {waypointName}");
            return;
        }

        var path = pathObject.GetComponent<WaypointSystem>();
        if (path == null)
        {
            Debug.LogError($"[NavMover] ❌ У объекта {waypointName} нет компонента WaypointSystem");
            return;
        }

        points = path.waypoints;
        if (points.Count == 0)
        {
            Debug.LogError("[NavMover] ❌ Нет точек пути!");
            return;
        }

        transform.position = points[0].position; 
        currentPoint = 1;
    }

    void Update()
    {
        if (reachedToEnd || points.Count == 0 || currentPoint >= points.Count)
            return;

        Vector3 target = points[currentPoint].position;
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target);

        
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (distance < reachDistance)
        {
            currentPoint++;

            if (currentPoint >= points.Count)
            {
                reachedToEnd = true;
                OnReachEnd();
            }
        }
    }

    void OnReachEnd()
    {
        Debug.Log($"[NavMover]  {gameObject.name} достиг конца пути");
       
    }
}
