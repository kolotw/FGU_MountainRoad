using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plyerMove : MonoBehaviour
{
    CharacterController ���;
    Vector3 ���ʤ�V;
    float �n�«��䥪�k�ʶq;
    float �n�«��䫫���ʶq;

    // Start is called before the first frame update
    void Start()
    {
        ��� = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        �n�«��䥪�k�ʶq = Input.GetAxis("Mouse X");
        this.transform.eulerAngles =
            new Vector3(0, this.transform.eulerAngles.y + �n�«��䥪�k�ʶq, 0);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        ���.Move(move * Time.deltaTime * 10);
        

        
        //�n�«��䫫���ʶq = Input.GetAxis("Vertical");
        
        //���ʤ�V = new Vector3(0, 0, �n�«��䫫���ʶq);
        //���.Move(���ʤ�V * Time.deltaTime * 3);
    }
}
