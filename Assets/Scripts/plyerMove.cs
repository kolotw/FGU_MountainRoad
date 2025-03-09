using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plyerMove : MonoBehaviour
{
    CharacterController 控制器;
    Vector3 移動方向;
    float 搖捍按鍵左右動量;
    float 搖捍按鍵垂直動量;

    // Start is called before the first frame update
    void Start()
    {
        控制器 = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        搖捍按鍵左右動量 = Input.GetAxis("Mouse X");
        this.transform.eulerAngles =
            new Vector3(0, this.transform.eulerAngles.y + 搖捍按鍵左右動量, 0);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        控制器.Move(move * Time.deltaTime * 10);
        

        
        //搖捍按鍵垂直動量 = Input.GetAxis("Vertical");
        
        //移動方向 = new Vector3(0, 0, 搖捍按鍵垂直動量);
        //控制器.Move(移動方向 * Time.deltaTime * 3);
    }
}
