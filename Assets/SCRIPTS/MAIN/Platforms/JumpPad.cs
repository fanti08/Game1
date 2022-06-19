using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private Vector2 forceDir;
    [SerializeField] private float sinkDistance;
    [SerializeField] private float sinkDuration;
    private Coroutine sinkRoutine;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(force * forceDir.normalized, ForceMode2D.Impulse);
        if (sinkRoutine != null) 
            StopCoroutine(sinkRoutine);
        sinkRoutine = StartCoroutine(Sink());
    }

    IEnumerator Sink()
    {
        Vector3 topScale = transform.localScale;
        Vector3 bottomScale = originalScale;
        bottomScale.y -= sinkDistance;
        float timer = 0f;
        while (timer < sinkDuration)
        {
            transform.localScale = Vector3.Lerp(topScale, bottomScale, timer / sinkDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        while (timer < sinkDuration)
        {
            transform.localScale = Vector3.Lerp(bottomScale, originalScale, timer / sinkDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}