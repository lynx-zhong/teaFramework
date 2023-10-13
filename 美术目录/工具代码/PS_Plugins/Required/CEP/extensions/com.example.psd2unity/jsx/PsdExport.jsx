// #target photoshop 
// 项目常量定义
// GetPngSavePath
// GetXmlPath

// 规则定义
var prefabLayerName = "导出"
var resourcesLayerName = "特殊处理"
var commonUINameStart = "ui_common_"
var iconUINameStart = "icon_"
var commonPngFolderName = "aUICommon"

// 字段定义
var xmlStr;
var duppedPsd;
var sourcePsdName;
var slicePaddingArr = new Array(0, 0, 0, 0);
var sliceOriArr = new Array(0, 0, 0, 0);
var resourcesSliceDic = new Array();
var logStr;

// Run()

function Run() {
    // logStr = "";

    if (!CheckIsHaveExportLayer())
        return;

    sourcePsdName = app.activeDocument.name;
    duppedPsd = app.activeDocument.duplicate();

    //
    CollectResourcesData();

    //
    HideAllLayer();

    //
    ExportAllLayer();

    //
    duppedPsd.close(SaveOptions.DONOTSAVECHANGES);

    //
    DelectedAllNotUsePng()

    // ShowMessageBox("导出完成");

    // WriteLog();
}

function DelectedAllNotUsePng() {
    var psdPath = app.documents[sourcePsdName].fullName.toString();
    psdPath = psdPath.replace(app.documents[sourcePsdName].name, "");

    var usePngDic = {}
    var xmlFolderPath = psdPath + "XmlFolder"
    var xmlFolder = new Folder(xmlFolderPath)
    if (xmlFolder.exists) {
        var xmlFiles = xmlFolder.getFiles("*.xml")
        for (var i = 0; i < xmlFiles.length; i++) {
            var xmlFile = xmlFiles[i]

            var xmlContent = ""
            var xmlFileObj = new File(xmlFile);
            xmlFileObj.open("r")
            while (!xmlFileObj.eof) {
                xmlContent += xmlFileObj.readln()
            }
            xmlFileObj.close()

            var xml = new XML(xmlContent)
            var imageNodes = xml..*.(attribute("BaseType") == "Image")

            for (var j = 0; j < imageNodes.length(); j++) {
                var imageNode = imageNodes[j]
                var nameAttribute = imageNode.@name.toString()
                usePngDic[nameAttribute] = nameAttribute
            }
        }
    }

    //
    var pngFolderPath = psdPath + "PngFolder"
    var pngFolder = new Folder(pngFolderPath)
    if (pngFolder.exists) {
        var pngFiles = pngFolder.getFiles("*.png")
        for (var i = 0; i < pngFiles.length; i++) {
            if (pngFiles[i] instanceof File) {
                var fileName = pngFiles[i].displayName.replace(/.png/g, "");
                if (!usePngDic.hasOwnProperty(fileName)) {
                    pngFiles[i].remove()
                }
            }
        }
    }
}

function HideAllLayer() {
    for (var index = 0; index < duppedPsd.layers.length; index++) {
        var element = duppedPsd.layers[index];
        SetAllLayerVisible(element, false);
    }
}

function CheckIsHaveExportLayer() {
    if (app.documents.length <= 0) {
        ShowMessageBox("未有打开的psd 文件");
        return false;
    }

    for (var index = 0; index < app.activeDocument.layers.length; index++) {
        var curLayer = app.activeDocument.layers[index];
        if (!curLayer.allLocked && curLayer.name == prefabLayerName) {
            return true;
        }
    }

    ShowMessageBox("根节点未检测到有: " + prefabLayerName + " 图层组");
    return false;
}

function CollectResourcesData() {
    var ResourcesLayer;
    for (var index = 0; index < duppedPsd.layers.length; index++) {
        var layerName = duppedPsd.layers[index].name;
        if (layerName == resourcesLayerName) {
            ResourcesLayer = duppedPsd.layers[index];
            break;
        }
    }

    if (ResourcesLayer == undefined)
        return;

    for (var index = 0; index < ResourcesLayer.layers.length; index++) {
        var curLayer = ResourcesLayer.layers[index];
        if (curLayer.allLocked) {
            continue;
        } else if (curLayer.typename == "LayerSet") {
            alert("Resources 中不能有图层组，图层组名字： " + curLayer.name);
        } else if (curLayer.typename == "ArtLayer") {
            duppedPsd.activeLayer = curLayer;
            if (HasLayerMask()) {
                var layerName = GetLayerName(curLayer);
                resourcesSliceDic[layerName] = GetSliceAreaParam(curLayer);

                // Log("");
            }
        }
    }
}

