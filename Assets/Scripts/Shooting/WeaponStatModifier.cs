using System;

[Serializable]
public class WeaponStatModifier
{
    public WeaponStatModifierType type;
    public float modifier = 0.2f;
    public int id = 0;
}

public enum WeaponStatModifierType
{
    Silenced,
    RecoilModifier,
    MuzzleParticleRemover,
    SpreadModifier,
    MuzzleVelocityModifier,
    RateOfFireModifier
}