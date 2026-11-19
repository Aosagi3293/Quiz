using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Question")]
public class QuestionSO : ScriptableObject
{
    public List<QuestionStatus> questions = new List<QuestionStatus>();

    // 回答の最大数を４つに固定する
    private void OnValidate()
    {
        foreach (var question in questions)
        {
            if (question.answerData == null)
            {
                question.answerData = new AnswerData[4];
            }
            else if (question.answerData.Length != 4)
            {
                System.Array.Resize(ref question.answerData, 4);
            }
        }
    }
}

[System.Serializable]
public class QuestionStatus
{
    public int id;
    public string question; // 問題文
    public AnswerData[] answerData = new AnswerData[4];
}

[System.Serializable]
public class AnswerData
{
    public string answer;   // 回答
    public bool isAnswer;   // 正解か否か
}