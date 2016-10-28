using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Sprites;

    public GameObject EnemyShot;

    GameObject _container;

    GameObject[,] _enemies;

    // Use this for initialization
    void Start()
    {
        Physics.IgnoreLayerCollision(8, 8);

        _container = new GameObject();

        _container.transform.position = new Vector3(0, 5, 0);

        var rigidbody = _container.AddComponent<Rigidbody2D>();

        rigidbody.isKinematic = true;
        rigidbody.velocity = new Vector2(-2, 0);

        _enemies = new GameObject[10, 5];

        for (int y = 5; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var obj = Instantiate<GameObject>(Sprites[(y - 5) / 2]);

                obj.transform.parent = _container.transform;
                obj.transform.position = new Vector3(x * 2, y * 2);
                obj.layer = 8;
                
                _enemies[x, y - 5] = obj;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        var rigidbody = _container.GetComponent<Rigidbody2D>();

        // handle if side enemies are all killed and go over further?
        if (_container.transform.position.x < -7 && rigidbody.velocity.x < 0)
        {
            StartCoroutine(MoveYOverSeconds(_container, _container.transform.position.y - 1, .15f));

            rigidbody.velocity = new Vector2(2, 0);
        }
        else if (_container.transform.position.x > 8 && rigidbody.velocity.x > 0)
        {
            StartCoroutine(MoveYOverSeconds(_container, _container.transform.position.y - 1, .15f));

            rigidbody.velocity = new Vector2(-2, 0);
        }

        if (Random.value < 0.05)
        {
            var x = Random.Range(0, 9);

            for (var y = 0; y < 5; y++)
            {
                var enemy = _enemies[x, y];

                if (enemy != null && _enemies[x, y].activeInHierarchy)
                {
                    var shot = Instantiate<GameObject>(EnemyShot);

                    shot.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y - 1, enemy.transform.position.z);

                    shot.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -15);

                    break;
                }
            }
        }
    }

    // http://answers.unity3d.com/questions/296347/move-transform-to-target-in-x-seconds.html
    public static IEnumerator MoveYOverSeconds(GameObject objectToMove, float endY, float seconds)
    {
        float elapsedTime = 0;
        float startingY = objectToMove.transform.position.y;

        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = new Vector3(objectToMove.transform.position.x, Mathf.Lerp(startingY, endY, (elapsedTime / seconds)));

            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = new Vector3(objectToMove.transform.position.x, endY);
    }
}
