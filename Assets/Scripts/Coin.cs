using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
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
            gameController.GetComponent<Controller>().PickUpCoin();

            //add score here
            
            Destroy(this.gameObject);
        }
    }
}
