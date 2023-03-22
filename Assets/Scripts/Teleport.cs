using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public string TeleportName;
    // provide the corresponding teleport position 
    public static Vector3 leftPos;
    public static Vector3 rightPos;
    void Start()
    {
        if(TeleportName.Equals("Left Teleport"))
        {
            leftPos = transform.position;
            leftPos.x += 3;
        }else if (TeleportName.Equals("Right Teleport"))
        {
            rightPos = transform.position;
            rightPos.x -= 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TeleportName.Equals("Left Teleport"))
        {
            collision.gameObject.transform.position = rightPos;
        }
        else if (TeleportName.Equals("Right Teleport"))
        {
            collision.gameObject.transform.position = leftPos;
        }
    }
}
