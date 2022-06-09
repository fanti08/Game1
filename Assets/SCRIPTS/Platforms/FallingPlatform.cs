using UnityEngine;
using System;
using System.Threading.Tasks;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelayTime = 3;
    [SerializeField] private float cooldownTime = 3;
    [SerializeField] private Color disableColor;
    [SerializeField] private Color enableColor;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D coll;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = enableColor;
        coll = GetComponent<Collider2D>();
    }

    private async void OnCollisionEnter2D(Collision2D collision)
    {
        await Task.Delay(TimeSpan.FromSeconds(fallDelayTime));
        coll.enabled = false;
        sprite.color = disableColor;
        await Task.Delay(TimeSpan.FromSeconds(cooldownTime));
        coll.enabled = true;
        sprite.color = enableColor;
    }
}
