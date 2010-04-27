using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using ICSharpCode.SharpZipLib;

// TODO: replace this with the type you want to import.
using TImport = System.Collections.Generic.List<Microsoft.Xna.Framework.Content.Pipeline.Graphics.Texture2DContent>;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace ContentProcessors
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".zip", DisplayName = "SpriteSetImporter", DefaultProcessor = "SpriteSetProcessor")]
    public class SpriteSetImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            ZipFile zip = new ZipFile(filename);
            var fz = new FastZip();
            string tmpOutName = Path.GetTempFileName();
            File.Delete(tmpOutName);
            var di = Directory.CreateDirectory(tmpOutName);
            fz.ExtractZip(filename, tmpOutName, null);
            List<Texture2DContent> result = new List<Texture2DContent>();

            TextureImporter ti = new TextureImporter();
            foreach (FileInfo fi in di.GetFiles())
            {
                result.Add(ti.Import(fi.FullName, context) as Texture2DContent);
            }

            di.Delete(true);

            return result;
        }
    }
}
