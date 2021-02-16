using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public float t = 0;
    //public float t1;
    //public float t2;
    Quaternion r1;
    Quaternion r2;

    void Start()
    {
        t = 0;
    }

    void Update()
    {
        if (t > 0)
        {
            t -= Time.deltaTime;
            transform.rotation = Quaternion.Lerp(r2, r1, t);
        }

        /*if (t1 > 0)
        {
            t1 -= Time.deltaTime;
            transform.Rotate(Vector3.up * Time.deltaTime * 90);
        }
        else
        {
            Vector3 r = transform.rotation.eulerAngles;
            r.y = Mathf.RoundToInt(r.y / 90f) * 90;
            transform.rotation = Quaternion.Euler(r);
        }
        if (t2 > 0)
        {
            t2 -= Time.deltaTime;
            transform.Rotate(Vector3.up * Time.deltaTime * 90);
        }
        else
        {
            Vector3 r = transform.rotation.eulerAngles;
            r.y = Mathf.RoundToInt(r.y / 90f) * 90;
            transform.rotation = Quaternion.Euler(r);
        }*/
    }

    public void RotateBoardLeft()
    {
        if (t <= 0)
        {
            r1 = transform.rotation;
            r2.eulerAngles = r1.eulerAngles + Vector3.up * -90;

            Transform[,,] transposedGrid = new Transform[10, 10, 10];

            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                        transposedGrid[9 - z, y, x] = GetComponent<PlaceZentraObject>().grid[x, y, z];//Anti-Transpose grid

            GetComponent<PlaceZentraObject>().grid = transposedGrid;
            t = 1;
        }
    }
    
    public void RotateBoardRight()
    {
        if (t <= 0)
        {
            r1 = transform.rotation;
            r2.eulerAngles = r1.eulerAngles + Vector3.up * 90;

            Transform[,,] transposedGrid = new Transform[10, 10, 10];

            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                        transposedGrid[x, y, z] = GetComponent<PlaceZentraObject>().grid[9 - z, y, x];//Transpose grid

            GetComponent<PlaceZentraObject>().grid = transposedGrid;
            t = 1;
        }
    }

    /*public void RotateBoardRight()
    {
        t1 = 1;
    }
    
    public void RotateBoardLeft()
    {
        t2 = 1;
    }*/
}
