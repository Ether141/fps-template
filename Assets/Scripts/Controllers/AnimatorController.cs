using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    private Animator anim;
    private PlayerController controller;
    private ShootController shootController;

    private const float SpeedSmoothTime = 0.15f;

    private readonly int isGroundedHash     = Animator.StringToHash("isGrounded");
    private readonly int isMovingHash       = Animator.StringToHash("isMoving");
    private readonly int isRunningHash      = Animator.StringToHash("isRunning");
    private readonly int isAimingHash       = Animator.StringToHash("isAiming");
    private readonly int isShootingHash     = Animator.StringToHash("isShooting");
    private readonly int shootingSpeedHash  = Animator.StringToHash("shootingSpeed");
    private readonly int reloadHash         = Animator.StringToHash("reload");
    private readonly int isInspectingHash   = Animator.StringToHash("isInspecting");

    private void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        shootController = GetComponent<ShootController>();
    }

    private void Update() => SetAnimatorParameters();

    public void ReloadTrigger() => anim.SetTrigger(reloadHash);

    public void EnableInspecting(bool enabled) => anim.SetBool(isInspectingHash, enabled);

    private void SetAnimatorParameters()
    {
        anim.SetBool(isGroundedHash, controller.IsGrounded);
        anim.SetBool(isMovingHash, controller.IsMoving);
        anim.SetBool(isRunningHash, controller.IsRunning);
        anim.SetBool(isAimingHash, shootController.IsAiming);
        anim.SetBool(isShootingHash, shootController.IsShooting);
        anim.SetFloat(shootingSpeedHash, 0.1f / (60f / shootController.Weapon.rateOfFire));

        float animSpeed = 0f;

        if (controller.IsMoving)
        {
            animSpeed = controller.IsRunning ? 1f : 0.5f;
        }

        anim.SetFloat("speed", animSpeed, SpeedSmoothTime, Time.deltaTime);
    }
}
