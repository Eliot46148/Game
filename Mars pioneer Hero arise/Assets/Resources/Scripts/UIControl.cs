using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    public Image selection;
    public Image itemsBar;
    public Texture2D texture;
    public GameObject iconPrefab;
    public Basic basic;

    public Backpack backpack;

    GameObject icons;
    List<BlockType> items;
    List<GameObject> itemsObject = new List<GameObject>();
    int iconSize;
    // Use this for initialization
    void Start ()
    {
        iconSize = (int)(itemsBar.rectTransform.sizeDelta.x / 9);
        icons = GameObject.FindGameObjectWithTag("Item");
        items = backpack.ItemsBar;
        for (int i = 0; i < 9; i++)
        {
            GameObject icon = Instantiate(iconPrefab, icons.transform);
            Vector2 pos = basic.Blocks[(int)items[i]].icon;
            Sprite iconSprite = Sprite.Create(texture, new Rect(pos.y * 32, (34 - pos.x) * 32, 32, 32), pos);
            icon.GetComponent<Image>().sprite = iconSprite;
            icon.GetComponent<RectTransform>().localPosition = new Vector3(iconSize * i - iconSize * 4, 0.5f, 0);
            icon.name = "Tool " + i;
            itemsObject.Add(icon);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        int index = backpack.CurrentBlock;
        selection.rectTransform.localPosition = new Vector3((iconSize * index - iconSize * 4), 0.5f, 0);
    }
}