using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDescriptionView : UIView
{
    [SerializeField] private Image imageView;
    [SerializeField] private TMP_Text descriptionView;

    private string description = null;
    private Sprite sprite = null;

    public void Initialize(string description)
    {
        this.description = description;
    }

    public void Initialize(Sprite sprite)
    {
        this.sprite = sprite;
    }

    private void Start()
    {
        if(sprite != null)
            imageView.sprite = sprite;
        if(description != null)
            descriptionView.text = description;
    }
}
