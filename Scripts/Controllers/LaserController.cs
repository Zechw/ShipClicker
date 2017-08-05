using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {

    public float velocity = 20;

	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * velocity * Time.deltaTime);
	}

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void offset(float distance)
    {
        transform.Translate(Vector3.right * distance);
    }
}
