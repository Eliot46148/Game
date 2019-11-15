using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Control : MonoBehaviour {

    bool isGrounded;
    bool isSprinting;

    public Transform cam;
    public World world;

    // 物理定義
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    // 玩家碰撞設定
    public float playerWidth = 0.3f;
    public float playerHHeight = 0.9f;

    // 滑鼠鍵盤輸入的變數
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    // UI 物件
    public Camera minimap;
    public Backpack backpack;

    public Transform highlightBlock;    // 選中的方塊位置參考物件
    public Transform placeBlock;        // 創建的方塊位置參考物件
    public float checkIncrement = 0.1f; // 模擬 Raycast 的增量
    public float reach = 8f;            // 玩家手長

    // 除錯 UI Text
    public GameObject debugScreen;

    void Start ()
	{
        world.inUI = 0;    // 讀取中
    }

    void FixedUpdate()
    {
        if (world.inUI == 1)
        {
            CalculateVelocity();
            if (jumpRequest)
                Jump();

            transform.Rotate(Vector3.up * mouseHorizontal * world.settings.mouseSensitivity);
            cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity);
            transform.Translate(velocity, Space.World);
            minimap.transform.position = new Vector3(transform.position.x, 200, transform.position.z);

            // 除錯畫面
            if (Input.GetKeyDown(KeyCode.F3))
                debugScreen.SetActive(!debugScreen.activeSelf);
        }
    }

    private void Update()
    {
        // 開啟背包UI (到 World -> inUI 的 Property去顯示Panel)
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (world.inUI == 0)
                world.inUI = 1;
            else if (world.inUI == 1)
                world.inUI = 0;
        }

        // 回到遊戲模式
        if (world.inUI == 1)
        {
            GetPlayerInputs();
            PlaceCursorBlocks();
        }
    }

    void Jump()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void CalculateVelocity()
    {
        // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // if we're sprinting, use the sprint multiplier.
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);
    }

    private void GetPlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;
        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;

        if (highlightBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
                world.GetChunkFromVector3s(World.vs(highlightBlock.position)).EditVoxel(highlightBlock.position, BlockType.Air);
            if (Input.GetMouseButtonDown(1))
                world.GetChunkFromVector3s(World.vs(placeBlock.position)).EditVoxel(placeBlock.position, backpack.CurrentBlockType);
        }
    }

    // 模擬 Raucast 來讓玩家編輯刪除方塊
    private void PlaceCursorBlocks()
    {

        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = cam.position + (cam.forward * step);

            if (world.CheckForVoxel(World.vs(pos)))
            {
                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x) + 0.5f, Mathf.FloorToInt(pos.y) + 0.5f, Mathf.FloorToInt(pos.z) + 0.5f);
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x) + 0.5f, Mathf.FloorToInt(pos.y) + 0.5f, Mathf.FloorToInt(pos.z) + 0.5f);
            step += checkIncrement;
        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    // 六個方向碰撞檢查
    private float checkDownSpeed(float downSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth))
           )
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float checkUpSpeed(float upSpeed)
    {
        if (
            world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth)) ||
            world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth))
           )
            return 0;
        else
            return upSpeed;
    }

    public bool front
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3s(transform.position.x, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3s(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
                )
                return true;
            else
                return false;
        }
    }
    public bool back
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3s(transform.position.x, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3s(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
                )
                return true;
            else
                return false;
        }
    }
    public bool left
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3s(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
    public bool right
    {
        get
        {
            if (
                world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y, transform.position.z)) ||
                world.CheckForVoxel(new Vector3s(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
                )
                return true;
            else
                return false;
        }
    }
}
