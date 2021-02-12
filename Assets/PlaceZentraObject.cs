using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceZentraObject : MonoBehaviour
{
    Vector3 pastPosition;
    Vector3 hitPosition;

    public GameObject[] cursors;
    public GameObject cursor;
    public Transform[,] grid;
    public Transform nextZentraObject;

    void Start()
    {
        grid = new Transform[10, 10];
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit;
            var ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);

            int layerMask = 1 << 4;//setting the layer mask to layer 4

            layerMask = ~layerMask;//setting the layer mask to "ignore" layer (~)

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (hit.collider.tag == "ZentraObject")//Choose ZentraObject
                {
                    //cursor atayacağız
                    if (nextZentraObject != null)
                    {
                        nextZentraObject.GetComponent<BoxCollider>().enabled = true;//Open past objects collider
                    }
                    nextZentraObject = hit.collider.transform;

                    for (int i = 0; i < cursors.Length; i++)
                        cursors[i].SetActive(false);
                    
                    cursor = cursors[nextZentraObject.GetComponent<ZentraObject>().prefabNumber];//choose objects cursor
                    cursor.SetActive(true);
                    nextZentraObject.GetComponent<BoxCollider>().enabled = false;//Close new objects collider
                }

                if (hit.collider.tag == "GameController" && nextZentraObject != null && cursor != null)//Before placing Zentra Object show place with cursor object
                {
                    hitPosition = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));

                    cursor.transform.position = hitPosition;

                    cursor.SetActive(true);

                    //eğer placevalid ise cursor position değişecek yoksa setactive false...
                    if (PlaceValid())
                    {
                        pastPosition = hitPosition;
                        //Debug.Log("Valid");
                    }
                    else
                    {
                        cursor.SetActive(false);
                        //Debug.Log("invalid");
                    }
                    //Transform instance = Instantiate(nextZentraObject, hitPosition, nextZentraObject.rotation);

                    //burası değişecek

                    /*if (!isEmpty(cursor.transform))
                    {
                        Debug.Log("Can't");
                    }
                    else
                    {
                        nextZentraObject.position = hitPosition;
                        AddToGrid();
                    }*/
                }
            }
        }
        else
        {
            //touch bırakıldığında yani = 0 olduğunda ve touchposition board üstünde placevalid ise objeyi yerleştir
            if (cursor != null && nextZentraObject != null)
            {
                cursor.SetActive(false);
                
                if (PlaceValid())
                {
                    nextZentraObject.position = hitPosition;
                    GetComponent<SpawnZentraObject>().isUsed[nextZentraObject.GetComponent<ZentraObject>().stationNumber] = true;
                    AddToGrid();
                    Destroy(nextZentraObject.gameObject);
                    //cursor = null;
                }
            }
            
        }

        CheckGridHorizontal();
        CheckGridVertical();

        /*for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
                if (grid[x, z] != null)
                    Debug.Log(x + "  " + z);*/
    }

    void CheckGridHorizontal()
    {
        for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
            {
                if (grid[x,z] == null)
                    break;

                if (z == 9)
                    ClearVerticalLine(x);

                //print(x + " " + z);/////////////////////////////
            }
    }
    
    void CheckGridVertical()
    {
        for (int z = 0; z < 10; z++)
            for (int x = 0; x < 10; x++)
            {
                if (grid[x,z] == null)
                    break;

                if (x == 9)
                    ClearHorizontalLine(z);

                //print(x + " " + z);/////////////////////////////
            }
    }

    void ClearVerticalLine(int x)
    {
        for (int z = 0; z < 10; z++)
        {
            if (grid[x, z] != null)
            {
                Destroy(grid[x, z].gameObject);
                grid[x, z] = null;
            }
        }
    }
    
    void ClearHorizontalLine(int z)
    {
        for (int x = 0; x < 10; x++)
        {
            if (grid[x, z] != null)
            {
                Destroy(grid[x, z].gameObject);
                grid[x, z] = null;
            }
        }
    }

    void AddToGrid()
    {
        int c = nextZentraObject.childCount;

        for (int i = 0; i < c; i++)
        {
            grid[Mathf.RoundToInt(nextZentraObject.GetChild(0).position.x), Mathf.RoundToInt(nextZentraObject.GetChild(0).position.z)] = nextZentraObject.GetChild(0);
            Debug.Log("Added " + nextZentraObject.GetChild(0).position.x + " " + nextZentraObject.GetChild(0).position.z);
            nextZentraObject.GetChild(0).SetParent(transform);
        }
    }

    bool PlaceValid()
    {
        foreach (Transform cube in cursor.transform)
            if (Mathf.RoundToInt(cube.position.x) < 10 && Mathf.RoundToInt(cube.position.x) >= 0//Board içinde bir yer mi x ekseninde?
            && Mathf.RoundToInt(cube.position.z) < 10 && Mathf.RoundToInt(cube.position.z) >= 0)//Board içinde bir yer mi z ekseninde?
            {
                if (grid[Mathf.RoundToInt(cube.position.x), Mathf.RoundToInt(cube.position.z)] != null)//bu kare boş mu?
                    return false;
            }
            else
                return false;

        return true;
    }
}