using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ShootingFX))]
public class ShootController : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    [Header("Shooting general settings")]
    [SerializeField] private LayerMask shotableLayer;

    [Header("Bullet force")]
    [SerializeField] private float rayLength = 1f;

    [Header("Gizmo")]
    [SerializeField] private bool drawPredictionLine = true;
    [SerializeField] private Color predictionLineColor = Color.red;

    [Header("Others")]
    [SerializeField] private WeaponModifier weaponModifier;

    private const float StepSize = 0.01f;

    private PlayerController playerController;
    private ShootingFX fx;
    private bool shootingBreak = false;
    private bool firstShot = true;
    private List<WeaponStatModifier> modifiers = new List<WeaponStatModifier>();

    private Camera Cam => Camera.main;
    private Ray FireRay
    {
        get
        {
            Vector3 origin = Cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            return new Ray(origin, Cam.transform.forward);
        }
    }

    public Weapon Weapon => weapon;
    public bool HasAssignedWeapon => weapon != null;
    public float DelayAfterShot => 60f / RateOfFireWithModifiers;
    public bool CanShoot => !playerController.IsRunning && !shootingBreak && CurrentAmmoInClip > 0 && !IsReloading && !IsInspecting; 
    public bool CanAim => !playerController.IsRunning && !IsReloading && !IsInspecting;
    public bool CanReload => CurrentAmmoInClip < weapon.clipAmmo && !IsReloading && !playerController.IsRunning && AmmoStock > 0 && !IsInspecting;
    public bool CanInspect => !playerController.IsMoving && CanShoot && !IsAiming && !IsShooting;
    public bool IsShooting { get; private set; } = false;
    public bool IsAiming { get; private set; } = false;
    public bool IsReloading { get; private set; } = false;
    public bool IsInspecting { get; private set; } = false;
    public float MuzzleVelocityWithModifiers
    {
        get
        {
            float modifier = GetTotalModifierValue(WeaponStatModifierType.MuzzleVelocityModifier);
            float vel = weapon.muzzleVelocity - (weapon.muzzleVelocity * modifier);
            return vel;
        }
    }
    public int RateOfFireWithModifiers
    {
        get
        {
            float modifier = GetTotalModifierValue(WeaponStatModifierType.RateOfFireModifier);
            int vel = Mathf.RoundToInt(weapon.rateOfFire + (weapon.rateOfFire * modifier));
            return vel;
        }
    }

    public int CurrentAmmoInClip { get; private set; } = 0;
    public int AmmoStock { get; private set; } = 0;

    public event Action ChangedAmmoCount;
    public event Action ChangedWeapon;
    public event Action<bool> ChangedAimState;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        fx = GetComponent<ShootingFX>();

        WeaponValidation();
    }

    private void WeaponValidation()
    {
        if (!HasAssignedWeapon)
            Debug.LogError("ShootController has no weapon preset assigned! Shooting will be impossible.");
        else
            ChangeWeapon(weapon);
    }

    private void Update()
    {
        Aiming();

        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchInspectState();

        if ((Input.GetKeyDown(KeyCode.R) && CanReload) || CurrentAmmoInClip == 0)
            Reload();

        if (Input.GetMouseButton(0))
        {
            if (CanShoot && HasAssignedWeapon)
            {
                IsShooting = true;

                if (Shoot(out RaycastHit hit))
                    HandleShot(in hit);

                shootingBreak = true;
                this.WaitAndDo(() => shootingBreak = false, DelayAfterShot);
            }  
        }
        else if (!shootingBreak)
        {
            IsShooting = false;
            firstShot = true;
        }
    }

    private void SwitchInspectState()
    {
        if (!IsInspecting && CanInspect)
        {
            IsInspecting = true;
            playerController.AnimatorController.EnableInspecting(true);
            playerController.canMove = false;
            weaponModifier.ShowUI();
        }
        else if (IsInspecting)
        {
            IsInspecting = false;
            playerController.AnimatorController.EnableInspecting(false);
            playerController.canMove = true;
            weaponModifier.HideUI();
        }
    }

    private void Aiming()
    {
        if (Input.GetMouseButton(1) && !IsAiming && CanAim)
        {
            IsAiming = true;
            playerController.canRun = false;
            ChangedAimState?.Invoke(true);
        }

        if ((Input.GetMouseButtonUp(1) || IsReloading) && IsAiming)
        {
            IsAiming = false;
            playerController.canRun = true;
            ChangedAimState?.Invoke(false);
        }
    }

    private void HandleShot(in RaycastHit hit)
    {
        fx.CreateBulletHole(hit.point, hit.normal, 60f, hit.transform);

        if (hit.transform.TryGetComponent(out IShotable shotable))
        {
            shotable.Damage(1);
        }
    }

    private bool Shoot(out RaycastHit result)
    {
        result = default;

        if (!CanShoot)
            return false;

        fx.PlayShotFX();
        CurrentAmmoInClip--;
        ChangedAmmoCount?.Invoke();
        Vector3 point1 = FireRay.origin;

        if (!IsAiming && !firstShot)
            ApplySpread(ref point1);

        firstShot = false;
        playerController.Camera.ApplyRecoil();

        Vector3 bulletVelocity = FireRay.direction * MuzzleVelocityWithModifiers;
        Vector3 point2;

        for (float step = 0f; step < rayLength; step += StepSize)
        {
            if (step > weapon.dropBulletAfter)
            {
                bulletVelocity += (0.05f * MuzzleVelocityWithModifiers * StepSize * FireRay.direction) + (Vector3.down * weapon.bulletDrop);
            }

            point2 = point1 + bulletVelocity * StepSize;

            if (Physics.Linecast(point1, point2, out RaycastHit hit, shotableLayer))
            {
                result = hit;
                return true;
            }

            point1 = point2;
        }

        return false;
    }

    private void Reload()
    {
        if (!CanReload)
            return;

        fx.PlayReloadSound();
        playerController.canRun = false;
        playerController.AnimatorController.ReloadTrigger();
        IsShooting = false;
        IsReloading = true;
    }

    // animation event
    public void FinishReloading()
    {
        IsReloading = false;
        AmmoStock -= weapon.clipAmmo - CurrentAmmoInClip;
        CurrentAmmoInClip = weapon.clipAmmo;
        ChangedAmmoCount?.Invoke();
        playerController.canRun = true;
    }

    private void ApplySpread(ref Vector3 pos)
    {
        float maxXSpread = playerController.IsMoving ? weapon.maxXWalkSpread : weapon.maxXIdleSpread;
        float maxYSpread = playerController.IsMoving ? weapon.maxYWalkSpread : weapon.maxYIdleSpread;

        float modifier = GetTotalModifierValue(WeaponStatModifierType.SpreadModifier);
        maxXSpread -= maxXSpread * modifier;
        maxYSpread -= maxYSpread * modifier;

        float additionalX = Random.Range(maxXSpread * -1f, maxXSpread);
        float additionalY = Random.Range(maxYSpread * -1f, maxYSpread);
        pos += (Cam.transform.right * additionalX) + (Cam.transform.up * additionalY);
    }

    public void ChangeWeapon(Weapon w)
    {
        if (w == null)
            return;

        weapon = w;
        CurrentAmmoInClip = w.clipAmmo;
        AmmoStock = w.maxAmmo - CurrentAmmoInClip;
        ChangedWeapon?.Invoke();
    }

    public void AddModifier(WeaponStatModifier modifier)
    {
        if (!modifiers.Any(m => m.id == modifier.id))
            modifiers.Add(modifier);
    }

    public void RemoveModifier(int id) => modifiers.RemoveAll(m => m.id == id);

    public bool HasModifier(WeaponStatModifierType type) => modifiers.Any(m => m.type == type);

    public bool HasModifier(int id) => modifiers.Any(m => m.id == id);

    public float GetTotalModifierValue(WeaponStatModifierType type) => modifiers.Where(m => m.type == type).Sum(m => m.modifier);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawPredictionLine && HasAssignedWeapon)
        {
            Vector3 point1 = FireRay.origin;
            Vector3 point2;
            Vector3 predictedBulletVelocity = FireRay.direction * MuzzleVelocityWithModifiers;
            bool x = false;

            for (float step = 0f; step < rayLength; step += StepSize)
            {
                if (step > weapon.dropBulletAfter)
                {
                    predictedBulletVelocity += (0.05f * MuzzleVelocityWithModifiers * StepSize * FireRay.direction) + (Vector3.down * weapon.bulletDrop);
                }

                point2 = point1 + predictedBulletVelocity * StepSize;

                Gizmos.color = x ? predictionLineColor : predictionLineColor - new Color(0f, 0f, 0f, 0.33f);
                x = !x;
                Gizmos.DrawLine(point1, point2);
                point1 = point2;
            }
        }
    }
#endif
}
