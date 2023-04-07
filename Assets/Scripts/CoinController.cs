using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CoinController : MonoBehaviour

{
    [SerializeField] private float _rotationSpeed = 0.1f;
    public static event Action _scoreForCoint;

    void Start()
    {
        _rotationSpeed += Random.Range(0, _rotationSpeed / 4.0f);
    }

    void Update()
    {
        transform.Rotate(0, 0, _rotationSpeed);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            transform.parent.gameObject.SetActive(false);
            _scoreForCoint?.Invoke();

        }

    }
}
