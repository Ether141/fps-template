using UnityEngine;

public class ShootingFX : MonoBehaviour
{
    [SerializeField] private GameObject bulletHole;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource audioSource;

    private ParticleSystem.MainModule[] muzzleParticles;
    private ShootController shootController;

    private void Awake()
    {
        muzzleParticles = new ParticleSystem.MainModule[muzzleFlash.transform.childCount + 1];
        muzzleParticles[0] = muzzleFlash.main;

        for (int i = 1; i < muzzleParticles.Length; i++)
            muzzleParticles[i] = muzzleFlash.transform.GetChild(i - 1).GetComponent<ParticleSystem>().main;

        shootController = GetComponent<ShootController>();
        shootController.ChangedWeapon += AdjustMuzzleFlash;
    }

    private void AdjustMuzzleFlash()
    {
        for (int i = 0; i < muzzleParticles.Length; i++)
            muzzleParticles[i].duration = shootController.DelayAfterShot;
    }

    public void PlayShotFX()
    {
        muzzleFlash.Play(!shootController.HasModifier(WeaponStatModifierType.MuzzleParticleRemover));
        audioSource.PlayOneShot(shootController.HasModifier(WeaponStatModifierType.Silenced) ? shootController.Weapon.silencedShotSound : shootController.Weapon.shotSound);
    }

    public void PlayReloadSound() => audioSource.PlayOneShot(shootController.Weapon.reloadSound);

    public void CreateBulletHole(Vector3 pos, Vector3 normal, float lifetime, Transform parent)
    {
        Transform bullet = Instantiate(bulletHole).transform;
        bullet.position = pos;
        bullet.up = normal;
        bullet.position += bullet.up * 0.02f;

        if (parent != null)
            bullet.SetParent(parent, true);

        Destroy(bullet.gameObject, lifetime);
    }
}
