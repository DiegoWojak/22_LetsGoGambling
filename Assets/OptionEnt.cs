using NUnit;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OptionEnt : MonoBehaviour
{
    public Image _Image;
    public Button btnRef;
    public int value;
    public GameObject _ToShow;
    public TextMeshProUGUI text;

    public void CreateEnti(Sprite _sprite, int value)
    {
        _Image.sprite = _sprite;
        this.value = value;
        _ToShow.SetActive(false);
        btnRef.onClick.AddListener(() => {
            _ToShow.SetActive(true);
            
            LeanTween.scale(_ToShow, Vector3.one * 1.1f, 0.5f).setLoopPingPong();
        });

        text.text = value.ToString();
    }
}
[CreateAssetMenu]
public class Entity: ScriptableObject
{
    public Sprite _Sprite;
    public int Value;
}
