using System.Collections;
using UnityEngine;

public enum ShootingMode
{
    Gun, Sword
}

public class Weapon : MonoBehaviour
{
    [Header("General Settings")]
    public ShootingMode shootingMode = ShootingMode.Gun;
    public GameObject projectile;
    public Transform shootPoint;
    public float force = 100f;
    public float shootingDelay = 1f;

    [Header("Sword Settings")]
    public int SwordDamage = 1;

    [Header("Additional Options")]
    public GameObject secondProjectile;
    public Transform secondShootPoint;

    [Header("Sound Settings")]
    public AudioClip fireClip;
    AudioSource audioSource;

    [HideInInspector] public bool canShoot = false;

    GameManagerMulty gameManager;

    IEnumerator Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManagerMulty>();

        while (true)
        {
            yield return new WaitForSeconds(shootingDelay);

            if (canShoot)
            {
                if (shootingMode == ShootingMode.Gun)
                {
                    GameObject bullet = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                    bullet.GetComponent<Rigidbody>().AddForce(shootPoint.forward * force);

                    if (secondProjectile)
                    {
                        GameObject bullet2 = Instantiate(secondProjectile, secondShootPoint.position, secondShootPoint.rotation);
                        bullet2.GetComponent<Rigidbody>().AddForce(secondShootPoint.forward * force);
                    }

                    if (audioSource && fireClip)
                        audioSource.PlayOneShot(fireClip);

                    if (TryGetComponent(out AnimationList animList) && animList.actor)
                        animList.actor.CrossFade(animList.fireClip);
                }

                if (shootingMode == ShootingMode.Sword)
                {
                    if (TryGetComponent(out AnimationList animList) && animList.actor)
                        animList.actor.CrossFade(animList.fireClip);

                    gameManager.Reduce_Tower_Health(gameManager.MySide, SwordDamage); 
                }
            }
        }
    }
}
