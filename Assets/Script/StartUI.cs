using UnityEngine;
using UnityEngine.UIElements;

public class StartUI : MonoBehaviour
{
    public static StartUI Instance {get; private set;}

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

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("StartButton").clicked += () =>
        {
            SEManager.Instance.ClickSound();    // クリックサウンド

            GameManager.Instance.ChangeGameMode(GameMode.Play);
        };
    }
}
