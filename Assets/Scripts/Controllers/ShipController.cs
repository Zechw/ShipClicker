using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour {

    //todo base ship, extended into player ship & enemy ship(s)

    public float power = 30;
    public float powerDrainRate = 5;
    public float powerGainPerClick = 5;
    public float laserPowerConsumption = 2;
    public float damageTakenOnHit = 10;

    public bool unlimitedPower = false;
    public bool noDeath = false;

    private float minX = -22;
    private float maxX = -10;
    private float minY = -11;
    private float maxY = 11;
    private float moveSpeed = 10;

    public Transform target;
    public Transform laser;
    public ParticleSystem explosion;
    public ParticleSystem shield;
    public ParticleSystem shieldsDown;
    private ParticleSystem shieldWarning1;
    private ParticleSystem shieldWarning2;


    [HideInInspector] public LevelManager activeLevel;

    private bool mouseIsOver;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //keyboard controls!
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(x, y) * moveSpeed * Time.deltaTime, Space.World);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY));




        power -= powerDrainRate * Time.deltaTime;
        transform.Find("alpha_mask").GetComponent<SpriteMask>().alphaCutoff = power / 100;

        if(power <= 10 && shieldWarning1 == null && shieldWarning2 == null)
        {
            shieldWarning1 = Instantiate(shieldsDown, transform.position, Quaternion.identity, transform);
            shieldWarning2 = Instantiate(shieldsDown, transform.position, Quaternion.Euler(0, 0, 180), transform);
        }

        if (power > 10 && shieldWarning1 != null && shieldWarning2 != null)
        {
            Destroy(shieldWarning1);
            Destroy(shieldWarning2);
        }
        

        //track to target
        Vector3 diff = target.position - transform.position;
        float angle = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);


        //fire weapon
        if(Input.GetMouseButtonDown(0) && !mouseIsOver)
        {
            FireWeapon();
        }

        power = Mathf.Clamp(power, 0, 100);
    }

    void OnMouseDown()
    {
        power += powerGainPerClick;
        activeLevel.AddScore(1);
    }

    private void OnMouseEnter()
    {
        mouseIsOver = true;
        target.GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void OnMouseExit()
    {
        mouseIsOver = false;
        target.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        power -= damageTakenOnHit;
        if (power <= 0 && !noDeath)
        {
            Die();
        }
        else
        {
            ParticleSystem shield1 = Instantiate(shield, transform.position, collision.transform.rotation);
            ParticleSystem shield2 = Instantiate(shield, transform.position, collision.transform.rotation);
            shield1.transform.Rotate(0, 0, -90 -5);
            shield2.transform.Rotate(0, 180, -90 -5);
            Destroy(shield1.gameObject, 2);
            Destroy(shield2.gameObject, 2);
        }
        Destroy(collision.gameObject);
    }

    private void Die()
    {
        AudioSource s = Instantiate(activeLevel.boomSound);
        Destroy(s, 3);

        ParticleSystem expClone1 = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expClone1.gameObject, 3);

        ParticleSystem expClone2 = Instantiate(explosion, transform.position, Quaternion.identity);
        expClone2.transform.localScale.Scale(Vector3.one * 4);
        Destroy(expClone2.gameObject, 3);

        Destroy(shieldWarning1);
        Destroy(shieldWarning2);

        Destroy(gameObject);
    }

    void FireWeapon()
    {
        if (power < laserPowerConsumption && !unlimitedPower) return;

        AudioSource s = Instantiate(activeLevel.laserSound);
        Destroy(s.gameObject, 1);

        power -= laserPowerConsumption;
        Transform laserClone1 = Instantiate(laser, transform.position, transform.rotation);
        laserClone1.GetComponent<LaserController>().offset(0.2f);
        Physics2D.IgnoreCollision(laserClone1.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        Transform laserClone2 = Instantiate(laser, transform.position, transform.rotation);
        laserClone2.GetComponent<LaserController>().offset(-0.2f);
        Physics2D.IgnoreCollision(laserClone2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
}
