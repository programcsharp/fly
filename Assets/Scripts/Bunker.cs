using System.Collections.Generic;
using UnityEngine;

public class Bunker : MonoBehaviour
{
    public AudioClip BunkerHit;

    Texture2D _texture;

    // Use this for initialization
    void Start ()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Texture2D sheet = renderer.sprite.texture;
        Texture2D tex = new Texture2D((int)renderer.sprite.rect.width, (int)renderer.sprite.rect.height, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;

        var pixels = sheet.GetPixels((int)renderer.sprite.rect.x, (int)renderer.sprite.rect.y, tex.width, tex.height);

        tex.SetPixels(pixels);

        tex.Apply();

        _texture = tex;

        renderer.sprite = Sprite.Create(_texture, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), renderer.sprite.pixelsPerUnit);
    }

    // Update is called once per frame
    void Update ()
    {
        List<GameObject> shots = new List<GameObject>();

        shots.AddRange(GameObject.FindGameObjectsWithTag("PlayerShot"));
        shots.AddRange(GameObject.FindGameObjectsWithTag("EnemyShot"));
        shots.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Collider2D collider = GetComponent<Collider2D>();

        foreach (var shot in shots)
        {
            if (collider.bounds.Contains(shot.transform.position))
            {
                var pos = transform.InverseTransformPoint(shot.transform.position);
                var x = (int)Mathf.Clamp(pos.x * renderer.sprite.pixelsPerUnit + _texture.width / 2, 0, _texture.width);
                var y = (int)Mathf.Clamp(pos.y * renderer.sprite.pixelsPerUnit + _texture.height / 2, 0, _texture.height);
                var color = _texture.GetPixel(x, y);

                if (color.a == 1)
                {
                    GetComponent<AudioSource>().PlayOneShot(BunkerHit);

                    if (shot.tag == "Enemy")
                        shot.GetComponent<Enemy>().ShowDeath();

                    Destroy(shot.gameObject);

                    color.a = 0;

                    Texture2D newTex = new Texture2D(_texture.width, _texture.height, TextureFormat.ARGB32, false);

                    newTex.filterMode = FilterMode.Point;

                    newTex.SetPixels32(_texture.GetPixels32());

                    Circle(newTex, x, y, 5, color);

                    newTex.Apply();

                    renderer.sprite = Sprite.Create(newTex, renderer.sprite.rect, new Vector2(0.5f, 0.5f), renderer.sprite.pixelsPerUnit);

                    Destroy(_texture);
                    _texture = newTex;
                }
            }
        }
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                if (px > 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, col);
                if (nx > 0 && nx < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(nx, py, col);

                if (px > 0 && px < tex.width && ny >= 0 && ny < tex.height)
                    tex.SetPixel(px, ny, col);
                if (nx > 0 && nx < tex.width && ny >= 0 && ny < tex.height)
                    tex.SetPixel(nx, ny, col);

            }
        }
    }
}
