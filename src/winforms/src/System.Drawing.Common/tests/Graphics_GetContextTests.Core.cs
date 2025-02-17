﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;
using System.Numerics;

namespace System.Drawing.Tests;

public partial class Graphics_GetContextTests
{
    [Fact]
    public void GetContextInfo_New_DefaultGraphics()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            graphics.GetContextInfo(out PointF offset);
            Assert.True(offset.IsEmpty);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.True(offset.IsEmpty);
            Assert.Null(clip);
        }
    }

    [Fact]
    public void GetContextInfo_New_Clipping()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Region initialClip = new(new Rectangle(1, 2, 9, 10)))
        {
            graphics.Clip = initialClip;

            graphics.GetContextInfo(out PointF offset);
            Assert.True(offset.IsEmpty);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.True(offset.IsEmpty);
            Assert.NotNull(clip);
            Assert.Equal(initialClip.GetBounds(graphics), clip.GetBounds(graphics));
            clip.Dispose();
        }
    }

    [Fact]
    public void GetContextInfo_New_Transform()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            graphics.TransformElements = Matrix3x2.CreateTranslation(1, 2);

            graphics.GetContextInfo(out PointF offset);
            Assert.Equal(new PointF(1, 2), offset);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.Null(clip);
            Assert.Equal(new PointF(1, 2), offset);
        }
    }

    [Fact]
    public void GetContextInfo_New_ClipAndTransform()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Region initialClip = new(new Rectangle(1, 2, 9, 10)))
        {
            graphics.Clip = initialClip;
            graphics.TransformElements = Matrix3x2.CreateTranslation(1, 2);

            graphics.GetContextInfo(out PointF offset);
            Assert.Equal(new PointF(1, 2), offset);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.NotNull(clip);
            Assert.Equal(new RectangleF(0, 0, 9, 10), clip.GetBounds(graphics));
            Assert.Equal(new PointF(1, 2), offset);
            clip.Dispose();
        }
    }

    [Fact]
    public void GetContextInfo_New_TransformAndClip()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Region initialClip = new(new Rectangle(1, 2, 9, 10)))
        {
            graphics.TransformElements = Matrix3x2.CreateTranslation(1, 2);
            graphics.Clip = initialClip;

            graphics.GetContextInfo(out PointF offset);
            Assert.Equal(new PointF(1, 2), offset);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.NotNull(clip);
            Assert.Equal(new RectangleF(1, 2, 9, 10), clip.GetBounds(graphics));
            Assert.Equal(new PointF(1, 2), offset);
            clip.Dispose();
        }
    }

    [Fact]
    public void GetContextInfo_New_ClipAndTransformSaveState()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Region initialClip = new(new Rectangle(1, 2, 9, 10)))
        {
            graphics.Clip = initialClip;
            graphics.TransformElements = Matrix3x2.CreateTranslation(1, 2);

            GraphicsState state = graphics.Save();

            graphics.GetContextInfo(out PointF offset);
            Assert.Equal(new PointF(2, 4), offset);

            graphics.GetContextInfo(out offset, out Region? clip);
            Assert.NotNull(clip);
            Assert.Equal(new RectangleF(0, 0, 8, 8), clip.GetBounds(graphics));
            Assert.Equal(new PointF(2, 4), offset);
            clip.Dispose();
        }
    }

    [Fact]
    public void GetContextInfo_New_ClipAndTransformSaveAndRestoreState()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            graphics.SetClip(new Rectangle(1, 2, 9, 10));
            graphics.TransformElements = Matrix3x2.CreateTranslation(1, 2);

            GraphicsState state = graphics.Save();
            graphics.GetContextInfo(out PointF offset, out Region? clip);
            graphics.Restore(state);

            Assert.NotNull(clip);
            Assert.Equal(new RectangleF(0, 0, 8, 8), clip.GetBounds(graphics));
            Assert.Equal(new PointF(2, 4), offset);
            clip.Dispose();
        }
    }
}
