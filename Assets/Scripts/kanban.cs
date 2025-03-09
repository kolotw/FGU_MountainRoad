using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kanban : MonoBehaviour
{
    Transform target;
    Vector3 方向;
    Quaternion 旋轉;
      

    // Update is called once per frame
    void Update()
    {
        target = Camera.main.transform;
        方向 = this.transform.position - target.position;
        if (方向 != Vector3.zero)
        {
            旋轉 = Quaternion.LookRotation(方向 * -1, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, 旋轉, 20 * Time.deltaTime);
            //transform.LookAt(目標);
            this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y, 0f);
        }
    }
}
