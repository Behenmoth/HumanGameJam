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

    [Header("Turn Indicator Settings")]
    public RectTransform turnIndicatorRect; // �� turnIndicatorText��RectTransform
    public float leftPositionX = -200f;     // �� 1P����X���W�i��: -200�j
    public float rightPositionX = 200f;    // �� 2P����X���W�i��: 200�j

    [Header("Item Card UI")]
    public Button[] itemCardButtons;         // �e�A�C�e���J�[�h

    [Header("Item Panel UI")]
    public GameObject itemPanel;            // �����ƑI�������܂ރp�l���S��
    public Image itemDescriptionImage;       // �A�C�e���̐�����\������Image�R���|�[�l���g
    public Button useItemButton;            // �u�g���v�{�^��
    public Button cancelItemButton;         // �u�L�����Z���v�{�^��

    public Sprite[] itemDescriptionSprites; // �A�C�e���摜


    [Header("Round Result UI")]
    public GameObject roundResultPanel;       // ���E���h���ʃp�l���S��
    public TextMeshProUGUI roundWinnerText;  // �����ҕ\���p�e�L�X�g (��: "1P WIN")
    public TextMeshProUGUI roundScoreText;   // �X�R�A�\���p�e�L�X�g (��: "SCORE 1 - 0")
    public Button nextButton;                 // NEXT�{�^��


    [Header("Game Info UI")]
    public TextMeshProUGUI roundNumberText; // �� ���E���h������ɕ\�����邽�߂̐V�����e�L�X�g
    public TextMeshProUGUI scoreText;       // �� �X�R�A����ɕ\�����邽�߂̐V�����e�L�X�g

    [Header("Round Start UI")] // �� �V�����w�b�_�[
    public GameObject roundStartPanel;        // ���E���h�J�n�p�l���S��
    public TextMeshProUGUI roundStartNumberText; // "ROUND ��" �\���p�e�L�X�g

    [Header("Game Info Rects")] // �� �V����RectTransform�ϐ�
    public RectTransform roundNumberRect;
    public RectTransform scoreRect;
    public float infoRightX = 200f;       // �� ���\�����E���ɗ���ꍇ��X���W (��: topPositionX)
    public float infoLeftX = -200f;      // �� ���\���������ɗ���ꍇ��X���W (��: bottomPositionX)

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
   
    [Header("Game Settings")]
    public int minStartCounter = 20;
    public int maxStartCounter = 40;

    [Header("Messages")]
    public string[] loserMessages = new string[] // �� ���������b�Z�[�W�̌��
    {
        "�������炨�O�̖��O�͕��������B",
        "�˔\�̍����B�Ƃŋ����B",
        "�s�k��m�肽���B",
        "�S�~���A������B",
        "�ӂ�...���F�A���̒��x�̒j��B",
        "�c�O�����A�˔\���^���Ȃ������ȁB"
    };

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

        // �� ���E���h���ʃp�l����������ԂŔ�\���ɂ���
        if (roundResultPanel != null)
        {
            roundResultPanel.SetActive(false);
        }


        // �� �Q�[���I�[�o�[�p�l���̃{�^�����X�i�[�Ə�����\����ǉ�
        nextButton.onClick.AddListener(OnNextButtonClicked);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
 
        // �� ���E���h�J�n�p�l����������ԂŔ�\���ɂ���
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }
        StartGame();
    }

    // --- �Q�[���t���[ ---
    void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        currentRound = 1;
        
        // �� ���E���h���������\��
        if (roundNumberText != null)
        {
            roundNumberText.text = $"ROUND {currentRound}";
        }
        
        // �� �X�R�A�������\��
        if (scoreText != null)
        {
            scoreText.text = $"SCORE {player1Score} - {player2Score}";
        }

        StartRound();
    }

    void StartRound()
    {
        currentCounter = Random.Range(minStartCounter, maxStartCounter + 1);
        counterText.text = currentCounter.ToString();
        isPlayer1Turn = (Random.Range(0, 2) == 0);
        
        // �� �^�[���J�n�������R���[�`���ɒu�������A���b�Z�[�W��\��
        StartCoroutine(ShowRoundStartMessage(2.0f));
    }
    
    IEnumerator ShowRoundStartMessage(float delay)
    {
        // ������ꎞ�I�Ƀ��b�N
        isClickingAllowed = false; 
        mainClickButton.interactable = false;
        passButton.interactable = false;
        
        // �� ���̏펞�\��UI���ꎞ�I�ɔ�\���ɂ���
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(false);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);


        // --- �� ���E���h�J�n�p�l����\�����A�e�L�X�g��ݒ� ---
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(true);
            if (roundStartNumberText != null)
            {
                roundStartNumberText.text = $"ROUND {currentRound}";
            }
        }
        

        // �� ���E���h���ƃX�R�A��\��
        turnIndicatorText.text = $"���E���h {currentRound} �J�n�I �X�R�A: {player1Score} - {player2Score}";

        // �ҋ@
        yield return new WaitForSeconds(delay);

        // --- �� ���E���h�J�n�p�l�����\���ɂ��� ---
        if (roundStartPanel != null)
        {
            roundStartPanel.SetActive(false);
        }
        
        // �� ���̏펞�\��UI���ĕ\������
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(true);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(true);
        if (scoreText != null) scoreText.gameObject.SetActive(true);


        // �ҋ@��Ɏ��ۂ̃^�[�����J�n
        StartTurn(); 
    }

    void StartTurn()
    {
        clicksMadeInTurn = 0;
        isClickingAllowed = true;
        mainClickButton.interactable = true;
        passButton.interactable = true;
        
        // ������UI��gameObject.SetActive(true)�͍s��Ȃ��B
        // ShowRoundStartMessage�̍Ō�ɍĕ\������邽�߁B

        // --- �^�[���C���W�P�[�^�[�̈ʒu���X�V ---
        float targetXTurn = isPlayer1Turn ? leftPositionX : rightPositionX;
        if (turnIndicatorRect != null)
        {
            turnIndicatorRect.anchoredPosition = new Vector2(targetXTurn, turnIndicatorRect.anchoredPosition.y); 
        }

        // --- ���E���h/�X�R�A�̈ʒu���X�V ---
        float targetXInfo = isPlayer1Turn ? infoRightX : infoLeftX;
        
        if (roundNumberRect != null)
        {
            roundNumberRect.anchoredPosition = new Vector2(targetXInfo, roundNumberRect.anchoredPosition.y);
        }

        if (scoreRect != null)
        {
            scoreRect.anchoredPosition = new Vector2(targetXInfo, scoreRect.anchoredPosition.y);
        }

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
        mainClickButton.interactable = false;
        passButton.interactable = false;
        ruleButton.interactable = false; // ���[���{�^����������

        // �A�C�e���⃋�[���p�l��������i�����J���Ă����ꍇ�j
        if (itemPanel.activeSelf) itemPanel.SetActive(false);
        if (rulePanel.activeSelf) rulePanel.SetActive(false);

        // �� �^�[���\���ƃ��E���h/�X�R�A�\�����\���ɂ���
        if (turnIndicatorText != null) turnIndicatorText.gameObject.SetActive(false);
        if (roundNumberText != null) roundNumberText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);


        if (playerWhoLost)
        {
            player2Score++;
        }
        else
        {
            player1Score++;
        }
        
        // �� �X�R�A���X�V
        if (scoreText != null)
        {
            scoreText.text = $"SCORE {player1Score} - {player2Score}";
        }

       // turnIndicatorText.text = $"���E���h�I���I �X�R�A: {player1Score} - {player2Score}";

        if (player1Score == 2 || player2Score == 2)
        {
            EndGame();
            return;
        }

       
        // --- �� ���E���h���ʃp�l���iYOU DIED��ʁj�̕\�����W�b�N ---
        // �s�k���b�Z�[�W�́A���E���h�I�����ɕ\������B
        if (gameOverPanel != null) //gameOverPanel��YOU DIED���
        {
            gameOverPanel.SetActive(true); // �p�l����\��

            string loserName = playerWhoLost ? "1P" : "2P"; // �����������v���C���[������
            string winnerName = playerWhoLost ? "2P" : "1P"; // ����v���C���[������

            // �e�L�X�g��ݒ�
            youDiedText.text = "YOU DIED";
            
            // 2. �� �����_���ȕ��������b�Z�[�W��I��
            string randomMessage = "";
            if (loserMessages.Length > 0)
            {
                int randomIndex = Random.Range(0, loserMessages.Length);
                randomMessage = loserMessages[randomIndex];
            } else
            {
                randomMessage = "�s�҂�A�Â��ɖ���B"; // ��₪�Ȃ��ꍇ�̃t�H�[���o�b�N
            }
            
            // 3. �������v���C���[���ƃ����_�����b�Z�[�W��\��
            loserMessageText.text = $"{randomMessage} ({loserName})"; 
            
            // ���������v���C���[����\��
            winnerText.text = $"{winnerName} WIN! (Round {currentRound})"; 

            // ���̃��E���h�ֈڍs
            StartCoroutine(WaitAndStartNextRound(3.0f)); 
        }
        else
        {
            // �p�l�����ݒ肳��Ă��Ȃ��ꍇ�̃t�H�[���o�b�N
            currentRound++;
            StartCoroutine(WaitAndStartNextRound(3.0f));
        }

    }

    IEnumerator WaitAndStartNextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        

        // �� YOU DIED�p�l�����\���ɂ���
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        currentRound++; // ���̃��E���h�֐i�߂�
        
        // �� �V�������E���h����\��
        if (roundNumberText != null)
        {
            roundNumberText.text = $"ROUND {currentRound}";
        }
        
        StartRound();
    }
    public void OnNextButtonClicked()
    {
        if (roundResultPanel != null)
        {
            roundResultPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // �p�l�����\���ɂ���
        }

        // �� �Q�[�������X�^�[�g����
        StartGame();

    }

    void EndGame()
    {  
        // �� �V���v���ȃX�R�A�p�l���iroundResultPanel�j�ōŏI���ʂ�\��
        if (roundResultPanel == null)
        {
            string winner = (player1Score == 2) ? "1P" : "2P";
            turnIndicatorText.text = $"Winner {winner}";
            return;
        }
        
        // --- �Q�[���I���p�l���i�V���v���ȃX�R�A��ʁj�̕\�����W�b�N ---

        roundResultPanel.SetActive(true); // �p�l����\��

        string finalWinnerName = (player1Score == 2) ? "1P" : "2P";

        // �e�L�X�g��ݒ�
        roundWinnerText.text = $"{finalWinnerName} WIN!";
        roundScoreText.text = $"SCORE {player1Score} - {player2Score}";
        
        // NOTE: �Q�[���I�����NEXT�{�^���iroundResultPanel���ɂ͂Ȃ��j���A�蓮�ł̃��X�^�[�g���K�v�ł��B
        // �iNEXT�{�^����gameOverPanel�ɂ̂ݎ�������Ă��邽�߁A���X�^�[�g�����͏ȗ����܂��j

    }
}