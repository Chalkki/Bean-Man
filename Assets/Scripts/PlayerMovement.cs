using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] GameObject castPos;
    private float z_angle;
    private float y_angle;
    // Start is called before the first frame update
    void Start()
    {
        // set up the chracter's euler angle in the beginning
        z_angle = 0f;
        y_angle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }


    void Movement()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            // let the player facing right
            z_angle = 0f;
            y_angle = 0f;
            // player moving horizontally
            if(Mathf.Sign(Input.GetAxis("Horizontal")) == -1)
            {

                // let the player facing left

                y_angle = 180f;
            }
            transform.eulerAngles = new Vector3(0f, y_angle, z_angle);
        }
        if(Input.GetAxis("Vertical") != 0)
        {

            // let the player facing down
            y_angle = 0f;
            z_angle = 90f;

            if (Mathf.Sign(Input.GetAxis("Vertical")) == -1)
            {

                // let the player facing up
                z_angle = -90f;
            }
            transform.eulerAngles = new Vector3(0f, y_angle, z_angle);        
        }
        // check if the player hit the wall
        RaycastHit2D wallInfo = Physics2D.Raycast(castPos.transform.position, castPos.transform.right, .6f);
        if (wallInfo != false && wallInfo.collider.CompareTag("Wall"))
        {
            // stop the player if hitting the wall
            transform.Translate(Vector3.zero);
            return;
        }
        // because the player is always moving towards its front, we do not need to add additional vertical or horizontal speed.
        transform.Translate(speed * Time.deltaTime, 0f, 0f);
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector2.right) * .6f;
        Gizmos.DrawRay(transform.position, direction);
    }
    public float GetPlayerSpeed()
    {
        return speed;
    }
}
