using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    private Transform plyer;
    private RawImage compassImage;
    void Start()
    {
        plyer = GameObject.FindGameObjectWithTag(TagName.Player).transform;
        compassImage = gameObject.GetComponent<RawImage>();
    }
    void Update()
    {
        //  プレイヤーの回転に応じてコンパスイメージを回転
        compassImage.uvRect = new Rect(plyer.transform.localEulerAngles.y / 360, 0.0f, 1f, 1f);
    }
}
