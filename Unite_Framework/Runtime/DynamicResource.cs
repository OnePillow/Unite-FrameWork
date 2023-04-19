using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Unite
{
    /// <summary>
    /// 基础动态加载资源组件
    /// </summary>
    public class DynamicResource : MonoBehaviour
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string path;
        /// <summary>
        /// 动态显示的组件
        /// </summary>
        public Behaviour[] targets;
        /// <summary>
        /// 视频分辨率
        /// </summary>
        public Vector2 videoResolution;

        /// <summary>
        /// 当前显示的组件
        /// </summary>
        Behaviour current;

        void Start()
        {
            path = Application.streamingAssetsPath + "/" + path;
            Load();
        }

        /// <summary>
        /// 切换显示的组件
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        Behaviour Open(ResourcesList.FileType fileType)
        {
            int target = (int)fileType;
            if (current != null) current.gameObject.SetActive(false);
            current = targets[target];
            current.gameObject.SetActive(true);
            return current;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        public int Load()
        {
            FileInfo file = new(path);
            if (!file.Exists)
            {
                Debug.Log(file.FullName + "is not exist");
                return -1;
            }
            string[] str = file.Name.Split('.');
            switch (str[str.Length - 1])
            {
                case "mp4": LoadVideo(file); break;
                case "png": LoadImage(file); break;
                case "jpg": LoadImage(file); break;
                case "txt": LoadText(file); break;
            }
            return 1;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileType">文件类型</param>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public int Load(ResourcesList.FileType fileType, string filePath = "")
        {
            FileInfo file;
            if (filePath == "") file = new(path);
            else file = new(filePath);
            if (!file.Exists) return -1;
            switch (fileType)
            {
                case ResourcesList.FileType.Image: LoadImage(file); break;
                case ResourcesList.FileType.Video: LoadVideo(file); break;
                case ResourcesList.FileType.Text: LoadText(file); break;
            }
            path = filePath;
            return 1;
        }

        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="file"></param>
        void LoadImage(FileInfo file)
        {
            Debug.Log("image");
            var image = Open(ResourcesList.FileType.Image) as Image;
            byte[] bytes = File.ReadAllBytes(file.FullName);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            image.sprite = sprite;
        }

        /// <summary>
        /// 读取视频
        /// </summary>
        /// <param name="file"></param>
        void LoadVideo(FileInfo file)
        {
            Debug.Log("video");
            var video = Open(ResourcesList.FileType.Video) as VideoPlayer;
            var tex = RenderTexture.GetTemporary(new((int)videoResolution.x,
                                                    (int)videoResolution.y));
            video.GetComponent<RawImage>().texture = tex;
            video.targetTexture = tex;
            video.url = file.FullName;
        }

        /// <summary>
        /// 读取文本
        /// </summary>
        /// <param name="file"></param>
        void LoadText(FileInfo file)
        {
            Debug.Log("text");
            var text = Open(ResourcesList.FileType.Text) as Text;
            text.text = File.ReadAllText(file.FullName);
        }
    }
}