using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.Collections.Generic.List<Microsoft.Xna.Framework.Content.Pipeline.Graphics.Texture2DContent>;
using TOutput = ContentProcessors.SpriteAnimationContent;
using System.ComponentModel;

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
    [ContentProcessor(DisplayName = "SpriteAnimationProcessor")]
    public class SpriteAnimationProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            var res = new TOutput();
            res.Sprites = input.ToArray();
            res.AnimationFPS = FPS;
            return res;
        }

        [DisplayName("FPS")]
        [Description("How many frames per second should this animation show?")]
        [DefaultValue(25f)]

        public float FPS { get; set; }
    }
    
    /*
    class SpriteAnimationWriter : ContentTypeWriter<SpriteAnimation>
    {

    }

    class SpriteAnimationReader : ContentTypeReader<SpriteAnimation>
    {

    }
    */
}