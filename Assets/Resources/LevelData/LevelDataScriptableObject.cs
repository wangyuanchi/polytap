using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName ="Level Data")]
public class LevelDataScriptableObject : ScriptableObject
{
    [Header("General")]
    [SerializeField] public AudioClip levelMusic;
    [SerializeField] public Sprite background;

    [Header("Particles")]
    [SerializeField] public GameObject inputParticles;
    [SerializeField] public ParticleSystem ambientParticles;

    [Header("Beat Map End Time: It must be at least 0.5s after the last note")]
    [SerializeField] public float beatMapEndTime;   
}
