using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : ManagerLocatable
{
    [SerializeField] private ParticleSystem blockClearParticlePrefab;
    [SerializeField] private ParticleSystem grinderParticlePrefab;
    [SerializeField] private ParticleSystem iceRevealParticlePrefab;

    public void PlayIceRevealParticle(Vector3 position)
    {
        var particle = Instantiate(iceRevealParticlePrefab, position, Quaternion.identity);
        particle.Play();
        var mainModule = particle.main;
        Destroy(particle.gameObject, mainModule.duration + mainModule.startLifetime.constantMax);
    }
}
