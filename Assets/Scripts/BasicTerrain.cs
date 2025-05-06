using UnityEngine;

public class BasicTerrain : MonoBehaviour
{
    [SerializeField] private TerrainCollider _terrain_col;
    [SerializeField] private Rigidbody _playerCar_body;

    // This is meant to provide a grace period for collisions, so that the same collision doesn't count multiple times
    private float _lastCollisionTime;

    private void Start()
    {
        _lastCollisionTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions for 1 second after colliding
        if (collision.body == _playerCar_body && Time.time - _lastCollisionTime >= 1)
        {
            CarController.carHealth -= (collision.rigidbody.mass * CarController.carSpeed) / 500;
            _lastCollisionTime = Time.time;
        }
    }
}
