using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
    bool onDropping = false;
    float colorOverTime = 0;
    Color cubeColor = new Color(241/255f, 195/255f, 47/255f);
    public bool upCube = false;

    public GameObject upCubePrefab;

    void Start()
    {
        if (!upCube)
        {
            int u = Random.Range(0, 3);

            if (u >= 1)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.4f, transform.rotation, transform.parent);
            if (u >= 2)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.4f * 2, transform.rotation, transform.parent);
            if (u == 3)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.4f * 3, transform.rotation, transform.parent);
        }
        else
            GetComponent<MeshRenderer>().material.color = Color.Lerp(cubeColor, Color.red, transform.position.y / 1.4f);
    }

    void Update()
    {
        if(onDropping)
        {
            colorOverTime += Time.deltaTime * 3;
            cubeColor = Color.Lerp(Color.red, new Color(.2f, .2f, .2f), colorOverTime);
            GetComponent<MeshRenderer>().material.color = cubeColor;
            Destroy(gameObject, 3);
        }
    }

    public void ChangeColor()
    {
        onDropping = true;
    }
}
