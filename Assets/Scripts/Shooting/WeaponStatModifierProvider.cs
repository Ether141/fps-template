using UnityEngine;

public class WeaponStatModifierProvider : MonoBehaviour
{
    [SerializeField] private WeaponStatModifier[] modifiers;
    public WeaponStatModifier[] Modifiers => modifiers;
}
