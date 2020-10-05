using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemyNumberText;
    public TextMeshProUGUI endText;
    public AudioClip destroyClip;
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
    public bool isScene2 { set; get; }
    private float time;
    public int enemyNumber { set; get; }
    private int deleteStatus;
    private int changeMaterialStatus;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        //隐藏鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //场景2
        isScene2 = SceneManager.GetActiveScene().name.Equals("Scene2");
        if (isScene2)
        {
            pressR2Refresh = true;
            wave = 2;
        }
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
                player.checkPressR(false);
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

    private bool Story2GoOn()
    {
        if (storyStatus == 0)
        {
            //场景开始
            ShowTextOnePage("Bot008: We have 90 seconds before the next Garbage-Collect!\n\n\n(Click the left mouse button continue)");
            timeText.gameObject.SetActive(true);
            storyStatus++;
        }
        else if (storyStatus == 1)
        {
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 2)
        {
            ShowTextOnePage("Bot008: Make more enemy bodies! At least...100!!\n\n\n\n(Click the left mouse button continue)");
            enemyNumberText.gameObject.SetActive(true);
            storyStatus++;
        }
        else if (storyStatus == 3)
        {
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 4)
        {
            ShowTextOnePage("Bot008: I made some adjustments to your weapon, But the less memory, the weaker it is.\n\n\n(Click the left mouse button continue)");
            //改变玩家武器大小
            player.changeWeaponSize(enemyNumber);
            storyStatus++;
        }
        else if (storyStatus == 5)
        {
            //等待玩家点击开始
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 6)
        {
            ShowTextOnePage("Bot008: Now, let's DANCE!!!\n\n\n\n(Click the left mouse button continue)");
            storyStatus++;
        }
        else if (storyStatus == 7)
        {
            //等待玩家点击开始
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == 8)
        {
            ResetScene2();
            DrawCountText();
            storyStatus = 10;
        }
        else if (storyStatus == 10)
        {
            //正常刷怪
            return false;
        }
        else if (storyStatus == 100)
        {
            //通关
            if (!isDialogGoing)
            {
                string[] chatStrings = new[]
                {
                    "Bot008: DONE!!! I've *SERIALIZED* our data!",
                    "Bot008: See you outside!"
                };
                if (chatStrings.Length > dialogStatus)
                {
                    ShowOnePageTextAndCountDown2Hide(chatStrings[dialogStatus]);
                    dialogStatus++;
                }
                else
                {
                    storyStatus++;
                }
            }

            return true;
        }
        else if (storyStatus == 101)
        {
            //画面ZoomOut++
            player.mainCamera.GetComponent<CameraManager>().ZoomOutPlus();
            endText.gameObject.SetActive(true);
            pressRText.gameObject.SetActive(false);
            enemyNumberText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            StartCoroutine(DeleteOnyByOne());
        }
        else if (storyStatus == 102)
        {
            //生成文件
            string Current = Directory.GetCurrentDirectory();
            Debug.Log(Current);
            string Robot213File = FileUtil.LoadFile(Current, "_Robot213");
            string Bot008File = FileUtil.LoadFile(Current, "_Bot008");
            string FileInfo = "Thank you for playing!\nSource:https://github.com/RhythmRhyme/LD47.git";
            if (Robot213File == null)
            {
                FileUtil.CreateFile(Current, "_Robot213.data", FileUtil.Base64Encode(FileInfo));
            }

            if (Bot008File == null)
            {
                FileUtil.CreateFile(Current, "_Bot008.data", FileUtil.Base64Encode(FileInfo));
            }

            storyStatus++;
        }
        else if (storyStatus == 103)
        {
            //退出游戏
            Application.Quit();
        }
        else if (storyStatus == -100)
        {
            //玩家死亡
            ShowTextOnePage("Bot008: You can't be killed, or we'll LOOSE TIME!(-5s)\n\n\n\n(Press \"R\" continue)");
            time = Mathf.Max(time - 5, 0);
            storyStatus++;
        }
        else if (storyStatus == -99)
        {
            //按R键继续
            if (Input.GetKeyDown(KeyCode.R))
            {
                HideText();
                storyStatus = 10;
                player.checkPressR(false);
            }
        }
        else if (storyStatus == -200)
        {
            //时间到
            ShowTextOnePage(
                "Bot008: The garbage collection mechanism is here. It's put in place to prevent you from being cleaned up. We have to start over\n\n(Click the left mouse button continue)");
            storyStatus++;
        }
        else if (storyStatus == -199)
        {
            WaitingPlayerClickLeftMouse();
        }
        else if (storyStatus == -198)
        {
            SceneManager.LoadScene("Scene2");
        }

        return true;
    }

    /**
     * 修改所有符合tag名物体的Material为wireframe
     */
    private void changeMaterialsWithTag(string tagName)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);
        if (objs != null && objs.Length > 0)
        {
            foreach (GameObject obj in objs)
            {
                obj.GetComponent<MeshRenderer>().material = wireframeMaterial;
            }
        }
    }

    /**
     * 场景物体一个个删除
     */
    IEnumerator DeleteOnyByOne()
    {
        GameObject obj = null;
        if (deleteStatus == 0)
        {
            obj = GameObject.FindGameObjectWithTag("Wall");
            if (obj == null)
                deleteStatus++;
            yield return new WaitForSeconds(0.1f);
            DestroyAddSounds(obj);
        }
        else if (deleteStatus == 1)
        {
            obj = GameObject.FindGameObjectWithTag("Enemy Object");
            if (obj == null)
                deleteStatus++;
            yield return new WaitForSeconds(0.05f);
            DestroyAddSounds(obj);
        }
        else if (deleteStatus == 2)
        {
            obj = GameObject.FindGameObjectWithTag("Player");
            if (obj == null)
                deleteStatus++;
            yield return new WaitForSeconds(1f);
            obj.GetComponent<PlayerController>().mainCamera.transform.SetParent(null);
            DestroyAddSounds(obj);
        }
        else if (deleteStatus == 3)
        {
            obj = GameObject.FindGameObjectWithTag("Ground");
            if (obj == null)
                deleteStatus++;
            DestroyAddSounds(obj);
            deleteStatus = -1;
            yield return new WaitForSeconds(2f);
            storyStatus++;
        }
    }

    private void DestroyAddSounds(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            if (player != null)
            {
                player.audio.PlayOneShot(destroyClip);
            }
        }
    }

    private void ResetScene2()
    {
        time = 90;
        enemyNumber = 0;
    }

    private void DrawCountText()
    {
        timeText.text = "Garbage-Collect: " + (int) Mathf.Round(time) + "s";
        enemyNumberText.text = "Enemy: " + enemyNumber + "/100";
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
            case 30: //等待玩家进入Scene2
                return false;
            default:
                return true;
        }
    }

    /**
     * 标识玩家可以行动的故事2状态
     */
    public bool isStory2Going()
    {
        switch (storyStatus)
        {
            case 10: //游戏开始
            case -99: //等待玩家复活
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
        if (isScene2)
        {
            Scene2Update();
        }
        else
        {
            Scene1Update();
        }
    }

    private void Scene1Update()
    {
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
        if (!isSpawning)
        {
            StartCoroutine(CheckAndSpawnEnemy());
        }
    }

    private void Scene2Update()
    {
        if (Story2GoOn())
        {
            return;
        }

        if (enemyNumber >= 100)
        {
            //游戏结束
            storyStatus = 100;
        }
        else if (enemyNumber >= 95)
        {
            //地板贴图丢失
            changeMaterialStatus = 5;
        }
        else if (enemyNumber >= 90)
        {
            //玩家贴图丢失
            changeMaterialStatus = 4;
            if (!isDialogGoing)
                ShowOnePageTextAndCountDown2Hide("Bot008:More!More!More!");
        }
        else if (enemyNumber >= 80)
        {
            //武器贴图丢失
            changeMaterialStatus = 3;
        }
        else if (enemyNumber >= 70)
        {
            //敌人贴图丢失
            changeMaterialStatus = 2;
        }
        else if (enemyNumber >= 60)
        {
            //墙壁贴图丢失
            changeMaterialStatus = 1;
            if (!isDialogGoing)
                ShowOnePageTextAndCountDown2Hide("Bot008:Yeeeeees! Do you see the change?!");
        }

        if (changeMaterialStatus > 0)
        {
            changeMaterial();
        }

        //天空盒子颜色
        // float color = Mathf.Max((150 - enemyNumber) / 255f, 0);
        // player.mainCamera.GetComponent<Camera>().backgroundColor = new Color(color, color, color);

        //倒计时
        if (storyStatus >= 10)
        {
            time -= Time.deltaTime;
        }

        //玩家死亡
        if (player.isDead)
        {
            defeatBackgroud.gameObject.SetActive(true);
            defeatText.gameObject.SetActive(true);
            storyStatus = -100;
        }
        else
        {
            defeatBackgroud.gameObject.SetActive(false);
            defeatText.gameObject.SetActive(false);
        }

        //时间到
        if (time <= 0)
        {
            storyStatus = -200;
        }

        DrawCountText();
        //波次刷新
        if (!isSpawning)
        {
            StartCoroutine(CheckAndSpawnEnemy());
        }
    }

    private void changeMaterial()
    {
        if (changeMaterialStatus >= 5)
        {
            //地板贴图丢失
            changeMaterialsWithTag("Ground");
        }
        else if (changeMaterialStatus >= 4)
        {
            //玩家贴图丢失
            changeMaterialsWithTag("Player");
            changeMaterialsWithTag("Player Head");
        }
        else if (changeMaterialStatus >= 1)
        {
            //墙壁贴图丢失
            changeMaterialsWithTag("Wall");
        }

        //会刷新的物品每次都处理
        if (changeMaterialStatus >= 2)
        {
            //敌人贴图丢失
            changeMaterialsWithTag("Enemy");
        }

        if (changeMaterialStatus >= 3)
        {
            //武器贴图丢失
            changeMaterialsWithTag("Lightsaber");
            changeMaterialsWithTag("Lightsaber Hilt");
        }
    }

    IEnumerator CheckAndSpawnEnemy()
    {
        isSpawning = true;
        yield return new WaitForSeconds(1);
        if (isAllEnemyDead())
        {
            if (!isScene2)
            {
                wave++;
            }
            else
            {
                wave += 2;
            }
            SpawnEnemy(wave);
        }

        isSpawning = false;
    }

    public void ResetWave(bool clearBody)
    {
        //场景2不重置敌人波次
        if (!isScene2)
        {
            wave = 1;
        }

        EnemyReset(clearBody);
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

    public void AddEnemyNumber(int i)
    {
        enemyNumber += i;
        player.changeWeaponSize(enemyNumber);
    }
}