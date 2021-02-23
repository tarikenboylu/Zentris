using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceZentraObject : MonoBehaviour
{
    Vector3 hitPosition;
    Vector3 has;

    public GameObject[] cursors;
    public GameObject cursor;
    public Transform[,,] grid;
    public Transform nextZentraObject;

    List<Transform> deleteList;

    void Start()
    {
        deleteList = new List<Transform>();
        grid = new Transform[10, 10, 10];
    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            var ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            int layerMask = 1 << 4;//setting the layer mask to layer 4

            layerMask = ~layerMask;//setting the layer mask to "ignore" layer (~)

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (hit.collider.tag == "ZentraObject")//Choose ZentraObject
                {
                    if (nextZentraObject != null)
                        nextZentraObject.GetComponent<BoxCollider>().enabled = true;//Open past objects collider

                    nextZentraObject = hit.collider.transform;//A chosen one

                    for (int i = 0; i < cursors.Length; i++)
                        cursors[i].SetActive(false);
                    
                    cursor = cursors[nextZentraObject.GetComponent<ZentraObject>().prefabNumber];//A chosen objects cursor
                    cursor.SetActive(true);
                    nextZentraObject.GetComponent<BoxCollider>().enabled = false;//Close new cursor objects collider
                }

                if (hit.collider.tag == "GameController" && nextZentraObject != null && cursor != null)//Before placing Zentra Object show place with cursor object
                {
                    hitPosition = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));//Ray hit position on board

                    cursor.transform.position = hitPosition;

                    cursor.SetActive(true);

                    //if (placevalid) change cursor position control lines
                    if (PlaceValid())
                        CheckGridBefore();
                    else
                        cursor.SetActive(false);
                }
            }

            if (!Physics.Raycast(ray) && cursor != null)
                cursor.SetActive(false);
        }

        if (!(Input.touchCount > 0))
        {
            //When touch release if (placevalid) place zentra object to cursor position
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

        for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
                if (grid[x, 0, z] != null)
                    Debug.Log(x + " " + z);

        CheckGridHorizontal();
        CheckGridVertical();
        StartCoroutine(RemoveCubes());
    }

    void CheckGridBefore()
    {
        Transform[,] tempGrid = new Transform[10, 10];
        for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
                tempGrid[x, z] = grid[x, 0, z];

        foreach (Transform cube in cursor.transform)
            tempGrid[Mathf.RoundToInt(cube.position.x), Mathf.RoundToInt(cube.position.z)] = cube;
        
        for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
            {
                if (tempGrid[x, z] == null)
                    break;

                if (z == 9)
                    MarkVerticalLine(x);
            }

        for (int z = 0; z < 10; z++)
            for (int x = 0; x < 10; x++)
            {
                if (tempGrid[x, z] == null)
                    break;

                if (x == 9)
                    MarkHorizontalLine(z);
            }
    }

    void MarkVerticalLine(int x)
    {
        for (int z = 0; z < 10; z++)
            if (grid[x, 0, z] != null)
            {
                grid[x, 0, z].GetComponent<Cubes>().SetMarked();
                print("Marked");
            }
    }

    void MarkHorizontalLine(int z)
    {
        for (int x = 0; x < 10; x++)
            if (grid[x, 0, z] != null)
            {
                grid[x, 0, z].GetComponent<Cubes>().SetMarked();
                print("Marked");
            }
    }

    void CheckGridHorizontal()
    {
        for (int x = 0; x < 10; x++)
            for (int z = 0; z < 10; z++)
            {
                if (grid[x, 0, z] == null)
                    break;

                if (z == 9)
                    ClearVerticalLine(x);
            }
    }
    
    void CheckGridVertical()
    {
        for (int z = 0; z < 10; z++)
            for (int x = 0; x < 10; x++)
            {
                if (grid[x, 0, z] == null)
                    break;

                if (x == 9)
                    ClearHorizontalLine(z);
            }
    }

    void ClearVerticalLine(int x)
    {
        for (int z = 0; z < 10; z++)
        {
            if (grid[x, 0, z] != null)
            {
                deleteList.Add(grid[x, 0, z]);

                if (grid[x, 1, z] != null)
                    StartCoroutine(DropUpCubes(x, 1, z));

                if (grid[x, 2, z] != null)
                    StartCoroutine(DropUpCubes(x, 2, z));

                if (grid[x, 3, z] != null) 
                    StartCoroutine(DropUpCubes(x, 3, z));

                grid[x, 0, z] = null;
            }
        }
    }
    
    void ClearHorizontalLine(int z)
    {
        for (int x = 0; x < 10; x++)
        {
            if (grid[x, 0, z] != null)
            {
                deleteList.Add(grid[x, 0, z]);

                if (grid[x, 1, z] != null)
                    StartCoroutine(DropUpCubes(x, 1, z));

                if (grid[x, 2, z] != null)
                    StartCoroutine(DropUpCubes(x, 2, z));

                if (grid[x, 3, z] != null)
                    StartCoroutine(DropUpCubes(x, 3, z));

                grid[x, 0, z] = null;
            }
        }
    }

    void AddToGrid()
    {
        int c = nextZentraObject.childCount;

        for (int i = 0; i < c; i++)
        {
            Transform t = nextZentraObject.GetChild(0);
            grid[Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y / 0.2f), Mathf.RoundToInt(t.position.z)] = t;
            Debug.Log("Added " + t.position.x + " " + t.position.y / 0.2f + " " + t.position.z);
            nextZentraObject.GetChild(0).SetParent(transform);
        }
    }

    bool PlaceValid()
    {
        foreach (Transform cube in cursor.transform)
            if (Mathf.RoundToInt(cube.position.x) < 10 && Mathf.RoundToInt(cube.position.x) >= 0//Board içinde bir yer mi (x ekseni için)?
            &&  Mathf.RoundToInt(cube.position.z) < 10 && Mathf.RoundToInt(cube.position.z) >= 0)//Board içinde bir yer mi (z ekseni için)?
            {
                if (grid[Mathf.RoundToInt(cube.position.x), 0, Mathf.RoundToInt(cube.position.z)] != null)//bu kare boş mu?
                    return false;
            }
            else
                return false;

        return true;
    }

    public void GetTouchPos()
    {
        if (Input.touchCount > 0)
            has = Input.GetTouch(0).position;
    }

    public void ControlTouchPos()
    {
        if (Input.touchCount > 0) 
        { 
            if (has.x < Input.GetTouch(0).position.x)
                GetComponent<Board>().RotateBoardLeft();//Rotate left

            if (has.x > Input.GetTouch(0).position.x)
                GetComponent<Board>().RotateBoardRight();//Rotate right
        }
    }

    IEnumerator DropUpCubes(int x, int y, int z)
    {
        Debug.Log("0.5f Before");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("0.5f After");
        grid[x, y, z].position -= Vector3.up * 0.2f;
        grid[x, y - 1, z] = grid[x, y, z];
        grid[x, y, z] = null;
    }

    IEnumerator RemoveCubes()
    {
        foreach (Transform cube in deleteList)
            cube.GetComponent<Cubes>().ChangeColor();

        yield return new WaitForSeconds(1f);

        foreach (Transform cube in deleteList)
        {
            cube.gameObject.AddComponent<Rigidbody>();
            cube.GetComponent<BoxCollider>().isTrigger = true;
        }

        deleteList.Clear();
    }

}