using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public enum PlayingType
{
    None,
    Question,
    Answer
}

public class PlayUI : MonoBehaviour
{
    public static PlayUI Instance { get; private set; }

    private PlayingType playingType = PlayingType.None;

    private float totalTime;
    private float questionTime = 10f;
    private float answerTime = 5f;

    private CircularTimerElement _circularTimer;

    private Label titleText;
    private Label questionText;
    private Button[] answerButtons = new Button[4];
    
    private float _currentTime;
    private bool _isTimerRunning = false;
    private bool isAnswered = false;

    // ログを出す秒数リスト
    private readonly HashSet<int> Seconds = new HashSet<int> { 3, 2, 1 };
    // 既にログを出した秒数を記録
    private readonly HashSet<int> _Seconds = new HashSet<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null) return;

        _circularTimer = root.Q<CircularTimerElement>("Timer");
        titleText = root.Q<Label>("QuestionTitle");
        questionText = root.Q<Label>("QuestionText");

        answerButtons[0] = root.Q<Button>("Answer1");
        answerButtons[1] = root.Q<Button>("Answer2");
        answerButtons[2] = root.Q<Button>("Answer3");
        answerButtons[3] = root.Q<Button>("Answer4");

        // 再生成された新しいButtonなので、重複登録の心配なくそのまま登録できる
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[index] != null)
            {
                answerButtons[index].clicked += () =>
                {
                    OnAnswerClicked(index);
                };
            }
        }
    }

    private void Start()
    {
        if (_circularTimer == null)
        {
            Debug.LogError("CircularTimerElementが見つかりません");
            return;
        }
        _circularTimer.Progress = 1f;
    }

    public void BeginPlaying()
    {
        playingType = PlayingType.Question;
        ChangePlayingType(playingType);

        if (_circularTimer != null)
        {
            _circularTimer.Progress = 1f;
        }
    }

    private void Update()
    {
        if (!_isTimerRunning || _circularTimer == null) return;

        _currentTime -= Time.deltaTime;
        _currentTime = Mathf.Max(_currentTime, 0f);

        _circularTimer.Progress = _currentTime / totalTime;

        // カウントダウンログ
        int currentSecond = Mathf.CeilToInt(_currentTime);
        if (Seconds.Contains(currentSecond) && !_Seconds.Contains(currentSecond))
        {
            SEManager.Instance.TimerSound();    // タイマーのサウンド
            _Seconds.Add(currentSecond);
        }

        if (_currentTime <= 0f)
        {
            _isTimerRunning = false;
            
            if(playingType == PlayingType.Question) Debug.Log("タイムアップ！");
            else if(playingType == PlayingType.Answer) Debug.Log("次の問題へ");

            playingType = (playingType == PlayingType.Question)
            ? PlayingType.Answer
            : PlayingType.Question;

            ChangePlayingType(playingType);
        }
    }

    private void ChangePlayingType(PlayingType type)
    {
        _Seconds.Clear();

        if (type == PlayingType.Question)
        {
            if (QuestionManager.Instance.questions.Count > 0)
            {
                totalTime = Mathf.Max(questionTime, 0.01f);
                _currentTime = totalTime;
                _isTimerRunning = true;

                titleText.text = "問題";

                // 問題更新
                QuestionManager.Instance.QuestionCreation();

                isAnswered = false;
            }
            else
            {
                _isTimerRunning = false;
                playingType = PlayingType.None;

                GameManager.Instance.ChangeGameMode(GameMode.Result);
                Debug.Log("問題がありません（全問終了）");
                return;
            }
        }

        if(type == PlayingType.Answer)
        {
            if (QuestionManager.Instance.questions.Count == 0) return;

            totalTime = Mathf.Max(answerTime, 0.01f);
            _currentTime = totalTime;
            _isTimerRunning = true;

            titleText.text = "正解は…";

            // 回答表示
            CreateAnswer();

            // listから０番目の要素を削除
            QuestionManager.Instance.questions.RemoveAt(0);
        }
    }

    // 問題文をUI表示
    public void CreateQuestion()
    {
        var qm = QuestionManager.Instance.questions[0];
        questionText.text = qm.question;
    }

    // 回答表示
    public void CreateAnswer()
    {
        QuestionManager qm = QuestionManager.Instance;
        string[] labels = { "A", "B", "C", "D" };

        for (int i = 0; i < qm.questions[0].answerData.Length; i++)
        {
            if (!qm.questions[0].answerData[i].isAnswer) continue;

            string num = (i < labels.Length) ? labels[i] : "";
            questionText.text = $"{num}. {qm.questions[0].answerData[i].answer}";

            break;
        }

        SEManager.Instance.AnswerShowSound();    // 回答表示時のサウンド
    }

    // 選択肢をUI表示
    public void CreateAnswers()
    {
        ResetAnswerUI();
        
        var qm = QuestionManager.Instance.questions[0];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].text = qm.answerData[i].answer;

            Debug.Log($"回答： {qm.answerData[i].answer}");
        }
    }

    // ボタンが押されたときの処理
    private void OnAnswerClicked(int buttonIndex)
    {
        if(isAnswered) return;
        if(playingType != PlayingType.Question) return;

        var qm = QuestionManager.Instance.questions[0];
        bool isCorrect = qm.answerData[buttonIndex].isAnswer;
        isAnswered = true;

        if(isCorrect) GameManager.Instance.AnswerCount();

        ChoiceAnswerUI(buttonIndex);

        SEManager.Instance.ClickSound();    // クリックサウンド

        Debug.Log($"ボタン {buttonIndex + 1} が押されました（正解かどうか: {isCorrect}）");
    }

    // 回答の選択がされたことを知らせるUIの更新
    public void ChoiceAnswerUI(int buttonIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if(i != buttonIndex)
            {
                answerButtons[i].style.opacity = 0.5f;  // 半透明
            }
        }
    }

    // 回答欄のUIをリセット
    public void ResetAnswerUI()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].style.opacity = 1f;  // 表示
        }
    }
}
