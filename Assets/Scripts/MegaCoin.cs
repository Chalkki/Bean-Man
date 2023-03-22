using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaCoin : MonoBehaviour
{
    GameObject gameController;

    private void Start()
    {
        gameController = GameObject.Find("Game Controller");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Pick Mega Coin");

            // change the state of ghosts here to vulnerable
            gameController.GetComponent<Controller>().PickUpBigCoin();
            Destroy(this.gameObject);
        }
    }
}
