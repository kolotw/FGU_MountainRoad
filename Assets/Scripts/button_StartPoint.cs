using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_StartPoint : MonoBehaviour
{
    public Transform[] startPoints;
    GameObject car;
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("/PlayerFirstMotor");
    }

    public void ¶³() {
        car.transform.position = startPoints[0].position;
        car.transform.eulerAngles = startPoints[0].eulerAngles;
    }
    public void ·¬() {
        car.transform.position = startPoints[1].position;
        car.transform.eulerAngles = startPoints[1].eulerAngles;
    }
    public void ªù() {
        car.transform.position = startPoints[2].position;
        car.transform.eulerAngles = startPoints[2].eulerAngles;
    }
}
