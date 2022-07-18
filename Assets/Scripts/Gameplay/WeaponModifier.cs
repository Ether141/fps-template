using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class WeaponModifier : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private ShootController shootController;

    [Header("Camo")]
    [SerializeField] private MeshRenderer[] meshesToChangeMaterial;
    [SerializeField] private Material[] camos;
    private int currentCamoIndex = 0;

    [Header("Sight")]
    [SerializeField] private GameObject[] sights;
    private int currentSightIndex = 0;

    [Header("Barrel")]
    [SerializeField] private GameObject[] barrels;
    private int currentBarrelIndex = 0;

    [Header("Side mount")]
    [SerializeField] private GameObject[] sideMounts;
    private int currentSideMountIndex = 0;

    [Header("UI")]
    [SerializeField] private GameObject ui;
    [SerializeField] private Text weaponNameText;
    [SerializeField] private Text weaponStatsText;

    public void ShowUI()
    {
        UpdateUI();
        ui.SetActive(true);
        cameraController.isControllerEnabled = false;
        CursorController.ShowCursor();
    }

    public void HideUI()
    {
        ui.SetActive(false);
        cameraController.isControllerEnabled = true;
        CursorController.HideCursor();
    }

    public void NextCamo()
    {
        currentCamoIndex++;
        if (currentCamoIndex == camos.Length)
            currentCamoIndex = 0;
        ChangeCamo(currentCamoIndex);        
    }

    public void PreviousCamo()
    {
        currentCamoIndex--;
        if (currentCamoIndex < 0)
            currentCamoIndex = camos.Length - 1;
        ChangeCamo(currentCamoIndex);
    }

    public void NextSight()
    {
        currentSightIndex++;
        if (currentSightIndex == sights.Length)
            currentSightIndex = 0;
        ChangeSight(currentSightIndex);
    }

    public void PreviousSight()
    {
        currentSightIndex--;
        if (currentSightIndex < 0)
            currentSightIndex = sights.Length - 1;
        ChangeSight(currentSightIndex);
    }

    public void NextBarrel()
    {
        currentBarrelIndex++;
        if (currentBarrelIndex == barrels.Length)
            currentBarrelIndex = 0;
        ChangeBarrel(currentBarrelIndex);
    }

    public void PreviousBarrel()
    {
        currentBarrelIndex--;
        if (currentBarrelIndex < 0)
            currentBarrelIndex = barrels.Length - 1;
        ChangeBarrel(currentBarrelIndex);
    }

    public void NextSideMount()
    {
        currentSideMountIndex++;
        if (currentSideMountIndex == sideMounts.Length)
            currentSideMountIndex = 0;
        ChangeSideMount(currentSideMountIndex);
    }

    public void PreviousSideMount()
    {
        currentSideMountIndex--;
        if (currentSideMountIndex < 0)
            currentSideMountIndex = sideMounts.Length - 1;
        ChangeSideMount(currentSideMountIndex);
    }

    public void ChangeCamo(int camoIndex)
    {
        camoIndex = Mathf.Clamp(camoIndex, 0, camos.Length - 1);
        foreach (var mesh in meshesToChangeMaterial)
            mesh.material = camos[camoIndex];
    }

    public void ChangeSight(int sightIndex)
    {
        sightIndex = Mathf.Clamp(sightIndex, 0, sights.Length - 1);
        for (int i = 0; i < sights.Length; i++)
            HandleAttachement(sights[i], i, sightIndex);
        UpdateUI();
    }

    public void ChangeBarrel(int barrelIndex)
    {
        barrelIndex = Mathf.Clamp(barrelIndex, 0, barrels.Length - 1);
        for (int i = 0; i < barrels.Length; i++)
            HandleAttachement(barrels[i], i, barrelIndex);
        UpdateUI();
    }

    public void ChangeSideMount(int sideMountIndex)
    {
        sideMountIndex = Mathf.Clamp(sideMountIndex, 0, sideMounts.Length - 1);
        for (int i = 0; i < sideMounts.Length; i++)
            HandleAttachement(sideMounts[i], i, sideMountIndex);
        UpdateUI();
    }

    private void HandleAttachement(GameObject obj, int i, int targetIndex)
    {
        WeaponStatModifierProvider provider;

        if (obj.activeSelf && i != targetIndex)
        {
            obj.SetActive(false);

            if (obj.TryGetComponent(out provider))
            {
                foreach (var modifier in provider.Modifiers)
                    shootController.RemoveModifier(modifier.id);
            }
        }
        
        if (!obj.activeSelf && i == targetIndex)
        {
            obj.SetActive(true);

            if (obj.TryGetComponent(out provider))
            {
                foreach (var modifier in provider.Modifiers)
                    shootController.AddModifier(modifier);
            }
        }
    }

    private void UpdateUI()
    {
        Weapon w = shootController.Weapon;

        weaponNameText.text = w.weaponName;
        weaponStatsText.text = string.Empty;

        weaponStatsText.text += $"Rate of fire: <size=25>{shootController.RateOfFireWithModifiers}</size>\n";
        weaponStatsText.text += $"Muzzle velocity: <size=25>{shootController.MuzzleVelocityWithModifiers}</size>\n";
        weaponStatsText.text += $"Horizontal recoil: <size=25>{shootController.MuzzleVelocityWithModifiers}</size>\n";
        weaponStatsText.text += $"Vertical recoil: <size=25>{w.yRecoil - (w.yRecoil * shootController.GetTotalModifierValue(WeaponStatModifierType.RecoilModifier))}</size>\n";
    }
}
