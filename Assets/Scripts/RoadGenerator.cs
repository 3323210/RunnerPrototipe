using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : Singleton<RoadGenerator>
{
    private List<GameObject> roads = new List<GameObject>();
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] private float _maxSpeed = 10;
    [SerializeField] private int _maxRoadCountn = 5;
    public float _speed = 0;


    void Start()
    {
        PoolManager.Instance.Preload(_roadPrefab, 10);
        ResetLevel();
    }

    void Update()
    {
        if (_speed == 0) return;
        foreach (GameObject road in roads)
        {
            road.transform.position -= new Vector3(0, 0, _speed * Time.deltaTime);
        }

        if (roads[0].transform.position.z < -40)
        {
            PoolManager.Instance.Despawn(roads[0]);
            roads.RemoveAt(0);
            CreateNextRoad();
        }
    }

    private void CreateNextRoad()
    {
        Vector3 pos = Vector3.zero;
        if (roads.Count > 0)
        {
            pos = roads[roads.Count - 1].transform.position + new Vector3(0, 0, 40);
        }
        GameObject go = PoolManager.Instance.Spawn(_roadPrefab, pos, Quaternion.identity);
        go.transform.SetParent(transform);
        roads.Add(go);
    }
    public void StartLevel()
    {
        _speed = _maxSpeed;
        SwipeManager.instance.enabled = true;
    }
    public void ResetLevel()
    {
        _speed = 0;
        while (roads.Count > 0)
        {
            PoolManager.Instance.Despawn(roads[0]);
            roads.RemoveAt(0);
        }
        for (int i = 0; i < _maxRoadCountn; i++)
        {
            CreateNextRoad();
        }
        SwipeManager.instance.enabled = false;
        MapGenerator.Instance.ResetMaps();
    }
}
