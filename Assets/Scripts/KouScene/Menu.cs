using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
	//　最初にフォーカスするゲームオブジェクト
	[SerializeField]
	private GameObject firstSelect;

	void Start()
    {
		EventSystem.current.SetSelectedGameObject(firstSelect);
	}
}
