using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 1f;
    public GameObject Shot;
    public GameObject DeathParticles;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x + Input.GetAxis("Horizontal") * Speed;

        transform.position = new Vector3(Mathf.Clamp(x, -8, 26), transform.position.y, transform.position.z);

        if (Input.GetButtonDown("Fire1"))
        {
            var shot = Instantiate<GameObject>(Shot);

            shot.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

            shot.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 20);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "EnemyShot")
        {
            Destroy(gameObject);

            Destroy(coll.collider.gameObject);

            if (DeathParticles != null)
            {
                var particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);

                var obj = ((GameObject)particles);
            }
        }
    }
}
