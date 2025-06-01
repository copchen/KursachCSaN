using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance;

    [Header("Общий список из 12 врагов")]
    public GameObject[] allEnemies; // 0–5 = B→A, 6–11 = A→B

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetEnemy(int index)
    {
        if (index < 0 || index >= allEnemies.Length)
        {
            Debug.LogError($" EnemyRegistry: индекс вне диапазона ({index})");
            return null;
        }
        return allEnemies[index];
    }
}
