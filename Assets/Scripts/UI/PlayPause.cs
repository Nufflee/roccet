
using UnityEngine;
using UnityEngine.UI;

public class PlayPause : MonoBehaviour
{
    public Sprite Play;
    public Sprite Pause;
    public Image image;

    public bool isPaused;
    
    public void togglePlay()
    {
        if(!isPaused)
        {
            isPaused = true;
            image.sprite = Play;
            Time.timeScale = 0f;
        }
        else
        {
            isPaused = false;
            image.sprite = Pause;
            Time.timeScale = 1f;
        }
    }
}
