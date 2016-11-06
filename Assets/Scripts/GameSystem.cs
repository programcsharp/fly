using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class TrapperKeeper
{
    public static bool IsRestartLevel = false;
}

public class GameSystem : MonoBehaviour
{
    public Text LivesText;
    public Text ScoreText;
    public Text WaveText;
    public Text StartText;
    public Text GameOverText;

    public GameObject Player;
    public GameObject[] Sprites;
    public GameObject Bossman;
    public GameObject Bunker;

    public GameObject EnemyShot;

    public AudioSource GameOverSound;
    public AudioSource BackgroundMusic;

    GameObject _player;

    GameObject _container;

    GameObject[,] _enemies;

    int _score = 0;
    int _lives = 3;
    int _wave = 0;

    public bool _isPlaying = false;
    public bool _isCreatingEnemies = false;

    public static GameSystem instance;

    // Use this for initialization
    void Start()
    {
        if (instance != this)
            instance = this;

        Physics2D.IgnoreLayerCollision(8, 8);

        _container = new GameObject();

        var rigidbody = _container.AddComponent<Rigidbody2D>();

        rigidbody.isKinematic = true;

        CreateEnemies();

        Time.timeScale = 0;
        GameOverText.enabled = false;

        if (TrapperKeeper.IsRestartLevel)
            StartGame();
    }

    void CreateEnemies()
    {
        _container.transform.position = new Vector3(0, 5, 0);

        var rigidbody = _container.GetComponent<Rigidbody2D>();

        rigidbody.velocity = new Vector2(-2, 0);

        _enemies = new GameObject[10, 5];

        for (int y = 5; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                var obj = Instantiate<GameObject>(Sprites[(y - 5) / 2]);

                obj.transform.parent = _container.transform;
                obj.transform.position = new Vector3(x * 2, y * 2);

                _enemies[x, y - 5] = obj;
            }
        }

        _wave++;

        if (_wave > 1)
            Time.timeScale += 0.1f;

        UpdateStatusDisplay();

        _isCreatingEnemies = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlaying)
        {
            if (_isCreatingEnemies)
                return;
            
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

            bool allDead = true;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var enemy = _enemies[x, y];
                
                    if (enemy != null && enemy.activeInHierarchy)
                    {
                        allDead = false;

                        break;
                    }
                }
            }

            if (allDead)
            {
                _isCreatingEnemies = true;

                Invoke("CreateEnemies", 1);
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            if (_lives > 0)
                StartGame();
            else
            {
                TrapperKeeper.IsRestartLevel = true;

                SceneManager.LoadScene("GameScene");
            }
        }
    }

    void SpawnEnemyShot()
    {
        if (UnityEngine.Random.value < 0.5)
        {
            var x = UnityEngine.Random.Range(0, 9);

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

    void SpawnBoss()
    {
        if (_container.transform.position.y < 4 && UnityEngine.Random.value < 0.8)
        {
            var boss = Instantiate<GameObject>(Bossman);

            bool isLeft = UnityEngine.Random.value <= 0.5;
            boss.transform.position = new Vector3(isLeft ? -9 : 26, 18);

            boss.GetComponent<Rigidbody2D>().velocity = new Vector2((isLeft ? 1 : -1) * 10, 0);
        }
    }

    void StartGame()
    {
        _lives = 3;
        _score = 0;
        _wave = 1;

        _isPlaying = true;
        Time.timeScale = 1;

        NewLife();

        StartText.enabled = false;

        for (int x = 0; x < 6; x++)
        {
            var obj = Instantiate<GameObject>(Bunker);

            obj.transform.position = new Vector3(-6 + x * 6, 2);
        }

        BackgroundMusic.PlayDelayed(1);

        InvokeRepeating("SpawnBoss", 15, 15);
        InvokeRepeating("SpawnEnemyShot", 3, .1f);

        TrapperKeeper.IsRestartLevel = false;
    }

    public void YoureDead()
    {
        _lives--;

        UpdateStatusDisplay();

        if (_lives > 0)
            Invoke("NewLife", 2);
        else
            GameOver();
    }

    private void GameOver()
    {
        BackgroundMusic.Stop();

        _isPlaying = false;
        GameOverText.enabled = true;

        GameOverSound.PlayDelayed(0.25f);

        StartCoroutine(FreezeOverSeconds(2, () =>
        {
            StartText.enabled = true;
        }));
    }

    public void GotOne(GameObject enemy, int scoreValue = 50)
    {
        _score += scoreValue;

        UpdateStatusDisplay();
    }

    public void UpdateStatusDisplay()
    {
        LivesText.text = _lives.ToString();
        ScoreText.text = _score.ToString();
        WaveText.text = _wave.ToString();
    }

    public void NewLife()
    {
        _player = Instantiate<GameObject>(Player);
        _player.transform.position = new Vector3(9.5f, -2);

        UpdateStatusDisplay();
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

    public static IEnumerator FreezeOverSeconds(float seconds, Action onComplete = null)
    {
        float elapsedTime = 0;
        float startingScale = Time.timeScale;

        while (elapsedTime < seconds)
        {
            Time.timeScale = Mathf.Lerp(startingScale, 0, (elapsedTime / seconds));

            elapsedTime += Time.unscaledDeltaTime;

            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 0;

        if (onComplete != null)
            onComplete();
    }
}
