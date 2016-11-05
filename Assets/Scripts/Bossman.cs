using UnityEngine;

public class Bossman : MonoBehaviour
{
    public GameObject DeathParticles;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PlayerShot")
        {
            GameSystem.instance.GotOne(gameObject, 150);

            Destroy(gameObject);

            Destroy(coll.collider.gameObject);

            if (DeathParticles != null)
            {
                var particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);

                var obj = ((GameObject)particles);
                var v = transform.GetComponent<Rigidbody2D>().velocity;
                var force = obj.GetComponent<ParticleSystem>().forceOverLifetime;

                force.enabled = true;
                force.x = new ParticleSystem.MinMaxCurve(-v.x * 2);
            }
        }
    }
}
