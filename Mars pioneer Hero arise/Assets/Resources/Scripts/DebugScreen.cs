using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour {

    World world;
    Text text;

    float frameRate;
    float timer;

    int halfWorldSizeInVoxels;
    int halfWorldSizeInChunks;

    public Text selected;
    List<BlockType> items;

    public Backpack backpack;

    // Use this for initialization
    void Start ()
    {
        items = backpack.ItemsBar;
        world = GameObject.Find("World").GetComponent<World>();
        text = GetComponent<Text>();

        halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
        halfWorldSizeInChunks = VoxelData.WorldSizeInChunks / 2;
    }
	
	// Update is called once per frame
	void Update () {
        string debugText = "Debugging (Press F3 to enable/disable)";
        debugText += "\n";
        debugText += "Frame Rate : " + frameRate + " fps";
        debugText += "\n";
        debugText += "Player XYZ : " + (Mathf.Floor(world.player.transform.position.x) - halfWorldSizeInVoxels) + ", " + Mathf.Floor(world.player.transform.position.y) + ", " + (Mathf.Floor(world.player.transform.position.z) - halfWorldSizeInVoxels);
        debugText += ", Chunks : " + (world.playerChunkCoord.x - halfWorldSizeInChunks) + ", " + (world.playerChunkCoord.z - halfWorldSizeInChunks);



        int index = backpack.CurrentBlock;
        selected.text = System.Enum.GetName(typeof(BlockType), items[index]);

        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
	}
}
