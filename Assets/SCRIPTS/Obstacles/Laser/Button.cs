using UnityEngine;
using UnityEngine.Events;
using System;
using System.Threading.Tasks;

public class Button : MonoBehaviour
{
    [SerializeField] private UnityEvent onStarted;
    [SerializeField] private UnityEvent onPerforming;
    [SerializeField] private UnityEvent onCanceled;
    [SerializeField] private float cooldownForCancel;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = disableColor;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        onStarted?.Invoke();
        sprite.color = enableColor;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        onPerforming?.Invoke();
    }

    private async void OnTriggerExit2D(Collider2D collider)
    {
        sprite.color = disableColor;
        await Task.Delay(TimeSpan.FromSeconds(cooldownForCancel));
        onCanceled?.Invoke();
    }
}