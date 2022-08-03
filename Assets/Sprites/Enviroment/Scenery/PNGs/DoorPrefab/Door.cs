using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //open door
        }
    }
}
