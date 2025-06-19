using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText; // Referencia al TextMeshPro
    [SerializeField] private CameraFollow cameraFollow;
    public bool GameOver = false; // Variable estática para indicar si el juego ha terminado
    public bool Win = false; // Variable estática para indicar si el jugador ha ganado
    public GameObject UiGameOver;
    public GameObject UiWin;

    private void Awake()
    {
        GameOver = false;
        Win = false;
    }

    void Update()
    {
        if (GameOver)
        {
            UiGameOver.gameObject.SetActive(true);
        }
    }

    public void Conteo()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int countdown = 5; // Tiempo inicial de la cuenta regresiva
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString(); // Actualizar el texto
            yield return new WaitForSeconds(1f); // Esperar 1 segundo
            countdown--; // Reducir el contador
        }
        
        cameraFollow.focusOnTarget = true;
        UiWin.gameObject.SetActive(true);
    }
    
    public void RestartGame()
    {
        GameOver = false; // Reiniciar el estado del juego
        Win = false; // Reiniciar el estado de victoria
        UiGameOver.gameObject.SetActive(false); // Desactivar la UI
        UiWin.gameObject.SetActive(false); // Desactivar la UI de victoria
        LevelManager.Instance.ReloadCurrentLevel(); // Recargar la escena actual
    }
    
    public void DesignBoat(int levelIndex)
    {
        GameOver = false; // Reiniciar el estado del juego
        Win = false; // Reiniciar el estado de victoria
        UiGameOver.gameObject.SetActive(false); // Desactivar la UI
        UiWin.gameObject.SetActive(false); // Desactivar la UI de victoria
        LevelManager.Instance.LoadLevelByIndex(levelIndex); // Cargar la escena por índice
    }
}
