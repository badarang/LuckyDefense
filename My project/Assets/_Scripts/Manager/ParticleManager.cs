using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    private Dictionary<string, GameObject> particleDictionary;
    [SerializeField] private Transform particleParent;
    
    public void OnLoad()
    {
        Statics.DebugColor("ParticleManager Loaded", new Color(.6f, 0, 1));
    }
    
    private void Awake()
    {
        base.Awake();
        particleDictionary = new Dictionary<string, GameObject>();
        GameObject[] particles = Resources.LoadAll<GameObject>("Prefabs/Particles");
        foreach (var particle in particles)
        {
            particleDictionary.Add(particle.name, particle.gameObject);
        }
    }
    
    public void SpawnParticle(ParticleType particleType, Vector3 position)
    {
        var particleName = particleType.ToString();
        if (particleDictionary.ContainsKey(particleName))
        {
            GameObject particle = Instantiate(particleDictionary[particleName], position, Quaternion.identity, particleParent);
            Destroy(particle, 5f);
        }
        else
        {
            Debug.LogError("Particle not found: " + particleName);
        }
    }
}

public enum ParticleType
{
    VFX_Shine_Orange,
    VFX_Shine_Purple,
    VFX_Star_Confetti,

}