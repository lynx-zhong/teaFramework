using UnityEngine;
using UnityEngine.UI;

public class Aaa
{
    #region field auto generate start
    public Button CodeGenerateButton;
    public Button newaaa;
    public Image dd_gaiming;
    public Button xinzengAniuNiuGassssaa;
    public CanvasRenderer ButtonCanvasRenderer;
    #endregion field auto generate end
}

public class CodeGenerate : Aaa
{
    private void Start()
    {
        // bind function start
        CodeGenerateButton.onClick.AddListener(OnCodeGenerateButtonButtonClick);
        newaaa.onClick.AddListener(OnnewaaaButtonClick);
        xinzengAniuNiuGassssaa.onClick.AddListener(OnxinzengAniuNiuGassssaaButtonClick);
        // bind function end
    }

    // fucntion auto generate start
    private void OnCodeGenerateButtonButtonClick()
    {

    }

    private void OnnewaaaButtonClick()
    {

    }

    private void OnxinzengAniuNiuGassssaaButtonClick()
    {

    }

    // fucntion auto generate end

    private void OnDestroy()
    {
        // unbind function start
        CodeGenerateButton.onClick.RemoveListener(OnCodeGenerateButtonButtonClick);
        newaaa.onClick.RemoveListener(OnnewaaaButtonClick);
        xinzengAniuNiuGassssaa.onClick.RemoveListener(OnxinzengAniuNiuGassssaaButtonClick);
        // unbind function end
    }
}
