using UnityEngine;
using System.Threading.Tasks;

public class Slippery : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float multiplier;
    [SerializeField] private float accel;
    [SerializeField] private float decel;
    [SerializeField] private float dash;
    [SerializeField] private float _accel;
    [SerializeField] private float _decel;
    [SerializeField] private float _dash;
    [SerializeField] private bool canCheck = true;
    [SerializeField] private int tilesTouched;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if (canCheck)
        {
            _accel = player.accelSpeed;
            _decel = player.decelSpeed;
            _dash = player.dashSpeed;
            multiplier = 1;
        }
        else
            multiplier = .5f;

        player.multiplier = multiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slippery"))
        {
            tilesTouched++;
            if (tilesTouched == 1)
            {
                canCheck = false;
                player.accelSpeed *= accel;
                player.decelSpeed *= decel;
                player.dashSpeed *= dash;
            }
        }
    }
    private async void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slippery"))
        {
            tilesTouched--;
            if (tilesTouched <= 0)
            {
                await Task.Delay(System.TimeSpan.FromSeconds(.125f));
                player.accelSpeed = _accel;
                player.decelSpeed = _decel;
                player.dashSpeed = _dash;
                canCheck = true;
            }
        }
    }
}