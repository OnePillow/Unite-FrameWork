using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unite {
    public class ResourcesList : MonoBehaviour
    {
        public enum FileType {
            Image = 0,
            Video,
            Text,
        }
        public FileType fileType;
        public string path;
        public DynamicResource[] resources;
        void Start()
        {
            path = Application.streamingAssetsPath + "/" + path;
        }
        public delegate bool Check(FileInfo dir);
        public void Load(Check Condition = null)
        {
            var files = new List<string>();
            var index = 0;
            foreach (var file in new DirectoryInfo(path).GetFiles())
            {
                if (Condition != null && !Condition(file)) continue;
                files.Add(file.FullName);
            }
            foreach (var resource in resources) resource.Load(fileType, files[index++]);
        }
    }
}