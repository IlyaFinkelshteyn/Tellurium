﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using Tellurium.MvcPages.Utils;

namespace Tellurium.VisualAssertions.Dashboard.Mvc
{
    public static class ActionResultFactory
    {
        public static ActionResult ImageResult(byte[] bytes)
        {
            using (var streak = new MemoryStream())
            {
                var srcImage = bytes.ToBitmap();
                srcImage.Save(streak, ImageFormat.Png);
                return new FileContentResult(streak.ToArray(),"image/png");
            }
        }

        public static ActionResult ImageResult(Bitmap diff)
        {
            using (var ms = new MemoryStream())
            {
                diff.Save(ms, ImageFormat.Png);
                return new FileContentResult(ms.ToArray(), "image/png");
            }
        }

        public static ActionResult AjaxSuccess()
        {
            return new JsonResult()
            {
                Data = new { success=true}
            };
        }
    }
}