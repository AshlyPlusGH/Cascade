using UnityEngine;

[CreateAssetMenu]
public class soDATA_ParticleLibrary : ScriptableObject
{
    public GenericDictionary<ENUM_ParticleType,GameObject> particleInstances = new();
}