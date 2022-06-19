using UnityEngine;
using System;
using System.Threading.Tasks;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelayTime = 3;
    [SerializeField] private float cooldownTime = 3;
    [SerializeField] private Collider2D coll;
    [SerializeField] private Collider2D F_coll;
    [SerializeField] private bool respawnable = false;
    [SerializeField] private bool canDo = true;
    [SerializeField] private SpriteRenderer _;
    [SerializeField] private SpriteRenderer __;
    [SerializeField] private SpriteRenderer ___;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        F_coll = transform.GetChild(0).GetComponent<Collider2D>();
        _ = transform.GetChild(1).GetComponent<SpriteRenderer>();
        __ = transform.GetChild(2).GetComponent<SpriteRenderer>();
        ___ = transform.GetChild(3).GetComponent<SpriteRenderer>();
    }

    private async void OnCollisionEnter2D(Collision2D collision)
    {
        if (canDo)
        {
            canDo = false;
            await Task.Delay(TimeSpan.FromSeconds(fallDelayTime / 3));
            _.enabled = false;
            __.enabled = true;
            await Task.Delay(TimeSpan.FromSeconds(fallDelayTime / 3));
            __.enabled = false;
            ___.enabled = true;
            await Task.Delay(TimeSpan.FromSeconds(fallDelayTime / 3));
            ___.enabled = false;
            coll.enabled = false;
            F_coll.enabled = false;
            if (respawnable)
            {
                await Task.Delay(TimeSpan.FromSeconds(cooldownTime));
                coll.enabled = true;
                F_coll.enabled = true;
                canDo = true;
            }
        }
    }
}
