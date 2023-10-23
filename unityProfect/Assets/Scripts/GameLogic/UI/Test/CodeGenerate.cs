using UnityEngine;
using UnityEngine.UI;

public class CodeGenerate
{
    // file auto generate start
    public CanvasRenderer CodeGenerateCanvasRenderer;
    public GameObject aaGameObject;
    public RectTransform sddRectTransform;
    public Button AaaButton;
    public Button BbbButton;
    // file auto generate end


    private void Start()
    {
        // bind function start
        AaaButton.onClick.AddListener(OnAaaButtonButtonClick);
        BbbButton.onClick.AddListener(OnBbbButtonButtonClick);
        // bind function end
    }

    // fucntion auto generate start

    private void OnAaaButtonButtonClick()
    {

    }

    private void OnBbbButtonButtonClick()
    {

    }

    // fucntion auto generate end

    private void OnDestory()
    {
        // unbind function start
        AaaButton.onClick.AddListener(OnAaaButtonButtonClick);
        BbbButton.onClick.AddListener(OnBbbButtonButtonClick);
        // unbind function end
    }
}
