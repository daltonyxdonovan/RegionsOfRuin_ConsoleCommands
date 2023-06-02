using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200008E RID: 142
public class customInteractInfo : MonoBehaviour
{
	// Token: 0x06000522 RID: 1314 RVA: 0x00046990 File Offset: 0x00044D90
	private void Awake()
	{
		customInteractInfo.interactInfo = this;
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00046998 File Offset: 0x00044D98
	private void Start()
	{
        int screen_height = Screen.height;
        int screen_width = Screen.width;
		this.info = this.infoText.gameObject;
		this.infoText.color = this.offColor;
		this.origin = new Vector3((int)screen_width/2, screen_height-(int)screen_height/8, 0);
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x000469D8 File Offset: 0x00044DD8
	public void inform(string msg, Color col)
	{
		msg = locHandler.getMiscTranslation(msg);
		this.info.transform.position = this.origin;
		base.StartCoroutine(this.showInfo());
		base.StartCoroutine(this.moveInfo());
		this.infoText.color = col;
		this.infoText.text = msg;
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00046A3A File Offset: 0x00044E3A
	public void showCleared()
	{
		if (dayCycleController.underground)
		{
			this.clearText.color = Color.white;
		}
		else
		{
			this.clearText.color = Color.black;
		}
		this.clear.SetActive(true);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00046A78 File Offset: 0x00044E78
	private IEnumerator moveInfo()
	{
		Vector2 newPos = this.info.transform.position;
		this.i = 0;
		while (this.i < 50)
		{
			newPos.y += 0f;
			this.info.transform.position = newPos;
			yield return new WaitForSeconds((this.timer + 0.35f) / 50f);
			this.i++;
		}
		yield break;
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00046A94 File Offset: 0x00044E94
	private IEnumerator showInfo()
	{
		yield return new WaitForSeconds(this.timer);
		Color fader = this.infoText.color;
		fader.a -= 0.25f;
		this.infoText.color = fader;
		yield return new WaitForSeconds(0.05f);
		fader.a -= 0.25f;
		this.infoText.color = fader;
		yield return new WaitForSeconds(0.05f);
		fader.a -= 0.25f;
		this.infoText.color = fader;
		yield return new WaitForSeconds(0.05f);
		fader.a -= 0.25f;
		this.infoText.color = fader;
		yield break;
	}

	// Token: 0x0400078D RID: 1933
	public GameObject clear;

	// Token: 0x0400078E RID: 1934
	public Text clearText;

	// Token: 0x0400078F RID: 1935
	private Vector2 origin;

	// Token: 0x04000790 RID: 1936
	private float timer = 1.25f;

	// Token: 0x04000791 RID: 1937
	private GameObject info;

	// Token: 0x04000792 RID: 1938
	public Text infoText;

	// Token: 0x04000793 RID: 1939
	private Color offColor = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04000794 RID: 1940
	public static customInteractInfo interactInfo;

	// Token: 0x04000795 RID: 1941
	private int i;
}