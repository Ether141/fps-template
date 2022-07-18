using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool isControllerEnabled = true;
    [SerializeField] private float yMinRot;
    [SerializeField] private float yMaxRot;
    [SerializeField] private float xSensitivity = 2f;
    [SerializeField] private float ySensitivity = 2f;

    private Camera cam;
    private ShootController shootController;
    private float rotAroundX, rotAroundY;

    private Vector3 targetRecoilRotation;
    private Vector3 currentRecoilRotation;

    void Start()
    {
        cam = GetComponent<Camera>();
        shootController = transform.parent.GetComponent<ShootController>();
        rotAroundX = transform.eulerAngles.x;
        rotAroundY = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (isControllerEnabled)
            CollectInput();

        CameraRotation();
        Recoil();
    }

    private void CollectInput()
    {
        rotAroundX += Input.GetAxis("Mouse X") * xSensitivity;
        rotAroundY += Input.GetAxis("Mouse Y") * ySensitivity;

        rotAroundY = Mathf.Clamp(rotAroundY, yMinRot, yMaxRot);
    }

    public void ApplyRecoil()
    {
        float xRecoil = shootController.IsAiming ? shootController.Weapon.xRecoil : shootController.Weapon.xRecoil / 2f;
        float yRecoil = shootController.IsAiming ? shootController.Weapon.yRecoil : shootController.Weapon.yRecoil / 2f;

        float modifier = shootController.GetTotalModifierValue(WeaponStatModifierType.RecoilModifier);
        xRecoil -= xRecoil * modifier;
        yRecoil -= yRecoil * modifier;

        targetRecoilRotation += new Vector3(xRecoil, Random.Range(-yRecoil, yRecoil), 0f);
    }

    private void Recoil()
    {
        targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, shootController.Weapon.recoilReturnSpeed * Time.deltaTime);
        currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, shootController.Weapon.recoilSnappiness * Time.fixedDeltaTime);
    }

    private void CameraRotation()
    {
        cam.transform.rotation = Quaternion.Euler(new Vector3(-rotAroundY, rotAroundX, 0) + currentRecoilRotation);
    }
}