function ExportAllLayer() {
    for (var index = 0; index < duppedPsd.layers.length; index++) {
        var layerName = GetLayerName(duppedPsd.layers[index])
        if (layerName == prefabLayerName) {
            Export(duppedPsd.layers[index])
            break
        }
    }
}

function Export(curLayer) {
    var fileName = app.documents[sourcePsdName].name.replace(".psd", "")
    var pngFolderPath = app.documents[sourcePsdName].fullName.fsName.replace(app.documents[sourcePsdName].name, "PngFolder")

    var parts = pngFolderPath.split("\\")
    pngFolderPath = "\\" + parts.slice(parts.length - 3).join("\\")

    var filePath = app.documents[sourcePsdName].fullName
    var folderPath = filePath.parent
    var folderName = folderPath.displayName

    xmlStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
    xmlStr += "\n<RootNode prefabName=\"" + fileName + "\"" + " moduleName=\"" + folderName + "\"" + " pngFolderPath=\"" + pngFolderPath + "\"" + " commonUIFolderPath=\"\\Psd目录\\" + commonPngFolderName + "\"" + 
        " IconUIMark=\"" + iconUINameStart + "\"" +
        " CommonUIMark=\"" + commonUINameStart + "\"" + ">";

    ExportLayer(curLayer);

    xmlStr += "\n</RootNode>";

    var xmlPath = GetXmlPath();
    var xmlFile = new File(xmlPath + ".xml");
    xmlFile.encoding = "utf-8";
    xmlFile.open('w');
    xmlFile.writeln(xmlStr);
    xmlFile.close();
}

function ExportLayer(curLayer) {
    if (curLayer.allLocked)
        return;

    if (curLayer.typename == "LayerSet") {
        var layerName = GetLayerName(curLayer);

        var laysetCustomStr = GetLayerSetExportParam(curLayer)

        xmlStr += "\n<Layer name=\"" + layerName + "\" " + laysetCustomStr + ">";

        for (var index = 0; index < curLayer.layers.length; index++) {
            var childLayer = curLayer.layers[index];
            ExportLayer(childLayer);
        }

        xmlStr += "\n</Layer>"
    } else if (curLayer.typename == "ArtLayer") {
        ExportArtLayer(curLayer);
    }
}

function GetLayerSetExportParam(targetLayer) {
    var str = ""

    if (targetLayer.name.search("@Button") >= 0 && targetLayer.name.search("@Button_") < 0) {
        str = str + "ComponentType=\"Button\" "
    }

    return str
}

function ExportArtLayer(curLayer) {
    if (curLayer.kind == LayerKind.TEXT) {
        ExportText(curLayer);
    } else {
        ExportImage(curLayer);
    }
}

function ExportText(targetLayer) {
    if (targetLayer.name.search("@ArtStatic") >= 0) {
        ExportImage(targetLayer)
        return
    }

    targetLayer.visible = true
    var rect = GetNodeRect(duppedPsd.duplicate())
    targetLayer.visible = false

    var textInfo = targetLayer.textItem;

    var color = textInfo.color.rgb.hexValue;
    var layerName = GetLayerName(targetLayer);

    var justification = "Justification.CENTER"
    try {
        justification = textInfo.justification;
    } catch (e) {}

    var Bold = false
    try {
        Bold = textInfo.fauxBold
    } catch (e) {}

    var Italic = false
    try {
        Italic = textInfo.fauxItalic
    } catch (e) {}

    var font = "undefine"
    try {
        font = textInfo.font
    } catch (e) {}

    var Underline = false
    try {
        Underline = textInfo.underline != UnderlineType.UNDERLINEOFF
    } catch (e) {}

    var Strikethrough = false
    try {
        Strikethrough = textInfo.strikeThru != StrikeThruType.STRIKEOFF
    } catch (e) {}

    var customStr = GetArtLayerCustomXmlString(targetLayer)
    xmlStr += "\n<Layer name=\"" + layerName + "\" Rect=\"" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + "\" BaseType=\"Lable\" " +
        "Font=\"" + font + "\"" + " Content=\"" + textInfo.contents + "\"" + " FontSize=\"" + Math.floor(textInfo.size.value) + "\"" + " Color=\"#" + color + "\"" + " Alignment=\"" + justification + "\"" + " Alpha=\"" + targetLayer.opacity + "\"" +
        " Bold=\"" + Bold + "\"" + " Italic=\"" + Italic + "\"" + " Underline=\"" + Underline + "\"" + " Strikethrough=\"" + Strikethrough + "\"" + customStr + ">" +
        "\n</Layer>"
}

