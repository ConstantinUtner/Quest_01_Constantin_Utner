using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinCounterText;

    [SerializeField]
    private Character character;

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private CanvasGroup hudCanvasGroup;

    [SerializeField]
    private CanvasGroup gameOverCanvasGroup;

    [SerializeField]
    private float fadingTime = 2.0f;

    private bool isFadingInGameOver = false;

    private IEnumerator FadeInGameOver()
    {
        this.isFadingInGameOver = true;
        float timer = 0.0f;

        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            this.gameOverCanvasGroup.alpha = percent;

            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = 0.0f;
        this.gameOverCanvasGroup.alpha = 1.0f;
    }

    private static UIManager instance = null;
    public static UIManager Instance => instance;

    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }

    private PlayerStatistics statistics;

    private void Update()
    {
        float percent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = percent;

        if (percent <= 0.0f && !this.isFadingInGameOver)
        {
            this.StartCoroutine(this.FadeInGameOver());
        }
    }

    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $"{this.statistics.coinCounter}";
        this.coinCounterText.text = coinText;
    }
}
