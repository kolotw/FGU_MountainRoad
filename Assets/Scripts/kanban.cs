using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kanban : MonoBehaviour
{
    Transform target;
    Vector3 ��V;
    Quaternion ����;
      

    // Update is called once per frame
    void Update()
    {
        target = Camera.main.transform;
        ��V = this.transform.position - target.position;
        if (��V != Vector3.zero)
        {
            ���� = Quaternion.LookRotation(��V * -1, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, ����, 20 * Time.deltaTime);
            //transform.LookAt(�ؼ�);
            this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y, 0f);
        }
    }
}