function GetNodeRect(psd) {
    var height = psd.height.value
    var width = psd.width.value
    var top = psd.height.value
    var left = psd.width.value

    psd.trim(TrimType.TRANSPARENT, true, true, false, false)
    top -= psd.height.value
    left -= psd.width.value

    psd.trim(TrimType.TRANSPARENT)
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    top = -(top - (height / 2))
    left -= (width / 2)

    height = psd.height.value
    width = psd.width.value

    var rec = {
        y: top,
        x: left,
        width: width,
        height: height
    }

    psd.close(SaveOptions.DONOTSAVECHANGES)
    return rec
}

function ExportImage(targetLayer) {
    targetLayer.visible = true;

    var layerName = GetLayerName(targetLayer);
    if (resourcesSliceDic.hasOwnProperty(layerName)) {
        ExportSliceImage(targetLayer, layerName);
    } else if (targetLayer.name.search("@LeftHalf") >= 0) {
        ExportLeftHalf(targetLayer, layerName);
    } else if (targetLayer.name.search("@BottomHalf") >= 0) {
        ExportBottomHalf(targetLayer, layerName);
    } else {
        ExportNormalImage(targetLayer, layerName);
    }

    targetLayer.visible = false;
}

function ExportNormalImage(targetLayer, layerName) {
    var rect = ExportPngToDisk(duppedPsd.duplicate(), layerName);
    var customStr = GetArtLayerCustomXmlString(targetLayer)

    xmlStr += "\n<Layer name=\"" + layerName + "\"" + " Rect=\"" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + "\"" + " BaseType=\"Image\"" +
        " ImageType=\"Simple\"" + " Alpha=\"" + targetLayer.opacity + "\"" + customStr + ">" +
        "\n</Layer>";
}

function ExportSliceImage(targetLayer, layerName) {
    var doc = duppedPsd.duplicate()
    var _obj = doc.activeLayer

    var rect = GetLayerSize(doc)

    var stemGroup = doc.layerSets.add();
    stemGroup.name = layerName

    doc.mergeVisibleLayers();
    trim(doc);

    var width = doc.width;
    var height = doc.height;
    var nums = resourcesSliceDic[layerName];
    for (var j = 0; j < slicePaddingArr.length; j++) {
        sliceOriArr[j] = num;
        var num = parseInt(nums[j])
        if (num == 0) {
            if ((j + 1) % 2 == 0)
                num = parseInt(height / 2)

            else
                num = parseInt(width / 2)
        }
        slicePaddingArr[j] = num
    }

    var _obj = doc.activeLayer

    var selRegion = Array(
        Array(Array(0, slicePaddingArr[1]), Array(0, 0), Array(slicePaddingArr[0], 0), Array(slicePaddingArr[0], slicePaddingArr[1])),
        Array(Array(width - slicePaddingArr[2], slicePaddingArr[1]), Array(width - slicePaddingArr[2], 0), Array(width, 0), Array(width, slicePaddingArr[1])),
        Array(Array(0, height), Array(0, height - slicePaddingArr[3]), Array(slicePaddingArr[0], height - slicePaddingArr[3]), Array(slicePaddingArr[0], height)),
        Array(Array(width - slicePaddingArr[2], height), Array(width - slicePaddingArr[2], height - slicePaddingArr[3]), Array(width, height - slicePaddingArr[3]), Array(width, height)),
    );

    for (var i = 0; i < selRegion.length; i++) {
        doc.activeLayer = _obj;
        doc.selection.select(selRegion[i]);
        executeAction(charIDToTypeID("CpTL"));
        var newStem = doc.activeLayer;
        newStem.name = layerName;
        var deltaX = 0;
        var deltaY = 0;
        if (selRegion[i][0][0] != 0) {
            deltaX = -(width - slicePaddingArr[0] - slicePaddingArr[2]);
        }
        if (selRegion[i][1][1] != 0) {
            deltaY = -(height - slicePaddingArr[1] - slicePaddingArr[3]);
        }
        newStem.translate(deltaX, deltaY);
    }

    _obj.visible = false;
    doc.mergeVisibleLayers();
    trim(doc);

    ExportPngToDisk(doc, layerName);

    var customStr = GetArtLayerCustomXmlString(targetLayer)
    xmlStr += "\n<Layer name=\"" + layerName + "\"" + " Rect=\"" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + "\"" + " BaseType=\"Image\"" +
        " ImageType=\"Sliced\"" + " SpriteBorder=\"" + nums + "\"" + " Alpha=\"" + targetLayer.opacity + "\"" + customStr + ">" +
        "\n</Layer>";
}

