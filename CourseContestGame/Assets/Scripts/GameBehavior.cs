using UnityEngine;
//1
using TMPro;

public class GameBehavior : MonoBehaviour
{
    //2
    public int MaxItems = 4;
    //3
    public TMP_Text HealthText;
    public TMP_Text ItemText;
    public TMP_Text ProgressText;

    //4
    private void Start()
    {
        ItemText.text += _itemCollected;
        HealthText.text += _playerHP;
    }

    private int _itemCollected = 0;
    public int Items
    {
        get { return _itemCollected; }
        set
        {
            _itemCollected = value;
            //Debug.LogFormat("Items: {0}", _itemCollected);
            //5
            ItemText.text = "Items: " + Items;
            //6
            if (_itemCollected >= MaxItems)
            {
                ProgressText.text = "You have found all the items!";
            }
            else
            {
                ProgressText.text = "Item found, only " + (MaxItems - _itemCollected) + " more!";
            }
        }
    }

    private int _playerHP = 10;
    public int HP
    {
        get { return _playerHP; }
        set
        {
            _playerHP = value;
            //7
            HealthText.text = "Health: " + HP;
            Debug.LogFormat("Lives: {0}", _playerHP);
        }
    }
}