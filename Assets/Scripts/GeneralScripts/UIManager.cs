using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UIManager : MonoBehaviour
{
    [Header("Lives UI")]
    [SerializeField] private Image[] hearts; // Heart1..Heart3

    [Header("Coins UI")]
    [SerializeField] private Text coinText;

    [Header("Kills UI")]
    [SerializeField] private Text killsText; // <- assign in Inspector

    private int coinCount = 0;
    private int killCount = 0;

    public int CurrentCoins => coinCount;
    public int CurrentKills => killCount;

    // ----- Lives -----
    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < currentLives;
    }

    // ----- Coins -----
    public void AddCoin(int amount = 1)
    {
        coinCount += amount;
        if (coinText) coinText.text = "Coins: " + coinCount;
    }

    public void ResetCoins()
    {
        coinCount = 0;
        if (coinText) coinText.text = "Coins: 0";
    }

    // ----- Kills -----
    public void AddKill(int amount = 1)
    {
        killCount += amount;
        if (killsText) killsText.text = "Enemies Killed: " + killCount;
    }

    public void ResetKills()
    {
        killCount = 0;
        if (killsText) killsText.text = "Enemies Killed: 0";
    }
}
