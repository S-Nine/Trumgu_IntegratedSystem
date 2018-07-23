using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Trumgu_IntegratedManageSystem.Utils
{
    public static class ImageUpload
    {
        private const string FileType = "jpg;gif;bmp;png"; //所支持的上传类型用"/"隔开 

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetExt(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// 检查上传的文件的类型，是否允许上传。
        /// </summary>
        private static  bool IsUpload(string ext)
        {
            ext = ext.Replace(".", "");
            var arrFileType = FileType.Split(';');
            return arrFileType.Any(str => string.Equals(str, ext, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 图片上传方法
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="savePath">webconfig地址</param>
        /// <param name="imgPath">返回保存地址</param>
        /// <returns></returns>
        public static bool Upload(IFormFile files,string savePath,out string imgPath)
        {
            if (files == null || files.FileName.Trim() == "")
            {
                imgPath = "";
                return false;
            }
            var ext = GetExt(files.FileName);
            if (!IsUpload(ext))
            {
                imgPath = "";
                return false;
            }
            try
            {
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                var filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                filename = savePath+ $@"{filename}";
                using (var fs = File.Create(filename))
                {
                    files.CopyTo(fs);
                    fs.Flush();
                }

                imgPath = filename;
                return true;
            }
            catch
            {
                imgPath = "";
                return false;
            }
        }

    }
}
