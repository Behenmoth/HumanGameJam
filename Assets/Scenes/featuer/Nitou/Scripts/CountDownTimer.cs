using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public static CountDownTimer instance;

    [Header("�^�C�}�[")]
    public float limitTimer;
    public float currentTimer;
    public bool isCountDown = false;
    public TMP_Text countDownTimerText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Update()
    {
        //�J�E���g�_�E�����̂ݓ���
        if (isCountDown)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                currentTimer = 0;
                isCountDown = false;

                //�J�E���g��0�ɂȂ�ƃQ�[���I�[�o�[
                GameManager.instance.GameOver();
            }
            UpdateTimerUI();
        }
    }

    //�^�C�}�[�̃J�E���g�_�E�����J�n����
    public void StartCountDownTimer()
    {
        currentTimer = limitTimer;
        isCountDown = true;

        Debug.Log("�^�C�}�[�J�n");
    }

    //�^�C�}�[�̃J�E���g�_�E���̃��Z�b�g
    public void ResetCountDownTimer()
    {
        currentTimer = limitTimer;
        isCountDown = false;
        UpdateTimerUI();

        Debug.Log("�^�C�}�[���Z�b�g");
    }

    //�^�C�}�[��UI���X�V
    public void UpdateTimerUI()
    {
        countDownTimerText.text = $"{currentTimer:F2}";
    }
}
