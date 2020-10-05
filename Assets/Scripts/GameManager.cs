using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public GameObject bot008Object;
    public GameObject enemyObject;
    private PlayerController player;
    private Bot008Controller bot008;
    public int playerDeathCount { set; get; }
    public int storyStatus;
    public TextMeshProUGUI pressRText;
    public Image ChatBackgroud;
    public TextMeshProUGUI chatText;
    public Image defeatBackgroud;
    public TextMeshProUGUI defeatText;
    public int wave;
    public bool pressR2Refresh;

    private bool isStartCoroutine;

    private List<GameObject> allEnemys = new List<GameObject>();

    private int firstChatLastPage = 17;

    public int firstStoryHelperStatus;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        //隐藏鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private bool StoryGoOn()
    {
        //开始进入剧情
        if (storyStatus == 0)
        {
            if (playerDeathCount == 3)
            {
                storyStatus = 1;
                return true;
            }
            return false;
        }
        else if (storyStatus == 1)
        {
            //剧情NPC出现
            if (bot008 == null)
            {
                float z = player.transform.rotation.z > 0 ? -5f : 5f;
                bot008 = Instantiate(bot008Object, player.transform.position + new Vector3(0, -1, z), Quaternion.Euler(0, 0, 0)).GetComponent<Bot008Controller>();
                bot008.player = this.player;
            }
            bool done = bot008.Do(1);
            if (done)
            {
                pressR2Refresh = true;
                storyStatus++;
                player.mainCamera.GetComponent<CameraManager>().ZoomOut();
            }
            else
            {
                player.mainCamera.GetComponent<CameraManager>().ZoomIn();
                player.mainCamera.transform.LookAt(bot008.transform);
            }
        }
        else if (storyStatus == 2)
        {
            //对话
            //ChatBackgroud.gameObject.SetActive(true);
            chatText.gameObject.SetActive(true);
            bool leftMouse = Input.GetButtonDown("Fire1");
            if (leftMouse && !isStartCoroutine)
            {
                chatText.pageToDisplay++;
                bot008.talkAction();
                if (chatText.pageToDisplay == firstChatLastPage - 2)
                {
                    StartCoroutine(ChangePressR());
                }
            }
        }
        else if (storyStatus == 3)
        {
            // NPC钻入地下
            bool done = bot008.Do(3);
            if (done)
            {
                //下一页
                chatText.pageToDisplay = firstChatLastPage;
                storyStatus++;
                player.mainCamera.GetComponent<CameraManager>().ZoomOut();
            }
            else
            {
                player.mainCamera.GetComponent<CameraManager>().ZoomIn();
                player.mainCamera.transform.LookAt(bot008.transform);
            }
        } 
        else if (storyStatus == 4)
        {
            //按R键继续
            if (Input.GetKeyDown(KeyCode.R))
            {
                HideText();
                storyStatus++;
                player.pressR(false);
            }
        }
        else if (storyStatus == 5)
        {
            // 标识已进行提示 防止循环StartCoroutine
            if (firstStoryHelperStatus == 0)
            {
                firstStoryHelperStatus = 1;
                StartCoroutine(firstStoryHelper2());
            }
            else if (firstStoryHelperStatus == 1)
            {
                StartCoroutine(firstStoryHelper1());
                firstStoryHelperStatus = 2;
            }
            else if (firstStoryHelperStatus == 2)
            {
                // 是否需要提示
                // 玩家已经出去了
                if (player.isOutFirstCircle)
                {
                    storyStatus = 10;
                }
            }
        }
        else if (storyStatus == 10)
        {
            float z = player.transform.rotation.z > 0 ? -5f : 5f;
            bot008.transform.position = player.transform.position + new Vector3(0, -1, z);
            Debug.Log("bot008.transform.position=" + bot008.transform.position);
            storyStatus = 11;
        }
        else if (storyStatus == 11)
        {
            //008出现
            bool done = bot008.Do(1);
            if (done)
            {
                ShowTextOnePage("Bot008:Good job! Next, let's find the bug of this loop! follow me.");
                storyStatus++;
                player.mainCamera.GetComponent<CameraManager>().ZoomOut();
            }
            else
            {
                player.mainCamera.GetComponent<CameraManager>().ZoomIn();
                player.mainCamera.transform.LookAt(bot008.transform);
            }
        }
        else if (storyStatus == 12)
        {
            //008前往故事地点
            bot008.Do(101);
            if (bot008.Do(10))
            {
                storyStatus++;
            }
        }
        else if (storyStatus == 13)
        {
            //008到达故事地点1
            bot008.Do(100);
            ShowTextOnePage("Bot008:Here it is!");
            storyStatus++;
        }
        else if (storyStatus == 14)
        {
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 15)
        {
            //008到达故事地点2
            ShowTextOnePage("Bot008:Let me make a little bit of a change...");
            storyStatus++;
        }
        else if (storyStatus == 16)
        {
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 17)
        {
            //显示BUG
            GameObject closet = bot008.GetComponent<Bot008Controller>().getClosetMemoryBug();
            closet.SetActive(true);
            storyStatus++;
        }
        //TODO 检测玩家是否触发

        return false;
    }

    private void WaitingPlayerClickLeftMouse()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            HideText();
            storyStatus++;
        }
    }

    private void ShowTextOnePage(string text)
    {
        ChatBackgroud.gameObject.SetActive(true);
        chatText.gameObject.SetActive(true);
        chatText.SetText(text);
        chatText.pageToDisplay = 1;
    }

    private void HideText()
    {
        ChatBackgroud.gameObject.SetActive(false);
        chatText.gameObject.SetActive(false);
    }

    /**
     * 第一次重新开始后的提示
     */
    IEnumerator firstStoryHelper1()
    {
        ShowTextOnePage("Bot008:Look! It works! The body didn't disappear.I think you can use it.");
        yield return new WaitForSeconds(3);
        HideText();
    }
    
    /**
     * 一段时间后玩家没有出圈则提示
     */
    IEnumerator firstStoryHelper2()
    {
        yield return new WaitForSeconds(60);
        if (firstStoryHelperStatus == 1)
        {
            ShowTextOnePage("Bot008: *Wisper* Well, by the way, you can press the space bar and step on their bodies and jump onto the wall.");
            storyStatus = 6;
            yield return new WaitForSeconds(5);
            HideText();
        }
    }
    
    /**
     * 标识玩家可以行动的故事状态
     */
    public bool isStoryGoing()
    {
        switch (storyStatus)
        {
            case 0:    //游戏开始
            case 5:    //打怪逃出
            case 12:   //NPC前往故事地点
                return false;
            default:
                return true;
        }
    }

    IEnumerator ChangePressR()
    {
        isStartCoroutine = true;
        string addString = "s;t;a;r;t";
        string[] addStringArr = addString.Split(';');
        int deleteTextNumber = 0;
        int addTextNumber = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            Debug.Log("deleteTextNumber=" + deleteTextNumber + " \t addTextNumber=" + addTextNumber);
            string text = pressRText.text;
            if (deleteTextNumber < 7)
            {
                pressRText.SetText(text.Substring(0, text.Length - 1));
                deleteTextNumber++;
            }
            else
            {
                pressRText.SetText(text + addStringArr[addTextNumber]);
                addTextNumber++;
            }

            if (addTextNumber == addStringArr.Length)
            {
                pressRText.color = new Color(0.2f, 0.2f, 0.63f);
                break;
            }
        }
        isStartCoroutine = false;
        chatText.pageToDisplay = firstChatLastPage - 1;
        storyStatus = 3;
    }

    // Update is called once per frame
    void Update()
    {
        //剧情控制
        if (StoryGoOn())
        {
            return;
        }

        //死亡画面
        if (player.isDead)
        {
            defeatBackgroud.gameObject.SetActive(true);
            defeatText.gameObject.SetActive(true);
        }
        else
        {
            defeatBackgroud.gameObject.SetActive(false);
            defeatText.gameObject.SetActive(false);
        }
        
        //玩家死亡
        if (player.isDead || storyStatus >= 3)
        {
            pressRText.gameObject.SetActive(true);
        }
        else
        {
            pressRText.gameObject.SetActive(false);
        }

        //波次刷新
        StartCoroutine(CheckAndSpawnEnemy());
    }

    IEnumerator CheckAndSpawnEnemy()
    {
        yield return new WaitForSeconds(1);
        if (isAllEnemyDead())
        {
            wave++;
            SpawnEnemy(wave);
        }
    }
    
    public void ResetWave(bool clearBody)
    {
        EnemyReset(clearBody);
        wave = 1;
        SpawnEnemy(wave);
    }

    private void SpawnEnemy(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject newEnemy = Instantiate(enemyObject, new Vector3(0, 1, 15), Quaternion.Euler(0, 180, 0));
            allEnemys.Add(newEnemy);
        }
    }

    bool isAllEnemyDead()
    {
        if (allEnemys == null || allEnemys.Count <= 0)
        {
            return true;
        }

        foreach (GameObject enemy in allEnemys)
        {
            if (!enemy.GetComponent<EnemyController>().isDead)
            {
                return false;
            }
        }

        return true;
    }

    public void EnemyReset(bool clearBody)
    {
        foreach (GameObject enemy in allEnemys)
        {
            EnemyController script = enemy.GetComponent<EnemyController>();
            if (!script.isDead)
            {
                enemy.SetActive(false);
                script.isDead = true;
            }
            else if (clearBody && script.isDead)
            {
                enemy.SetActive(false);
            }
        }
    }
}