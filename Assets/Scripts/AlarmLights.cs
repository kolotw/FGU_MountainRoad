using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLights : MonoBehaviour
{
    private Material g;
    Vector4 baseColor;
    Vector4 offColor;
    Vector4 currentColor;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Renderer>().material.SetColor("_Color", new Vector4(1, 1, 1, 1));
        g = GetComponent<Renderer>().material;
        baseColor = g.GetColor("_Color");
        offColor = new Vector4(0, 0.5f, 1, 1);
        //StartCoroutine(gloom());
    }

    private IEnumerator gloom()
    {
        float i;
        while (true)
        {
            i = 0;
            while (i < 1)
            {

                currentColor = Vector3.Lerp(baseColor, offColor, i);
                //print(currentColor);
                g.SetColor("_Color", currentColor);
                yield return new WaitForSeconds(.05f);
                i += 0.2f;

            }
            i = 1;
            g.SetColor("_Color", offColor);
            yield return new WaitForSeconds(.05f);
            while (i > 0)
            {

                currentColor = Vector3.Lerp(baseColor, offColor, i);
                //print(currentColor);
                g.SetColor("_Color", currentColor);
                yield return new WaitForSeconds(.05f);
                i -= 0.2f;

            }
            g.SetColor("_Color", baseColor);
            yield return new WaitForSeconds(.05f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car"))
        {
            StartCoroutine(gloom());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("car"))
        {
            StopCoroutine(gloom());
        }
    }
}
