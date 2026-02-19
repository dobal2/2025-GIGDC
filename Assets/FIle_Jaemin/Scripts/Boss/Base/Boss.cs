using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Boss : Monster
{

    [SerializeField] protected float phase2Hp;
    protected int currentPhase = 1;
    protected bool battleStarted = false;

    [SerializeField] protected RuntimeAnimatorController phase2Anim;

    [Header("HP Bar")]
    [SerializeField] protected GameObject hpBarPrefab;
    protected Slider hpSlider;

    public static Boss Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void StartBattle()
    {
        battleStarted = true;
    }
    
    protected override void Start()
    {
        counterTextPrefab = null;
        base.Start();

        if (hpBarPrefab != null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            Canvas overlayCanvas = null;

            foreach (var canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    overlayCanvas = canvas;
                    break;
                }
            }

            if (overlayCanvas != null)
            {
                GameObject hpBarObj = Instantiate(hpBarPrefab, overlayCanvas.transform);
                hpSlider = hpBarObj.GetComponentInChildren<Slider>();

                if (hpSlider != null)
                {
                    hpSlider.maxValue = maxHp;
                    hpSlider.value = hp;
                }
            }
        }

        StartBattle();
    }

    public override void KnockBack(Transform attacker, float knockBackForce, float knockBackAngle, float duration)
    {
        
    }


    protected virtual void Update()
    {
        if (hp <= 0) Die();
    }

    protected abstract override void Die();
    
    public override void TakeDamage(float amount)
    {
        hp -= amount;

        if (hpSlider != null)
        {
            hpSlider.value = hp;
        }

        if (hp <= 0) Die();
    }

    public void UpdateHPBar()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }
    }
    


}
