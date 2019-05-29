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
}
