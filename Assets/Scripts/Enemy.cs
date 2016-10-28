using UnityEngine;

public class Enemy : MonoBehaviour
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

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PlayerShot")
        {
            GameSystem.instance.GotOne(gameObject);

            Destroy(gameObject);

            Destroy(coll.collider.gameObject);

            if (DeathParticles != null)
            {
                var particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);

                var obj = ((GameObject)particles);
                
                obj.transform.parent = transform.parent;

                var v = transform.parent.GetComponent<Rigidbody2D>().velocity;
                var force = obj.GetComponent<ParticleSystem>().forceOverLifetime;

                force.enabled = true;
                force.x = new ParticleSystem.MinMaxCurve(-v.x * 2);
            }
        }
    }
}
