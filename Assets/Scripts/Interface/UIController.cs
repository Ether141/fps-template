using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class UIController : MonoBehaviour
{
    [SerializeField] private ShootController shootController;
    [Space]
    [SerializeField] private Image crosshair;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text weaponNameText;
    [SerializeField] private GameObject reloadTip;

    private void Start()
    {
        shootController.ChangedAmmoCount += UpdateUI;
        shootController.ChangedWeapon += UpdateUI;
        shootController.ChangedAimState += ShootController_ChangedAimState;
    }

    private void Update() => reloadTip.SetActive(shootController.CurrentAmmoInClip <= shootController.Weapon.clipAmmo * 0.2f);

    private void ShootController_ChangedAimState(bool isAiming)
    {
        if (isAiming)
            crosshair.CrossFadeAlpha(0f, 0.1f, true);
        else
            crosshair.CrossFadeAlpha(1f, 0.25f, true);
    }

    public void UpdateUI()
    {
        ammoText.text = $"{shootController.CurrentAmmoInClip} / {shootController.AmmoStock}";
        weaponNameText.text = shootController.Weapon.weaponName;
    }
}
