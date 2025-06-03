using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour
{
    public string targetTag = "Enemy";
    public int damageValue;
    public GameObject damageParticle;
    public float lifeTime = 5f;
    private string mySide;
    public bool isOwner;

    IEnumerator Start()
    {
        mySide = GetComponentInParent<Health>()?.side;
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
                
                bool isOwner = GetComponent<Projectile>()?.isOwner ?? false;
                if (isOwner)
                {
                   
                    string msg = $"TOWER_HIT;{hp.side};{damageValue}";
                    NetworkConnector.Instance.SendMessageToPeer(msg);

                    
                    GameManagerMulty g = GameObject.FindObjectOfType<GameManagerMulty>();
                    g.Reduce_Tower_Health(hp.side, damageValue);
                }
            }
            if (hp.targetType == TargetType.Enemy && hp.side != mySide)
            {
                hp.ApplyDamage(damageValue);
            }
        }

        if (damageParticle)
            Instantiate(damageParticle, transform.position, transform.rotation);

        Destroy(gameObject);
    }


    public void SetSide(string side)
    {
        mySide = side;
    }


}
