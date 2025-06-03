using UnityEngine;

public class EnemySpawnerMulti : MonoBehaviour
{
    [Header("Точки спавна (0–5: B → A, 6–11: A → B)")]
    public Transform[] spawnPoints;

    private NetworkConnector net;

    private void Start()
    {
        net = FindObjectOfType<NetworkConnector>();
        if (net == null)
        {
            Debug.LogError(" NetworkConnector не найден в сцене");
            return;
        }

        NetworkConnector.OnMessageReceived += OnMessageReceived;
    }

    private void OnDestroy()
    {
        NetworkConnector.OnMessageReceived -= OnMessageReceived;
    }

    GameManagerMulty g = GameObject.FindObjectOfType<GameManagerMulty>();
    public void SpawnHostEnemy()
    {
        
            int globalEnemyIndex = Random.Range(6, 12); // A → B
            SpawnOneEnemy(globalEnemyIndex);
           
    }

    public void SpawnClientEnemy()
    {
            int globalEnemyIndex = Random.Range(0, 6); // B → A
            SpawnOneEnemy(globalEnemyIndex);
    }

    public void SpawnOneEnemy(int globalEnemyIndex)
    {
        var gm = FindObjectOfType<GameManagerMulty>();
        int cost = 1000;

            if (gm.TotalCoins < cost)
        {
           
            return;
        };

        bool isHost = net.isHost;

        int spawnIndex = isHost
            ? Random.Range(6, 12)
            : Random.Range(0, 6);

        gm.ReduceCoins(cost);

        string enemyId = $"Enemy_{globalEnemyIndex}_{spawnIndex}_{Random.Range(1000, 9999)}";
        string msg = $"SPAWN_ENEMY_GLOBAL;{globalEnemyIndex};{spawnIndex};{enemyId}";

        SpawnEnemyByIndex(globalEnemyIndex, spawnIndex, enemyId);
        net.SendMessageToPeer(msg);

        Debug.Log($"[SPAWN] Отправка: {msg}");
    }

    private void OnMessageReceived(string msg)
    {
        if (!msg.StartsWith("SPAWN_ENEMY_GLOBAL")) return;

        string[] parts = msg.Split(';');
        if (parts.Length < 4) return;

        int globalEnemyIndex = int.Parse(parts[1]);
        int spawnIndex = int.Parse(parts[2]);
        string enemyId = parts[3];

        if (ShouldSkipMyOwnSpawn(globalEnemyIndex)) return;

        SpawnEnemyByIndex(globalEnemyIndex, spawnIndex, enemyId);
    }

    private bool ShouldSkipMyOwnSpawn(int globalEnemyIndex)
    {
        bool isHost = net.isHost;
        return (isHost && globalEnemyIndex >= 6) || (!isHost && globalEnemyIndex < 6);
    }

    public void SpawnEnemyByIndex(int globalIndex, int spawnIndex, string enemyId)
    {
        GameObject prefab = EnemyRegistry.Instance.GetEnemy(globalIndex);
        if (prefab == null)
        {
            Debug.LogError($"❌ Не найден враг с индексом {globalIndex}");
            return;
        }

        Transform spawnPoint = spawnPoints[Mathf.Clamp(spawnIndex, 0, spawnPoints.Length - 1)];
        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        enemy.name = enemyId;

        bool isOwner = (net.isHost && globalIndex >= 6) || (!net.isHost && globalIndex < 6);

        // Назначаем сторону по префабу (0-5 → B, 6-11 → A)
        string side = (globalIndex < 6) ? "B" : "A";

        if (enemy.TryGetComponent(out Enemy_AI ai))
        {
            ai.mySide = side;
            ai.enabled = true; 
        }

        if (enemy.TryGetComponent(out NavMover mover))
        {
            mover.enabled = true; 
        }

        if (enemy.TryGetComponent(out Health health))
        {
            health.side = side;  
        }


        if (enemy.TryGetComponent(out EnemySyncTracker tracker))
        {
            tracker.enemyId = enemyId;
            tracker.isOwner = isOwner;
        }

        Debug.Log($" Враг создан: {enemyId} на точке #{spawnIndex} | isOwner = {isOwner}");
    }
}
