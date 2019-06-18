﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitSoundPreview : MonoBehaviour {

    public HitSoundDatabase hitSoundDatabase; // Required for loading all the hit sounds in the game
    public AudioSource hitSoundAudioSource; // The audio source that plays the hit sounds
    public int hitSoundChosenIndex; // The hit sound selected
    private float hitSoundVolume = 0.0f; // Volume of the hit sound
    private float missSoundVolume = 0.5f; // Volume of the miss sound

    // Level changer
    public LevelChanger levelChanger; // The level changer

    // The hit sound selected number text
    public TextMeshProUGUI hitSoundSelectedNumberText;



    void Start()
    {
        hitSoundChosenIndex = 0;
        hitSoundDatabase = FindObjectOfType<HitSoundDatabase>();

        // Update the selected number text
        hitSoundSelectedNumberText.text = hitSoundChosenIndex.ToString();


        // Load the saved hit sound selected index if it exists
        LoadPlayerPrefsHitSoundSelectedIndex();
    }

    void Update()
    {
        // Find the current level
        levelChanger = FindObjectOfType<LevelChanger>();

        // Get the hit sound controller, an array is used as another will spawn when going back to the main menu, to prevent multiple from existing we store in an array
        GameObject[] hitSoundController = GameObject.FindGameObjectsWithTag("HitSoundManager");
        // Get the hit sound audio source for playing the hit sounds
        hitSoundAudioSource = GameObject.FindGameObjectWithTag("HitSoundManager").GetComponent<AudioSource>();


        // If the song select, gameplay or results scene do not destroy but destroy for all other scenes
        if (levelChanger.currentLevelIndex == levelChanger.songSelectSceneIndex || levelChanger.currentLevelIndex == levelChanger.gameplaySceneIndex || levelChanger.currentLevelIndex == levelChanger.resultsSceneIndex)
        {
            // Dont destroy the hit sound manager
            DontDestroyOnLoad(this.gameObject);
        }

        // However if escape is pressed delete the hit sound manager
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("destroyed");
            Destroy(this.gameObject);
        }


        // Check if there are more than 1 hit sound controllers
        if (hitSoundController.Length > 1)
        {
            // Destroy is there is more than 1
            Destroy(hitSoundController[1].gameObject);
        }
        else
        {
            // Do not destroy any
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Set player prefs hit sound selected index 
    private void SetPlayerPrefsHitSoundSelectedIndex()
    {
        PlayerPrefs.SetInt("hitSoundChosenIndex", hitSoundChosenIndex);
        PlayerPrefs.Save();
    }

    // Load the hit sound selected index from the player prefs loading
    private void LoadPlayerPrefsHitSoundSelectedIndex()
    {
        if (PlayerPrefs.HasKey("hitSoundChosenIndex"))
        {
            hitSoundChosenIndex = PlayerPrefs.GetInt("hitSoundChosenIndex");

            UpdateHitSoundChosenIndexText();
        }
    }

    // Update the selected number text
    private void UpdateHitSoundChosenIndexText()
    {
        // Update the selected number text
        hitSoundSelectedNumberText.text = hitSoundChosenIndex.ToString();
    }

    // Increment the hit sound selected
    public void IncrementHitSoundSelected()
    {
        if (hitSoundChosenIndex == hitSoundDatabase.hitSoundClip.Length - 1)
        {
            // Do not increment
            Debug.Log("Do not increment");
        }
        else
        { 
            Debug.Log("increment");
            // Increase the hitsound chosen index
            hitSoundChosenIndex++;
            // Update the selected number text
            hitSoundSelectedNumberText.text = hitSoundChosenIndex.ToString();

            // Set the new hit sound selected in the player prefs for saving
            SetPlayerPrefsHitSoundSelectedIndex();

            // Play the new hit sound
            PlayHitSound();
        }
    }

    // Decrement the hit sound selected
    public void DecrementHitSoundSelected()
    {
        if (hitSoundChosenIndex == 0)
        {
            // Do not decrement
            Debug.Log("Do not decrement");
        }
        else
        {
            Debug.Log("decrement");
            // Decrement the chosen hit sound index
            hitSoundChosenIndex--;
            // Update the selected number text
            hitSoundSelectedNumberText.text = hitSoundChosenIndex.ToString();

            // Set the new hit sound selected in the player prefs for saving
            SetPlayerPrefsHitSoundSelectedIndex();

            // Play the hit sound
            PlayHitSound();
        }
    }

    // Play the hit sound chosen
    public void PlayHitSound()
    {
        hitSoundAudioSource.PlayOneShot(hitSoundDatabase.hitSoundClip[hitSoundChosenIndex], hitSoundVolume);
    }

    // Play miss sound
    public void PlayMissSound()
    {
        hitSoundAudioSource.PlayOneShot(hitSoundDatabase.missSoundClip, missSoundVolume);
    }

    public void MuteHitSound()
    {
        hitSoundVolume = 0f;
    }

    public void UnMuteHitSound()
    {
        hitSoundVolume = 0.5f;
    }
}
