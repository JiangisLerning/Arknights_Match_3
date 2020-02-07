using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharManager : MonoBehaviour
{
    [SerializeField] private Text currencyText;

    public CharBtn ClickedBtn { get ; private set; }
    public GridManager GridRef { get; private set; }
    public Settlement Currency { get; private set; }

    private void Start()
    {
        GridRef = FindObjectOfType<GridManager>();
        Currency = new Settlement(99, 0, true, 3f);
        Currency.OnValueChanged += Currency_OnValueChanged;
        StartCoroutine(Currency.AutoGrowth());
    }

    private void Currency_OnValueChanged(object sender, System.EventArgs e)
    {
        currencyText.text = Currency.GetValue().ToString();
    }

    public void PickChar(CharBtn charBtn)
    {
        ClickedBtn = charBtn;
    }

    public void CreateChar()
    {
        if (ClickedBtn.IsAvailable())
        {
            Char charTemp;
            Vector2 mouseInWld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject newChar = Instantiate(ClickedBtn.CharPrefab, mouseInWld, Quaternion.identity);
            newChar.transform.SetParent(transform);

            charTemp = newChar.GetComponent<Char>();
            charTemp.init(ClickedBtn.CharName, this);
            StartCoroutine(charTemp.CharController.Hover);
        }
    }

    public void PlaceChar(Char _char)
    {
        GridRef.SpawnNewUnit(_char.XInGrid, _char.YInGrid, GridManager.UnitType.SP_Char);//在grid中生成占位unit
        _char.CharController.Move(_char.XInGrid, _char.YInGrid);//移动到该网格
        StopCoroutine(_char.CharController.Hover);
        _char.CharController.DirSelectUI(true);
        _char.IsPlaced = true;
        ClickedBtn.IsUsed = true;
        Currency.Expense(ClickedBtn.Cost);
    }

    public void ReleaseChar(Char _char)
    {
        Vector3 currMouseInWld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x, y;
        Unit pointedUnit;
        GridRef.GetGridIndex(currMouseInWld, out x, out y,out pointedUnit);

        if (pointedUnit!=null && pointedUnit.tag=="ReplaceableUnit")
        {
            _char.XInGrid = pointedUnit.X;
            _char.YInGrid = pointedUnit.Y;
            GridRef.ClearUnit(_char.XInGrid,_char.YInGrid);
            PlaceChar(_char);
        }
        else
        {
            ClearChar(_char);
        }
    }

    public void ClearChar(Char _char)
    {
        int x = _char.XInGrid;
        int y = _char.YInGrid;
        bool isPlaced = _char.IsPlaced;

        _char.CharController.Clear();

        if (isPlaced)
        {//clean the sp_unit in grid
            GridRef.ClearUnit(x, y);
            StartCoroutine(GridRef.Fill());
        }

        ClickedBtn.IsUsed = false;
    }
}
