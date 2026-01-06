using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UIの動作チェック")]
    public bool isUIBlocking = false;

    [Header("ラウンドUI")]
    [SerializeField] private GameObject roundUI;
    [SerializeField] private CanvasGroup roundImage;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text bombText;

    [Header("リザルトUI")]
    [SerializeField] private GameObject resultUI;
    [SerializeField] private CanvasGroup resultImage;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("演出設定")]
    [SerializeField] private float roundUIInitialAlpha = 0.6f;
    [SerializeField] private float resultUiInitialAlpha = 0f;
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        roundUI.SetActive(false);
        roundImage.alpha = 0f;
        roundImage.interactable = false;
        roundImage.blocksRaycasts = false;

        resultUI.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ラウンド開始UIを表示する
    public void ShowRoundStart(int round,int count)
    {
        RoundStart(round, count).Forget();
    }

    //ゲーム終了時のリザルトUIを表示する
    public void ShowResult(string winnerName,int player1,int player2)
    {
        Result(winnerName, player1, player2).Forget();
    }

    //ラウンド表示処理
    private async UniTaskVoid RoundStart(int round,int count)
    {
        //UIロック
        isUIBlocking = true;
        //アルファ値の初期化
        roundImage.alpha = roundUIInitialAlpha;
        
        roundUI.SetActive(true);

        roundText.text = $"Round {round}";
        bombText.text = $"爆弾カウント {count}";

        //表示する
        await UniTask.Delay((int)displayTime * 1000);

        //フェードアウト
        await Fade(roundImage,roundUIInitialAlpha, 0f,fadeDuration);

        roundUI.SetActive(false);

        //ロック解除
        isUIBlocking = false;
    }

    //リザルト表示処理
    private async UniTaskVoid Result(string winnerName, int player1, int player2)
    {
        //UIロック
        isUIBlocking = true;
        resultImage.alpha = resultUiInitialAlpha;

        resultUI.SetActive(true);

        winnerText.text = $"{winnerName} Win";
        scoreText.text = $"Score {player1} - {player2}";

        // フェードイン
        await Fade(resultImage, resultUiInitialAlpha, 1f, fadeDuration);

        // 操作可能にする
        resultImage.interactable = true;
        resultImage.blocksRaycasts = true;
    }

    //フェードさせる
    private async UniTask Fade(CanvasGroup canvasgroup, float from, float to, float duration)
    {
        float time = 0f;
        canvasgroup.alpha = from;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasgroup.alpha = Mathf.Lerp(from, to, time / duration);
            await UniTask.Yield();
        }

        canvasgroup.alpha = to;
    }
}
