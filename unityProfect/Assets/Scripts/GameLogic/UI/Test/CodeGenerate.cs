using UnityEngine;
using UnityEngine.UI;

public class CodeGenerate : MonoBehaviour
{
    // field auto generate start
    public GameObject aa;
    public Button ww;
    public Button vv;
    public Image dd_gaiming;
    public Button xinzengAniuNiuGassssaa;
    // field auto generate end


    private void Start()
    {
        // bind function start
        ww.onClick.AddListener(OnwwButtonClick);
        vv.onClick.AddListener(OnvvButtonClick);
        xinzengAniuNiuGassssaa.onClick.AddListener(OnxinzengAniuNiuGassssaaButtonClick);
        // bind function end
    }

    // fucntion auto generate start
    private void OnwwButtonClick()
    {

    }

    private void OnvvButtonClick()
    {

    }

    private void OnxinzengAniuNiuButtonClick()
    {

    }

    private void OnxinzengAniuNiuGassssaaButtonClick()
    {

    }

    // fucntion auto generate end

    private void OnDestroy()
    {
        // unbind function start
        ww.onClick.RemoveListener(OnwwButtonClick);
        vv.onClick.RemoveListener(OnvvButtonClick);
        xinzengAniuNiuGassssaa.onClick.RemoveListener(OnxinzengAniuNiuGassssaaButtonClick);
        // unbind function end
    }
}
