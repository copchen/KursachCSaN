using System.Globalization;
using UnityEngine;

public class EnemySyncReceiver : MonoBehaviour
{
    void OnEnable()
    {
        NetworkConnector.OnUDPMessageReceived += HandleUDP;
    }

    void OnDisable()
    {
        NetworkConnector.OnUDPMessageReceived -= HandleUDP;
    }

    void HandleUDP(string msg)
    {
        if (!msg.StartsWith("ENEMY_POS;")) return;

        string[] parts = msg.Split(';');
        if (parts.Length < 5) return;

        string id = parts[1];

        if (!EnemySyncTracker.All.TryGetValue(id, out var tracker))
            return;

        if (tracker.isOwner) return; 

        float x = float.Parse(parts[2].Replace(',', '.'), CultureInfo.InvariantCulture);
        float y = float.Parse(parts[3].Replace(',', '.'), CultureInfo.InvariantCulture);
        float z = float.Parse(parts[4].Replace(',', '.'), CultureInfo.InvariantCulture);

        tracker.SetRemotePosition(new Vector3(x, y, z));
    }
}
