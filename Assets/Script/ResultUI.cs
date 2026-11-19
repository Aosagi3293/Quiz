using UnityEngine;
using UnityEngine.UIElements;

public class ResultUI : MonoBehaviour
{
    public static ResultUI Instance {get; private set;}

    private Label resultText;
    private Button reStartButton;
    private Button titleButton;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null) return;

        resultText = root.Q<Label>("ResultText");
        reStartButton = root.Q<Button>("ReStartButton");
        titleButton = root.Q<Button>("TitleButton");

        root.Q<Button>("ReStartButton").clicked += () =>
        {
            SEManager.Instance.ClickSound();    // クリックサウンド

            GameManager.Instance.ChangeGameMode(GameMode.Play);
        };

        root.Q<Button>("TitleButton").clicked += () =>
        {
            SEManager.Instance.ClickSound();    // クリックサウンド

            GameManager.Instance.ChangeGameMode(GameMode.Start);
        };
    }

    public void CreateLog()
    {
        var qm = QuestionManager.Instance;
        var gm = GameManager.Instance;

        int percent = qm.NumberOfQuestions > 0 
        ? (gm.answerCount * 100) / qm.NumberOfQuestions 
        : 0;

        resultText.text = ($"正解率は {percent}% でした");

        Debug.Log($"今回の正解は{qm.NumberOfQuestions}中、{gm.answerCount}問でした！");
    }
}
