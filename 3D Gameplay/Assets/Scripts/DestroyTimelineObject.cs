﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyTimelineObject : MonoBehaviour {

    private PlacedObject placedObject;
    public float timelineHitObjectSpawnTime;
    public Slider timelineSlider;
    public MetronomePro_Player metronome_Player;
    private int timelineObjectListIndex;

    private AudioSource menuSFXAudioSource;
    private HitSoundDatabase hitSoundDatabase;
    private AudioClip selectedSoundClip;
    private AudioClip clickedSoundClip;
    private AudioClip deletedSoundClip;

    private void Start()
    {
        timelineObjectListIndex = 0;

        placedObject = FindObjectOfType<PlacedObject>();

        // Get the reference to the timelines own slider
        timelineSlider = this.gameObject.GetComponent<Slider>();

        // Get the reference to the metronome_Player
        metronome_Player = FindObjectOfType<MetronomePro_Player>();

        // Find the menuSFXAudioSource
        menuSFXAudioSource = GameObject.FindGameObjectWithTag("MenuSFXAudioSource").GetComponentInChildren<AudioSource>();

        // Get the hit sound database
        hitSoundDatabase = FindObjectOfType<HitSoundDatabase>();


        if (hitSoundDatabase != null)
        {
            // Assign the selected sound clip
            selectedSoundClip = hitSoundDatabase.hitSoundClip[36];

            // Assign the clicked sound clip
            clickedSoundClip = hitSoundDatabase.hitSoundClip[0];

            // Assign the deleteed sound clip
            deletedSoundClip = hitSoundDatabase.hitSoundClip[42];
        }
    }


    // Play deleted sound
    public void PlayDeletedSound()
    {
        menuSFXAudioSource.PlayOneShot(deletedSoundClip);
    }

    // Play the selected sound
    public void PlaySelectedSound()
    {
        menuSFXAudioSource.PlayOneShot(selectedSoundClip);
    }

    // Play clicked sound
    public void PlayClickedSound()
    {
        menuSFXAudioSource.PlayOneShot(clickedSoundClip);
    }

    public void DestroyEditorTimelineObject()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Destroy the instantiated editorHitObject
            placedObject.DestroyInstantiatedEditorHitObject();
            // Play deleted sound effect
            PlayDeletedSound();
            // Check null timeline objects and update the list order/remove null objects from all lists
            placedObject.RemoveTimelineObject();
            // Destroy the game object
            Destroy(this.gameObject);
        }
    }
    
    public void FindIndexOfTimelineObject()
    {
        // Pass this game object
        timelineObjectListIndex = placedObject.GetIndexOfRaycastTimelineObject(this.gameObject);
    }

    // Update the timelines spawn time
    public void UpdateTimelineHitObjectSpawnTime()
    {
        if (metronome_Player != null)
        {
            timelineHitObjectSpawnTime = metronome_Player.UpdateTimelineHitObjectSpawnTimes(timelineSlider);
            // Send the new spawn time and the editorHitObject index to update
            placedObject.UpdateEditorHitObjectSpawnTime(timelineHitObjectSpawnTime, timelineObjectListIndex);
        }
    }

}
