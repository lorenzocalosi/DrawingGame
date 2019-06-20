using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextByFloat : MonoBehaviour
{
	private Text text;

	private void Start() {
		text = GetComponent<Text>();
	}

	public void SetTextValueByFloat(float value) {
		text.text = value.ToString();
	}
}
