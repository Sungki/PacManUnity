using System.Collections;
using UnityEngine;

public class ScriptLocator
{
    private static GameObject _pacman;
    public static GameObject pacman
    {
        get
        {
            if (!_pacman)
            {
                _pacman = GameObject.FindGameObjectWithTag("Pacman");

            }

            return _pacman;
        }
    }

    private static GameObject _gamemanager;
    public static GameObject gamemanager
    {
        get
        {
            if (!_gamemanager)
            {
                _gamemanager = GameObject.Find("GameManager");

            }

            return _gamemanager;
        }
    }
}
