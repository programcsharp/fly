using UnityEngine;

public class Shot : MonoBehaviour
{

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
        // done in enemy
        //if (coll.gameObject.tag == "Enemy")
        //    Destroy(gameObject);
    }
}
