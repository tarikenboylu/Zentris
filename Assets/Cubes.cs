using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
    bool onDropping = false;
    float colorOverTime = 0;
    Color cubeColor = new Color(241 / 255f, 195 / 255f, 47 / 255f);

    public bool upCube = false;

    public float markedTime = 0;
    float plsTime = 0;
    bool pls = true;

    public GameObject upCubePrefab;

    void Start()
    {
        if (!upCube)
        {
            int u = Random.Range(0, 3);

            if (u >= 1)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.2f, transform.rotation, transform.parent);
            if (u >= 2)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.2f * 2, transform.rotation, transform.parent);
            if (u == 3)
                Instantiate(upCubePrefab, transform.position + Vector3.up * 0.2f * 3, transform.rotation, transform.parent);
        }
    }

    void Update()
    {
        if (onDropping)
        {
            colorOverTime += Time.deltaTime * 3;
            cubeColor = Color.Lerp(new Color(241 / 255f, 195 / 255f, 47 / 255f), new Color(.2f, .2f, .2f), colorOverTime);
            GetComponent<MeshRenderer>().material.color = cubeColor;
            Destroy(gameObject, 3);
        }
        else
            GetComponent<MeshRenderer>().material.color = Color.Lerp(cubeColor, Color.red, transform.position.y / 1.4f);

        if (markedTime > 0 && ! onDropping)
        {
            markedTime -= Time.deltaTime;

            if (pls)
            {
                plsTime += Time.deltaTime * 2;
                pls = plsTime < 1;
            }
            else
            {
                plsTime -= Time.deltaTime * 2;
                pls = plsTime < 0;
            }

            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.white/2, plsTime));
        }

    }

    public void ChangeColor()
    {
        onDropping = true;
    }

    public void SetMarked()
    {
        if(markedTime <= 0)
            markedTime = 4;
    }
}
