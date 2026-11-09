using UnityEngine;

public class Particle
{
    public static GenericDictionary<ENUM_ParticleType, GameObject> library => Resources.Load<soDATA_ParticleLibrary>("Particles/Default").particleInstances; //UGLY SOLUTION

    public static void SpawnParticle(ENUM_ParticleType type, Vector3 position)
    {
        Object.Instantiate(library[type],position,library[type].transform.rotation);
    }
}

public enum ENUM_ParticleType
{
    None,
    HullDamaged,
    ArmorDamaged,
    ShieldDamaged,
    ShipDestroyed,

}