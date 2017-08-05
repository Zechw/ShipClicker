using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldController : MonoBehaviour {

    public LevelManager activeLevel;

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
        {
           // child.GetComponent<ParticleSystem>().Pause();
        }
	}
	
	void Update ()
    {
        /*
        //rotate to player
        if (activeLevel.activePlayer != null)
        {
            transform.rotation = activeLevel.activePlayer.transform.rotation;
        }

        //set speed based on distance to cursor
        Transform crosshairs = activeLevel.activeUI.transform.Find("Crosshairs");
        float distance = Vector3.Magnitude(transform.position - crosshairs.position);
        
        foreach (Transform child in transform)
        {
            child.GetComponent<ParticleSystem>().Simulate(distance / 300, true, false, true);
        }
        */

	}
}
