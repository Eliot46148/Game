using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shat
{
    public BlockType tool, block;
    public float ratio;
    public shat(BlockType _tool, BlockType _block, float _ratio)
    {
        tool = _tool; block = _block; ratio = _ratio;
    }
}
public class AnimPlane : MonoBehaviour
{

    public Material[] frames;
    public bool loop = false;
    public float delayTime = 0.01f;
    public int index;

    bool bEnableAnim = false;

    public static List<shat> shatteringCompare = new List<shat>
    {
        new shat(BlockType.WoodenAxe, BlockType.OakLog, 0.5f),
        new shat(BlockType.IronAxe, BlockType.OakLog, 0.3f),
        new shat(BlockType.GoldenAxe, BlockType.OakLog, 0.1f),
        new shat(BlockType.WoodenAxe, BlockType.Wooden, 0.5f),
        new shat(BlockType.IronAxe, BlockType.Wooden, 0.3f),
        new shat(BlockType.GoldenAxe, BlockType.Wooden, 0.1f),
        new shat(BlockType.WoodenPickaxe, BlockType.CoalOre, 0.5f),
        new shat(BlockType.IronPickaxe, BlockType.CoalOre, 0.3f),
        new shat(BlockType.GoldenPickaxe, BlockType.CoalOre, 0.1f),
        new shat(BlockType.WoodenPickaxe, BlockType.IronOre, 0.5f),
        new shat(BlockType.IronPickaxe, BlockType.IronOre, 0.3f),
        new shat(BlockType.GoldenPickaxe, BlockType.IronOre, 0.1f),
        new shat(BlockType.WoodenPickaxe, BlockType.GoldOre, 0.5f),
        new shat(BlockType.IronPickaxe, BlockType.GoldOre, 0.3f),
        new shat(BlockType.GoldenPickaxe, BlockType.GoldOre, 0.1f),
        new shat(BlockType.WoodenPickaxe, BlockType.Stone, 0.5f),
        new shat(BlockType.IronPickaxe, BlockType.Stone, 0.3f),
        new shat(BlockType.GoldenPickaxe, BlockType.Stone, 0.1f),
        new shat(BlockType.WoodenPickaxe, BlockType.Cobblestone, 0.5f),
        new shat(BlockType.IronPickaxe, BlockType.Cobblestone, 0.3f),
        new shat(BlockType.GoldenPickaxe, BlockType.Cobblestone, 0.1f),
        new shat(BlockType.WoodenShavel, BlockType.Dirt, 0.5f),
        new shat(BlockType.IronShavel, BlockType.Dirt, 0.3f),
        new shat(BlockType.GoldenShavel, BlockType.Dirt, 0.1f),
        new shat(BlockType.WoodenShavel, BlockType.Grass, 0.5f),
        new shat(BlockType.IronShavel, BlockType.Grass, 0.3f),
        new shat(BlockType.GoldenShavel, BlockType.Grass, 0.1f),
        new shat(BlockType.WoodenShavel, BlockType.Sand, 0.5f),
        new shat(BlockType.IronShavel, BlockType.Sand, 0.3f),
        new shat(BlockType.GoldenShavel, BlockType.Sand, 0.1f)
    };


    void Start()
    {

    }

    // Use this for initialization
    protected void Init()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayAnim()
    {
        gameObject.SetActive(true);
        bEnableAnim = true;
        StartCoroutine(IEPlayAnim());
    }

    public void StopAnim()
    {
        bEnableAnim = false;
        StopCoroutine("IEPlayAnim");
        gameObject.SetActive(false);
    }

    IEnumerator IEPlayAnim()
    {
        index = 0;
        while (bEnableAnim)
        {
            GetComponent<Renderer>().material = frames[index];
            index++;
            if (index >= frames.Length)
            {
                if (loop) index = 0;
                else
                {
                    gameObject.SetActive(false);
                    break;
                }
            }
            //yield return null;
            yield return new WaitForSeconds(delayTime);
        }
    }

}

public class Shatter :AnimPlane {

     // Use this for initialization
     void Start () {
          base.Init();

          PlayAnim();
     }

    public IEnumerator Play(float delay)
    {
        delayTime = delay;
        PlayAnim();
        yield return null;
    }

    public bool IsCompleted()
    {
        return index >= frames.Length;
    }
}