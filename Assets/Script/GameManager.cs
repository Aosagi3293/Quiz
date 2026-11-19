using UnityEngine;
using UnityEngine.UIElements;

public enum GameMode
{
    None,
    Start,
    Play,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIDocument startDocument;
    [SerializeField] private UIDocument playDocument;
    [SerializeField] private UIDocument resultDocument;

    public GameMode gameMode = GameMode.Start;

    private VisualElement startRoot;
    private VisualElement playRoot;
    private VisualElement resultRoot;

    public int answerCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        startRoot = startDocument.rootVisualElement;
        playRoot = playDocument.rootVisualElement;
        resultRoot = resultDocument.rootVisualElement;

        ChangeGameMode(GameMode.Start);
    }

    private void Reset()
    {
        answerCount = 0;
    }

    public void AnswerCount()
    {
        answerCount++;
    }

    public void ChangeGameMode(GameMode newGameMode)
    {
        gameMode = newGameMode;

        SetVisible(startRoot, gameMode == GameMode.Start);
        SetVisible(playRoot, gameMode == GameMode.Play);
        SetVisible(resultRoot, gameMode == GameMode.Result);

        // UIの表示を切り替えた後、明示的にゲームを開始する
        if (gameMode == GameMode.Play)
        {
            Reset();
            QuestionManager.Instance.Reset();

            PlayUI.Instance.BeginPlaying();
        }

        if(gameMode == GameMode.Result) ResultUI.Instance.CreateLog();
    }

    private void SetVisible(VisualElement root, bool visible)
    {
        if (root == null) return;

        root.style.display = visible
            ? DisplayStyle.Flex
            : DisplayStyle.None;
    }
}