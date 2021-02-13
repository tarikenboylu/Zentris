using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceZentraObject : MonoBehaviour
{
    Vector3 hitPosition;

    public GameObject[] cursors;
    public GameObject cursor;
    public Transform[,,] grid;
    public Transform nextZentraObject;

    Color cubeColor = Color.red;

    List<Transform> deleteList;
    List<Transform> uptoDownList;

    void Start()
    {
        deleteList = new List<Transform>();
        grid = new Transform[10, 10, 10];
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

                if (hit.collider.CompareTag("Rotator") && nextZentraObject == null)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved) 
                    { 
                        if (Input.GetTouch(0).deltaPosition.x < 0)
                            GetComponent<Board>().RotateBoardRight();
                        
                        if (Input.GetTouch(0).deltaPosition.x > 0)
                            GetComponent<Board>().RotateBoardLeft();
                    }

                    //transpose grid////////////////////////////////////////////////////////
                }

                if (hit.collider.tag == "GameController" && nextZentraObject != null && cursor != null)//Before placing Zentra Object show place with cursor object
                {
                    hitPosition = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));

                    cursor.transform.position = hitPosition;

                    cursor.SetActive(true);

                    //eğer placevalid ise cursor position değişecek yoksa setactive false...
                    if (!PlaceValid())
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
        StartCoroutine(RemoveCubes());

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
            grid[Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y / 0.4f), Mathf.RoundToInt(t.position.z)] = t;
            Debug.Log("Added " + t.position.x + " " + t.position.y / 0.4f + " " + t.position.z);
            nextZentraObject.GetChild(0).SetParent(transform);
        }
    }

    bool PlaceValid()
    {
        foreach (Transform cube in cursor.transform)
            if (Mathf.RoundToInt(cube.position.x) < 10 && Mathf.RoundToInt(cube.position.x) >= 0//Board içinde bir yer mi (x ekseni için)?
            && Mathf.RoundToInt(cube.position.z) < 10 && Mathf.RoundToInt(cube.position.z) >= 0)//Board içinde bir yer mi (z ekseni için)?
            {
                if (grid[Mathf.RoundToInt(cube.position.x), Mathf.RoundToInt(cube.position.y / 0.4f), Mathf.RoundToInt(cube.position.z)] != null)//bu kare boş mu?
                    return false;
            }
            else
                return false;

        return true;
    }

    IEnumerator DropUpCubes(int x, int y, int z)
    {
        Debug.Log("0.5f Before");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("0.5f After");
        grid[x, y, z].position -= Vector3.up * 0.4f;
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