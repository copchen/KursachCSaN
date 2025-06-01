using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour
{
    public string targetTag = "Enemy";
    public int damageValue;
    public GameObject damageParticle;
    public float lifeTime = 5f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        Health hp = col.transform.GetComponent<Health>();
        if (hp != null)
        {
            if (hp.targetType == TargetType.Tower)
            {
                GameManagerMulty g = GameObject.FindObjectOfType<GameManagerMulty>();
                g.Reduce_Tower_Health(hp.side, damageValue);
            }
            else
            {
                hp.ApplyDamage(damageValue);
            }
        }

        if (damageParticle)
            Instantiate(damageParticle, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
