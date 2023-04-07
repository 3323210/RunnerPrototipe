using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    int itemSpace = 15;
    int itemCountInMap = 5;
    public float laneOffset = 2f;
    int coinsCounInItem = 10;
    float coinsHeight = 0.5f;
    int mapSize;
    enum TrackPos { Left = -1, Center = 0, Right = 1 };
    enum CoinStyle { Line, Jump, Rump }

    public GameObject WhallPrefab;
    public GameObject WhallBottomPrefab;
    public GameObject WhallTopPrefab;
    public GameObject RumpPrefab;
    public GameObject CoinPrefab;

    public List<GameObject> maps = new List<GameObject>();
    public List<GameObject> activeMaps = new List<GameObject>();


    struct MapItem
    {
        public void StValues(GameObject obstacle, TrackPos trackPos, CoinStyle coinStyle)
        {
            this.obstacle = obstacle;
            this.trackPos = trackPos;
            this.coinStyle = coinStyle;
        }
        public GameObject obstacle;
        public TrackPos trackPos;
        public CoinStyle coinStyle;

    }

    private void Awake()
    {
        mapSize = itemCountInMap * itemSpace;
        maps.Add(MakeMap1());
        maps.Add(MakeMap2());
        maps.Add(MakeMap1());
        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
    }
    void Start()
    {

    }

    void Update()
    {
        if (RoadGenerator.Instance._speed == 0) return;
        foreach (GameObject map in activeMaps)
        {
            map.transform.position -= new Vector3(0, 0, RoadGenerator.Instance._speed * Time.deltaTime);
        }
        if (activeMaps[0].transform.position.z < -mapSize)
        {
            RemoveFirstActiveMap();
            AddActiveMap();
        }

    }
    void RemoveFirstActiveMap()
    {
        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    public void ResetMaps()
    {
        while (activeMaps.Count > 0)
        {
            RemoveFirstActiveMap();
        }
        AddActiveMap();
        AddActiveMap();
    }
    void AddActiveMap()
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        go.SetActive(true);
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(true);
        }
        go.transform.position = activeMaps.Count > 0 ? activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize : new Vector3(0, 0, 10);
        maps.RemoveAt(r);
        activeMaps.Add(go);
    }

    GameObject MakeMap1()
    {
        GameObject result = new GameObject("Map1");
        result.transform.SetParent(transform);
        MapItem item = new MapItem();
        for (int i = 0; i < itemCountInMap; i++)
        {
            item.StValues(null, TrackPos.Center, CoinStyle.Line);
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;
            CoinStyle coinStyle = CoinStyle.Line;

            if (i == 2)
            {
                trackPos = TrackPos.Left;
                obstacle = RumpPrefab;
                coinStyle = CoinStyle.Rump;
            }
            else if (i == 3)
            {
                trackPos = TrackPos.Right;
                obstacle = WhallBottomPrefab;
                coinStyle = CoinStyle.Jump;
            }
            else if (i == 4)
            {
                trackPos = TrackPos.Right;
                obstacle = WhallBottomPrefab;
                coinStyle = CoinStyle.Jump;
            }
            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            CreateCoins(coinStyle, obstaclePos, result);
            if (obstacle != null)
            {
                GameObject go = Instantiate(obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }

    GameObject MakeMap2()
    {
        GameObject result = new GameObject("Map2");
        result.transform.SetParent(transform);
        MapItem item = new MapItem();
        for (int i = 0; i < itemCountInMap; i++)
        {
            item.StValues(null, TrackPos.Center, CoinStyle.Line);
            GameObject obstacle = null;
            TrackPos trackPos = TrackPos.Center;
            CoinStyle coinStyle = CoinStyle.Line;

            if (i == 2)
            {
                trackPos = TrackPos.Right;
                obstacle = WhallBottomPrefab;
                coinStyle = CoinStyle.Jump;
            }
            else if (i == 3)
            {
                trackPos = TrackPos.Center;
                obstacle = WhallTopPrefab;
                coinStyle = CoinStyle.Line;
            }
            else if (i == 4)
            {
                trackPos = TrackPos.Right;
                obstacle = RumpPrefab;
                coinStyle = CoinStyle.Rump
                    ;
            }
            Vector3 obstaclePos = new Vector3((int)trackPos * laneOffset, 0, i * itemSpace);
            CreateCoins(coinStyle, obstaclePos, result);
            if (obstacle != null)
            {
                GameObject go = Instantiate(obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }
    void CreateCoins(CoinStyle style, Vector3 pos, GameObject parentObject)
    {
        Vector3 coinPos = Vector3.zero;
        if (style == CoinStyle.Line)
        {
            for (int i = -coinsCounInItem / 2; i < coinsCounInItem / 2; i++)
            {
                coinPos.y = coinsHeight;
                coinPos.z = i * ((float)itemSpace / coinsCounInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);

            }
        }
        else if (style == CoinStyle.Jump)
        {
            for (int i = -coinsCounInItem / 2; i < coinsCounInItem / 2; i++)
            {
                coinPos.y = Mathf.Max(-1 / 2f * Mathf.Pow(i, 2) + 3, coinsHeight);
                coinPos.z = i * ((float)itemSpace / coinsCounInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);

            }
        }
        else if (style == CoinStyle.Rump)
        {
            for (int i = -coinsCounInItem / 2; i < coinsCounInItem / 2; i++)
            {
                coinPos.y = Mathf.Min(Mathf.Max(1.2f * (i + 3), coinsHeight), 2.5f);
                coinPos.z = i * ((float)itemSpace / coinsCounInItem);
                GameObject go = Instantiate(CoinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);

            }
        }
    }
}
