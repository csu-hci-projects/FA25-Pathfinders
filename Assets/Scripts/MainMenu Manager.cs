using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        // This makes it stop play mode inside the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
