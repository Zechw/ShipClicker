using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {


    [HideInInspector] public LevelManager activeLevel;

    public ParticleSystem explosion;

    private float velocity;
    private float spin;
    private float scale;

	// Use this for initialization
	void Start () {
        velocity = Random.Range(2, 10);
        spin = Random.Range(-150, 150);
        scale = Random.Range(.5f, 1.5f);
        transform.localScale = Vector3.one * scale;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.left * velocity * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward * spin * Time.deltaTime);
	}

    private void OnDestroy()
    {
    }

    private void OnBecameInvisible()
    {
        //Assumes asteroids only move down
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        AudioSource s = Instantiate(activeLevel.boomSound);
        Destroy(s, 3);

        activeLevel.AddScore(5);

        ParticleSystem expClone = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone.gameObject, 3);

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
