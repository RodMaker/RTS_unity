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
    private AudioSource unitSelectionChannel;
    private AudioSource unitCommandChannel;
    public AudioClip unitSelectionSound;
    public AudioClip unitCommandSound;

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
        infantryAttackChannel.volume = 0.1f;
        infantryAttackChannel.playOnAwake = false;

        unitSelectionChannel = gameObject.AddComponent<AudioSource>();
        unitSelectionChannel.volume = 0.1f;
        unitSelectionChannel.playOnAwake = false;

        unitCommandChannel = gameObject.AddComponent<AudioSource>();
        unitCommandChannel.volume = 0.1f;
        unitCommandChannel.playOnAwake = false;

        constructionBuildingChannel = gameObject.AddComponent<AudioSource>();
        constructionBuildingChannel.volume = 0.1f;
        constructionBuildingChannel.playOnAwake = false;

        destructionBuildingChannel = gameObject.AddComponent<AudioSource>();
        destructionBuildingChannel.volume = 0.1f;
        destructionBuildingChannel.playOnAwake = false;

        extraBuildingChannel = gameObject.AddComponent<AudioSource>();
        extraBuildingChannel.volume = 0.1f;
        extraBuildingChannel.playOnAwake = false;
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
        if (unitSelectionChannel.isPlaying == false)
        {
            unitSelectionChannel.PlayOneShot(unitSelectionSound);
        }
    }

    public void PlayUnitCommandSound()
    {
        if (unitCommandChannel.isPlaying == false)
        {
            unitCommandChannel.PlayOneShot(unitCommandSound);
        }
    }
}
