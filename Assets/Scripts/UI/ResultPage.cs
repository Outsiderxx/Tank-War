using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Page))]
public class ResultPage : MonoBehaviour
{
    [SerializeField] private Text result;
    [SerializeField] private Image[] dividers;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    private Page page;

    private void Awake()
    {
        this.page = this.GetComponent<Page>();
    }

    public void DisplayResult(bool isWin)
    {
        if (this.page.IsOpen)
        {
            return;
        }
        if (isWin)
        {
            this.result.text = "YOU WIN!";
            this.result.color = this.winColor;
            foreach (Image divider in this.dividers)
            {
                divider.color = this.winColor;
            }
        }
        else
        {
            this.result.text = "YOU LOSE!";
            this.result.color = this.loseColor;
            foreach (Image divider in this.dividers)
            {
                divider.color = this.loseColor;
            }
        }
        Cursor.lockState = CursorLockMode.None;
        this.page.Open();
    }
}