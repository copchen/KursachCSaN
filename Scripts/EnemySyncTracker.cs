using UnityEngine;
using System.Collections.Generic;

public class EnemySyncTracker : MonoBehaviour
{
    [Header("Sync Settings")]
    public string enemyId;
    public bool isOwner = false;

    private Vector3 lastSentPos;
    private NetworkConnector net;

    public static Dictionary<string, EnemySyncTracker> All = new();

    void Awake()
    {
        if (string.IsNullOrEmpty(enemyId))
            enemyId = gameObject.name;

        if (!All.ContainsKey(enemyId))
            All.Add(enemyId, this);
        else
            Debug.LogWarning($"[EnemySyncTracker]  Дубликат enemyId: {enemyId}");
    }

    void OnDestroy()
    {
        if (All.ContainsKey(enemyId))
            All.Remove(enemyId);
    }

    void Start()
    {
        net = FindObjectOfType<NetworkConnector>();
        lastSentPos = transform.position;

        if (isOwner)
            InvokeRepeating(nameof(SendPos), 0f, 0.1f); 
    }

    void SendPos()
    {
        if (net == null) return;

        Vector3 pos = transform.position;
        if (Vector3.Distance(pos, lastSentPos) > 0.05f)
        {
            string msg = $"ENEMY_POS;{enemyId};{pos.x:F2};{pos.y:F2};{pos.z:F2}";
            net.SendUDP(msg);
            lastSentPos = pos;
        }
    }

    public void SetRemotePosition(Vector3 pos)
    {
        if (!isOwner)
        {
            
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 100f);
        }
    }
}
