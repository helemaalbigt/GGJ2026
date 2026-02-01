using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    // default SFX
    [SerializeField] private List<AudioSource> onCollisionSfx;


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayOnCollisionSfx(float volume)
    {
        if (this.onCollisionSfx.Count == 0) return;
        int randomIndex = Random.Range(0, this.onCollisionSfx.Count);
        onCollisionSfx[randomIndex].volume = volume;
        onCollisionSfx[randomIndex].Play();
    }
}