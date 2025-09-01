using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{

    [SerializeField] private SoundList[] soundList;
    [HideInInspector] public AudioSource audioSource;
    public static SoundManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        if (instance == null)
        {
            instance = this;
        }
        
        
        // instance = this;
        audioSource = GetComponent<AudioSource>();
    }

   
    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        
        instance = this;
        audioSource = GetComponent<AudioSource>();
        
        if (!Application.isPlaying) return;
        Debug.Log(instance.audioSource.volume);
    }
    
    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (randomClip == null || clips.Length == 0) return;
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static void PlaySoundWithDelay(SoundType sound, float volume = 1.0f, float delay = 0f)
    {
        instance.StartCoroutine(PlaySoundCoroutine(sound, volume, delay));
    }

    private static IEnumerator PlaySoundCoroutine(SoundType sound, float volume = 1.0f, float delay = 0f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        if (randomClip == null || clips.Length == 0) yield break;
        yield return new WaitForSeconds(delay);
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static void StopSound()
    {
        instance.audioSource.Stop();
    }

    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType)); 
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }

        PlayerHealth.onPlayerDeath += OnPlayerDeath;
    }

    public static AudioClip GetSound(SoundType sound)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips.Length == 0)
        {
            Debug.LogWarning("SoundType: " + sound + " has no clips!");
            return null;
        }
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        return randomClip;
    }

    private void OnDisable()
    {
        PlayerHealth.onPlayerDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        StopSound();
    }
}

public enum SoundType
{ 
    JUMP = 0,
    LAND = 1,
    SHOOT = 2,
    BOLTFOWARD = 3,
    CLIPHIT = 4,
    CLIPIN = 5,
    CLIPOUT = 6,
    COVERDOWN = 7,
    COVERUP = 8,
    BOLTBACK = 9,
    RELOAD = 10,
    FOOTSTEPS = 11,
    DRAW = 12,
    PICKUP = 13,
    
    // UI
    UI_HOVER = 14,
    UI_PLAY = 15,
    UI_CLICK = 16,
    UI_OPEN = 17,
    UI_CLOSE = 18,
    
    // Enemy
    ENEMY_ATTACK = 19,
    ENEMY_SCREAM = 20,
    ENEMY_SLASHEDNPC = 21,
    ENEMY_DEATH = 22,
    ENEMY_BOTHCLAWATTACK = 23,
    ENEMY_BITE = 24,
    ENEMY_TEETH = 25,

    // NPC
    NPC_MALEHURT = 26,
    NPC_FEMALEHURT = 27,
    NPC_TUMBLE1 = 28,
    NPC_TUMBLE2 = 29,

    // Bullets
    BULLET_IMPACT_FLESH = 30,
    BULLET_IMPACT_WALL = 31,

    // Game
    GAME_OVER = 32
}



[System.Serializable]
public struct SoundList
{
    public AudioClip[] Sounds => sounds;
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}

