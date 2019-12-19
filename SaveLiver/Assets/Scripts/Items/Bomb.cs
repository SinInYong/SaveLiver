﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ItemManager, IItem
{
    public float itemDuration = 2f;
    private float bombItemTime = 0f;
    private bool hasItem = false;

    public Animator anim;

    public GameObject boomEffect;

    void Start()
    {
        StartCoroutine("TimeCheckAndDestroy");
    }

    void Update()
    {
        ItemDurationAndDestroy();
    }


    /**************************************
    * @ Shield와 동일
    */
    private void ItemDurationAndDestroy()
    {
        if (Time.time - bombItemTime >= itemDuration && hasItem)
        {
            hasItem = false;
            Instantiate(boomEffect, transform.position, Quaternion.identity); // 폭발효과
            Destroy(gameObject);
        }
    }


    /**************************************
    * @함수명: Use
    * @작성자: zeli
    * @입력: void
    * @출력: void
    * @설명: 폭탄 아이템과 충돌 시 발동
    *        Coliider를 꺼줌
    *        폭탄에 불을 붙히는 애니메이션 실행
    */
    public void Use()
    {
        anim.SetBool("Fire", true);
        GetComponent<Collider2D>().enabled = false;
        bombItemTime = Time.time;
        hasItem = true;
    }


    /**************************************
    * @ Shield와 동일
    */
    IEnumerator TimeCheckAndDestroy()
    {
        yield return new WaitForSeconds(itemLifeTime);
        if (!hasItem)
        {
            Destroy(gameObject);
        }
    }
}
