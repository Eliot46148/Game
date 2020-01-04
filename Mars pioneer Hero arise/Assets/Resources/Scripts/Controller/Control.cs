using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Control : MonoBehaviour {

    bool isGrounded;
    bool isSprinting;

    public Transform cam;
    public World world;

    // 物理定義
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 3f;

    // 滑鼠鍵盤輸入的變數
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private bool jumpRequest;

    // UI 物件
    public Camera minimap;
    public Toolbar toolbar;

    public Transform highlightBlock;    // 選中的方塊位置參考物件
    public Transform placeBlock;        // 創建的方塊位置參考物件
    public float checkIncrement = 0.1f; // 模擬 Raycast 的增量
    public float reach = 8f;            // 玩家手長
    public int CurrentBlock = 2;

    // 除錯 UI Text
    public GameObject debugScreen;
    public GameObject escPanel;
    public GameObject InventoryPanel;
    public GameObject CursorSlot;

    private Collider collider;

    void Start ()
	{
        //world.UIState = 0;    // 讀取中
        collider = transform.GetComponent<Collider>();       
    }

    void FixedUpdate()
    {
        if (world.UIState == 1)
        {
            if (isSprinting)
                collider.velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
            else
                collider.velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

            if (jumpRequest)
                Jump();

            transform.Rotate(Vector3.up * mouseHorizontal * world.settings.mouseSensitivity);
            cam.Rotate(Vector3.right * -mouseVertical * world.settings.mouseSensitivity);
            minimap.transform.position = new Vector3(transform.position.x, 200, transform.position.z);

            // 除錯畫面
            if (Input.GetKeyDown(KeyCode.F3))
                debugScreen.SetActive(!debugScreen.activeSelf);
        }
    }

    private void Update()
    {
        // 開啟背包UI (到 World -> inUI 的 Property去顯示Panel)
        if (Input.GetKeyDown(KeyCode.E))
        {
            InventoryPanel.SetActive(!InventoryPanel.activeSelf);
            world.UIState = (InventoryPanel.activeSelf ? 0 : 1);
            CursorSlot.SetActive(!CursorSlot.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);
            world.UIState = (escPanel.activeSelf ? 0 : 1);
        }

        // 回到遊戲模式
        if (world.UIState == 1)
        {
            GetPlayerInputs();
            PlaceCursorBlocks();
        }

        isGrounded = collider.isGround;
    }

    void Jump()
    {
        collider.verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void GetPlayerInputs()
    {
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
            Vector3s pos = World.vs(highlightBlock.position);
            Chunk chunk = world.GetChunkFromVector3s(pos);
            BlockType id = BlockType.Air;
            if (chunk != null)
                id = chunk.model.voxelMap[(int)pos.x % VoxelData.ChunkWidth, (int)pos.y % VoxelData.ChunkHeight, (int)pos.z % VoxelData.ChunkWidth].id;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        hit.transform.GetComponent<CharacterStats>().TakeDamage(GetComponent<CharacterStats>().damage.GetValue());
                    }
                }
                else
                {
                    world.DropItem(world.GetChunkFromVector3s(World.vs(highlightBlock.position)).GetBlockID(highlightBlock.position), highlightBlock.position);
                    world.GetChunkFromVector3s(World.vs(highlightBlock.position)).EditVoxel(highlightBlock.position, BlockType.Air);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {                
                switch (id)
                {
                    case (BlockType.Box):
                        world.Exit();
                        break;

                    default:
                        {
                            if (toolbar.slots[toolbar.slotindex].HasItem)
                            {
                                bool flag = true;
                                List<BlockType> banList = new List<BlockType> { BlockType.Stick, BlockType.Sword, BlockType.Water, BlockType.Lava };
                                foreach (BlockType type in banList)
                                    if (id == type)
                                        flag = false;
                                if (!flag)
                                    break;
                                world.GetChunkFromVector3s(World.vs(placeBlock.position)).EditVoxel(placeBlock.position, (BlockType)toolbar.slots[toolbar.slotindex].itemSlot.stack.id);
                                toolbar.slots[toolbar.slotindex].itemSlot.Take(1);
                            }
                        }
                        break;
                }
            }
        }
        
    }

    // 模擬 Raycast 來讓玩家編輯刪除方塊
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
}
