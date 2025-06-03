using System.Collections;
using System.Globalization;
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
    private bool isOwner;

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


    void Awake()
    {
        isOwner = GetComponent<Health>()?.side == (FindObjectOfType<NetworkConnector>()?.isHost == true ? "A" : "B");
    }
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
                    var bullet = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                    var health = GetComponent<Health>();
                    if (bullet.TryGetComponent<Projectile>(out var proj) && health != null)
                    {
                        proj.GetComponent<Projectile>().SendMessage("SetSide", health.side, SendMessageOptions.DontRequireReceiver);
                    }
                    if (bullet.TryGetComponent(out Projectile owner))
                        owner.isOwner = isOwner;

                    if (isOwner)
                    {
                        Vector3 shootPos = shootPoint.position;
                        Vector3 shootDir = shootPoint.forward;

                        string msg = $"DEF_SHOOT;{shootPos.x:F2};{shootPos.y:F2};{shootPos.z:F2};{shootDir.x:F2};{shootDir.y:F2};{shootDir.z:F2}";
                        NetworkConnector.Instance.SendUDP(msg);
                    }

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