using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
public class MenuHandlers : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Text fpsOption;
    public TMP_Text fpsUi;
    private float deltaTime = 0.0f;
    private bool showfps = false;
    void Start()
    {
        fpsOption.text = "Limite de frames: 60";
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    
    void Update()
    {
        if (showfps)
        {
            float fps = 1.0f / Time.deltaTime;
            fpsUi.text = "FPS: " + Mathf.Ceil(fps).ToString();
        }
    }
    
    public void Setfps(float fps)
    {
        int targetFps = Mathf.RoundToInt(fps); 
        
        if (targetFps > 119)
        {
            fpsOption.text = "Limite de frames: ilimitado";
            Debug.Log("FPS set to unlimited");
            Application.targetFrameRate = -1;
        }
        else
        {
            fpsOption.text = "Limite de frames: " + targetFps;
            Application.targetFrameRate = targetFps;
            Debug.Log("FPS set to: " + fps);
        }
    }
    public void setvolume(float volume)
    {
        audioMixer.SetFloat("Musica", volume);
    }
    
    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void helpTips(bool isOn )
    {
        if (isOn)
        {
            Debug.Log("help tips is On");
        }
        else
        {
            Debug.Log("help tips is Off");
        }
    }
    
    public void FpsText(bool isOn)
    {
        if (isOn)
        {
            showfps = true;
            fpsUi.gameObject.SetActive(true);
        }
        else
        {
            showfps = false;
            fpsUi.gameObject.SetActive(false);
        }
    }
    
    public void ScreenShake(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("Screen Shake is On");
        }
        else
        {
            Debug.Log("Screen Shake is Off");
        }
    }
    
    public void Mutetoggle(bool isOn) 
    { 
        if (isOn)
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", 0);
        }
    }
    
}