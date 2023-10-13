using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.U2D;
using UnityEditor.U2D;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PsdExport
{
    public class PsdExportManeger
    {
        static GameObject canvasGo;

        static string moduleName;

        public static void StartExport(string xmlPath, string artWorkSpacePath)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PsdRootNodeSerializer));
                StreamReader reader = new StreamReader(xmlPath);
                PsdRootNodeSerializer psdRootNodeSerializer = (PsdRootNodeSerializer)xmlSerializer.Deserialize(reader);
                moduleName = psdRootNodeSerializer.ModuleName;

                ExportCommonPngToUnity(psdRootNodeSerializer, artWorkSpacePath);
                ExportSystemPngToUnity(psdRootNodeSerializer, artWorkSpacePath);
                ExportPngSetting(psdRootNodeSerializer);
                ExportPrefab(psdRootNodeSerializer);

                Debug.Log($"导出成功   {psdRootNodeSerializer.PrefabName}");
            }
            catch (System.Exception)
            {
                Debug.LogError($"导出失败：   {xmlPath}");
                throw;
            }
        }

        private static void ExportCommonPngToUnity(PsdRootNodeSerializer psdRootNodeSerializer, string artWorkSpacePath)
        {
            bool isHaveCommonUI = CheckIsNeedExportCommonUI(psdRootNodeSerializer.RootLayer, psdRootNodeSerializer.CommonUIMark);
            if (isHaveCommonUI)
            {
                //
                string unityPath = PsdExportConfig.Instance.UnityCommonPngFolder.Replace("Assets/", "");
                string direPath = Path.Combine(Application.dataPath, unityPath);
                if (!Directory.Exists(direPath))
                    Directory.CreateDirectory(direPath);
                else
                {
                    foreach (var item in Directory.GetFiles(direPath))
                    {
                        File.Delete(item);
                    }
                }

                //
                foreach (string fileFullPath in Directory.GetFiles(artWorkSpacePath + psdRootNodeSerializer.CommonUIFolderPath, "*.png"))
                {
                    string fileName = Path.GetFileName(fileFullPath);
                    string targetFullName = Path.Combine(Application.dataPath, unityPath, fileName);
                    File.Copy(fileFullPath, targetFullName, true);
                }

                AssetDatabase.Refresh();

                //
                string atlasPath = Path.Combine(PsdExportConfig.Instance.SpriteAtlasFolder + "/UICommon.spriteatlas");
                ExportSpriteAtlas(PsdExportConfig.Instance.UnityCommonPngFolder, atlasPath);
            }
        }

        static bool CheckIsNeedExportCommonUI(PsdLayerSerializer layerSerializer, string commonUIMark)
        {
            if (layerSerializer.BaseType == PsdBaseType.Image && layerSerializer.NodeName.StartsWith(commonUIMark))
            {
                return true;
            }

            foreach (var item in layerSerializer.LayerList)
            {
                bool childResult = CheckIsNeedExportCommonUI(item, commonUIMark);
                if (childResult)
                    return true;
            }

            return false;
        }

        static void ExportSystemPngToUnity(PsdRootNodeSerializer psdRootNodeSerializer, string artWorkSpacePath)
        {
            string unityPath = PsdExportConfig.Instance.UnityUIPngFolder.Replace("Assets/", "");
            string direPath = Path.Combine(Application.dataPath, unityPath, psdRootNodeSerializer.ModuleName);
            if (!Directory.Exists(direPath))
                Directory.CreateDirectory(direPath);
            else
            {
                foreach (var item in Directory.GetFiles(direPath))
                {
                    File.Delete(item);
                }
            }

            foreach (string fileFullPath in Directory.GetFiles(artWorkSpacePath + psdRootNodeSerializer.PngFolderPath, "*.png"))
            {
                string fileName = Path.GetFileName(fileFullPath);
                string targetFullName = Path.Combine(direPath, fileName);
                File.Copy(fileFullPath, targetFullName, true);
            }

            AssetDatabase.Refresh();

            //
            string pngFolderPath = Path.Combine(PsdExportConfig.Instance.UnityUIPngFolder, psdRootNodeSerializer.ModuleName);
            string atlasPath = Path.Combine(PsdExportConfig.Instance.SpriteAtlasFolder, psdRootNodeSerializer.ModuleName + ".spriteatlas");
            ExportSpriteAtlas(pngFolderPath, atlasPath);
        }

        private static void ExportPngSetting(PsdRootNodeSerializer psdRootNodeSerializer)
        {
            //
            Dictionary<string, Vector4> slicedData = GetAllSlicedData(psdRootNodeSerializer.RootLayer.LayerList, psdRootNodeSerializer);
            foreach (var item in slicedData)
            {
                TextureImporter importer = AssetImporter.GetAtPath(item.Key) as TextureImporter;
                if (importer == null)
                    continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.spriteBorder = item.Value;
                importer.mipmapEnabled = false;

                AssetDatabase.WriteImportSettingsIfDirty(item.Key);
                AssetDatabase.ImportAsset(item.Key);
            }

            AssetDatabase.Refresh();
        }

        static Dictionary<string, Vector4> GetAllSlicedData(List<PsdLayerSerializer> psdLayers, PsdRootNodeSerializer psdRootNodeSerializer)
        {
            Dictionary<string, Vector4> slicedData = new Dictionary<string, Vector4>();

            foreach (PsdLayerSerializer childLayer in psdLayers)
            {
                if (childLayer.BaseType == PsdBaseType.Image && childLayer.ImageType == Image.Type.Sliced)
                {
                    string targetFullName;

                    if (childLayer.NodeName.StartsWith(psdRootNodeSerializer.CommonUIMark))
                        targetFullName = Path.Combine(PsdExportConfig.Instance.UnityCommonPngFolder, childLayer.NodeName + ".png");
                    else
                        targetFullName = Path.Combine(PsdExportConfig.Instance.UnityUIPngFolder, psdRootNodeSerializer.ModuleName, childLayer.NodeName + ".png");

                    slicedData.Add(targetFullName, childLayer.SpriteBorder);
                }

                Dictionary<string, Vector4> temp = GetAllSlicedData(childLayer.LayerList, psdRootNodeSerializer);
                foreach (var kvp in temp)
                {
                    slicedData.Add(kvp.Key, kvp.Value);
                }
            }

            return slicedData;
        }

        static void ExportSpriteAtlas(string targetPngFolderPath, string atlasPath)
        {
            SpriteAtlas spriteAtlas = new SpriteAtlas();
            spriteAtlas.SetIncludeInBuild(true);

            // packing setting
            SpriteAtlasPackingSettings atlasPackingSettings = spriteAtlas.GetPackingSettings();
            atlasPackingSettings.enableRotation = false;
            atlasPackingSettings.enableTightPacking = false;
            atlasPackingSettings.enableAlphaDilation = false;
            atlasPackingSettings.padding = 4;
            spriteAtlas.SetPackingSettings(atlasPackingSettings);

            // texture
            SpriteAtlasTextureSettings atlasTextureSettings = spriteAtlas.GetTextureSettings();
            atlasTextureSettings.generateMipMaps = false;
            atlasTextureSettings.readable = false;
            atlasTextureSettings.sRGB = true;
            spriteAtlas.SetTextureSettings(atlasTextureSettings);

            // texture format

            //
            Object targetFoloder = AssetDatabase.LoadMainAssetAtPath(targetPngFolderPath);
            SpriteAtlasExtensions.Add(spriteAtlas, new Object[] { targetFoloder });

            //
            string unityAtlasPath = PsdExportConfig.Instance.SpriteAtlasFolder.Replace("Assets/", "");
            string fullFolder = Path.Combine(Application.dataPath, unityAtlasPath);
            if (!Directory.Exists(fullFolder))
                Directory.CreateDirectory(fullFolder);

            //
            AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static string GetPlatformName(BuildTarget target)
        {
            string platformName = "";
            switch (target)
            {
                case BuildTarget.Android:
                    platformName = "Android";
                    break;
                case BuildTarget.iOS:
                    platformName = "iPhone";
                    break;
                case BuildTarget.PS4:
                    platformName = "PS4";
                    break;
                case BuildTarget.XboxOne:
                    platformName = "XboxOne";
                    break;
                case BuildTarget.NoTarget:
                    platformName = "DefaultTexturePlatform";
                    break;
                default:
                    platformName = "Standalone";
                    break;
            }
            return platformName;
        }

        static void ExportPrefab(PsdRootNodeSerializer psdRootNodeSerializer)
        {
            CreateCanvas();

            //
            GameObject rootGo = new GameObject(psdRootNodeSerializer.PrefabName);
            rootGo.transform.SetParent(canvasGo.transform);
            rootGo.transform.localScale = Vector3.one;
            rootGo.transform.localPosition = Vector3.zero;

            //
            RectTransform rootRectTran = rootGo.AddComponent<RectTransform>();
            rootRectTran.anchorMin = Vector3.zero;
            rootRectTran.anchorMax = Vector3.one;
            rootRectTran.offsetMin = Vector2.zero;
            rootRectTran.offsetMax = Vector2.zero;

            //
            ExportLayer(psdRootNodeSerializer, psdRootNodeSerializer.RootLayer, rootGo.transform);

            //
            SetLayerSetSizeAndAnchors(rootRectTran.transform.GetChild(0).GetComponent<RectTransform>(), rootRectTran);
        }

        static void CreateCanvas()
        {
            if (canvasGo != null)
                return;

            canvasGo = new GameObject("UICanvas");

            //
            CanvasScaler canvasScaler = canvasGo.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = PsdExportConfig.Instance.UIRefrenceResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            //
            Camera uiCamera = new GameObject("uiCamera").AddComponent<Camera>();
            uiCamera.transform.parent = canvasGo.transform;
            uiCamera.clearFlags = CameraClearFlags.Color;
            uiCamera.orthographic = true;
            uiCamera.transform.position = new Vector3(0, 0, -500);

            //
            Canvas canvas;
            canvasGo.TryGetComponent<Canvas>(out canvas);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;
        }

        static void ExportLayer(PsdRootNodeSerializer psdRootNodeSerializer, PsdLayerSerializer layer, Transform parentTran)
        {
            if (layer.IsNotExport)
                return;

            GameObject go = CreateGameObject(layer.NodeName, parentTran);

            // base type
            if (layer.BaseType == PsdBaseType.Lable)
                PsdExportLable.Exporte(go, layer);
            else if (layer.BaseType == PsdBaseType.Image)
                PsdExportImage.Exporte(go, layer, psdRootNodeSerializer, moduleName);

            // component type
            if (layer.ComponentType == PsdComponentType.Button)
                PsdExportButton.Exporte(go, layer, psdRootNodeSerializer, moduleName);

            //
            for (int i = layer.LayerList.Count - 1; i >= 0; i--)
            {
                PsdLayerSerializer childLayer = layer.LayerList[i];
                ExportLayer(psdRootNodeSerializer, childLayer, go.transform);
            }
        }

        static GameObject CreateGameObject(string goName, Transform parentTran)
        {
            GameObject go = new GameObject(goName);
            go.transform.SetParent(parentTran);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            RectTransform rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = Vector2.one;
            return go;
        }

        static void SetLayerSetSizeAndAnchors(RectTransform curRectTransform, RectTransform rootRectTransform)
        {
            SetSingleLayerSetSizeAndAnchors(curRectTransform, rootRectTransform);

            for (int i = 0; i < curRectTransform.childCount; i++)
            {
                RectTransform childRectTransform = curRectTransform.GetChild(i).GetComponent<RectTransform>();

                if (childRectTransform.childCount > 0)
                    SetLayerSetSizeAndAnchors(childRectTransform, rootRectTransform);
            }
        }

        static void SetSingleLayerSetSizeAndAnchors(RectTransform rectTransform, RectTransform rootRectTranmsform)
        {
            Dictionary<int, Vector2> childPositionDic = new Dictionary<int, Vector2>();
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                Transform childTran = rectTransform.GetChild(i);
                childPositionDic.Add(childTran.gameObject.GetInstanceID(), childTran.position);
            }

            VertexPoint maxVertexPoint = new VertexPoint();
            maxVertexPoint = GetLayerMaxSize(rectTransform.GetComponent<RectTransform>(), maxVertexPoint);

            //
            Transform curParent = rectTransform.parent;
            int index = rectTransform.GetSiblingIndex();
            rectTransform.SetParent(rootRectTranmsform);

            //
            rectTransform.sizeDelta = maxVertexPoint.GetSize();
            rectTransform.anchoredPosition = maxVertexPoint.GetPosition();

            //
            rectTransform.SetParent(curParent);
            rectTransform.SetSiblingIndex(index);
            rectTransform.localScale = Vector3.one;

            //
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                Transform childTran = rectTransform.GetChild(i);
                if (childPositionDic.ContainsKey(childTran.gameObject.GetInstanceID()))
                {
                    childTran.position = childPositionDic[childTran.gameObject.GetInstanceID()];
                }
            }

            //
            SetSingleNodeAnchors(rectTransform, maxVertexPoint.GetPosition());
        }

        // vector4 x-> 左下  y -> 左上  w -> 右上  z -> 右下
        static VertexPoint GetLayerMaxSize(RectTransform targetRectTransform, VertexPoint curPoints)
        {

            if (targetRectTransform.childCount == 0)
            {
                Vector3[] corners = GetCorners(targetRectTransform);
                VertexPoint vertexPoint = new VertexPoint(corners);

                //
                curPoints.rightDown.x = vertexPoint.rightDown.x > curPoints.rightDown.x ? vertexPoint.rightDown.x : curPoints.rightDown.x;
                curPoints.rightDown.y = vertexPoint.rightDown.y < curPoints.rightDown.y ? vertexPoint.rightDown.y : curPoints.rightDown.y;

                //
                curPoints.rigthTop.x = vertexPoint.rigthTop.x > curPoints.rigthTop.x ? vertexPoint.rigthTop.x : curPoints.rigthTop.x;
                curPoints.rigthTop.y = vertexPoint.rigthTop.y > curPoints.rigthTop.y ? vertexPoint.rigthTop.y : curPoints.rigthTop.y;

                //
                curPoints.leftDown.x = vertexPoint.leftDown.x < curPoints.leftDown.x ? vertexPoint.leftDown.x : curPoints.leftDown.x;
                curPoints.leftDown.y = vertexPoint.leftDown.y < curPoints.leftDown.y ? vertexPoint.leftDown.y : curPoints.leftDown.y;

                //
                curPoints.leftTop.x = vertexPoint.leftTop.x < curPoints.leftTop.x ? vertexPoint.leftTop.x : curPoints.leftTop.x;
                curPoints.leftTop.y = vertexPoint.leftTop.y > curPoints.leftTop.y ? vertexPoint.leftTop.y : curPoints.leftTop.y;
            }

            for (int i = 0; i < targetRectTransform.childCount; i++)
            {
                curPoints = GetLayerMaxSize(targetRectTransform.GetChild(i).GetComponent<RectTransform>(), curPoints);
            }

            return curPoints;
        }

        static Vector3[] GetCorners(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            Vector2 sizeDelta = rectTransform.sizeDelta;
            Vector3 center = rectTransform.anchoredPosition;

            corners[0] = center + new Vector3(-sizeDelta.x / 2, -sizeDelta.y / 2);
            corners[1] = center + new Vector3(-sizeDelta.x / 2, sizeDelta.y / 2);
            corners[2] = center + new Vector3(sizeDelta.x / 2, sizeDelta.y / 2);
            corners[3] = center + new Vector3(sizeDelta.x / 2, -sizeDelta.y / 2);

            return corners;
        }

        static void SetSingleNodeAnchors(RectTransform rect, Vector2 anchoredPosition)
        {
            if (anchoredPosition.x < 0 && anchoredPosition.y > 0)       // 左上
            {
                SetAnchors(rect, new Vector2(0, 1), new Vector2(0, 1));
            }
            else if (anchoredPosition.x > 0 && anchoredPosition.y > 0)      // 右上
            {
                SetAnchors(rect, new Vector2(1, 1), new Vector2(1, 1));
            }
            else if (anchoredPosition.x < 0 && anchoredPosition.y < 0)      // 左下
            {
                SetAnchors(rect, new Vector2(0, 0), new Vector2(0, 0));
            }
            else if (anchoredPosition.x > 0 && anchoredPosition.y < 0)      // 右下
            {
                SetAnchors(rect, new Vector2(1, 0), new Vector2(1, 0));
            }
        }

        static void SetAnchors(RectTransform rect, Vector2 minAnchor, Vector2 maxAnchor)
        {
            Vector2 oldAnchorPoint = rect.anchorMin;
            Vector2 oldPosition = rect.position;

            Vector2 offset = new Vector2((oldAnchorPoint.x - rect.pivot.x) * rect.rect.width, (oldAnchorPoint.y - rect.pivot.y) * rect.rect.height);

            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;

            rect.position = oldPosition + offset;
        }
    }
}