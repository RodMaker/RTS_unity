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
    private AudioSource unitVoiceChannel;
    public AudioClip[] unitSelectionSounds;
    public AudioClip[] unitCommandSounds;

    [Header("Buildings")]
    private AudioSource constructionBuildingChannel;
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

        unitVoiceChannel = gameObject.AddComponent<AudioSource>();
        //unitVoiceChannel.volume = 1f;
        //unitVoiceChannel.playOnAwake = false;

        constructionBuildingChannel = gameObject.AddComponent<AudioSource>();
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
        if (constructionBuildingChannel.isPlaying == false)
        {
            constructionBuildingChannel.PlayOneShot(buildingConstructionSound);
        }
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
        if (unitVoiceChannel.isPlaying == false)
        {
            AudioClip randomClip = unitSelectionSounds[Random.Range(0, unitSelectionSounds.Length)];
            unitVoiceChannel.PlayOneShot(randomClip);
        }
    }

    public void PlayUnitCommandSound()
    {
        if (unitVoiceChannel.isPlaying == false)
        {
            AudioClip randomClip = unitCommandSounds[Random.Range(0, unitCommandSounds.Length)];
            unitVoiceChannel.PlayOneShot(randomClip);
        }
    }
}
