using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileScript : MonoBehaviour
{
    public bool hasMergedAlready = false;
    public int indexRow;
    public int indexCol;
    public int Number
    {
        get
        {
            return number;
        }
        set
        {
            number = value;
            if (number == 0)
                SetEmpty();
            else
            {
                ApplyStyle(number);
                SetVisible();
            }
        }
    }

    private TextMeshProUGUI tileNumber;
    private Image tileImage;
    private int number;

    private void Awake()
    {
        tileNumber = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        tileImage = transform.GetChild(0).GetComponent<Image>();
    }

    private void ApplyStyleFromStyleSheet(int _index)
    {
        tileNumber.text = TileStyleSheet.Instance.tileStyles[_index].number.ToString();
        tileNumber.color = TileStyleSheet.Instance.tileStyles[_index].numberColor;
        tileImage.color = TileStyleSheet.Instance.tileStyles[_index].tileColor;
    }

    private void ApplyStyle(int _tileNumber)
    {
        switch (_tileNumber)
        {
            case 2:
                ApplyStyleFromStyleSheet(0);
                break;

            case 4:
                ApplyStyleFromStyleSheet(1);
                break;

            case 8:
                ApplyStyleFromStyleSheet(2);
                break;

            case 16:
                ApplyStyleFromStyleSheet(3);
                break;

            case 32:
                ApplyStyleFromStyleSheet(4);
                break;

            case 64:
                ApplyStyleFromStyleSheet(5);
                break;

            case 128:
                ApplyStyleFromStyleSheet(6);
                break;

            case 256:
                ApplyStyleFromStyleSheet(7);
                break;

            case 512:
                ApplyStyleFromStyleSheet(8);
                break;

            case 1024:
                ApplyStyleFromStyleSheet(9);
                break;

            case 2048:
                ApplyStyleFromStyleSheet(10);
                break;
        }
    }

    private void SetVisible()
    {
        tileImage.enabled = true;
        tileNumber.enabled = true;
    }

    private void SetEmpty()
    {
        tileImage.enabled = false;
        tileNumber.enabled = false;
    }
}
