using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum UnitType
    {
        ELITE_SCOUT,
        ELITE_TANK,
        ELITE_ICEMAGE,
        NORMAL,//到此为可随机生成种类
        COUNT_RND,//可随机生成种类数量
        EMPTY,
        BLOCK,
        SP_Char,//仅占位
        SP_RECOVER,
        SP_ROW_CLR,
        SP_COL_CLR,
        COUNT,
    };

    [System.Serializable]
    public struct unitPrefab
    {
        public UnitType type;
        public GameObject prefab;
        public float probability; //生成概率、权重
    };

    public int xDim;
    public int yDim;
    public float xOrigin = -4.0f;
    public float yOrigin = 4.0f;
    public float cellSize = 1f;
    public float fillTime;

    private float[] probabilities;
    public unitPrefab[] unitPrefabs;
    public GameObject backgroundPrefab;
    private Unit[,] units;
    private Dictionary<UnitType, GameObject> unitPrefabDict;
    private Unit unitTemp;
    public Unit PressedUnit { get; set; } = null;
    public Unit EnteredUnit { get; set; } = null;
    public bool NeedsRefill { get; private set; }
    public int NumScout { get; set; } = 0;
    public Mask Mask { get; private set; }
    public BossHandler Boss { get; set; }
    public CharManager CharManager { get; private set; }
    public Canvas Canvas { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Mask = FindObjectOfType<Mask>();
        Boss = FindObjectOfType<BossHandler>();
        CharManager = FindObjectOfType<CharManager>();
        Canvas = FindObjectOfType<Canvas>();

        unitPrefabDict = new Dictionary<UnitType, GameObject>();

        for (int i = 0; i < unitPrefabs.Length; i++)
        {
            if (!unitPrefabDict.ContainsKey(unitPrefabs[i].type))
            {
                unitPrefabDict.Add(unitPrefabs[i].type, unitPrefabs[i].prefab);
            }
        }

        probabilities = new float[(int)UnitType.COUNT_RND];

        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i] = unitPrefabs[i].probability;
        }

        //生成背景
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.SetParent(transform);
            }
        }

        //生成空unit
        units = new Unit[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewUnit(x, y, UnitType.EMPTY);
            }
        }
        
        //开启协程Fill
        StartCoroutine(Fill());

        //启动boss
        Boss.Startup(this);
    }

    private void Update()
    {
        if (PressedUnit == null)
        {
            if (Input.GetMouseButtonDown(0)) //clicked
            {
                //get the hit position
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    PressedUnit = hit.collider.gameObject.GetComponent<Unit>();
                }
            }
        }
        else if (EnteredUnit == null)
        {
            if (Input.GetMouseButton(0)) //drag the unit
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    if (PressedUnit.gameObject != hit.collider.gameObject)
                    {
                        EnteredUnit = hit.collider.gameObject.GetComponent<Unit>();
                    }
                }
            }
            else //release without draging
            {
                PressedUnit = null;
            }
        }

        if (PressedUnit != null && EnteredUnit != null)
        {
            if (!IsAdjacent(PressedUnit, EnteredUnit))
            {
                EnteredUnit = units[EnteredUnit.X, PressedUnit.Y];//非相邻（及斜对角）改为同排 
                if (!IsAdjacent(PressedUnit, EnteredUnit))
                {
                    PressedUnit = null;
                    EnteredUnit = null;
                    unitTemp = null;
                }
            }
            else
            {
                if (unitTemp != PressedUnit)
                {
                    SwapUnits(PressedUnit, EnteredUnit);
                    unitTemp = PressedUnit;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (GetMatch(PressedUnit) != null || GetMatch(EnteredUnit) != null)//matched? then clear and refill
                    {
                        //Debug.Log(PressedUnit.X + "," + PressedUnit.Y + "   ;   " + EnteredUnit.X + "," + EnteredUnit.Y);
                        ClearAllValidMatiches();
                        StartCoroutine(Fill());
                    }
                    else// undo swap
                    {
                        SwapUnits(PressedUnit, EnteredUnit);
                    }

                    PressedUnit = null;
                    EnteredUnit = null;
                    unitTemp = null;
                }
            }

        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, -y) * cellSize + transform.position + new Vector3(xOrigin, yOrigin);
    }

    public void GetGridIndex(Vector3 wldPos, out int x, out int y)
    {
        Vector3 gridPos;
        gridPos = wldPos - transform.position - new Vector3(xOrigin, yOrigin);
        x = Mathf.RoundToInt(gridPos.x / cellSize);
        y = -Mathf.RoundToInt(gridPos.y / cellSize);
    }

    public void GetGridIndex(Vector3 wldPos, out int x, out int y, out Unit unit)
    {
        Vector3 gridPos;
        gridPos = wldPos - transform.position - new Vector3(xOrigin, yOrigin);
        x = Mathf.RoundToInt(gridPos.x / cellSize);
        y = -Mathf.RoundToInt(gridPos.y / cellSize);
        if (x >= 0 && y >= 0 && x < xDim && y < yDim)
        {
            unit = units[x, y];
        }
        else
        {
            unit = null;
        }

    }

    public Unit SpawnNewUnit(int x, int y, UnitType type)
    {
        GameObject newUnit = Instantiate(unitPrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newUnit.name = "unit(" + x + "," + y + ")";
        newUnit.transform.SetParent(transform);

        units[x, y] = newUnit.GetComponent<Unit>();
        units[x, y].init(x, y, this, type);

        return units[x, y];
    }

    public Unit GetUnit(int x, int y)
    {
        return units[x, y];
    }

    public IEnumerator Fill()
    {
        NeedsRefill = true;

        while (NeedsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                yield return new WaitForSeconds(fillTime);
            }

            ClearAllValidMatiches();
        }
    }

    public bool FillStep()
    {
        bool movedUnit = false;

        for (int y = yDim - 2; y >= 0; y--)//从下向上遍历
        {
            for (int x = 0; x < xDim; x++)
            {
                Unit unit = units[x, y];

                if (unit.IsMovable())
                {
                    //向下搜寻第一个EMPTY进行填充
                    for (int yBelow = y + 1; yBelow < yDim; yBelow++)
                    {
                        Unit unitBelow = units[x, yBelow];

                        if (unitBelow.Type == UnitType.EMPTY)
                        {
                            Destroy(unitBelow.gameObject);
                            unit.MovableComponent.Move(x, yBelow, fillTime);
                            units[x, yBelow] = unit;
                            SpawnNewUnit(x, y, UnitType.EMPTY);
                            movedUnit = true;
                            break;
                        }
                    }
                }
            }

        }


        for (int x = 0; x < xDim; x++)
        {
            Unit unitBelow = units[x, 0];//遍历0行unit，如果是空，在-1行生成新随机元素

            if (unitBelow.Type == UnitType.EMPTY)
            {
                UnitType randomType = (UnitType)ProbChoose.Choose(probabilities);//随机UnitType， 概率都是0会生成normal

                Destroy(unitBelow.gameObject);
                GameObject newUnit = Instantiate(unitPrefabDict[randomType], GetWorldPosition(x, -1), Quaternion.identity);
                newUnit.transform.SetParent(transform);

                units[x, 0] = newUnit.GetComponent<Unit>();
                units[x, 0].init(x, -1, this, randomType);

                if (units[x, 0].IsColored())
                {
                    units[x, 0].ColorComponent.SetColor((UnitColor.ColorType)Random.Range(0, units[x, 0].ColorComponent.NumColors));
                }
                units[x, 0].MovableComponent.Move(x, 0, fillTime);

                movedUnit = true;
            }
        }

        return movedUnit;
    }

    public bool IsAdjacent(Unit unit1, Unit unit2)
    {
        return (unit1.X == unit2.X && (int)Mathf.Abs(unit1.Y - unit2.Y) == 1)
            || (unit1.Y == unit2.Y && (int)Mathf.Abs(unit1.X - unit2.X) == 1);
    }

    public void SwapUnits(Unit unit1, Unit unit2)
    {
        units[unit1.X, unit1.Y] = unit2;
        units[unit2.X, unit2.Y] = unit1;

        int unit1X = unit1.X;
        int unit1Y = unit1.Y;
        unit1.MovableComponent.Move(unit2.X, unit2.Y, fillTime);
        unit2.MovableComponent.Move(unit1X, unit1Y, fillTime);

    }

    public List<Unit> GetMatch(Unit unit, int xNew, int yNew)
    {
        if (unit.IsColored())
        {
            UnitColor.ColorType color = unit.ColorComponent.Color;
            List<Unit> unitsHorizontal = new List<Unit>();
            List<Unit> unitsVertical = new List<Unit>();
            List<Unit> unitsMatching = new List<Unit>();

            //横向检测
            unitsHorizontal.Add(unit);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0)//Left
                    {
                        x = xNew - xOffset;
                    }
                    else//Right
                    {
                        x = xNew + xOffset;
                    }

                    if (x < 0 || x >= xDim)//Out of Range
                    {
                        break;
                    }
                    if (units[x, yNew].IsColored() && units[x, yNew].ColorComponent.Color == color)
                    {
                        unitsHorizontal.Add(units[x, yNew]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (unitsHorizontal.Count >= 3)
            {
                for (int i = 0; i < unitsHorizontal.Count; i++)
                {
                    unitsMatching.Add(unitsHorizontal[i]);

                    //纵向检测
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0)//Up
                            {
                                y = yNew - yOffset;
                            }
                            else//Down
                            {
                                y = yNew + yOffset;
                            }

                            if (y < 0 || y >= yDim)//Out of Range
                            {
                                break;
                            }

                            if (units[unitsHorizontal[i].X, y].IsColored() && units[unitsHorizontal[i].X, y].ColorComponent.Color == color)
                            {
                                unitsMatching.Add(units[unitsHorizontal[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (unitsMatching.Count >= 3)
            {
                return unitsMatching;
            }

            //纵向检测
            unitsVertical.Add(unit);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0)//Up
                    {
                        y = yNew - yOffset;
                    }
                    else//Down
                    {
                        y = yNew + yOffset;
                    }

                    if (y < 0 || y >= yDim)//Out of Range
                    {
                        break;
                    }

                    if (units[xNew, y].IsColored() && units[xNew, y].ColorComponent.Color == color)
                    {
                        unitsVertical.Add(units[xNew, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (unitsVertical.Count >= 3)
            {
                for (int i = 0; i < unitsVertical.Count; i++)
                {
                    unitsMatching.Add(unitsVertical[i]);

                    //横向检测
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0)//Left
                            {
                                x = xNew - xOffset;
                            }
                            else//Right
                            {
                                x = xNew + xOffset;
                            }

                            if (x < 0 || x >= xDim)//Out of Range
                            {
                                break;
                            }
                            if (units[x, unitsVertical[i].Y].IsColored() && units[x, unitsVertical[i].Y].ColorComponent.Color == color)
                            {
                                unitsMatching.Add(units[x, unitsVertical[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (unitsMatching.Count >= 3)
            {
                return unitsMatching;
            }
        }

        return null;
    }

    public List<Unit> GetMatch(Unit unit)
    {
        return GetMatch(unit, unit.X, unit.Y);
    }

    public List<Unit> GetCovered(Char placedChar)
    {
        List<Unit> CoveredUnits = new List<Unit>();
        if (placedChar.IsPlaced)
        {
            foreach (var index in placedChar.GetRange())
            {
                if (index.x >= 0 && index.y >= 0 && index.x < xDim && index.y < yDim)
                {
                    CoveredUnits.Add(units[index.x, index.y]);
                }
            }
        }

        return CoveredUnits;
    }

    public bool ClearAllValidMatiches()
    {
        NeedsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (units[x, y].IsClearable())
                {
                    List<Unit> match = GetMatch(units[x, y], x, y);

                    if (match != null)
                    {
                        //非人为操作的消除，在随机位置生成随机特殊Unit
                        UnitType spUnitType = UnitType.COUNT;
                        Unit randomUnit = match[Random.Range(0, match.Count)];
                        int spUnitX = randomUnit.X;
                        int spUnitY = randomUnit.Y;

                        //连消4个生成回费unit
                        if (match.Count == 4)
                        {
                            spUnitType = UnitType.SP_RECOVER;
                        }

                        //连消5个生成行列清除Unit
                        if (match.Count >= 5)
                        {
                            if (PressedUnit == null || EnteredUnit == null)
                            {
                                spUnitType = (UnitType)Random.Range((int)UnitType.SP_ROW_CLR, (int)UnitType.SP_COL_CLR + 1);
                            }
                            else if (PressedUnit.Y == EnteredUnit.Y)
                            {
                                spUnitType = UnitType.SP_ROW_CLR;
                            }
                            else
                            {
                                spUnitType = UnitType.SP_COL_CLR;
                            }
                        }

                        HashSet<Unit> elites = new HashSet<Unit>();//收集被消除unit周围的elite
                        int spRecoverAmount = 0;

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearUnit(match[i].X, match[i].Y, ref elites, ref spRecoverAmount))//cleared?
                            {
                                
                                NeedsRefill = true;

                                if (match[i] == PressedUnit || match[i] == EnteredUnit)//在交换位置生成特殊unit
                                {
                                    spUnitX = match[i].X;
                                    spUnitY = match[i].Y;
                                }
                            }
                        }

                        CharManager.Currency.Earn(match.Count * spRecoverAmount);//消除的总数量*包含的回复unit的数量 回复cost

                        foreach (Unit elite in elites)
                        {
                            OnePunch(elite);//重拳出击
                        }

                        //生成特殊元素
                        if (spUnitType != UnitType.COUNT)
                        {
                            Destroy(units[spUnitX, spUnitY]);
                            Unit newUnit = SpawnNewUnit(spUnitX, spUnitY, spUnitType);

                            if (spUnitType >= UnitType.SP_RECOVER && spUnitType <= UnitType.SP_COL_CLR
                                && newUnit.IsColored() && match[0].IsColored())
                            {
                                newUnit.ColorComponent.SetColor(match[0].ColorComponent.Color);
                            }
                        }
                    }
                }
            }
        }

        return NeedsRefill;
    }

    //清除unit
    public bool ClearUnit(int x, int y)
    {
        if (units[x, y].IsClearable() && !units[x, y].ClearableComponent.IsBeingCleared)
        {
            units[x, y].ClearableComponent.Clear();
            if (units[x, y].Type == UnitType.SP_RECOVER) CharManager.Currency.Earn(1);
            SpawnNewUnit(x, y, UnitType.EMPTY);

            return true;//cleared
        }

        return false;//not yet
    }

    //清除unit并获取周围相邻的elite
    public bool ClearUnit(int x, int y, ref HashSet<Unit> elites, ref int spRecoverAmount)
    {
        if (units[x, y].IsClearable() && !units[x, y].ClearableComponent.IsBeingCleared)
        {

            for (int xi = x - 1; xi <= x + 1; xi++)
            {
                for (int yi = y - 1; yi <= y + 1; yi++)
                {
                    if (xi >= 0 && xi < xDim && yi >= 0 && yi < yDim)
                    {
                        if (IsAdjacent(units[xi, yi], units[x, y]))
                        {

                            if (units[xi, yi].IsElite())
                            {

                                elites.Add(units[xi, yi]);
                            }
                        }
                    }
                }
            }
            units[x, y].ClearableComponent.Clear();
            if (units[x, y].Type == UnitType.SP_RECOVER) spRecoverAmount += 1;
            SpawnNewUnit(x, y, UnitType.EMPTY);
            return true;//cleared
        }

        return false;//not yet
    }

    public void ClearRaw(int raw)
    {
        for (int x = 0; x < xDim; x++)
        {
            if (units[x, raw].Type != UnitType.SP_Char)
            {
                ClearUnit(x, raw);
            }

        }
    }

    public void ClearColumn(int col)
    {
        for (int y = 0; y < yDim; y++)
        {
            if (units[col, y].Type != UnitType.SP_Char)
            {
                ClearUnit(col, y);
            }

        }
    }

    public void ClearCoveredUnits(Char _char)
    {

        NeedsRefill = false;

        foreach (Unit unit in GetCovered(_char))
        {
            if (unit.IsElite())
            {
                OnePunch(unit);
            }
            else
            {
                ClearUnit(unit.X, unit.Y);
            }

        }

        NeedsRefill = true;
    }

    //对精英怪重拳出击
    public bool OnePunch(Unit elite)
    {
        Effect effector = elite.GetComponentInChildren<Effect>();
        Animator animator = effector.GetComponent<Animator>();

        animator.Play(effector.AttackEffectAnim.name);
        elite.EliteHandler.HealthSystem.Expense(1);

        if (elite.IsClearable())
        {
            return ClearUnit(elite.X, elite.Y);
        }

        return false;
    }

}
