using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // �� TextMeshPro���g�p���邽�߂ɒǉ�

public class GameManager_HotSeat : MonoBehaviour
{
    // --- UI/�ݒ� ---
    // �^��TextMeshProUGUI�ɓ���
    public TextMeshProUGUI counterText;       // �� TMPRO�ɏC��
    public Button mainClickButton;
    public Button passButton;
    public TextMeshProUGUI turnIndicatorText; // �� TMPRO�ɏC��

    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

    // --- ��ԕϐ� ---
    private int currentCounter;
    private int clicksMadeInTurn;
    private bool isPlayer1Turn = true; // Player 1 (true) �܂��� Player 2 (false)
    private bool isClickingAllowed;
    private const int MAX_CLICKS_PER_TURN = 3;

    // �X�R�A�����O (3���E���h���A2�{���)
    private int player1Score = 0;
    private int player2Score = 0;
    private int currentRound = 1;

    void Start()
    {
        // UI�C�x���g�̐ݒ�
        mainClickButton.onClick.AddListener(HandlePushClick);
        passButton.onClick.AddListener(HandlePass);

        StartGame();
    }

    // --- �Q�[���t���[ ---
    void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        currentRound = 1;

        StartRound();
    }

    void StartRound()
    {
        // �J�E���^�[�������_���ɏ�����
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString(); // TMPro��Text�ւ̑��

        // �ŏ��̃^�[���̓����_���Ɍ���
        isPlayer1Turn = (Random.Range(0, 2) == 0);

        StartTurn();
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        passButton.gameObject.SetActive(false);
        isClickingAllowed = true;
        mainClickButton.interactable = true;

        // �ǂ���̃v���C���[�̃^�[������\��
        string playerTurnName = isPlayer1Turn ? "�v���C���[ 1" : "�v���C���[ 2";
        turnIndicatorText.text = playerTurnName + " �̃^�[���ł�"; // TMPro��Text�ւ̑��
    }

    // --- �v���C���[�A�N�V���� ---
    public void HandlePushClick()
    {
        if (!isClickingAllowed) return; // �N���b�N��������Ă��Ȃ��ꍇ�͖���

        // �� �����ɃN���b�N�𖳌��� (�A���N���b�N�h�~)
        isClickingAllowed = false;

        // 1. �J�E���^�[�����炷
        currentCounter--;
        clicksMadeInTurn++;
        counterText.text = currentCounter.ToString();

        // 2. �����`�F�b�N
        if (currentCounter <= 0)
        {
            EndRound(isPlayer1Turn); // �������������̃v���C���[���s�k
            return;
        }

        // 3. PASS�{�^���ƌp���ۂ̔���
        if (clicksMadeInTurn >= 1 && clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            // 1��ڂ܂���2��ڂ̃N���b�N��
            passButton.gameObject.SetActive(true); // PASS�{�^����\��
            isClickingAllowed = true; // �� PASS���p���N���b�N�̑I������^���邽�ߍėL����
        }
        else if (clicksMadeInTurn == MAX_CLICKS_PER_TURN)
        {
            // 3��ڂ̃N���b�N������ -> �����^�[�����
            mainClickButton.interactable = false;
            StartCoroutine(WaitAndEndTurn(0.5f));
        }
        else
        {
            // PASS�������ꂸ�A����3�񖢖��̏ꍇ�́A�p���N���b�N���\�Ȃ��ߍėL����
            isClickingAllowed = true;
        }
    }

    public void HandlePass()
    {
        if (!isClickingAllowed) return;

        isClickingAllowed = false; // �� �^�[���I���O�ɃN���b�N�𖳌���

        // PASS�{�^���𖳌������A�^�[�����
        mainClickButton.interactable = false;
        passButton.gameObject.SetActive(false);
        StartCoroutine(WaitAndEndTurn(0.1f));
    }

    // --- �^�[���I�� ---
    IEnumerator WaitAndEndTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        isPlayer1Turn = !isPlayer1Turn; // �^�[�������
        StartTurn();
    }

    // --- ���E���h�I���ƃX�R�A�����O ---
    void EndRound(bool playerWhoLost)
    {
        // �s�ґ��𔻒肵�A����ɃX�R�A�����Z
        if (playerWhoLost)
        {
            player2Score++; // �v���C���[1�������������ꍇ�A�v���C���[2�̏���
            turnIndicatorText.text = "�v���C���[ 2 �̏����I\n�X�R�A: " + player1Score + " - " + player2Score;
        }
        else
        {
            player1Score++; // �v���C���[2�������������ꍇ�A�v���C���[1�̏���
            turnIndicatorText.text = "�v���C���[ 1 �̏����I\n�X�R�A: " + player1Score + " - " + player2Score;
        }

        mainClickButton.interactable = false;

        // �Q�[���S�̂̏��s�`�F�b�N (2�{���)
        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

        // ���̃��E���h��
        currentRound++;
        StartCoroutine(WaitAndStartNextRound(3.0f));
    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }

    void EndGame()
    {
        string winner = (player1Score == 2) ? "�v���C���[ 1" : "�v���C���[ 2";
        turnIndicatorText.text = winner + "���Q�[���̏��҂ł��I";
        mainClickButton.interactable = false; // �N���b�N���~
    }
}