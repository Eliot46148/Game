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
        float height = texture.height / basic.rows;
        float width = texture.width / basic.cols;


        for (int i = 0; i < 9; i++)
        {
            Vector2 pos = World.s2v(basic.Blocks[(int)items[i]].icon);
            if (pos.x >= 0 && pos.x < basic.rows && pos.y >=0 && pos.y < basic.cols)
            {
                GameObject icon = Instantiate(iconPrefab, icons.transform);
                Sprite iconSprite = Sprite.Create(texture, new Rect(pos.y * width, (basic.rows - pos.x - 1) * height, width, height), pos);
                icon.GetComponent<Image>().sprite = iconSprite;
                icon.GetComponent<RectTransform>().localPosition = new Vector3(iconSize * i - iconSize * 4, 0.5f, 0);
                icon.name = "Tool " + i;
                itemsObject.Add(icon);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        int index = backpack.CurrentBlock;
        selection.rectTransform.localPosition = new Vector3((iconSize * index - iconSize * 4), 0.5f, 0);
    }
}