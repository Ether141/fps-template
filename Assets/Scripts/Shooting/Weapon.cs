using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "FPS Template/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("General")]
    public string weaponName = "New weapon";
    public int clipAmmo = 30;
    public int maxAmmo = 330;

    [Header("Shooting")]
    public FireType fireType;
    public int rateOfFire = 500;
    public float muzzleVelocity = 300f;

    [Header("Bullet physics")]
    public float bulletDrop = 0.4f;
    public float dropBulletAfter = 0.4f;

    [Header("Spread")]
    public float maxXIdleSpread = 0.2f;
    public float maxYIdleSpread = 0.2f;
    [Space]
    public float maxXWalkSpread = 0.4f;
    public float maxYWalkSpread = 0.4f;

    [Header("Recoil")]
    public float xRecoil;
    public float yRecoil;
    [Space]
    public float recoilSnappiness = 0.2f;
    public float recoilReturnSpeed = 1.2f;

    [Header("Sounds")]
    public AudioClip shotSound;
    public AudioClip silencedShotSound;
    public AudioClip reloadSound;
}
