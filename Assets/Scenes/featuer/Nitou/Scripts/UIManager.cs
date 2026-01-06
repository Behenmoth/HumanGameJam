using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("ラウンドUI")]
    [SerializeField] private GameObject roundUI;
    [SerializeField] private Image roundImage;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text bombText;

    [Header("演出設定")]
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
        RoundStart(round, count);
    }

    private async UniTaskVoid RoundStart(int round,int count)
    {
        roundText.text = $"Round {round}";
        bombText.text = $"爆弾カウント {count}";

        roundUI.SetActive(true);

        //表示する
        await UniTask.Delay((int)displayTime * 1000);

        //フェードアウト
        await Fade(1f, 0f);

    }

    //フェードさせる
    private async UniTask Fade(float from,float to)
    {
        float time = 0f;


        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, time / fadeDuration);
            SetAlpha(alpha);
            await UniTask.Yield();
        }

        SetAlpha(to);
    }

    //アルファ値を設定
    private void SetAlpha(float alpha)
    {
        Color color = roundImage.color;
        color.a = alpha;
        roundImage.color = color;
    }
}
