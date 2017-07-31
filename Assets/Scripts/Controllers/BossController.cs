using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {

    public ParticleSystem explosion;
    public ParticleSystem shield;
    public Transform laser;

    private float desiredX = 13;
    private float maxSpeed = 3;

    public float power = 100;
    public float rechargeRate = 4;
    public float damageTakenOnHit = 2;
    

    [HideInInspector] public LevelManager activeLevel;

    // Use this for initialization
    void Start () {
        InvokeRepeating("FireWeapon", 4,0.2f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 move = transform.position - new Vector3(desiredX, player.transform.position.y);
        move = new Vector3(Mathf.Clamp(move.x, -maxSpeed, maxSpeed), Mathf.Clamp(move.y, -maxSpeed, maxSpeed));
        
        transform.position = transform.position - (move * Time.deltaTime);

        power = Mathf.Clamp(power + rechargeRate * Time.deltaTime, 0, 100);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        power -= damageTakenOnHit;
        if (power <= 0)
        {
            Die();
        }
        else
        {
            ParticleSystem shield1 = Instantiate(shield, transform.position, collision.transform.rotation);
            ParticleSystem shield2 = Instantiate(shield, transform.position, collision.transform.rotation);
            shield1.transform.Rotate(0, 0, -90 - 5);
            shield2.transform.Rotate(0, 180, -90 - 5);
            shield1.transform.localScale = Vector3.one * 4;
            shield2.transform.localScale = Vector3.one * 4;
            Destroy(shield1.gameObject, 2);
            Destroy(shield2.gameObject, 2);
        }
        Destroy(collision.gameObject);
    }

    private void Die()
    {
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        activeLevel.endGame(true);
        CancelInvoke("FireWeapon");

        AudioSource s1 = Instantiate(activeLevel.boomSound);
        Destroy(s1, 3);

        ParticleSystem expClone1 = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone1.gameObject, 3);

        yield return new WaitForSeconds(2);

        AudioSource s2 = Instantiate(activeLevel.boomSound);
        Destroy(s2, 3);

        ParticleSystem expClone2 = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone2.gameObject, 3);

        yield return new WaitForSeconds(0.2f);

        AudioSource s3 = Instantiate(activeLevel.boomSound);
        Destroy(s3, 3);

        ParticleSystem expClone3 = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone3.gameObject, 3);

        Destroy(gameObject);

    }

    private void FireWeapon()
    {
        AudioSource s = Instantiate(activeLevel.laserSound);
        s.volume = 0.1f;
        Destroy(s.gameObject, 1);

        Transform laserClone1 = Instantiate(laser, transform.position, Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90)));
        Physics2D.IgnoreCollision(laserClone1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
}
