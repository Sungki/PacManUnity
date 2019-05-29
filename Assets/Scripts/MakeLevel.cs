using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeLevel : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject pacmanPrefab;
    public GameObject cookiePrefab;

    const int level_col = 20;
    const int level_row = 10;
    void Start()
    {
        InitLevel();
    }

    void Update()
    {

    }

    void InitLevel()
    {
        // Read the level text file
        TextAsset level = (TextAsset)Resources.Load("level1", typeof(TextAsset));
        string levelData = level.text;
        int col = 0;
        int row = 0;

        for (int i = 0; i < levelData.Length; i++)
        {
            if (levelData[i] == '1')
            {
                // generate walls
                Instantiate(blockPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
            }
            else if(levelData[i] == '2')
            {
                // create pacman
                Instantiate(pacmanPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
            }
            else if (levelData[i] == '0')
            {
                // create cookies
                Instantiate(cookiePrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
            }
            else if (levelData[i] == '\n')
            {
                // line break
                col = 0;
                row--;
            }
            else
            {
                print("Not defined");
            }
        }
    }
}
