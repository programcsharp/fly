using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().startLifetime);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
