using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    private Char _char;
    private Vector3 vec3CharInScr;// 目标物体的屏幕空间坐标  
    private Vector3 vec3CharInWld;// 目标物体的世界空间坐标  
    private Vector3 vec3MouseInScr;// 鼠标的屏幕空间坐标  
    private Vector3 vec3Offset;// 偏移  


    public AnimationClip clearAnimation;

    public IEnumerator Hover { get; private set; }

    private void Awake()
    {
        _char = GetComponent<Char>();

        Hover = HoverCoroutine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !_char.IsPlaced)
        {
            _char.CharManager.ReleaseChar(_char);
        }
        else if (_char.IsPlaced && _char.Direction != Char.Dir.NONE && !_char.IsAttackting)
        {
            StartCoroutine(CharAttack());
        }
    }

    private IEnumerator HoverCoroutine()
    {
        vec3CharInScr = Camera.main.WorldToScreenPoint(transform.position);
        vec3MouseInScr = new Vector3(Input.mousePosition.x, Input.mousePosition.y, vec3CharInScr.z);
        vec3Offset = transform.position - Camera.main.ScreenToWorldPoint(vec3MouseInScr);

        while (Input.GetMouseButton(0))
        {
            vec3MouseInScr = new Vector3(Input.mousePosition.x, Input.mousePosition.y, vec3CharInScr.z);
            vec3CharInWld = Camera.main.ScreenToWorldPoint(vec3MouseInScr) + vec3Offset;
            transform.position = vec3CharInWld;

            yield return new WaitForFixedUpdate();
        }

        yield break;

    }

    public void Move(int xIdx, int yIdx)
    {
        Vector3 endPos = _char.CharManager.GridRef.GetWorldPosition(xIdx, yIdx);
        transform.position = endPos;
    }

    public IEnumerator CharAttack()
    {
        _char.IsAttackting = true;
        Transform animTf = transform.Find("Anim");
        Effect effector = animTf.Find("AtkEff").GetComponent<Effect>();
        Animator animEff = effector.GetComponent<Animator>();

        animTf.rotation = Quaternion.Euler(Vector3.forward * 90 * ((int)_char.Direction - 1));

        for (int i = 0; i < _char.GetAtkTimes(); i++)
        {
            if (animEff)
            {
                animEff.Play(effector.AttackEffectAnim.name, 0, 0f);

                yield return new WaitForSeconds(effector.AttackEffectAnim.length);
            }

            _char.CharManager.GridRef.ClearCoveredUnits(_char);

            yield return StartCoroutine(_char.CharManager.GridRef.Fill());
        }

        _char.CharManager.ClearChar(_char);
    }

    public void DirSelectUI(bool act)
    {
        transform.Find("DirSelectUI").gameObject.SetActive(act);
    }

    public void Towards(GameObject clickedDir)
    {
        switch (clickedDir.name)
        {
            case "dir_1":
                _char.Direction = Char.Dir.UP;
                break;
            case "dir_2":
                _char.Direction = Char.Dir.LEFT;
                break;
            case "dir_3":
                _char.Direction = Char.Dir.DOWN;
                break;
            case "dir_4":
                _char.Direction = Char.Dir.RIGHT;
                break;
        }

        DirSelectUI(false);
    }

    public virtual void Clear()
    {
        _char.IsBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearAnimation.name);

            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }

    }
}
