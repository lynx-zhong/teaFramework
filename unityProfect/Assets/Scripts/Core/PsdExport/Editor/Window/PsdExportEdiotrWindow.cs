using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Diagnostics;


namespace PsdExport
{
    public class PsdExportEdiotrWindow : EditorWindow
    {

        string artWorkSpacePath;
        string psdFolderPath => Path.Combine(artWorkSpacePath, PsdExportConfig.Instance.ArtPsdFolderName);
        string selectedPsdFolderName = "";
        string artFolderCacheKey = "ArtFolderCacheKey";
        Label artWorkSpacePathLable;
        List<string> selectedXmlList;


        [MenuItem("UITool/PsdExport")]
        public static void ShowExample()
        {
            PsdExportEdiotrWindow wnd = GetWindow<PsdExportEdiotrWindow>("PsdExport");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PsdExportConfig.Instance.RootUxmlPath);
            VisualElement labelFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUXML);

            // init
            selectedXmlList = new List<string>();

            //
            RefreshArtWorkspacePath();

            //
            RefrshFolderNode();

            //
            RefreshCurSelectedFolderInfo();
        }

        void RefreshArtWorkspacePath()
        {
            //
            Button selectArtWorkspaceButton = rootVisualElement.Q<Button>("artWorkspaceSelectButton");
            selectArtWorkspaceButton.clicked -= OnSelectArtWorkspaceButtonClick;
            selectArtWorkspaceButton.clicked += OnSelectArtWorkspaceButtonClick;

            //
            Button refreshSvnButton = rootVisualElement.Q<Button>("refreshSvnButton");
            refreshSvnButton.clicked -= OnRefreshSvnButtonClick;
            refreshSvnButton.clicked += OnRefreshSvnButtonClick;

            //
            artWorkSpacePath = PlayerPrefs.GetString(artFolderCacheKey, "");
            artWorkSpacePathLable = rootVisualElement.Q<Label>("artWorkspacePathLable");
            artWorkSpacePathLable.text = $"美术目录：{artWorkSpacePath}";

            //
            Button syncIconFolderButton = rootVisualElement.Q<Button>("syncIcon");
            syncIconFolderButton.clicked -= OnSyncIconButtonClick;
            syncIconFolderButton.clicked += OnSyncIconButtonClick;
        }

        void OnSyncIconButtonClick()
        {
            if (artWorkSpacePath == "")
                return;

            string iconFolderPath = Path.Combine(artWorkSpacePath, "Icon目录");
            if (!Directory.Exists(iconFolderPath))
                return;

            // 重复资源名检测
            string[] allFolderPath = Directory.GetDirectories(iconFolderPath);
            List<string> allPngFiles = new List<string>();
            List<string> repeatIconPathList = new List<string>();
            Dictionary<string,string> allPngFilesDic = new Dictionary<string, string>();

            foreach (var item in allFolderPath)
            {
                string[] files = Directory.GetFiles(item);
                allPngFiles.AddRange(files);

                foreach (var path in files)
                {
                    string fileName = Path.GetFileName(path);
                    if (allPngFilesDic.ContainsKey(fileName))
                        repeatIconPathList.Add(fileName);
                    else
                    {
                        allPngFilesDic.Add(fileName,path);
                    }
                }
            }
            
            if (repeatIconPathList.Count > 0)
            {
                int buttonClicked = EditorUtility.DisplayDialogComplex("资源名重复", "检测到有重复的icon资源，已停止导入。", "开打Icon目录", "关闭","");
                if (buttonClicked == 0)
                {
                    Process.Start("explorer.exe", Path.GetFullPath(iconFolderPath));
                }

                Debug.LogError("资源重复停止导入");

                foreach (var item in repeatIconPathList)
                {
                    Debug.LogError(item);
                }
                return;
            }

            foreach (var item in allPngFiles)
            {
                string[] strArr = item.Split("\\");
                string fileName = strArr[strArr.Length - 1];
                string folderName = strArr[strArr.Length - 2];
                string iconFolder = PsdExportConfig.Instance.UnityIconFolder.Replace("Assets/", "");
                string targetFullName = Path.Combine(Application.dataPath, iconFolder, folderName, fileName);
                string needCreateFolderPath = Path.Combine(Application.dataPath, iconFolder, folderName);
                if (!Directory.Exists(needCreateFolderPath))
                    Directory.CreateDirectory(needCreateFolderPath);

                File.Copy(item, targetFullName, true);
            }

            AssetDatabase.Refresh();
        }


        void OnSelectArtWorkspaceButtonClick()
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择美术工作目录", "", "");
            if (folderPath != null)
            {
                PlayerPrefs.SetString(artFolderCacheKey, folderPath);

                artWorkSpacePath = folderPath;
                artWorkSpacePathLable.text = $"美术目录：{artWorkSpacePath}";

                RefrshFolderNode();
            }
        }

        private void OnRefreshSvnButtonClick()
        {
            // Process svnProgress = new Process();
        }

        void RefrshFolderNode()
        {
            if (!Directory.Exists(psdFolderPath))
            {
                if (psdFolderPath != PsdExportConfig.Instance.ArtPsdFolderName)
                    Debug.LogError($"选择的美术文件夹错误，目录下没有 {PsdExportConfig.Instance.ArtPsdFolderName}");
                return;
            }

            TextField searchFolder = rootVisualElement.Q<TextField>("folderSearchField");
            searchFolder.RegisterValueChangedCallback(OnSearchValueChange);

            RefreshAllPsdFolder();
        }

        private void OnSearchValueChange(ChangeEvent<string> evt)
        {
            RefreshAllPsdFolder(evt.newValue);
        }

        void RefreshAllPsdFolder(string searchFolderName = "")
        {
            string[] allPsdFolderPath = Directory.GetDirectories(psdFolderPath);

            Foldout foldout = rootVisualElement.Q<Foldout>("folderList");
            foldout.Clear();
            for (int i = 0; i < allPsdFolderPath.Length; i++)
            {
                string folderName = Path.GetFileName(allPsdFolderPath[i]);
                if (folderName.IndexOf(searchFolderName, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    string path = allPsdFolderPath[i];
                    Button button = new Button(() => OnFolderSelected(path));
                    button.text = folderName;
                    foldout.Add(button);
                }
            }

            if (allPsdFolderPath.Length > 0)
                OnFolderSelected(allPsdFolderPath[0]);
        }

        void OnFolderSelected(string folderPath)
        {
            selectedPsdFolderName = Path.GetFullPath(folderPath);
            RefreshCurSelectedFolderInfo();
        }

        private void RefreshCurSelectedFolderInfo()
        {
            VisualElement exportNode = rootVisualElement.Q<VisualElement>("exportNode");

            if (selectedPsdFolderName == "")
            {
                exportNode.style.visibility = Visibility.Hidden;
                return;
            }

            string xmlPath = Path.Combine(selectedPsdFolderName, PsdExportConfig.Instance.ArtXmlFolderName);
            if (!Directory.Exists(xmlPath))
            {
                exportNode.style.visibility = Visibility.Hidden;
                return;
            }

            exportNode.style.visibility = Visibility.Visible;
            selectedXmlList.Clear();

            //
            Label selectedPsdFolderLable = exportNode.Q<Label>("curPsdFolderPath");
            selectedPsdFolderLable.text = $"当前选择的文件夹：{selectedPsdFolderName}";

            //
            TextField xmlSearch = exportNode.Q<TextField>("xmlSearch");
            xmlSearch.RegisterValueChangedCallback(OnXmlSearchValueChange);
            RefreshXmlFiles();

            //
            Button exportButton = exportNode.Q<Button>();
            exportButton.clicked -= Export;
            exportButton.clicked += Export;
        }

        private void OnXmlSearchValueChange(ChangeEvent<string> evt)
        {
            RefreshXmlFiles(evt.newValue);
        }

        void RefreshXmlFiles(string searchXmlName = "")
        {
            VisualElement itemsParent = rootVisualElement.Q<VisualElement>("itemsParent");
            itemsParent.Clear();

            //
            string xmlPath = Path.Combine(selectedPsdFolderName, PsdExportConfig.Instance.ArtXmlFolderName);
            string[] xmlFiles = Directory.GetFiles(xmlPath, "*.xml");
            for (int i = 0; i < xmlFiles.Length; i++)
            {
                string fileName = Path.GetFileName(xmlFiles[i]);
                string filePath = Path.GetFullPath(xmlFiles[i]);

                if (fileName.IndexOf(searchXmlName, StringComparison.CurrentCultureIgnoreCase) != -1)
                {
                    VisualElement xmlItemElement = new VisualElement();
                    xmlItemElement.style.flexDirection = FlexDirection.Row;
                    xmlItemElement.style.justifyContent = Justify.SpaceBetween;
                    xmlItemElement.style.paddingTop = 3;

                    //
                    Label desc = new Label(fileName);
                    xmlItemElement.Add(desc);

                    //
                    Toggle select = new Toggle();
                    select.name = filePath;
                    select.RegisterValueChangedCallback(OnXmlToggleValueChange);

                    //
                    xmlItemElement.Add(select);

                    //
                    itemsParent.Add(xmlItemElement);
                }
            }
        }

        private void OnXmlToggleValueChange(ChangeEvent<bool> evt)
        {
            Toggle toggle = evt.target as Toggle;
            string name = toggle.name;

            if (evt.newValue)
            {
                selectedXmlList.Add(name);
            }
            else
            {
                selectedXmlList.Remove(name);
            }
        }

        void Export()
        {
            foreach (var item in selectedXmlList)
            {
                PsdExportManeger.StartExport(item, artWorkSpacePath);
            }
        }
    }
}
