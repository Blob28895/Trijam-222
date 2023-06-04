using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Scoring : MonoBehaviour
{
    [System.Serializable]
    public class PointThreshold
    {
        public float percentage;
        public int score;
    }
    [SerializeField] private PointThreshold[] pointThresholds;
    public TextMeshProUGUI scoreInt;

    private int score = 0;

    public void updateScore(float paintedPercentage)
    {
        addWallScore(paintedPercentage);

        scoreInt.text = score.ToString();
    }

    private void addWallScore(float paintedPercentage)
    {
        for (int i = pointThresholds.Length - 1; i > -1; --i)
        {
            if (paintedPercentage >= pointThresholds[i].percentage)
            {
                score += pointThresholds[i].score;
                break;
            }
        }
    }
}
