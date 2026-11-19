using UnityEngine;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    public List<QuestionStatus> questions = new List<QuestionStatus>();

    [SerializeField] private QuestionSO questionSO;

    public int NumberOfQuestions = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Reset()
    {
        var questionsList = questionSO.questions;
        var maxCount = questionsList.Count;
        int choiceId;

        // listの中身をリセット
        questions.Clear();

        // 登録問題数が足りない場合の無限ループ防止
        int targetCount = Mathf.Min(NumberOfQuestions, maxCount);

        // 指定数の問題を作成する
        for(int i = 0; i < targetCount; i++)
        {
            choiceId = 1 + Random.Range(0, maxCount);

            foreach (QuestionStatus ql in questionsList)
            {
                if(ql.id == choiceId)
                {
                    // 追加する前に、すでに同じ問題がリストに入っているかチェックする
                    bool isAlreadyAdded = false;
                    foreach (QuestionStatus q in questions)
                    {
                        if (q.id == ql.id)
                        {
                            isAlreadyAdded = true;
                            break;
                        }
                    }
                    
                    if (isAlreadyAdded) i--;    // 既に同じ問題があったなら追加せず、もう一度回す
                    else questions.Add(ql);     // 重複していなければリストに追加する

                    break; // 見つかったら無駄な検索を終わらせる
                }
            }
        }
    }

    public void QuestionCreation()
    {
        var playUI = PlayUI.Instance;
        // 問題文をUIに出力
        playUI.CreateQuestion();

        // 回答択をシャッフル
        ShuffleArray(questions[0].answerData);

        // 回答択をUIに出力
        playUI.CreateAnswers();
    }

    // 配列のシャッフル
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
