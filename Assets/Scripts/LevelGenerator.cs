using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D map;
    private float x_offset = -9.5f;
    private float y_offset = -9.5f;
    public ColorToPrefab[] colorMappings;
    // make classcification for each object
    public GameObject[] classes;

    GameObject gameController;
    void Start()
    {
        gameController = GameObject.Find("Game Controller");
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int x = 0; x<map.width; x++)
        {
            for(int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }
        // once the level is generated, the game controller can start to count the coins
        gameController.GetComponent<Controller>().SetCoinCount();
    }

    void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);
        if(pixelColor.a == 0)
        {
            return;
        }
        Vector3 position = new Vector3(x + x_offset , y + y_offset, 0f);

        // set an index to parent each child
        int i = 0;
        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                GameObject tile = Instantiate(colorMapping.prefab,position, Quaternion.identity);
                tile.transform.parent = classes[i].transform;
                break;
            }
            i++;
        }
    }
}
