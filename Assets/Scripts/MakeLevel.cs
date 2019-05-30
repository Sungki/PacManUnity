using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeLevel : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject pacmanPrefab;
    public GameObject cookiePrefab;
    public GameObject enemyPrefab;

    GameObject enemyClone;
    int enemyCount = 0;

    public static bool[,] collisionMap = new bool[20,17];

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
                collisionMap[col, -row] = true;
                Instantiate(blockPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
            }
            else if(levelData[i] == '2')
            {
                // create pacman
                Instantiate(pacmanPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
                ScriptLocator.pacman.GetComponent<Pacman>().mapX = col -1;
                ScriptLocator.pacman.GetComponent<Pacman>().mapY = -row;
            }
            else if (levelData[i] == '0')
            {
                // create cookies
                Instantiate(cookiePrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
            }
            else if (levelData[i] == '3')
            {
                // create enemies
                enemyClone = Instantiate(enemyPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
                enemyClone.GetComponent<Enemy>().mapX = col - 1;
                enemyClone.GetComponent<Enemy>().mapY = -row;
                if (enemyCount == 0) enemyClone.GetComponent<Enemy>().color = Color.red;
                else if (enemyCount == 1) enemyClone.GetComponent<Enemy>().color = Color.green;
                else if (enemyCount == 2) enemyClone.GetComponent<Enemy>().color = Color.cyan;
                else if (enemyCount == 3) enemyClone.GetComponent<Enemy>().color = Color.black;
                enemyCount++;
            }
            else if (levelData[i] == '\n')
            {
                // line break
                col = 0;
                row--;
            }
            else
            {
                // Not defined
            }
        }
    }
}