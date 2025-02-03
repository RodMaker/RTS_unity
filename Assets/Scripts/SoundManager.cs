using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Infantry")] // example specific attacks
    private AudioSource infantryAttackChannel;
    public AudioClip infantryAttackClip;

    [Header("Unit")]
    public int poolSize = 2;
    private int unitCurrentPoolIndex = 0;
    private int constructionCurrentPoolIndex = 0;

    private AudioSource[] unitVoiceChannelPool;
    public AudioClip[] unitSelectionSounds;
    public AudioClip[] unitCommandSounds;

    [Header("Buildings")]
    private AudioSource[] constructionBuildingChannelPool;
    private AudioSource destructionBuildingChannel;
    private AudioSource extraBuildingChannel;
    public AudioClip buildingConstructionSound;
    public AudioClip buildingDestructionSound;
    public AudioClip sellingSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        infantryAttackChannel = gameObject.AddComponent<AudioSource>();
        //infantryAttackChannel.volume = 0.1f;
        //infantryAttackChannel.playOnAwake = false;

        // Create array of audiosources with a "poolSize"
        unitVoiceChannelPool = new AudioSource[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            unitVoiceChannelPool[i] = gameObject.AddComponent<AudioSource>();
        }

        //unitVoiceChannel.volume = 1f;
        //unitVoiceChannel.playOnAwake = false;

        constructionBuildingChannelPool = new AudioSource[3];

        for (int i = 0; i < poolSize; i++)
        {
            constructionBuildingChannelPool[i] = gameObject.AddComponent<AudioSource>();
        }

        //constructionBuildingChannel.volume = 1f;
        //constructionBuildingChannel.playOnAwake = false;

        destructionBuildingChannel = gameObject.AddComponent<AudioSource>();
        //destructionBuildingChannel.volume = 1f;
        //destructionBuildingChannel.playOnAwake = false;

        extraBuildingChannel = gameObject.AddComponent<AudioSource>();
        //extraBuildingChannel.volume = 1f;
        //extraBuildingChannel.playOnAwake = false;
    }

    public void PlayInfantryAttackSound()
    {
        if (infantryAttackChannel.isPlaying == false)
        {
            infantryAttackChannel.PlayOneShot(infantryAttackClip);
        }
    }

    public void PlayBuildingConstructionSound()
    {
        constructionBuildingChannelPool[constructionCurrentPoolIndex].PlayOneShot(buildingConstructionSound);

        constructionCurrentPoolIndex = (constructionCurrentPoolIndex + 1) % poolSize;
    }

    public void PlayBuildingDestructionSound()
    {
        if (destructionBuildingChannel.isPlaying == false)
        {
            destructionBuildingChannel.PlayOneShot(buildingDestructionSound);
        }
    }

    public void PlayBuildingSellingSound()
    {
        if (extraBuildingChannel.isPlaying == false)
        {
            extraBuildingChannel.PlayOneShot(sellingSound);
        }
    }

    public void PlayUnitSelectionSound()
    {
        AudioClip randomClip = unitSelectionSounds[Random.Range(0, unitSelectionSounds.Length)];
        
        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);

        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }

    public void PlayUnitCommandSound()
    {
        AudioClip randomClip = unitCommandSounds[Random.Range(0, unitCommandSounds.Length)];

        unitVoiceChannelPool[unitCurrentPoolIndex].PlayOneShot(randomClip);

        unitCurrentPoolIndex = (unitCurrentPoolIndex + 1) % poolSize;
    }
}
