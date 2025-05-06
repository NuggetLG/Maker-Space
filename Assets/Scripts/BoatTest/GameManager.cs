using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool GameOver = false; // Variable estática para indicar si el juego ha terminado
    public static bool Win = false; // Variable estática para indicar si el jugador ha ganado
    public GameObject Ui;
    
    void Update()
    {
        if (GameOver)
        {
            Ui.gameObject.SetActive(true);   
        }
    }
}
