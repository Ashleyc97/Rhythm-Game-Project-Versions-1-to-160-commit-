﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyTimelineObject : MonoBehaviour {

    private PlacedObject placedObject;
    public float timelineHitObjectSpawnTime;
    public Slider timelineSlider;
    public MetronomePro_Player metronome_Player;

    private void Start()
    {
        placedObject = FindObjectOfType<PlacedObject>();

        // Get the reference to the timelines own slider
        timelineSlider = this.gameObject.GetComponent<Slider>();

        // Get the reference to the metronome_Player
        metronome_Player = FindObjectOfType<MetronomePro_Player>();
    }


    public void DestroyEditorTimelineObject()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Destroy the game object
            Destroy(this.gameObject);
        }
    }
    
    public void FindIndexOfTimelineObject()
    {
        // Pass this game object
        placedObject.GetIndexOfRaycastTimelineObject(this.gameObject);
    }

    // Update the timelines spawn time
    public void UpdateTimelineHitObjectSpawnTime()
    {
        if (metronome_Player != null)
        {
            timelineHitObjectSpawnTime = metronome_Player.UpdateTimelineHitObjectSpawnTimes(timelineSlider);
        }

    }

}
