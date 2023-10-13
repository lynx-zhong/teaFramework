(function () {
    'use strict';

    var csInterface = new CSInterface();
    
    
    function init() {
                
        themeManager.init();
        
        //
        $("#btn_output").click(function () {
            csInterface.evalScript('Run()');
        });
        
        //
        $("#btn_ArtStatic").click(function () {
            csInterface.evalScript('changeLayerNameEnd("@ArtStatic")');
        });

        //
        $("#btn_Button").click(function () {
            csInterface.evalScript('changeLayerNameEnd("@Button")');
        });
        $("#btn_Normal").click(function () {
            csInterface.evalScript('changeLayerNameEnd("@Button_Normal")');
        });
        $("#btn_Pressed").click(function () {
            csInterface.evalScript('changeLayerNameEnd("@Button_Pressed")');
        });
        $("#btn_Disabled").click(function () {
            csInterface.evalScript('changeLayerNameEnd("@Button_Disabled")');
        });

        //
        $("#btn_export_folder_name").click(function () {
            csInterface.evalScript('changeLayerName("导出")');
        });
        $("#btn_special_folder_name").click(function () {
            csInterface.evalScript('changeLayerName("特殊处理")');
        });
        $("#btn_ui_common_name").click(function () {
            csInterface.evalScript('changeLayerNameStart("ui_common_")');
        });
        $("#btn_ui_icon_name").click(function () {
            csInterface.evalScript('changeLayerNameStart("icon_")');
        });
    }
        
    init();

}());
    
