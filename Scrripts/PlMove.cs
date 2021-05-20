using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlMove : Photon.MonoBehaviour
{
    [Range(0,100f)]
    public float moveSpeed;
    [Range(0, 800f)]
    public float jumpForce;

    //buff
    float buffMoveSpeed = 1f;
    float buffjumpForce = 1f;
    bool isSkillActive = false;
    float cooldownSkill = 30f;
    float durationSkill = 5f;
    public float curTimeDurationSkill;
    public float curTimeCDSkill;
    public GameObject rollerCoverBuff;
    public GameObject countDownObj_BuffUI;
    TextMeshProUGUI countDownText_BuffUI;
    Image rollerCoverBuffImg;

    public PhotonView photonView;
    public PhotonPlayer photonPlayer;
         
    private Vector3 selfPos;
    public VirtualJoyStick joystick;

    public bool devTesting = false;
    public bool isFacingRight = true;

    private GameObject sceneCam;

    public GameObject plCam;

    public Text plNameText;

    public GameObject textObj;
    Vector3 scale;
    Vector3 textObjScale;

    // audio
    public SFXcreator sfxCreator;
    public AudioClip addPoint, throwTrash, jump, hit;

    public Animator anim;
    public Rigidbody2D rigid;
    // jump
    public bool isGrounded = false;
    private PlatformEffector2D effector;
    float waitTimeForEffector;
    public GameObject rollerCoverJumpUI;
    public GameObject rollerCoverDownUI;
    public GameObject rollerCoverPickupUI;
    public GameObject rollerCoverThrowUI;
    public int jumpCount = 1;
    public int curJumpCount = 0;

    // item
    public GameObject hitEffectPrefabs;
    public GameObject[] trashImg;
    public GameObject[] trashPrefabs;
    public GameObject[] throwTrashPrefabs;
    public GameObject[] trashIcon = new GameObject[8];
    public GameObject[] trashIconText = new GameObject[4];
    private GameObject throwIcon, emptyText, groupThrowPanelUI;
    public GameObject countDownText;
    public GameObject addingScorePanel;
    public GameObject addingTwentyScorePrefab;
    public GameObject throwRoot;
    public List<GameObject> trashInHand = new List<GameObject>();
    public List<int> trashInHandIndex = new List<int>();
    public bool isPickup = false;
    float timeToDestroyTrashInHand = 15f;
    public float curTimeToDestroyTrashInHand = 0;
    int indexTrashPrefabs;
    GameManager gmr;

    Button buffBtn;
    ButtonController pickupBtn;
    ButtonController jumpBtn;
    ButtonController downBtn;
    ButtonController throwBtn;

    private void Awake()
    {
        rollerCoverThrowUI = GameObject.Find("RollerCoverThrowUI");
        rollerCoverJumpUI = GameObject.Find("RollerCoverJumpUI");
        rollerCoverDownUI = GameObject.Find("RollerCoverDownUI");
        rollerCoverPickupUI = GameObject.Find("RollerCoverPickupUI");
        rollerCoverBuff = GameObject.Find("RollerCoverBuff");
        countDownObj_BuffUI = GameObject.Find("CountDownText_BuffUI");
        throwIcon = GameObject.Find("ThrowEmptyLogo");
        emptyText = GameObject.Find("EmptyText");
        countDownText = GameObject.Find("CountDownText_ThrowUI");
        groupThrowPanelUI = GameObject.Find("GroupThrowPanelUI");
        for (int i = 0; i < trashIcon.Length;i++)
        {
            trashIcon[i] = groupThrowPanelUI.transform.GetChild(i + 3).gameObject;
            trashIcon[i].SetActive(false);
        }
        for(int j = 0; j < trashIconText.Length;j++)
        {
            trashIconText[j] = groupThrowPanelUI.transform.GetChild(j + 14).gameObject;
            trashIconText[j].SetActive(false);
        }
        photonHandler phoHand = photonHandler.instance;
        photonPlayer = phoHand.photonPlayer;
        print(photonPlayer);
        scale = transform.localScale;
        textObjScale = textObj.transform.localScale;
        gmr = GameManager.instance;

        if (!devTesting && photonView.isMine)
        {
            sceneCam = GameObject.Find("Main Camera");
            if (sceneCam != null)
            {
                sceneCam.SetActive(false);
            }
            plCam.SetActive(true);

            photonView.RPC("SFXCeatorFind", PhotonTargets.AllBuffered);
            plNameText.text = PhotonNetwork.playerName;
            joystick = GameObject.Find("RollerScrollPanel").GetComponent<VirtualJoyStick>();
            countDownText_BuffUI = countDownObj_BuffUI.GetComponent<TextMeshProUGUI>();
            rollerCoverBuffImg = rollerCoverBuff.GetComponent<Image>();
            buffBtn = rollerCoverBuff.GetComponent<Button>();
            pickupBtn = rollerCoverPickupUI.GetComponent<ButtonController>();
            jumpBtn = rollerCoverJumpUI.GetComponent<ButtonController>();
            downBtn = rollerCoverDownUI.GetComponent<ButtonController>();
            throwBtn = rollerCoverThrowUI.GetComponent<ButtonController>();
            countDownText.SetActive(false);
            countDownObj_BuffUI.SetActive(false);
            AddBuffButton();
        }

        if(!photonView.isMine)
        {
            plNameText.text = photonView.owner.name;
        }
    }
    [PunRPC]
    private void SFXCeatorFind()
    {
        sfxCreator = GameObject.Find("SFXcreator").GetComponent<SFXcreator>();
    }
    private void Start()
    {
        SetParent();
        photonPlayer.AddScore(0);
    }

    private void Update()
    {
        if (gmr.curTime > 0)
        {
            if (!devTesting)
            {
                if (photonView.isMine)
                    checkInput();
                else
                    smoothNetMovement();
            }
            else
            {
                checkInput();
            }
        }
    }
    private void AddBuffButton()
    {
        buffBtn.onClick.AddListener(() => BuffSpeedMovementAndJumpForce());
    }
    private void BuffSpeedMovementAndJumpForce()
    {
        if (curTimeCDSkill <= 0 && !isSkillActive)
        {
            curTimeCDSkill = cooldownSkill;
            curTimeDurationSkill = durationSkill;
            countDownObj_BuffUI.SetActive(true);
            rollerCoverBuffImg.fillAmount = 1;
            isSkillActive = true;
        }
    }
    private void checkInput()
    {
        FlipAddingScoreText();
        var move = new Vector3(joystick.Horizontal(), 0);
        if(waitTimeForEffector >= 0)
        {
            waitTimeForEffector -= Time.deltaTime;
        }
        if (curTimeCDSkill > 0 && !isSkillActive)
        {
            curTimeCDSkill -= Time.deltaTime;
            if (curTimeDurationSkill > 0)
            {
                curTimeDurationSkill -= Time.deltaTime;
                buffMoveSpeed *= 1.0003f;
                buffjumpForce *= 1.0003f;
            }
            else
            {
                buffMoveSpeed = 1f;
                buffjumpForce = 1f;
            }
            countDownText_BuffUI.text = curTimeCDSkill.ToString(curTimeCDSkill < 10f ? "#" : "##");
            rollerCoverBuffImg.fillAmount = curTimeCDSkill/cooldownSkill;
            if (curTimeCDSkill <= 0)
            {
                countDownObj_BuffUI.SetActive(false);
            }
        }
        else if (curTimeCDSkill > 0 && isSkillActive)
        {
            isSkillActive = false;
        }

        if (((Input.GetKeyDown(KeyCode.Space) || jumpBtn.pressed) && !anim.GetBool("IsJump") && isGrounded) && curJumpCount <= 0)
        {
            curJumpCount++;
            sfxCreator.Create(jump, 0.3f, 1f);
            PerformJumpPlus();
        }
        
        if((!jumpBtn.pressed && curJumpCount > 0) && isGrounded)
        {
            curJumpCount = 0;
        }

        if ((Input.GetKeyDown(KeyCode.E) || pickupBtn.pressed) && !isPickup)
        {
            anim.SetTrigger("Pickup");
            photonView.RPC("PickUpAnimationTrigger", PhotonTargets.Others);
        }

        WaitForDestroyTrashInHand();

        if ((Input.GetKeyDown(KeyCode.Mouse0) || throwBtn.pressed) && !isPickup)
        {
            anim.SetTrigger("Throw");
            sfxCreator.Create(throwTrash, 1f, 1f);
            photonView.RPC("ThrowAnimationTrigger", PhotonTargets.Others);
        }
        else if ((Input.GetKeyDown(KeyCode.Mouse0) || throwBtn.pressed) && isPickup)
        {
            anim.SetTrigger("Throw");
            sfxCreator.Create(throwTrash, 1f, 1f);
            for (int i = 0; i < trashImg.Length; i++)
            {
                if(trashImg[i].activeSelf)
                {
                    trashImg[i].SetActive(false);
                    photonView.RPC("ShowTrash", PhotonTargets.OthersBuffered, i, trashImg[i].activeSelf);
                }
            }
            photonView.RPC("ThrowAnimationTrigger", PhotonTargets.Others);
            ThrowTrash();
            isPickup = false;
        }

        if (anim.GetBool("IsWalk") && (move.x >= -0.15f || move.x <= 0.15f))
        {
            anim.SetBool("IsWalk", false);
            photonView.RPC("WalkAnimationFalse", PhotonTargets.Others);
        }

        if (Input.GetKey(KeyCode.D) || joystick.Horizontal() > 0)
        {
            transform.position += move * (moveSpeed * buffMoveSpeed) * Time.deltaTime;
            if (!anim.GetBool("IsWalk") && move.x > 0)
            {
                anim.SetBool("IsWalk", true);
                photonView.RPC("WalkAnimationTrue", PhotonTargets.Others);
            }

            if (transform.localScale.x < 0)
            {
                isFacingRight = !isFacingRight;
                scale.x *= -1;
                textObjScale.x *= -1;
                transform.localScale = scale;
                textObj.transform.localScale = textObjScale;
                photonView.RPC("OnSpriteFlip", PhotonTargets.Others);
            }
        }
        else if (Input.GetKey(KeyCode.A) || joystick.Horizontal() < 0)
        {
            transform.position += move * (moveSpeed * buffMoveSpeed) * Time.deltaTime;
            if (!anim.GetBool("IsWalk") && move.x < 0)
            {
                anim.SetBool("IsWalk", true);
                photonView.RPC("WalkAnimationTrue", PhotonTargets.Others);
            }

            if (transform.localScale.x > 0)
            {
                isFacingRight = !isFacingRight;
                scale.x *= -1;
                textObjScale.x *= -1;
                transform.localScale = scale;
                textObj.transform.localScale = textObjScale;
                photonView.RPC("OnSpriteFlip", PhotonTargets.Others);
            }
        }
    }

    private void FlipAddingScoreText()
    {
        print("isFacingRight = " + isFacingRight);
        print("addingScorePanel.transform.childCount = " + addingScorePanel.transform.childCount);
        if (addingScorePanel.transform.childCount > 0)
        {
            print("addingScorePanel.transform.childCount = " + addingScorePanel.transform.childCount);
            if (isFacingRight && addingScorePanel.transform.GetChild(0).localPosition.x <= -1)
            {
                addingScorePanel.transform.GetChild(0).localScale = new Vector3(-1f, 1f, 1f);
            }
            else if(!isFacingRight && addingScorePanel.transform.GetChild(0).localPosition.x >= 1)
            {
                addingScorePanel.transform.GetChild(0).localScale = Vector3.one;
            }
        }
    }
    private void CheckPlatformEffector2D(Collision2D collision)
    {
        if(collision.transform.tag == "Ground" && collision.transform.GetComponent<PlatformEffector2D>() != null)
        {
            effector = collision.transform.GetComponent<PlatformEffector2D>();
            if((Input.GetKeyUp(KeyCode.S) && (Input.GetKey(KeyCode.Space)) || downBtn.pressed))
            {
                waitTimeForEffector = 0.25f;
            }
            if((Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.Space)) || downBtn.pressed))
            {
                if (waitTimeForEffector <= 0)
                {
                    effector.rotationalOffset = 180f;
                    waitTimeForEffector = 0.25f;
                }
            }
            if((Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.Space)) || jumpBtn.pressed))
            {
                effector.rotationalOffset = 0f;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!devTesting)
        {
            if (photonView.isMine)
            {
                if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
                {
                    isGrounded = true;
                    anim.SetBool("IsJump", false);
                    photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                    CheckPlatformEffector2D(collision);
                }
                if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                    collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
                {
                    isGrounded = true;
                    anim.SetBool("IsJump", false);
                    photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                    if ((Input.GetKeyDown(KeyCode.E) || pickupBtn.pressed) && !isPickup)
                    {
                        CheckCharactersNameForPickUp(collision);
                    }
                }

            }
        }
        else
        {
            if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
            {
                isGrounded = true;
                anim.SetBool("IsJump", false);
                photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                CheckPlatformEffector2D(collision);
            }

            if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
            {
                isGrounded = true;
                anim.SetBool("IsJump", false);
                photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                if ((Input.GetKeyDown(KeyCode.E) || pickupBtn.pressed) && !isPickup)
                {
                    CheckCharactersNameForPickUp(collision);
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!devTesting)
        {
            if (photonView.isMine)
            {
                if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
                {
                    isGrounded = true;
                    anim.SetBool("IsJump", false);
                    photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                    CheckPlatformEffector2D(collision);
                }
                if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                    collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
                {
                    isGrounded = true;
                    anim.SetBool("IsJump", false);
                    photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                    if ((Input.GetKeyDown(KeyCode.E) || pickupBtn.pressed) && !isPickup)
                    {
                        CheckCharactersNameForPickUp(collision);
                    }
                }

            }
        }
        else
        {
            if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
            {
                isGrounded = true;
                anim.SetBool("IsJump", false);
                photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                CheckPlatformEffector2D(collision);
            }

            if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
            {
                isGrounded = true;
                anim.SetBool("IsJump", false);
                photonView.RPC("JumpAnimationFalse", PhotonTargets.Others);
                if ((Input.GetKeyDown(KeyCode.E) || pickupBtn.pressed) && !isPickup)
                {
                    CheckCharactersNameForPickUp(collision);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "GTItem" || collision.transform.tag == "HTItem" ||
            collision.transform.tag == "OTItem" || collision.transform.tag == "RTItem")
        {
            if (collision.transform.GetComponent<PhotonView>().owner.NickName != photonView.owner.NickName)
            {
                HitTarget(gameObject.GetComponent<BoxCollider2D>());
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!devTesting)
        {
            if (photonView.isMine)
            {
                if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
                {
                    isGrounded = false;
                }
                if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                    collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
                {
                    isGrounded = false;
                }
            }
        }
        else
        {
            if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Player")
            {
                isGrounded = false;
            }

            if (collision.transform.tag == "GTrash" || collision.transform.tag == "HTrash" ||
                collision.transform.tag == "OTrash" || collision.transform.tag == "RTrash")
            {
                isGrounded = false;
            }
        }
    }
    private void WaitForDestroyTrashInHand()
    {
        if (isPickup && curTimeToDestroyTrashInHand > 0)
        {
            curTimeToDestroyTrashInHand -= Time.deltaTime;
            countDownText.GetComponent<TextMeshProUGUI>().text = curTimeToDestroyTrashInHand.ToString(curTimeToDestroyTrashInHand < 10 ? "#" : "##");
        }
        else if (isPickup && curTimeToDestroyTrashInHand <= 0)
        {
            CheckActiveTrashIconsForDeActive();
            curTimeToDestroyTrashInHand = 0;
            trashInHand.RemoveAt(0);
            trashInHandIndex.RemoveAt(0);
            CloseTrashImg();
            photonView.RPC("CloseTrashImg", PhotonTargets.Others);
            isPickup = false;
        }
    }
    private void CheckCharactersNameForPickUp(Collision2D collision)
    {
        if (trashInHand.Count < 1)
        {
            string clone = "(Clone)";
            if (collision.transform.name == trashPrefabs[0].name + clone || collision.transform.name == trashPrefabs[1].name + clone)
            {
                if (this.name == "Character01" + clone)
                {
                    AddScorePlayer_EatCondition(collision, clone);
                }
                else
                {
                    AddItem(collision, clone);
                }
            }
            else if(collision.transform.name == trashPrefabs[2].name + clone || collision.transform.name == trashPrefabs[3].name + clone)
            {
                if (this.name == "Character02" + clone)
                {
                    AddScorePlayer_EatCondition(collision, clone);
                }
                else
                {
                    AddItem(collision, clone);
                }
            }
            else if (collision.transform.name == trashPrefabs[4].name + clone || collision.transform.name == trashPrefabs[5].name + clone)
            {
                if (this.name == "Character03" + clone)
                {
                    AddScorePlayer_EatCondition(collision, clone);
                }
                else
                {
                    AddItem(collision, clone);
                }
            }
            else if (collision.transform.name == trashPrefabs[6].name + clone || collision.transform.name == trashPrefabs[7].name + clone)
            {
                if (this.name == "Character04" + clone)
                {
                    AddScorePlayer_EatCondition(collision, clone);
                }
                else
                {
                    AddItem(collision, clone);
                }
            }

        }
    }
    private void CreateAddingTwentyScorePrefab()
    {
        GameObject thisScoreText = Instantiate(addingTwentyScorePrefab, addingScorePanel.transform.localPosition, addingTwentyScorePrefab.transform.rotation);
        thisScoreText.transform.SetParent(addingScorePanel.transform);
        thisScoreText.transform.localPosition = Vector3.zero;
        thisScoreText.transform.localScale = addingTwentyScorePrefab.transform.localScale;
    }
    private void AddScorePlayer_EatCondition(Collision2D collision, string clone)
    {
        print(transform.name + " Eat : " + collision.transform.name + clone);
        if (photonView.isMine && PhotonNetwork.connected)
        {
            ItemProperties item = collision.transform.GetComponent<ItemProperties>();
            photonPlayer.AddScore(20);
            sfxCreator.Create(addPoint, 0.658f, 1f);
            CreateAddingTwentyScorePrefab();
            item.CheckTrashForRemove();
        }
        //Eat Animation
    }
    private void CheckTrashImgForActiveTrashIcons(int i)
    {
        throwIcon.SetActive(false);
        emptyText.SetActive(false);
        trashIcon[i].SetActive(true);
        if(i == 0 || i == 1)
        {
            trashIconText[0].SetActive(true);
        }
        else if (i == 2 || i == 3)
        {
            trashIconText[1].SetActive(true);
        }
        else if (i == 4 || i == 5)
        {
            trashIconText[2].SetActive(true);
        }
        else if (i == 6 || i == 7)
        {
            trashIconText[3].SetActive(true);
        }
        countDownText.SetActive(true);
    }
    private void CheckActiveTrashIconsForDeActive()
    {
        for (int i = 0; i < trashIconText.Length;i++)
        {
            trashIconText[i].SetActive(false);
        }
        for (int j = 0; j < trashIcon.Length; j++)
        {
            trashIcon[j].SetActive(false);
        }
        countDownText.SetActive(false);
        throwIcon.SetActive(true);
        emptyText.SetActive(true);
    }
    private void AddItem(Collision2D collision,string clone)
    {
        for (int i = 0; i < trashPrefabs.Length; i++)
        {
            if (collision.transform.name == trashPrefabs[i].name + clone)
            {
                print(transform.name + " Pick up : " + trashPrefabs[i].name + clone);
                trashImg[i].SetActive(true);
                countDownText.GetComponent<TextMeshProUGUI>().text = "15";
                CheckTrashImgForActiveTrashIcons(i);
                photonView.RPC("ShowTrash", PhotonTargets.OthersBuffered, i, trashImg[i].activeSelf);
                trashInHand.Add(throwTrashPrefabs[i]);
                trashInHandIndex.Add(i);
                if (photonView.isMine && PhotonNetwork.connected)
                {
                    ItemProperties item = collision.transform.GetComponent<ItemProperties>();
                    item.CheckTrashForRemove();
                }
                //photonView.RPC("DestroyTrash", PhotonTargets.AllBufferedViaServer, collision.transform.name);
                curTimeToDestroyTrashInHand = timeToDestroyTrashInHand;
                countDownText.GetComponent<TextMeshProUGUI>().text = curTimeToDestroyTrashInHand.ToString(curTimeToDestroyTrashInHand < 10? "#":"##");
                isPickup = true;

            }
        }
    }
    private void ThrowTrash()
    {
        // PhotonNetwork.Instantiate() บน Client คนไหน คนนั้นคือ ผู้ควบคุมวัตถุนั้น 
        CheckActiveTrashIconsForDeActive();
        GameObject thisTrash = PhotonNetwork.Instantiate(trashInHand[0].name, throwRoot.transform.localPosition, throwRoot.transform.localRotation, 0);
        Rigidbody2D trashRigid = thisTrash.transform.GetComponent<Rigidbody2D>();
        ThrowItem throwItem = thisTrash.transform.GetComponent<ThrowItem>();
        throwItem.photonPlayer = this.photonPlayer;
        thisTrash.transform.SetParent(throwRoot.transform);
        thisTrash.transform.localPosition = new Vector3(0,0,50);
        thisTrash.transform.localScale = trashPrefabs[trashInHandIndex[0]].transform.localScale;
        thisTrash.transform.SetParent(GameObject.Find("Canvas").transform);
        thisTrash.transform.TransformPoint(Vector3.right*2);
        thisTrash.transform.localScale = trashPrefabs[trashInHandIndex[0]].transform.localScale;
        photonView.RPC("SetThrowTrash", PhotonTargets.OthersBuffered, thisTrash.transform.name,throwRoot.name,this.name);
        if (!isFacingRight)
        {
            trashRigid.AddForce(new Vector2(-210, 210), ForceMode2D.Force);
        }
        else
        {
            trashRigid.AddForce(new Vector2(210, 210), ForceMode2D.Force);
        }
        trashInHandIndex.RemoveAt(0);
        trashInHandIndex.Clear();
        trashInHand.Remove(thisTrash);
        trashInHand.Clear();
    }
    private PhotonPlayer CheckMasterClientPhotonPlayers()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].IsMasterClient)
            {
                return players[i];
            }
        }
        print("CheckMasterClientPhotonPlayers Error.");
        return photonPlayer;
    }
    private void PerformJump()
    {
        rigid.AddForce(Vector2.up * jumpForce);
        anim.SetBool("IsJump", true);
        photonView.RPC("JumpAnimationTrue", PhotonTargets.Others);
    }
    private void PerformJumpPlus()
    {
        rigid.AddForce(Vector2.up * (jumpForce*buffjumpForce));
        anim.SetBool("IsJump", true);
        photonView.RPC("JumpAnimationTrue", PhotonTargets.Others);
    }
    //        NET Code
    //           v
    //           v
    private void smoothNetMovement()
    {
        transform.position = Vector3.Lerp(transform.position, selfPos, Time.deltaTime * 8);
    }
    private void SettingPlayerCharacter()
    {

        if (transform.name == "Character01(Clone)")
        {
            transform.localScale = new Vector3(25f, 25f, 25f);
        }
        else if (transform.name == "Character04(Clone)")
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }

    }
    private void SetParent()
    {
        print("parent : "+transform.parent);
        transform.SetParent(GameObject.Find("Canvas").transform);
        print("parent : " + transform.parent);
        SettingPlayerCharacter();
        SetRefObj();
    }
    private void HitTarget(Collider2D collision)
    {
        sfxCreator.Create(hit, 0.8f, 1f);
        GameObject thisEffect = Instantiate(hitEffectPrefabs, collision.transform.localPosition, hitEffectPrefabs.transform.rotation);
        GameObject player = GameObject.Find(collision.transform.name);
        thisEffect.transform.parent = player.transform;
        thisEffect.transform.localPosition = new Vector3(0, 0, -0.1f);
        thisEffect.transform.localScale = new Vector3(7f, 7f, 7f);
    }
    private void SetRefObj()
    {
        GameObject spawnPoint;
        spawnPoint = GameObject.Find("SpawnPoint");
        float rndSpawn = Random.Range(-700f, 700f);
        print("rndSpawn : " + rndSpawn);

        transform.localPosition = new Vector3(spawnPoint.transform.localPosition.x + rndSpawn,
                                              spawnPoint.transform.localPosition.y, -180);
    }
    private void CheckTrashObjNameToRemoveFromList(string trashName,GameObject thisTrash)
    {
        GameManager gm = GameManager.instance;
        if (trashName == gm.generalTrash[0].name + "(Clone)" || transform.name == gm.generalTrash[1].name + "(Clone)")
        {
            gm.gTrashCount--;
            gm.gTrashListing.Remove(thisTrash);
        }
        else if (trashName == gm.harzadousTrash[0].name + "(Clone)" || transform.name == gm.harzadousTrash[1].name + "(Clone)")
        {
            gm.hTrashCount--;
            gm.hTrashListing.Remove(thisTrash);
        }
        else if (trashName == gm.organicTrash[0].name + "(Clone)" || transform.name == gm.organicTrash[1].name + "(Clone)")
        {
            gm.oTrashCount--;
            gm.oTrashListing.Remove(thisTrash);
        }
        else if (trashName == gm.recycleTrash[0].name + "(Clone)" || transform.name == gm.recycleTrash[1].name + "(Clone)")
        {
            gm.rTrashCount--;
            gm.rTrashListing.Remove(thisTrash);
        }
    }
    
    [PunRPC]
    private void SetThrowTrash(string throwTrashName,string throwRootName,string playerName)
    {
        int indexTrashPrefabs = 0;
        Vector3 scale;
        GameObject player = GameObject.Find(playerName);
        PlMove playerPlMove = player.transform.GetComponent<PlMove>();
        GameObject thisTrash = GameObject.Find(throwTrashName);
        GameObject throwRoot = GameObject.Find(throwRootName);

        thisTrash.transform.SetParent(throwRoot.transform);
        thisTrash.transform.localPosition = new Vector3(0,0,50);
        for(int i = 0; i < trashPrefabs.Length; i++)
        {
            if(thisTrash.name == throwTrashPrefabs[i].name+"(Clone)")
            {
                indexTrashPrefabs = i;
                print(trashPrefabs[indexTrashPrefabs]);
                print(trashPrefabs[indexTrashPrefabs].transform.localScale);
                thisTrash.transform.localScale = trashPrefabs[indexTrashPrefabs].transform.localScale;
                scale = thisTrash.transform.localScale;
                thisTrash.transform.SetParent(GameObject.Find("Canvas").transform);
                //thisTrash.transform.TransformPoint(scale);
                thisTrash.transform.localScale = scale;
                break;
            }
        }
    }
    [PunRPC]
    private void CloseTrashImg()
    {
        for(int i = 0; i < trashPrefabs.Length;i++)
        {
            if(trashImg[i].activeSelf)
            {
                trashImg[i].SetActive(false);
            }
        }
    }
    [PunRPC]
    private void ShowTrash(int index,bool activeSelf)
    {
        trashImg[index].SetActive(activeSelf);
    }
    [PunRPC]
    private void PickUpAnimationTrigger()
    {
        anim.SetTrigger("Pickup");
    }
    [PunRPC]
    private void ThrowAnimationTrigger()
    {
        anim.SetTrigger("Throw");
    }
    [PunRPC]
    private void JumpAnimationTrue()
    {
        anim.SetBool("IsJump", true);
    }
    [PunRPC]
    private void JumpAnimationFalse()
    {
        anim.SetBool("IsJump", false);
    }
    [PunRPC]
    private void WalkAnimationFalse()
    {
        anim.SetBool("IsWalk", false);
    }
    [PunRPC]
    private void WalkAnimationTrue()
    {
        anim.SetBool("IsWalk", true);
    }
    [PunRPC]
    private void OnSpriteFlip()
    {
        isFacingRight = !isFacingRight;
        scale.x *= -1;
        textObjScale.x *= -1;
        transform.localScale = scale;
        textObj.transform.localScale = textObjScale;
    }

    private void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            selfPos = (Vector3)stream.ReceiveNext();
        }
    }
}