function HasLayerMask() {
    var hasLayerMask = false;
    try {
        var ref = new ActionReference();
        var keyUserMaskEnabled = app.charIDToTypeID('UsrM');
        ref.putProperty(app.charIDToTypeID('Prpr'), keyUserMaskEnabled);
        ref.putEnumerated(app.charIDToTypeID('Lyr '), app.charIDToTypeID('Ordn'), app.charIDToTypeID('Trgt'));

        var desc = executeActionGet(ref);
        if (desc.hasKey(keyUserMaskEnabled)) {
            hasLayerMask = true;
        }
    } catch (e) {
        hasLayerMask = false;
    }

    return hasLayerMask;
}

function GetSliceAreaParam(curLayer) {
    selectionFromLayerMask();

    var retParam = new Array();
    var selection = app.activeDocument.selection;
    if (selection != undefined) {
        selection.invert();

        retParam[0] = Math.max(selection.bounds[0].value - curLayer.bounds[0].value, 0);
        retParam[1] = Math.max(selection.bounds[1].value - curLayer.bounds[1].value, 0);
        retParam[2] = Math.max(curLayer.bounds[2].value - selection.bounds[2].value, 0);
        retParam[3] = Math.max(curLayer.bounds[3].value - selection.bounds[3].value, 0);

        // 左 上 右 下
        selection.deselect();
    }

    return retParam;
}

function selectionFromLayerMask(layerName, isTarget) {
    var idsetd = charIDToTypeID("setd");
    var desc384 = new ActionDescriptor();
    var idnull = charIDToTypeID("null");
    var ref213 = new ActionReference();
    var idChnl = charIDToTypeID("Chnl");
    var idfsel = charIDToTypeID("fsel");
    ref213.putProperty(idChnl, idfsel);
    desc384.putReference(idnull, ref213);
    var idT = charIDToTypeID("T   ");
    var ref214 = new ActionReference();
    var idChnl = charIDToTypeID("Chnl");
    var idChnl = charIDToTypeID("Chnl");
    var idMsk = charIDToTypeID("Msk ");
    ref214.putEnumerated(idChnl, idChnl, idMsk);

    if (!isTarget) {
        if (layerName) {
            ref214.putName(charIDToTypeID("Lyr "), layerName);
        }
    } else {
        var ref174 = new ActionReference();
        var idLyr = charIDToTypeID("Lyr ");
        var idOrdn = charIDToTypeID("Ordn");
        var idTrgt = charIDToTypeID("Trgt");
        ref214.putEnumerated(idLyr, idOrdn, idTrgt);
    }
    desc384.putReference(idT, ref214);
    executeAction(idsetd, desc384, DialogModes.NO);
}

