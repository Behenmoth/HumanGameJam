using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Game : MonoBehaviour
{
    // --- UI/�ݒ� ---
    public TextMeshPro counterText;       // 3D�I�u�W�F�N�g�̃J�E���^�[�\���p
    public Button mainClickButton;
    public Button passButton;
    public Text turnIndicatorText; // Canvas��̃^�[���\���p

    [Header("Item Card UI")]
    public Button[] itemCardButtons;         // �e�A�C�e���J�[�h

    [Header("Item Panel UI")]
    public GameObject itemPanel;            // �����ƑI�������܂ރp�l���S��
    public Image itemDescriptionImage;       // �A�C�e���̐�����\������Image�R���|�[�l���g
    public Button useItemButton;            // �u�g���v�{�^��
    public Button cancelItemButton;         // �u�L�����Z���v�{�^��

    public Sprite[] itemDescriptionSprites; // �A�C�e���摜
    
    [Header("Rule UI")]
    public Button ruleButton;             // ���[���\���J�n�{�^��
    public GameObject rulePanel;          // ���[���p�l���S��
    public Button closeRuleButton;        // ���[���p�l�������{�^��
    public Image ruleImage;               // ���[���摜 (Image�R���|�[�l���g)
    public Sprite ruleSprite;             // Unity�G�f�B�^�Őݒ肷�郋�[���摜

    [Header("Game Over UI")]
    public GameObject gameOverPanel;          // �Q�[���I�[�o�[�p�l���S��
    public Text winnerText;       // WINNER�\���p�e�L�X�g
    public Text youDiedText;      // YOU DIED�\���p�e�L�X�g
    public Text loserMessageText; // ���������b�Z�[�W�p�e�L�X�g
    public Button nextButton;                 // NEXT�{�^��

    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

    // --- ��ԕϐ� ---
    private int currentCounter;
    private int clicksMadeInTurn;
    private bool isPlayer1Turn = true;
    private bool isClickingAllowed;
    private const int MAX_CLICKS_PER_TURN = 3;

    // ���ݑI�𒆂̃A�C�e����ێ�����ϐ� (�����ł̓C���f�b�N�X�𕶎���ŕۑ�����̂���ʓI)
    private string selectedItemName = ""; // �� ����͎g��Ȃ����A�����̋@�\�g���̂��߂Ɏc��

    // �X�R�A�����O (3���E���h���A2�{���)
    private int player1Score = 0;
    private int player2Score = 0;
    private int currentRound = 1;

    void Start()
    {
        // UI�C�x���g�̐ݒ�
        mainClickButton.onClick.AddListener(HandlePushClick);
        passButton.onClick.AddListener(HandlePass);
    
        // �� ���[���{�^���̃��X�i�[��ǉ�
        ruleButton.onClick.AddListener(ShowRulePanel);
        closeRuleButton.onClick.AddListener(HideRulePanel);

        // �� ���X�i�[�o�^�̃u���b�N��1��ɂ܂Ƃ߂�
        for (int i = 0; i < itemCardButtons.Length; i++)
        {
            int index = i;
            // �����_�����g���A�N���b�N���ɂ��̃{�^���̃C���f�b�N�X��n��
            itemCardButtons[index].onClick.AddListener(() => OnItemCardClicked(index));
        }

        // �A�C�e���p�l���̃{�^�����X�i�[
        useItemButton.onClick.AddListener(UseItem);
        cancelItemButton.onClick.AddListener(HideItemPanel);

        // ������ԂŃA�C�e���p�l�����\���ɂ���
        if (itemPanel != null)
        {
            itemPanel.SetActive(false);
        }

        // �� ������ԂŃ��[���p�l�����\���ɂ���
        if (rulePanel != null)
        {
            rulePanel.SetActive(false);
        }

        // �� �Q�[���I�[�o�[�p�l���̃{�^�����X�i�[�Ə�����\����ǉ�
        nextButton.onClick.AddListener(OnNextButtonClicked);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

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
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString();
        isPlayer1Turn = (Random.Range(0, 2) == 0);
        StartTurn();
    }
    
    IEnumerator ShowRoundStartMessage(float delay)
    {
        // ������ꎞ�I�Ƀ��b�N
        isClickingAllowed = false; 
        mainClickButton.interactable = false;
        passButton.interactable = false;
        
        // �� ���E���h���ƃX�R�A��\��
        turnIndicatorText.text = $"���E���h {currentRound} �J�n�I �X�R�A: {player1Score} - {player2Score}";

        // �ҋ@
        yield return new WaitForSeconds(delay);

        // �ҋ@��Ɏ��ۂ̃^�[�����J�n
        StartTurn(); 
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        isClickingAllowed = true;
        mainClickButton.interactable = true;

 // �� �ǉ�: �O�̃^�[���Ŗ��������ꂽPASS�{�^����L�������āA������ĊJ����
        passButton.interactable = true; 

        string playerTurnName = isPlayer1Turn ? "1P" : "2P";
        turnIndicatorText.text = playerTurnName + " turn";
    }

    // --- ���[���\���A�N�V���� ---
    // ���[���{�^�����N���b�N���ꂽ�Ƃ��ɁA���[���p�l����\�����܂��B
    public void ShowRulePanel()
    {
        if (rulePanel == null || ruleSprite == null)
        {
            Debug.LogError("���[���p�l���܂��̓��[���摜���ݒ肳��Ă��܂���B");
            return;
        }

        // 1. ���[���摜��Image�R���|�[�l���g�ɐݒ�
        ruleImage.sprite = ruleSprite;

        // 2. �p�l����\��
        rulePanel.SetActive(true);

        // 3. ���̃��C��������ꎞ�I�Ƀ��b�N
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false;

        // ���[���{�^�����̂��A�Ŗh�~�̂��ߖ�����
        ruleButton.interactable = false;
    }
    
    // ����{�^�����N���b�N���ꂽ�Ƃ��ɁA���[���p�l�����\���ɂ��܂��B

    public void HideRulePanel()
    {
        if (rulePanel == null) return;

        // 1. �p�l�����\��
        rulePanel.SetActive(false);

        // 2. ���C������̃��b�N�������i�^�[���J�n���Ɠ�����Ԃɖ߂��j
        isClickingAllowed = true;
        
        // ���݂�clicksMadeInTurn�Ɋ�Â��A���C���{�^����PASS�{�^���̏�Ԃ𕜌�
        if (clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            mainClickButton.interactable = true;
        }
        else
        {
            // 3��v�b�V����̏ꍇ�́APASS�̂݉\
            mainClickButton.interactable = false;
        }
        
        // PASS�{�^���ƃ��[���{�^���͏�ɗL����
        passButton.interactable = true;
        ruleButton.interactable = true; 
    }

    // --- �A�C�e���A�N�V���� ---

    // �� �A�C�e���J�[�h�̃N���b�N���ɌĂяo����郁�\�b�h
    public void OnItemCardClicked(int itemIndex)
    {
        // �^�[�����łȂ��A�܂��͊��ɃA�C�e���p�l�����\�����̏ꍇ�͖���
        if (!isClickingAllowed || itemPanel.activeSelf) return;

        // �f�[�^�̃`�F�b�N
        if (itemIndex < 0 || itemIndex >= itemDescriptionSprites.Length)
        {
            Debug.LogError("�A�C�e���C���f�b�N�X�������ł�: " + itemIndex);
            return;
        }

        // 1. �����摜��؂�ւ���
        itemDescriptionImage.sprite = itemDescriptionSprites[itemIndex];

        // 2. ���C���̑�����ꎞ��~���A�p�l����\��
        mainClickButton.interactable = false;
        passButton.interactable = false; // �� PASS�{�^��������s�\�ɂ���
        isClickingAllowed = false; // �A�C�e���������͑��̑�������b�N

        // �p�l����\��
        itemPanel.SetActive(true);
    }

    // �� �u�g���v�{�^���������ꂽ�Ƃ��̏���
    public void UseItem()
    {
        Debug.Log("�A�C�e���̎g�p���m�肵�܂����i���ʂȂ��j�B�^�[���͌p�����܂��B");

        // 1. �p�l�����\���ɂ���
        HideItemPanel();

    }

    // �� �u�L�����Z���v�{�^���������ꂽ�Ƃ��̏���
    public void HideItemPanel()
    {
        itemPanel.SetActive(false);
        selectedItemName = "";

        // 1. ���C���{�^���̍ėL����
        if (clicksMadeInTurn < MAX_CLICKS_PER_TURN)
        {
            // 3��v�b�V�������Ȃ烁�C���{�^����L���ɂ���
            mainClickButton.interactable = true;
        }
        else
        {
            // 3��v�b�V���ς݂̏ꍇ�͖����̂܂܁iPASS�K�{�j
            mainClickButton.interactable = false;
        }

        // 2. PASS�{�^����L����
        
        passButton.interactable = true;
        

        // 3. �Q�[���S�̂̃N���b�N���b�N������
        isClickingAllowed = true;
    }

    // --- �v���C���[�A�N�V���� ---
    public void HandlePushClick()
    {
        if (!isClickingAllowed) return;

        isClickingAllowed = false; // �A���N���b�N�h�~

        // 1. �J�E���^�[�����炷
        currentCounter--;
        clicksMadeInTurn++;
        counterText.text = currentCounter.ToString();

        // 2. �����`�F�b�N
        if (currentCounter <= 0)
        {
            isClickingAllowed = false;
            mainClickButton.interactable = false;
            passButton.interactable = false;
            EndRound(isPlayer1Turn);
            return;
        }

        // 3. PASS�{�^���ƌp���ۂ̔���
      
            isClickingAllowed = true; // �p���N���b�N�܂���PASS�I���̂��ߍėL����

        if (clicksMadeInTurn == MAX_CLICKS_PER_TURN)
        {
            mainClickButton.interactable = false; // 3��v�b�V����̓��C���{�^���𖳌���
                // PASS�{�^���͗L���̂܂�
        }
        else
        {
            // 1��ځA2��ڂȂ�v�b�V���{�^���͗L���̂܂�
            mainClickButton.interactable = true;
        }
        
        
    }

    public void HandlePass()
    {
        Debug.Log("HandlePass() Called. Attempting turn end."); // �� �m�F�p���O

        if (!isClickingAllowed) return;

        //  1����v�b�V�����Ă��Ȃ��ꍇ�͏����𒆒f����
        if (clicksMadeInTurn == 0)
        {
            Debug.Log("PASS�{�^��: 1����v�b�V�����Ă��Ȃ����ߖ����ł��B");
            return;
        }
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false; 
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
        isClickingAllowed = false;

        if (playerWhoLost)
        {
            player2Score++;
        }
        else
        {
            player1Score++;
        }

        turnIndicatorText.text = $"���E���h�I���I �X�R�A: {player1Score} - {player2Score}";

        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

        currentRound++;
        StartCoroutine(WaitAndStartNextRound(3.0f));
    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartRound();
    }
    public void OnNextButtonClicked()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // �p�l�����\���ɂ���
        }

        // �� �Q�[�������X�^�[�g����
        StartGame();

        // �K�v�ɉ����āA����UI�v�f�i�Ⴆ��turnIndicatorText�j��������Ԃɖ߂�
        turnIndicatorText.text = "Game Start!"; // ��
    }

    void EndGame()
    {  // �S�ẴQ�[����������b�N
        isClickingAllowed = false;
        mainClickButton.interactable = false;
        passButton.interactable = false;

        // �A�C�e���⃋�[���p�l��������i�����J���Ă����ꍇ�j
        if (itemPanel.activeSelf) itemPanel.SetActive(false);
        if (rulePanel.activeSelf) rulePanel.SetActive(false);


        // ���҂𔻒�
        string winnerName = (player1Score == 2) ? "1P" : "2P";
        string loserName = (player1Score == 2) ? "2P" : "1P"; // �������v���C���[

        // WINNER�\��
        winnerText.text = $"WINNER {winnerName}";

        // YOU DIED�ƕ��������b�Z�[�W�͌Œ�e�L�X�g
        youDiedText.text = "YOU DIED";
        loserMessageText.text = $"�������炨�O�̖��O�͕��������B ({loserName})"; // �N����������������悤��

        // �Q�[���I�[�o�[�p�l����\��
        gameOverPanel.SetActive(true);

    }
}