using UnityEngine;
using System.Collections;

public class AnimPlane : MonoBehaviour
{

    public Material[] frames;
    public bool loop = false;
    public float delayTime = 0.01f;
    public int index;

    bool bEnableAnim = false;

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