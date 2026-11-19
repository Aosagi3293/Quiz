using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CircularTimerElement : VisualElement
{
    private float _progress = 1.0f;

    [UxmlAttribute("progress")]
    public float Progress
    {
        get => _progress;
        set
        {
            _progress = Mathf.Clamp01(value);
            MarkDirtyRepaint();
        }
    }

    public CircularTimerElement()
    {
        generateVisualContent += OnGenerateVisualContent;

        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.translate = new StyleTranslate(
            new Translate(Length.Percent(-50f), Length.Percent(-50f))
        );

        style.width = 200;
        style.height = 200;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;

        float width = contentRect.width;
        float height = contentRect.height;

        float radius = Mathf.Min(width, height) / 2f;
        Vector2 center = new Vector2(width / 2f, height / 2f);

        if (radius <= 0) return;

        // 1. 背景の灰色の円
        painter.fillColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        DrawSector(painter, center, radius, 0f, 360f, 100);

        // 2. 残り時間の緑色のゲージ
        if (_progress > 0f)
        {
            painter.fillColor = Color.green;

            // 消費した分だけ開始位置を時計回りに移動
            float startAngle = -90f + 360f * (1f - _progress);

            // 残り時間分の緑色を描画
            float sweepAngle = 360f * _progress;

            int steps = Mathf.Max(1, Mathf.CeilToInt(100f * _progress));

            DrawSector(
                painter,
                center,
                radius,
                startAngle,
                sweepAngle,
                steps
            );
        }
    }

    // 円形の扇形を描画する
    private void DrawSector(
        Painter2D painter,
        Vector2 center,
        float radius,
        float startAngle,
        float sweepAngle,
        int steps)
    {
        painter.BeginPath();

        // 中心から開始
        painter.MoveTo(center);

        for (int i = 0; i <= steps; i++)
        {
            float angle = (startAngle + sweepAngle * i / steps) * Mathf.Deg2Rad;

            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            painter.LineTo(new Vector2(x, y));
        }

        painter.ClosePath();
        painter.Fill();
    }
}