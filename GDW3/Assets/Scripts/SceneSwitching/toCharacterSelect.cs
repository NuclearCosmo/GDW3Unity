﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class toCharacterSelect : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = rayResult();
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name == this.gameObject.name)
                {
                    SceneManager.LoadScene("CharacterSelect");
                }
            }
        }
    }
    static List<RaycastResult> rayResult()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> rayRes = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rayRes);
        return rayRes;
    }

}