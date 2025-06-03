using System.Collections;
using UnityEngine;

public enum TargetType { Enemy, Tower, Defender }

public class Health : MonoBehaviour
{
    public TargetType targetType;
    public string side = "A"; // A или B

    public int healthValue = 100;
    public int destroyAwardedCoins = 100;
    public GameObject damageParticle;
    public float destroyDelay = 0;

    public MeshRenderer healthColor;

    private GameManagerMulty gManager;
    private bool isDead;
    private bool isOwner;

    void Start()
    {
        gManager = GameObject.FindObjectOfType<GameManagerMulty>();
        isOwner = (side == (FindObjectOfType<NetworkConnector>().isHost ? "A" : "B"));
        if (healthColor)
            healthColor.material.color = Color.green;
    }

    public void ApplyDamage(int damage)
    {
        healthValue -= damage;

        if (healthColor)
        {
            if (healthValue > 0)
                healthColor.transform.localScale = new Vector3(healthColor.transform.localScale.x, healthColor.transform.localScale.y, healthValue);
            else
                healthColor.transform.localScale = Vector3.zero;
        }

        if (targetType == TargetType.Enemy && healthValue <= 0)
        {
            if (!isDead)
            {
                isDead = true;

                
                bool isOwner = (side == (FindObjectOfType<NetworkConnector>().isHost ? "A" : "B"));
                if (isOwner)
                {
                    NetworkConnector.Instance.SendMessageToPeer($"ENEMY_DEAD;{gameObject.name}");
                }

                HandleDeath();
            }
        }

        if (targetType == TargetType.Defender && healthValue <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Destroy_Delay());
        }
    }


    IEnumerator Destroy_Delay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }


    public void RemoteKill()
    {
        if (!isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (GetComponent<CapsuleCollider>())
            GetComponent<CapsuleCollider>().enabled = false;

        if (GetComponent<Weapon>())
        {
            GetComponent<Weapon>().canShoot = false;
            GetComponent<Weapon>().shootingDelay = 999f;
        }

        gManager.AddCoins(destroyAwardedCoins);

        if (damageParticle)
            Instantiate(damageParticle, transform.position, transform.rotation);

        if (healthColor && healthColor.transform.parent)
            Destroy(healthColor.transform.parent.gameObject);

        StartCoroutine(Destroy_Delay());
    }


}
