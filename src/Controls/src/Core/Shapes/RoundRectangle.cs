﻿#nullable disable
using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Shapes
{
	public sealed partial class RoundRectangle : Shape, IShape, IRoundRectangle
	{
		public RoundRectangle() : base()
		{
			Aspect = Stretch.Fill;
		}

		/// <summary>Bindable property for <see cref="CornerRadius"/>.</summary>
		public static readonly BindableProperty CornerRadiusProperty =
			BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(RoundRectangle), new CornerRadius());

		public CornerRadius CornerRadius
		{
			set { SetValue(CornerRadiusProperty, value); }
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
		}
		double _fallbackWidth;
		double _fallbackHeight;

		internal override double WidthForPathComputation
		{
			get
			{
				var width = Width;

				return width == -1 ? _fallbackWidth : width;
			}
		}

		internal override double HeightForPathComputation
		{
			get
			{
				var height = Height;

				return height == -1 ? _fallbackHeight : height;
			}
		}

		// TODO this should move to a remapped mapper
		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == CornerRadiusProperty.PropertyName)
				Handler?.UpdateValue(nameof(IShapeView.Shape));
		}

		public override PathF GetPath()
		{
			var width = WidthForPathComputation;
			var height = HeightForPathComputation;

			var path = new PathF();

			float x = (float)StrokeThickness / 2;
			float y = (float)StrokeThickness / 2;

			float w = (float)(width - StrokeThickness);
			float h = (float)(height - StrokeThickness);

			float topLeftCornerRadius = (float)CornerRadius.TopLeft;
			float topRightCornerRadius = (float)CornerRadius.TopRight;
			float bottomLeftCornerRadius = (float)CornerRadius.BottomLeft;
			float bottomRightCornerRadius = (float)CornerRadius.BottomRight;

			path.AppendRoundedRectangle(x, y, w, h, topLeftCornerRadius, topRightCornerRadius, bottomLeftCornerRadius, bottomRightCornerRadius);

			return path;
		}

		PathF IShape.PathForBounds(Graphics.Rect viewBounds)
		{
			_fallbackHeight = viewBounds.Height;
			_fallbackWidth = viewBounds.Width;

			var path = GetPath();

			TransformPathForBounds(path, viewBounds);

			return path;
		}

		internal PathF GetInnerPath(float strokeWidth)
		{
			var width = WidthForPathComputation;
			var height = HeightForPathComputation;

			var path = new PathF();

			float x = (float)StrokeThickness / 2;
			float y = (float)StrokeThickness / 2;

			float w = (float)(width - StrokeThickness);
			float h = (float)(height - StrokeThickness);

			float topLeftCornerRadius = (float)Math.Max(0, CornerRadius.TopLeft - strokeWidth);
			float topRightCornerRadius = (float)Math.Max(0, CornerRadius.TopRight - strokeWidth);
			float bottomLeftCornerRadius = (float)Math.Max(0, CornerRadius.BottomLeft - strokeWidth);
			float bottomRightCornerRadius = (float)Math.Max(0, CornerRadius.BottomRight - strokeWidth);

			path.AppendRoundedRectangle(x, y, w, h, topLeftCornerRadius, topRightCornerRadius, bottomLeftCornerRadius, bottomRightCornerRadius);

			return path;
		}

		PathF IRoundRectangle.InnerPathForBounds(Rect viewBounds, float strokeWidth)
		{
			_fallbackHeight = viewBounds.Height;
			_fallbackWidth = viewBounds.Width;

			var path = GetInnerPath(strokeWidth);

			TransformPathForBounds(path, viewBounds);

			return path;
		}
	}
}