function ExportLeftHalf(_layer, layerName) {
    duppedPsd.activeLayer = _layer
    var rectPos = GetLayerSize(duppedPsd)

    var psd = duppedPsd.duplicate();
    // psd.mergeVisibleLayers();

    trim(psd);

    var width = psd.width;
    var height = psd.height;
    var side = width / 2;

    var tempLayer = psd.activeLayer

    var region = Array(Array(0, height), Array(0, 0), Array(side, 0), Array(side, height));
    var selectRect = psd.selection.select(region);
    psd.selection.copy();
    var newStem = psd.paste();
    newStem.name = layerName;

    tempLayer.visible = false
    // psd.mergeVisibleLayers();

    trim(psd);
    newStem.translate(1, 0);

    // 导出
    var pngFile = new File(GetPngSavePath() + layerName + ".png");
    var pngSaveOptions = new ExportOptionsSaveForWeb();
    pngSaveOptions.format = SaveDocumentType.PNG;
    pngSaveOptions.PNG8 = false;
    psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);

    var customStr = GetArtLayerCustomXmlString(_layer)
    xmlStr += "\n<Layer name=\"" + layerName + "\" " + "Type=\"Image\" ImageType=\"LeftHalf\" " + customStr + "RectPos=\"" + rectPos.x + "," + rectPos.y + "," + rectPos.width + "," + rectPos.height + "\" Alpha=\"" + _layer.opacity + "\">\n</Layer>";

    psd.close(SaveOptions.DONOTSAVECHANGES);
}

function ExportBottomHalf(_layer, layerName) {
    duppedPsd.activeLayer = _layer
    var rectPos = GetLayerSize(duppedPsd)

    var psd = duppedPsd.duplicate();
    // psd.mergeVisibleLayers();

    trim(psd);

    var width = psd.width;
    var height = psd.height;

    var tempLayer = psd.activeLayer

    var region = Array(Array(0, height), Array(0, height / 2), Array(width, height / 2), Array(width, height));
    var selectRect = psd.selection.select(region);
    psd.selection.copy();
    var newStem = psd.paste();
    newStem.name = layerName;

    tempLayer.visible = false
    // psd.mergeVisibleLayers();

    trim(psd);

    newStem.translate(0, -1);

    // 导出
    var pngFile = new File(GetPngSavePath() + layerName + ".png");
    var pngSaveOptions = new ExportOptionsSaveForWeb();
    pngSaveOptions.format = SaveDocumentType.PNG;
    pngSaveOptions.PNG8 = false;
    psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);

    var customStr = GetArtLayerCustomXmlString(_layer)
    xmlStr += "\n<Layer name=\"" + layerName + "\" " + "Type=\"Image\" ImageType=\"BottomHalf\" " + customStr + " RectPos=\"" + rectPos.x + "," + rectPos.y + "," + rectPos.width + "," + rectPos.height + "\" Alpha=\"" + _layer.opacity + "\">\n</Layer>";

    psd.close(SaveOptions.DONOTSAVECHANGES);
}

function GetArtLayerCustomXmlString(curLayer) {
    var str = ""

    if (curLayer.name.search("@Button") >= 0 && curLayer.name.search("@Button_") < 0) {
        str = str + " ComponentType=\"Button\" "
    }
    if (curLayer.name.search("@Button_Normal") >= 0) {
        str = str + " ButtonStatus=\"Button_Normal\" "
    }
    if (curLayer.name.search("@Button_Highlighted") >= 0) {
        str = str + " ButtonStatus=\"Button_Highlighted\" IsNotExport=\"true\" "
    }
    if (curLayer.name.search("@Button_Pressed") >= 0) {
        str = str + " ButtonStatus=\"Button_Pressed\" IsNotExport=\"true\" "
    }
    if (curLayer.name.search("@Button_Selected") >= 0) {
        str = str + " ButtonStatus=\"Button_Selected\" IsNotExport=\"true\" "
    }
    if (curLayer.name.search("@Button_Disabled") >= 0) {
        str = str + " ButtonStatus=\"Button_Disabled\" IsNotExport=\"true\" "
    }

    return str
}

function SetAllLayerVisible(_layer, isOpen) {
    if (_layer.typename == "LayerSet") {
        for (var index = 0; index < _layer.layers.length; index++) {
            var tempLayer = _layer.layers[index];
            SetAllLayerVisible(tempLayer, isOpen);
        }
    } else {
        _layer.visible = isOpen;
    }
}

