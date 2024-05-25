using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class InputParticlesScript : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SpawnParticles(float timing)
    {
        if (PlayerPrefs.GetString("Particles") == "false") return;

        if (timing < 0.05f)
        {
            ModifyBurstCount(30);
        }
        else if (timing < 0.1f)
        {
            ModifyBurstCount(20);
        }
        else 
        {
            ModifyBurstCount(10);
        }
        ps.Play();
    }

    // Only one burst is currently set
    private void ModifyBurstCount(int newBurstCount)
    {
        ParticleSystem.Burst burst = ps.emission.GetBurst(0);
        burst.count = newBurstCount;
        ps.emission.SetBurst(0, burst);
    }
}
