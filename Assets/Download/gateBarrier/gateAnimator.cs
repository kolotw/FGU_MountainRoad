using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateAnimator : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "car"))
        {
            open();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((other.tag == "car"))
        {
            close();
        }
    }
    public void open()
    {
        anim.SetTrigger("OPEN");
    }
   void close()
    {
        anim.SetTrigger("CLOSE");
    }
}