function ExportPngToDisk(psd, layerName) {
    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    top -= psd.height.value;
    left -= psd.width.value;

    psd.trim(TrimType.TRANSPARENT);
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;

    var rec = {
        y: top,
        x: left,
        width: width,
        height: height
    }

    if (layerName.search(iconUINameStart) >= 0) {
        // 不导出
    } else if (layerName.search(commonUINameStart) >= 0) {
        var pngFile = new File(GetCommonPngSavePath() + layerName + ".png");
        var pngSaveOptions = new ExportOptionsSaveForWeb();
        pngSaveOptions.format = SaveDocumentType.PNG;
        pngSaveOptions.PNG8 = false;
        psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);
    } else {
        var pngFile = new File(GetPngSavePath() + layerName + ".png");
        var pngSaveOptions = new ExportOptionsSaveForWeb();
        pngSaveOptions.format = SaveDocumentType.PNG;
        pngSaveOptions.PNG8 = false;
        psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);
    }

    psd.close(SaveOptions.DONOTSAVECHANGES);
    return rec;
}

function GetLayerSize(originalPsd) {
    var psd = originalPsd.duplicate()

    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    top -= psd.height.value;
    left -= psd.width.value;

    psd.trim(TrimType.TRANSPARENT);
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;

    var rec = {
        y: top,
        x: left,
        width: width,
        height: height
    };

    psd.close(SaveOptions.DONOTSAVECHANGES);

    return rec;
}

function GetCommonPngSavePath() {
    var folderPath = app.documents[sourcePsdName].path.parent.fsName + "/" + commonPngFolderName + "/"

    var _folder = new Folder(folderPath);
    if (!_folder.exists) {
        _folder.create();
    }

    return folderPath;
}

function GetPngSavePath() {
    var psdPath = app.documents[sourcePsdName].fullName.toString();
    psdPath = psdPath.replace(app.documents[sourcePsdName].name, "");

    var folderPath = psdPath + "PngFolder/";

    var _folder = new Folder(folderPath);
    if (!_folder.exists) {
        _folder.create();
    }

    return folderPath;
}

function GetXmlPath() {
    var psdPath = app.documents[sourcePsdName].fullName.toString();
    psdPath = psdPath.replace(app.documents[sourcePsdName].name, "");

    var xmlFolderPath = psdPath + "XmlFolder";
    var xmlFolder = new Folder(xmlFolderPath);

    if (!xmlFolder.exists) {
        xmlFolder.create();
    }

    return xmlFolderPath + "/" + app.documents[sourcePsdName].name.replace(".psd", "");
}

function GetLayerName(_layer) {
    var tempName = _layer.name;

    if (_layer.name.indexOf("@") != -1) {
        tempName = _layer.name.substring(0, tempName.indexOf("@"));
    }

    return tempName;
}

function trim(doc) {
    doc.trim(TrimType.TRANSPARENT, true, true, true, true);
}

function showDialog() {
    var startWindow = new Window("dialog", "命令选择");
    startWindow.btnRun = startWindow.add("button", undefined, "Run")
    startWindow.btnRun.onClick = function() {
        startWindow.close(1);
    }

    startWindow.btnClear = startWindow.add("button", undefined, "清除所有参数")
    startWindow.btnClear.onClick = function() {
        startWindow.close(2);
    }

    startWindow.btnOpenLayeys = startWindow.add("button", undefined, "打开所有图层")
    startWindow.btnOpenLayeys.onClick = function() {
        startWindow.close(3);
    }

    // 返回 1 2 3 
    return startWindow.show();
}

function ShowMessageBox(message) {
    var myWindow = new Window("dialog");
    var myMessage = myWindow.add("statictext");
    myMessage.text = message;
    myWindow.show();
}

// ------------------------------------------------------- 功能辅助 -------------------------------------------------------

function changeLayerName(name) {
    var layer = app.activeDocument.activeLayer
    layer.name = name
}

function changeLayerNameStart(startName) {
    var layer = app.activeDocument.activeLayer
    layer.name = startName + layer.name
}

function changeLayerNameEnd(endName) {
    var layer = app.activeDocument.activeLayer
    layer.name = layer.name + endName
}

function Log(content) {
    logStr = logStr + content + "\n";
}

function WriteLog() {
    var logPath = "../ExportLog.log"
    var logFile = new File(logPath);
    logFile.encoding = "utf-8";
    logFile.open('w');
    logFile.writeln(logStr);
    logFile.close();
}
// ------------------------------------------------------- 功能辅助 -------------------------------------------------------