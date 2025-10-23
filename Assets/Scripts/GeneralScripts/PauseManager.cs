using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public static bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;

        // Unlock mouse for UI aiming
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;

        // Don’t instantly lock the cursor again!
        // Give the player a frame to move the mouse and regain aim position
        StartCoroutine(ReLockCursorAfterDelay());
        Input.ResetInputAxes(); // flush old clicks
    }

    private System.Collections.IEnumerator ReLockCursorAfterDelay()
    {
        yield return null; // wait one frame
        Cursor.lockState = CursorLockMode.None; // let mouse move freely again
        Cursor.visible = true; // keep it visible so aiming works
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
