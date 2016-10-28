using UnityEngine;

public class PlayerDeath : MonoBehaviour
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
