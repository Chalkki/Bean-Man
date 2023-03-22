using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RedGhostMovement : MonoBehaviour
{
    private float speed;
    GameObject player;
    private Vector3 targetPos;
    private bool startToMove;
    private float frightenDuration;
    // renderer to change to tranparency when get into frighten mode
    private bool frightenMode;
    private SpriteRenderer _renderer;

    // set up the cage status and time for the ghost
    private bool isCaged;
    private float cagedStartTime;
    private float cagedDuration;

    // set up the game controller
    private GameObject gameController;

    //parameters for the ghost to move
    // facing Direction (Global direction): 0 for right, 1 for left, 2 for up, 3 for down
    // up direction (up local direciton for each facing direction):
    // 2 for fd index 0, 2 for fd index 1,0 for fd index 2, 0 for fd index 3 
    // down direction (down local direciton for each facing direction):
    // 3 for fd index 0, 3 for fd index 1,1 for fd index 2, 1 for fd index 3 
    private int facingDirection = 0;
    private readonly int[] up_direction = { 2, 2, 0, 0 };
    private readonly int[] down_direction = { 3, 3, 1, 1 };
    [SerializeField] private Vector3[] modelDirection;
    Vector3[] movingOffest;
    void Start()
    {
        gameController = GameObject.Find("Game Controller");

        // set up the status and duration that the ghost stays in the cage
        cagedStartTime = Time.time;
        cagedDuration = 2f;
        isCaged = true;
        // change the transparency of the ghosts while in the cage
        StartCoroutine(CageSpawnTransparency());

        // set up the renderer for changing transparency
        _renderer = GetComponent<SpriteRenderer>();
        // set the ghost's speed to be 90 percent of the player's speed
        player = GameObject.FindWithTag("Player");
        if(player !=null )
        {
            speed = player.GetComponent<PlayerMovement>().GetPlayerSpeed() * 0.85f;
            movingOffest = new Vector3[] { new Vector3(speed,0,0), new Vector3(-speed,0,0),
                    new Vector3(0, speed,0), new Vector3(0,-speed,0)};
        }
        frightenMode = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (isCaged)
        {
            if(Time.time - cagedStartTime > cagedDuration)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y +2, transform.position.y);
                isCaged = false;
            }
            return;
        }
        if(player == null)
        {
            player = GameObject.FindWithTag("Player");
            startToMove = false;
        }
        else
        {
            targetPos = player.transform.position;
            if(speed == 0)
            {
                speed = player.GetComponent<PlayerMovement>().GetPlayerSpeed() * 0.85f;
                // moving offest arranged by global index: 0 for right , 1 for left, 2 for up, 3 for down
                movingOffest = new Vector3[] { new Vector3(speed,0,0), new Vector3(-speed,0,0), 
                    new Vector3(0, speed,0), new Vector3(0,-speed,0)};
            }
            // decide next move for the ghost
            float time_elapsed = Time.deltaTime;
            if (!startToMove)
            {
                startToMove = true;
                StartCoroutine(NextMove());
            }
        
            transform.Translate(speed * time_elapsed, 0, 0);
        }
        // move in three direction, front, up, down
        // need only to rotate the euler angle and make the ghost move right


    }


    IEnumerator NextMove()
    {
        while (frightenMode == false)
        {
            float min_dis = Mathf.Infinity;
            int nextDirection = facingDirection;
            float time_elapsed = Time.deltaTime;
            float dist;
            //check the distance if moving towards facing direction
            // need to check whether hit wall
            RaycastHit2D[] frontHits = Physics2D.RaycastAll(transform.position, transform.right, .6f);
            bool frontHitWall = false;

            //check the distance if moving towards up relative to facing direction
            RaycastHit2D[] upHits = Physics2D.RaycastAll(transform.position, transform.up, .6f);
            bool upHitWall = false;
            foreach (RaycastHit2D hit in upHits)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    upHitWall = true;
                }
            }
            if (!upHitWall)
            {
                Vector3 offset = movingOffest[up_direction[facingDirection]] * time_elapsed;
                dist = Vector3.Distance(transform.position + offset, targetPos);
                if (min_dis > dist)
                {
                    min_dis = dist;
                    nextDirection = up_direction[facingDirection];
                }
            }

            foreach (RaycastHit2D hit in frontHits)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    frontHitWall = true;
                }
            }
            if (!frontHitWall)
            {
                Vector3 offset = movingOffest[facingDirection] * time_elapsed;
                dist = Vector3.Distance(transform.position + offset, targetPos);
                if (min_dis > dist)
                {
                    min_dis = dist;
                    nextDirection = facingDirection;
                }
            }



            //check the distance if moving towards down relative to facing direction
            RaycastHit2D[] downHits = Physics2D.RaycastAll(transform.position, -(transform.up), .6f);
            bool downHitWall = false;
            foreach (RaycastHit2D hit in downHits)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    downHitWall = true;
                }
            }
            if (!downHitWall)
            {
                Vector3 offset = movingOffest[down_direction[facingDirection]] * time_elapsed;
                dist = Vector3.Distance(transform.position + offset, targetPos);
                if (min_dis > dist)
                {
                    min_dis = dist;
                    nextDirection = down_direction[facingDirection];
                }
            }

            // set facing direction to the next direction to move
            facingDirection = nextDirection;
            transform.eulerAngles = modelDirection[facingDirection];
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Frightened(float frightenTime)
    {
        if(isCaged)
        {
            return;
        }
        frightenDuration += frightenTime;
        if (!frightenMode)
        {
            frightenMode = true;
            StartCoroutine(FrightenedMove());
            StartCoroutine(FrightenedTransparency());
        }
    }
    public void Chase()
    {
        frightenMode = false;
        ChangeTransparency(1f);
        StartCoroutine(NextMove());
    }
    IEnumerator FrightenedMove()
    {
        float startTime = Time.time;
        // turn back immediately
        facingDirection = down_direction[facingDirection];
        transform.eulerAngles = modelDirection[facingDirection];

        while (frightenMode)
        {
            if(Time.time - startTime > frightenDuration)
            {
                frightenDuration = 0;
                Chase();
                break;
            }

            // vulnerable = true;
            facingDirection = Random.Range(0, 3);
            transform.eulerAngles = modelDirection[facingDirection];
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator FrightenedTransparency()
    {
        float startTime = Time.time;
        // change the transparency while frightened
        float alpha = 0.5f;
        ChangeTransparency(alpha);

        while (frightenMode)
        {
            if(frightenDuration - (Time.time - startTime) < 1f)
            {
                // change the transparency while frightened
                if (GetComponent<Renderer>().material.color.a == 1)
                {
                    alpha = 0.5f;
                }
                else
                {
                    alpha = 1;
                }
                ChangeTransparency(alpha);
            }
            else
            {
                if(alpha != 0.5f)
                {
                    alpha = 0.5f;
                    ChangeTransparency(alpha);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    void ChangeTransparency(float alpha)
    {
        Color oldColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
    }

    IEnumerator CageSpawnTransparency()
    {
        float alpha = 0.0f;
        float interval = 1 / (cagedDuration * cagedDuration);
        float timeInterval = 1 / cagedDuration;
        while (isCaged)
        {
            alpha += interval;
            ChangeTransparency(alpha);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (frightenMode)
            {
                gameController.GetComponent<Controller>().GhostRespawn(this.gameObject,0);
                gameController.GetComponent<Controller>().EatGhost();
            }
            else
            {
                gameController.GetComponent<Controller>().PlayerRespawn();
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(transform.right) * .6f;
        Gizmos.DrawRay(transform.position, direction);
    }
}
