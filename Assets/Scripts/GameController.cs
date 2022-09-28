using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private Page menu;
    [SerializeField] private ResultPage resultPage;
    [SerializeField] private Tank[] allyTanks;
    [SerializeField] private Tank[] enemyTanks;

    private void Awake()
    {
        menu.OnOpen += () =>
        {
            Cursor.lockState = CursorLockMode.None;
        };
        menu.OnClose += () =>
        {
            Cursor.lockState = CursorLockMode.Locked;
        };
        foreach (Tank tank in this.allyTanks)
        {
            tank.onStateChanged += this.CheckGameResult;
        }
        foreach (Tank tank in this.enemyTanks)
        {
            tank.onStateChanged += this.CheckGameResult;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!this.resultPage.isOpen && Input.GetKeyUp(KeyCode.Escape))
        {
            if (this.menu.IsOpen)
            {
                this.menu.Close();
            }
            else
            {
                this.menu.Open();
            }
        }
    }

    public bool isPause
    {
        get
        {
            return this.menu.IsOpen || this.resultPage.isOpen;
        }
    }

    public void BackToMainPage()
    {
        SceneManager.LoadScene("Main");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void CheckGameResult()
    {
        if (this.allyTanks.All(tank => !tank.isAlive))
        {
            this.resultPage.DisplayResult(false);
        }
        else if (this.enemyTanks.All(tank => !tank.isAlive))
        {
            this.resultPage.DisplayResult(true);
        }
    }
}
