using UnityEngine;
using UnityEngine.UI;

public class ImageCycler : MonoBehaviour
{
    public Sprite[] sprites;
    public Image targetImage;
    public float interval = 1.0f;

    int currentIndex = 0;
    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            currentIndex = (currentIndex + 1) % sprites.Length;
            targetImage.sprite = sprites[currentIndex];
            timer = 0f;
        }
    }
}