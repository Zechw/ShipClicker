using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public ParticleSystem explosion;
    public Transform laser;

    public float minDistance = 10;
    public float maxDistance = 14;
    public float moveSpeed = 2;
    public bool creepingIn = true;

    [HideInInspector] public LevelManager activeLevel;

    private float slideSpeed = 0;

    // Use this for initialization
	void Start ()
    {
        InvokeRepeating("FireWeapon", 2f, Random.Range(1.1f, 2.2f));
        InvokeRepeating("ChangeSlide", 0, 3);
	}

	void Update () {
        //face player
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 diff = player.transform.position - transform.position;
        float angle = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //creep closer
        transform.Translate((creepingIn? Vector3.up : Vector3.down) * moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(player.transform.position, transform.position) < minDistance)
        {
            creepingIn = false;
        }

        if (Vector3.Distance(player.transform.position, transform.position) > maxDistance)
        {
            creepingIn = true;
        }

        //slide
        transform.Translate(Vector3.right * slideSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        AudioSource s = Instantiate(activeLevel.boomSound);
        Destroy(s, 3);
        activeLevel.AddScore(10);

        ParticleSystem expClone = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone.gameObject, 3);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }

    private void FireWeapon()
    {
        AudioSource s = Instantiate(activeLevel.laserSound);
        Destroy(s.gameObject, 1);

        Transform laserClone1 = Instantiate(laser, transform.position, transform.rotation);
        laserClone1.GetComponent<LaserController>().offset(0.58f);
        Physics2D.IgnoreCollision(laserClone1.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Transform laserClone2 = Instantiate(laser, transform.position, transform.rotation);
        laserClone2.GetComponent<LaserController>().offset(-0.58f);
        Physics2D.IgnoreCollision(laserClone2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void ChangeSlide()
    {
        slideSpeed = Random.Range(-3, 3);
    }
}
