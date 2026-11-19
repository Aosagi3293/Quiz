using UnityEditor;

[CustomEditor(typeof(QuestionSO))]
public class QuestionSOEditor : Editor
{
    private int lastCount = -1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty questionsProp = serializedObject.FindProperty("questions");

        // 初回表示時、または「＋」ボタンでリストの要素数が増えたとき
        if (lastCount == -1 || questionsProp.arraySize > lastCount)
        {
            questionsProp.isExpanded = true;

            for (int i = 0; i < questionsProp.arraySize; i++)
            {
                // 初回表示時は展開のみ、追加時（lastCount != -1 かつ新規要素）はリセット＋展開
                if (lastCount == -1 || i >= lastCount)
                {
                    SerializedProperty qElement = questionsProp.GetArrayElementAtIndex(i);
                    qElement.isExpanded = true;

                    SerializedProperty answerProp = qElement.FindPropertyRelative("answerData");
                    answerProp.isExpanded = true;

                    // 「＋」ボタンで新しく追加された要素の場合、中身をリセットする
                    if (lastCount != -1 && i >= lastCount)
                    {
                        qElement.FindPropertyRelative("id").intValue = 0;
                        qElement.FindPropertyRelative("question").stringValue = "";
                        answerProp.arraySize = 4; // 4つに固定
                    }

                    // 4つの選択肢を展開（新規追加時は中身もリセット）
                    for (int j = 0; j < answerProp.arraySize; j++)
                    {
                        SerializedProperty ansElement = answerProp.GetArrayElementAtIndex(j);
                        ansElement.isExpanded = true;

                        if (lastCount != -1 && i >= lastCount)
                        {
                            ansElement.FindPropertyRelative("answer").stringValue = "";
                            ansElement.FindPropertyRelative("isAnswer").boolValue = false;
                        }
                    }
                }
            }
        }

        // 現在の要素数を更新
        lastCount = questionsProp.arraySize;

        // 変更（リセット処理）を反映させてから標準のインスペクターを描画
        serializedObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }
}
