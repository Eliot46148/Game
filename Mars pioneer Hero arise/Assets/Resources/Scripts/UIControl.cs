using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    public Image selection;
    public Image itemsBar;
    public Texture2D texture;
    public GameObject iconPrefab;
    GameObject icons;
    bool first = true;
    // Use this for initialization
    void Start ()
    {
        icons = GameObject.FindGameObjectWithTag("Item");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (first)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject icon = Instantiate(iconPrefab, icons.transform);
                Vector2 pos = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildBlockMesh>().ItemsBar[i].Icon;
                icon.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(pos.y * 32, (34 - pos.x) * 32, 32, 32), pos);
                icon.GetComponent<RectTransform>().localPosition = new Vector3(30 * i - 120, 0.5f, 0);
            }
            first = false;
        }
        selection.rectTransform.localPosition = new Vector3(((int)(itemsBar.rectTransform.sizeDelta.x/9) * (int)GameObject.FindGameObjectWithTag("Player").GetComponent<BuildBlockMesh>().CurrentBlock - (int)(itemsBar.rectTransform.sizeDelta.x/9 * 4)), 0.5f, 0);
    }
}