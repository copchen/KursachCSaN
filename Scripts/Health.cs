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

    void Start()
    {
        gManager = GameObject.FindObjectOfType<GameManagerMulty>();
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
            if (GetComponent<CapsuleCollider>()) GetComponent<CapsuleCollider>().enabled = false;
            if (GetComponent<Weapon>()) GetComponent<Weapon>().canShoot = false;
            gManager.AddCoins(destroyAwardedCoins);

            if (damageParticle)
                Instantiate(damageParticle, transform.position, transform.rotation);

            if (healthColor && healthColor.transform.parent) Destroy(healthColor.transform.parent.gameObject);

            if (GetComponent<Weapon>()) GetComponent<Weapon>().shootingDelay = 999f;

            if (!isDead)
            {
                isDead = true;
                StartCoroutine(Destroy_Delay());
            }
        }

        if (targetType == TargetType.Defender && healthValue <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                StartCoroutine(Destroy_Delay());
            }
        }
    }

    IEnumerator Destroy_Delay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
