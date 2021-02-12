using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZentraObject : MonoBehaviour
{
    public GameObject[] zentraObjectPrefabs;
    public Transform[] spawnStations;
    public Dictionary<int, bool> isUsed = new Dictionary<int, bool>();

    void Start()
    {
        SpawnNew();
        for (int i = 0; i < spawnStations.Length; i++)
            isUsed[i] = false;
    }

    void Update()
    {
        if (spawnPointsEmpty())
            SpawnNew();

        /*for (int i = 0; i < 3; i++)
            Debug.Log(isUsed[i]);*/
    }

    void SpawnNew()
    {
        for (int i = 0; i < spawnStations.Length; i++)
        {
            int n = Random.Range(0, zentraObjectPrefabs.Length);
            GameObject z = Instantiate(zentraObjectPrefabs[n], spawnStations[i].position, Quaternion.identity);
            z.GetComponent<ZentraObject>().stationNumber = i;
            z.GetComponent<ZentraObject>().prefabNumber = n;
            isUsed[i] = false;
        }
    }

    bool spawnPointsEmpty()
    {
        for (int i = 0; i < spawnStations.Length; i++)
            if (!isUsed[i])
                return false;
        return true;
    }
}
