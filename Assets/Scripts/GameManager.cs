using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public GameObject bot008Object;
    public GameObject enemyObject;
    public Material wireframeMaterial;
    public int playerDeathCount { set; get; }
    public int storyStatus;
    public TextMeshProUGUI pressRText;
    public Image ChatBackgroud;
    public TextMeshProUGUI chatText;
    public Image defeatBackgroud;
    public TextMeshProUGUI defeatText;
    public int wave;
    public bool pressR2Refresh;
    public int firstStoryHelperStatus;

    private PlayerController player;
    private Bot008Controller bot008;
    private bool isStartCoroutine;
    private int firstChatLastPage = 20;
    private int dialogStatus;
    private bool isDialogGoing;
    private bool isSpawning;
    
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
                float x = player.transform.rotation.z > 0 ? 2f : -2f;
                bot008 = Instantiate(bot008Object, player.transform.position + new Vector3(x, -1, 1.5f), Quaternion.Euler(0, 0, 0)).GetComponent<Bot008Controller>();
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
                //修改press文字
                if (chatText.pageToDisplay == firstChatLastPage - 3)
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
                //最后一页
                chatText.pageToDisplay = firstChatLastPage;
                storyStatus++;
                player.mainCamera.GetComponent<CameraManager>().ZoomOut();
            }
            else
            {
                chatText.pageToDisplay = firstChatLastPage - 1;
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
                ShowOnePageTextAndCountDown2Hide("Bot008:Look! It works! The body didn't disappear.I think you can use it.");
            }
            else if (firstStoryHelperStatus == 1)
            {
                CountDown4ShowOnPageText("Bot008: *Wisper* Well, by the way, you can press the space bar and step on their bodies and jump onto the wall.");
                firstStoryHelperStatus = 2;
            }
            else if (firstStoryHelperStatus == 2)
            {
                // 是否需要提示
                // 玩家已经出去了
                if (player.isOutFirstCircle)
                {
                    storyStatus = 10;
                    firstStoryHelperStatus = 0;
                }
            }
        }
        else if (storyStatus == 10)
        {
            bot008.transform.position = player.transform.position + new Vector3(0, -2, -1.5f);
            Debug.Log("bot008.transform.position=" + bot008.transform.position);
            storyStatus = 11;
        }
        else if (storyStatus == 11)
        {
            //008出现
            bool done = bot008.Do(1);
            if (done)
            {
                ShowOnePageTextAndCountDown2Hide("Bot008:Good job! Next, follow me.");
                storyStatus++;
                dialogStatus = 0;
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
            bot008.Do(101); //显示光环
            if (bot008.Do(10))
            {
                storyStatus++;
            }
            else if (!isDialogGoing)
            {
                string[] chatStrings = new[]
                {
                    "Bot008: Now let me explain to you, This program is a LOOP...",
                    "Bot008: If we want to break it, then we must find its jump out logic...",
                    "Bot008: Or let the STACK OVERFLOW directly!",
                    "Bot008: We just need to reduce the runnable space of the program",
                    "Bot008: And then we keep making enemies that won't be reset to devour memory",
                    "Bot008: Like... Dead robots",
                    "Bot008: But I don't have combat scripts, so...",
                    "Bot008: You are a very important part of the plan!",
                    "Bot008: do you dare to take the risk?",
                    "[ME]: ...",
                    "Bot008: I forgot you don't have language scripts... Let's JUST DO IT!"
                };
                if (chatStrings.Length > dialogStatus)
                {
                    ShowOnePageTextAndCountDown2Hide(chatStrings[dialogStatus]);
                    dialogStatus++;
                }
            }
        }
        else if (storyStatus == 13)
        {
            bot008.Do(100); //隐藏光环
            if (!isDialogGoing)
            {
                ShowOnePageTextAndCountDown2Hide("Bot008:There it is!");
                storyStatus++;
            }
        }
        else if (storyStatus == 14)
        {
            if (!isDialogGoing)
            {
                ShowOnePageTextAndCountDown2Hide("Bot008:Let me make a little bit of a change...");
                storyStatus++;
            }
        }
        else if (storyStatus == 15)
        {
            if (!isDialogGoing)
            {
                //显示BUG球
                GameObject closet = bot008.GetComponent<Bot008Controller>().getClosetMemoryBug();
                closet.transform.position += new Vector3(0, 5f, 0);
                ShowOnePageTextAndCountDown2Hide("Now, get inside of it!");
                storyStatus = 30;
            }
        }
        else if (storyStatus == 30)
        {
            //等待玩家进入BUG
        }

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

    private void ShowOnePageTextAndCountDown2Hide(string text)
    {
        StartCoroutine(ShowOnePageTextAndCountDown2HideCorotine(text));
    }

    private void CountDown4ShowOnPageText(string text)
    {
        StartCoroutine(CountDown4ShowOnPageTextCorotine(text));
    }

    /**
     * 显示文字，倒数后消失
     */
    IEnumerator ShowOnePageTextAndCountDown2HideCorotine(string text)
    {
        isDialogGoing = true;
        ShowTextOnePage(text);
        yield return new WaitForSeconds(5);
        HideText();
        isDialogGoing = false;
    }

    /**
     * 一段时间后玩家没有出圈则提示
     */
    IEnumerator CountDown4ShowOnPageTextCorotine(string text)
    {
        yield return new WaitForSeconds(30);
        //玩家依然在圈内
        if (firstStoryHelperStatus == 2)
        {
            ShowTextOnePage(text);
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
            case 0: //游戏开始
            case 5: //打怪逃出
            case 12: //NPC前往故事地点 玩家跟随
            case 13:
            case 14:
            case 15:
            case 30: //等待玩家进入BUG
                return false;
            default:
                return true;
        }
    }

    IEnumerator ChangePressR()
    {
        isStartCoroutine = true;
        string addString = "r;e;s;e;t";
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
        chatText.pageToDisplay = firstChatLastPage - 2;
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
        isSpawning = true;
        yield return new WaitForSeconds(1);
        if (isAllEnemyDead())
        {
            wave++;
            SpawnEnemy(wave);
        }

        isSpawning = false;
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
            Instantiate(enemyObject, new Vector3(0, 1, 15), Quaternion.Euler(0, 180, 0));
        }
    }

    bool isAllEnemyDead()
    {
        GameObject[] allEnemys = GameObject.FindGameObjectsWithTag("Enemy Object");
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
        GameObject[] allEnemys = GameObject.FindGameObjectsWithTag("Enemy Object");
        foreach (GameObject enemy in allEnemys)
        {
            EnemyController script = enemy.GetComponent<EnemyController>();
            if (clearBody || !script.isDead)
            {
                //清除所有敌人 or 清除活着的敌人
                Destroy(enemy);
            }
        }
    }
}