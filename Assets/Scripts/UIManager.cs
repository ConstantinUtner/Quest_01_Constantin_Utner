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
    private CanvasGroup victoryCanvasGroup;

    [SerializeField]
    private float fadingTime = 0.25f;

    private bool isFadingInGameOver = false;
    private bool isFadingInVictory = false;

    private static UIManager instance = null;
    public static UIManager Instance => instance;

    private PlayerStatistics statistics;

    private class PlayerStatistics
    {
        public int coinCounter = 0;
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

    private void Update()
    {
        // Aktualisiert die Healthbar basierend auf dem aktuellen Leben
        float percent = this.character.GetCurrentHealth() / this.character.GetMaxHealth();
        this.healthBar.fillAmount = percent;

        // Wenn das Leben 0 erreicht, starte das Einblenden des Game Over Screens
        if (percent <= 0.0f && !this.isFadingInGameOver)
        {
            this.StartCoroutine(this.FadeInGameOver());
        }
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        this.coinCounterText.text = $"{this.statistics.coinCounter}";
    }

    // --- GAME OVER BUTTON FUNKTIONEN ---

    public void OnRespawnClicked()
    {
        // BUGFIX: EventSystem-Auswahl leeren, damit die Taste während des Ausblendens
        // nicht versehentlich erneut (z. B. durch Enter) ausgelöst wird.
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        // 1. Münzen auf 0 setzen
        this.statistics.coinCounter = 0;
        this.coinCounterText.text = "0";

        // 2. Leben auf 100% setzen
        this.character.ResetHealth();

        // 3. Sieg-Status zurücksetzen
        this.character.SetVictory(false);

        // 4. Spieler an den Startpunkt teleportieren
        this.character.Respawn();

        // 5. Alle Juwelen zurücksetzen
        Jewel[] jewels = Object.FindObjectsByType<Jewel>(FindObjectsSortMode.None);
        foreach (var j in jewels)
        {
            j.ResetJewel();
        }

        // 6. Bildschirme ausfaden (nur wenn sie auch wirklich sichtbar sind)
        // & Flackern verhindern, indem vor dem Ausblenden geprüft wird, ob die Screens aktiv sind.
        if (this.gameOverCanvasGroup.alpha > 0)
        {
            this.StartCoroutine(this.FadeOutGameOver());
        }

        if (this.victoryCanvasGroup.alpha > 0)
        {
            this.StartCoroutine(this.FadeOutVictory());
        }
    }

    public void OnExitClicked()
    {
        // BUGFIX: Versehentliches Beenden durch Leeren der Auswahl verhindern.
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        // Beendet das fertige Spiel
        Application.Quit();

#if UNITY_EDITOR
        // Stoppt den Play-Mode, falls wir im Unity Editor sind
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- FADING LOGIK ---

    // Blendet den Sieg-Bildschirm ein und deaktiviert die Spielersteuerung
    public void ShowVictory()
    {
        if (!this.isFadingInVictory)
        {
            this.character.SetVictory(true);
            this.StartCoroutine(this.FadeInVictory());
        }
    }

    private IEnumerator FadeInVictory()
    {
        this.isFadingInVictory = true;
        float timer = 0.0f;

        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = 1.0f - percent;
            this.victoryCanvasGroup.alpha = percent;

            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = 0.0f;
        this.victoryCanvasGroup.alpha = 1.0f;
        this.victoryCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOutVictory()
    {
        float timer = 0.0f;

        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = percent;
            this.victoryCanvasGroup.alpha = 1.0f - percent;

            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = 1.0f;
        this.victoryCanvasGroup.alpha = 0.0f;
        this.victoryCanvasGroup.blocksRaycasts = false;
        this.isFadingInVictory = false;
    }

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

        // Aktiviert die Interaktion mit dem Game Over Screen.
        // Nur wenn blocksRaycasts auf 'true' steht, können die Buttons (Respawn/Exit) angeklickt werden.
        this.gameOverCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOutGameOver()
    {
        float timer = 0.0f;

        while (timer < this.fadingTime)
        {
            float percent = timer / this.fadingTime;
            this.hudCanvasGroup.alpha = percent;
            this.gameOverCanvasGroup.alpha = 1.0f - percent;

            yield return null;
            timer += Time.deltaTime;
        }

        this.hudCanvasGroup.alpha = 1.0f;
        this.gameOverCanvasGroup.alpha = 0.0f;

        // Deaktiviert die Interaktion mit dem unsichtbaren Game Over Screen.
        // 'false' sorgt dafür, dass der Screen keine Klicks schluckt, die eigentlich für das Spiel gedacht sind.
        this.gameOverCanvasGroup.blocksRaycasts = false;

        this.isFadingInGameOver = false;
    }
}
