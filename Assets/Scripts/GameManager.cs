using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject cookiePrefab;
    public GameObject enemyPrefab;
    public GameObject pacmanPrefab;
    public GameObject powerballPrefab;

    GameObject enemyClone;
    Text scoreText;
    int enemyCount = 0;
    int score = 0;

    public bool[,] collisionMap = new bool[20, 20];

    List<Vector2> enemyPos = new List<Vector2>();

    void Start()
    {
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        InitLevel();
    }

    public void SetScore(int _score)
    {
        score += _score;
        scoreText.text = "Score: " + score;
    }

    public void SpawnEnemy(Color color)
    {
        if (color == Color.red)
        {
            enemyClone = Instantiate(enemyPrefab, new Vector3(enemyPos[0].x, 0.5f, enemyPos[0].y), Quaternion.identity);
            enemyClone.GetComponent<Enemy>().mapX = (int)enemyPos[0].x;
            enemyClone.GetComponent<Enemy>().mapY = -(int)enemyPos[0].y;
            enemyClone.GetComponent<Enemy>().color = Color.red;
        }
        else if (color == Color.green)
        {
            enemyClone = Instantiate(enemyPrefab, new Vector3(enemyPos[1].x, 0.5f, enemyPos[1].y), Quaternion.identity);
            enemyClone.GetComponent<Enemy>().mapX = (int)enemyPos[1].x;
            enemyClone.GetComponent<Enemy>().mapY = -(int)enemyPos[1].y;
            enemyClone.GetComponent<Enemy>().color = Color.green;
        }
        else if (color == Color.cyan)
        {
            enemyClone = Instantiate(enemyPrefab, new Vector3(enemyPos[2].x, 0.5f, enemyPos[2].y), Quaternion.identity);
            enemyClone.GetComponent<Enemy>().mapX = (int)enemyPos[2].x;
            enemyClone.GetComponent<Enemy>().mapY = -(int)enemyPos[2].y;
            enemyClone.GetComponent<Enemy>().color = Color.cyan;
        }
        else if (color == Color.black)
        {
            enemyClone = Instantiate(enemyPrefab, new Vector3(enemyPos[3].x, 0.5f, enemyPos[3].y), Quaternion.identity);
            enemyClone.GetComponent<Enemy>().mapX = (int)enemyPos[3].x;
            enemyClone.GetComponent<Enemy>().mapY = -(int)enemyPos[3].y;
            enemyClone.GetComponent<Enemy>().color = Color.black;
        }
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
            else if (levelData[i] == '2')
            {
                // create pacman
                Instantiate(pacmanPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
                ScriptLocator.pacman.GetComponent<Pacman>().mapX = col - 1;
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
                enemyPos.Add(new Vector2(col - 1, row));
                if (enemyCount == 0) enemyClone.GetComponent<Enemy>().color = Color.red;
                else if (enemyCount == 1) enemyClone.GetComponent<Enemy>().color = Color.green;
                else if (enemyCount == 2) enemyClone.GetComponent<Enemy>().color = Color.cyan;
                else if (enemyCount == 3) enemyClone.GetComponent<Enemy>().color = Color.black;
                enemyCount++;
            }
            else if (levelData[i] == '4')
            {
                // create power ball
                Instantiate(powerballPrefab, new Vector3(col++, 0.5f, row), Quaternion.identity);
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
