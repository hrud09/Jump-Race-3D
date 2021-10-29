using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<string> nameList;
    public int playerCount;
    public Animator playerAnim;
    public GameObject gameOverPanel;
    public RectTransform playerLeaderBoard;
    public GameObject leaderBoardListItem;
    public Text winMessage;
    public GameObject instruction;


    //Rewarded Section
    public GameObject[] rewardText;
    public RectTransform rewardTextPos;

    public int numberOfTotalPlayer;
    public bool gameStarted;

    //PlatformCount
    public Transform countTextParent;
    public TMP_Text[] countText;
    [SerializeField] LineRenderer line;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        countText = countTextParent.GetComponentsInChildren<TMP_Text>();
        line.positionCount = countText.Length;
        for (int i = 0; i < countText.Length; i++)
        {
            countText[i].text = (countText.Length - i).ToString();
            StartCoroutine(DrawLine(i));
        }
     
    }

    IEnumerator DrawLine(int index)
    {
        yield return new WaitForSeconds(0.2f);
        line.SetPosition(index, countText[index].transform.parent.parent.parent.transform.position);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            gameStarted = true;
            instruction.SetActive(false);
        }
    }

    public void SetPlayer(bool isPlayer,bool isDead=false)
    {
        if (isPlayer && !isDead)
        {
            playerCount++;
            Player bot = new Player();
            bot.Position = playerCount;
            bot.Name = "You";
            leaderBoardListItem.transform.GetChild(0).GetComponent<Text>().text = bot.Position + " :   " + bot.Name;
            Instantiate(leaderBoardListItem, playerLeaderBoard);
            for (int i = 0; i < numberOfTotalPlayer; i++)
            {
                RegisterBot();
            }           
            GameOver(bot.Position);          
        }
        else if (isPlayer && isDead)
        {
           
            for (int i = 0; i < numberOfTotalPlayer; i++)
            {

                RegisterBot();
            }
            playerCount++;
            Player bot = new Player();
            bot.Position = playerCount;
            bot.Name = "You";
            leaderBoardListItem.transform.GetChild(0).GetComponent<Text>().text = bot.Position + " :   " + bot.Name;         
            Instantiate(leaderBoardListItem, playerLeaderBoard);
            GameOver(bot.Position);
        }
        else
        {
            numberOfTotalPlayer--;
            RegisterBot();
        }  

    }


    private void RegisterBot()
    {
        playerCount++;
        Player bot = new Player();
        bot.Position = playerCount;
        int r = Random.Range(0, nameList.Count);
        bot.Name = nameList[r];
        nameList.Remove(nameList[r]);
        leaderBoardListItem.transform.GetChild(0).GetComponent<Text>().text = bot.Position + " :   " + bot.Name;
        Instantiate(leaderBoardListItem, playerLeaderBoard);
    }


    GameObject reward;
    public void ShowReward(int index)
    {
        if (reward)
        {
            Destroy(reward);
        }
        reward = Instantiate(rewardText[index],rewardTextPos);
        Destroy(reward, 1);
    }


    public void GameOver(int position)
    {
        if (position==1)
        {
            playerAnim.Play("Winner");
            winMessage.text = "Winner!";
        }
        else
        {
            playerAnim.Play("Defeat");
            winMessage.text = "Defeat!";
        }
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < 2) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
       
    }
}

[System.Serializable]
public class Player
{
    public string Name;
    public int Position;
}
