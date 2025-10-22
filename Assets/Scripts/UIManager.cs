using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //added for image definition
public class UIManager : MonoBehaviour
{
    [SerializeField] private Sprite[] LifeImages = null;
    //hold all life pics
    [SerializeField] private Image LivesDisplay = null;

    private int score = 0;
    [SerializeField] private Text ScoreDisplay = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateLives(int count) //parameter formally 
    {
        LivesDisplay.sprite = LifeImages[count];
    }

    public void UpdateScore()
    {
        score += 10;
        ScoreDisplay.text = "Score: " + score; //update UI
    }

}

