using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinCounterText;

    private static UIManager instance = null;
    public static UIManager Instance => instance;

    private class PlayerStatistics
    {
        public int coinCounter = 0;
    }

    private PlayerStatistics statistics;

    private void Awake()
    {
        instance = this;
        this.statistics = new PlayerStatistics() { coinCounter = 0 };
    }

    public void CollectCoin()
    {
        this.statistics.coinCounter++;
        string coinText = $"{this.statistics.coinCounter}";
        this.coinCounterText.text = coinText;
    }
}
