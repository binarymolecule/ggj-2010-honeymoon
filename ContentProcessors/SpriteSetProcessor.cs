using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = System.Collections.Generic.List<Microsoft.Xna.Framework.Content.Pipeline.Graphics.TextureContent>;
using TOutput = System.Collections.Generic.List<Microsoft.Xna.Framework.Content.Pipeline.Graphics.Texture2DContent>;

namespace ContentProcessors
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "SpriteSetProcessor")]
    public class SpriteSetProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            TOutput t2d = new TOutput();

            foreach (TextureContent tc in input)
            {
                t2d.Add(tc as Texture2DContent);
            }

            return t2d;
        }
    }
}