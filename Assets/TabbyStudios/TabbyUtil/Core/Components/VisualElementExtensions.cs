using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace TabbyStudios
{
    public static class VisualElementExtensions
    {
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static Vector2 WorldPosition(this VisualElement e)
        {
            //world position gives good values with everything included even the tab bar at the top if it's in an editor window
            //world position seems correct independent of scale
            //world position seems correct independent of scrolling
            return e.worldTransform.GetPosition();
        }
        
        public static Vector2 LocalPosition(this VisualElement e)
        {
            return new Vector2(e.resolvedStyle.left, e.resolvedStyle.top);
        }
        
        public static Vector2 PositionIgnoringEditorWindowTabBar(this VisualElement e)
        {
            //position seems correct independent of scale
            return GetPosition(e);
        }
        
        public static Vector2 ScreenPosition(this VisualElement e)
        {
            return e.ContainingSpace().ScreenPosition(e);
        }
        
        public static Vector2 RelativePosition(this VisualElement e, VisualElement to)
        {
            return e.ScreenPosition() - to.ScreenPosition();
        }
        
        public static Vector2 RelativePosition(this VisualElement e)
        {
            return e.ScreenPosition() - e.parent.ScreenPosition();
        }
        
        public static void SetAbsolutePositionRelative(this VisualElement e, Vector2 pos)
        {
            //Assert.AreEqual(Position.Absolute, e.style.position); //only tested for this case
            Assert.IsNotNull(e.parent, "Can't set absolute position on visual element without parent");
            var scale = e.Scale();
            e.style.left = pos.x / scale;
            e.style.top = pos.y / scale;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
    
        public static float Scale(this VisualElement e)
        {
            return e.parent is null ? e.resolvedStyle.scale.value.x : e.resolvedStyle.scale.value.x * e.parent.Scale();
        }
    
        public static void SetScale(this VisualElement e, float scale)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            e.transform.scale = new Vector3(scale,scale, 0);
            #pragma warning restore CS0618 // Type or member is obsolete
        }
    
        public static float ScaledValue(this VisualElement e, float value)
        {
            return e.Scale() * value;
        }        
    
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static float Width(this VisualElement e)
        {
            return e.resolvedStyle.width * e.Scale();
        }
        
        public static float UnscaledWidth(this VisualElement e)
        {
            return e.resolvedStyle.width;
        }
        
        public static void SetWidth(this VisualElement e, float width)
        {
            e.style.width = width / e.Scale();
        }
        
        public static void SetUnscaledWidth(this VisualElement e, float width)
        {
            e.style.width = width;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static float Height(this VisualElement e)
        {
            return e.resolvedStyle.height * e.Scale();
        }
        
        public static float UnscaledHeight(this VisualElement e)
        {
            return e.resolvedStyle.height;
        }
        
        public static void SetHeight(this VisualElement e, float height)
        {
            e.style.height = height;
        }
        
        public static void SetUnscaledHeight(this VisualElement e, float height)
        {
            e.style.height = height / e.Scale();
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static Vector2 Size(this VisualElement e)
        {
            return new Vector2(e.resolvedStyle.width,e.resolvedStyle.height) * e.Scale();
        }
        
        public static Vector2 UnscaledSize(this VisualElement e)
        {
            return new Vector2(e.resolvedStyle.width,e.resolvedStyle.height);
        }
        
        public static void SetSize(this VisualElement e, Vector2 size)
        {
            var scale = e.Scale();
            e.style.width = size.x / scale;
            e.style.height = size.y / scale;
        }
        
        public static void SetSize(this VisualElement e, float width, float height)
        {
            var scale = e.Scale();
            e.style.width = width / scale;
            e.style.height = height / scale;
        }
        
        public static void SetUnscaledSize(this VisualElement e, Vector2 size)
        {
            e.style.width = size.x;
            e.style.height = size.y;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static float Border(this VisualElement e)
        {
            AssertBorder(e);
            return e.resolvedStyle.borderTopWidth * e.Scale();
        }
        
        public static void SetBorder(this VisualElement e, float value)
        {
            e.style.borderBottomWidth = value;
            e.style.borderLeftWidth = value;
            e.style.borderTopWidth = value;
            e.style.borderRightWidth = value;
        }
        
        public static void SetBorder(this VisualElement e, float value, StyleColor color)
        {
            e.SetBorder(value);
            e.SetBorderColor(color);
        }

        public static void SetBorderColor(this VisualElement e, StyleColor value)
        {
            e.style.borderBottomColor = value;
            e.style.borderLeftColor = value;
            e.style.borderTopColor = value;
            e.style.borderRightColor = value;
        }

        public static void SetBorderRadius(this VisualElement e, float value)
        {
            e.style.borderBottomLeftRadius = value;
            e.style.borderBottomRightRadius = value;
            e.style.borderTopLeftRadius = value;
            e.style.borderTopRightRadius = value;
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void SetMargin(this VisualElement e, float value)
        {
            e.style.marginTop = value;
            e.style.marginLeft = value;
            e.style.marginBottom = value;
            e.style.marginRight = value;
        }
        
        public static float Margin(this VisualElement e)
        {
            AssertMargin(e);
            return e.resolvedStyle.marginTop * e.Scale();
        }
        
        public static float MarginX(this VisualElement e)
        {
            AssertMarginX(e);
            return e.resolvedStyle.marginTop * e.Scale();
        }
        
        public static float MarginY(this VisualElement e)
        {
            AssertMarginY(e);
            return e.resolvedStyle.marginTop * e.Scale();
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static void SetPadding(this VisualElement e, float value)
        {
            e.style.paddingBottom = value;
            e.style.paddingLeft = value;
            e.style.paddingRight = value;
            e.style.paddingTop = value;
        }
    
        public static float PaddingRight(this VisualElement e)
        {
            return e.resolvedStyle.paddingRight * e.Scale();
        }
    
        public static float PaddingLeft(this VisualElement e)
        {
            return e.resolvedStyle.paddingLeft * e.Scale();
        }
    
        public static float PaddingTop(this VisualElement e)
        {
            return e.resolvedStyle.paddingTop * e.Scale();
        }
    
        public static float PaddingBottom(this VisualElement e)
        {
            return e.resolvedStyle.paddingBottom * e.Scale();
        }
        
        public static float Padding(this VisualElement e)
        {
            AssertPadding(e);
            return e.resolvedStyle.paddingTop * e.Scale();
        }
        
        public static float PaddingX(this VisualElement e)
        {
            AssertPaddingX(e);
            return PaddingLeft(e);
        }
        
        public static float PaddingY(this VisualElement e)
        {
            AssertPaddingY(e);
            return PaddingTop(e);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        public static float TotalSpacing(this VisualElement e)
        {
            var parentStyle = e.resolvedStyle;
            AssertPadding(e.parent);
            AssertBorder(e.parent);
            AssertMargin(e.parent);
            AssertMargin(e);
            return parentStyle.borderTopWidth + parentStyle.marginTop + parentStyle.paddingTop + e.resolvedStyle.marginTop;
        }
        
        public static Vector2 SpacingDisplacement(this VisualElement e)
        {
            var parentStyle = e.parent.resolvedStyle;
            var x = parentStyle.borderLeftWidth + parentStyle.marginLeft + parentStyle.paddingLeft + e.resolvedStyle.marginLeft;
            var y = parentStyle.borderTopWidth + parentStyle.marginTop + parentStyle.paddingTop + e.resolvedStyle.marginTop;
            return new(x,y);
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        private static bool HasImage(this VisualElement e)
        {
            return e.style.backgroundImage.value.texture is not null;
        }
        
        public static Color Color(this VisualElement e)
        {
            return e.HasImage() ? e.resolvedStyle.backgroundColor : e.resolvedStyle.unityBackgroundImageTintColor;
        }
    
        public static void SetColor(this VisualElement e, Color color)
        {
            if (e.HasImage())
            {
                e.style.unityBackgroundImageTintColor = color;
            }
            else
            {
                e.style.backgroundColor = color;
            }
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void AssertMargin(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.marginTop,style.marginBottom);
            Assert.AreEqual(style.marginTop,style.marginLeft);
            Assert.AreEqual(style.marginTop,style.marginRight);
        }
        
        public static void AssertMarginX(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.marginRight,style.marginRight);
        }
        
        public static void AssertMarginY(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.marginTop,style.marginBottom);
        }
        
        public static void AssertPadding(this VisualElement e)
        {
            // var style = e.resolvedStyle;
            // Assert.AreEqual(style.paddingTop,style.paddingBottom);
            // Assert.AreEqual(style.paddingTop,style.paddingLeft);
            // Assert.AreEqual(style.paddingTop,style.paddingRight);
        }
        
        public static void AssertPaddingX(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.paddingLeft,style.paddingRight);
        }
        
        public static void AssertPaddingY(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.paddingTop,style.paddingBottom);
        }
        
        public static void AssertBorder(this VisualElement e)
        {
            var style = e.resolvedStyle;
            Assert.AreEqual(style.borderTopWidth,style.borderBottomWidth);
            Assert.AreEqual(style.borderTopWidth,style.borderLeftWidth);
            Assert.AreEqual(style.borderTopWidth,style.borderRightWidth);
        }
        
        public static void Hide(this VisualElement e)
        {
            e.SetVisible(false);
        }   
    
        public static void Show(this VisualElement e)
        {
            e.SetVisible(true);
        } 
    
        public static void SetVisible(this VisualElement e, bool visible)
        {
            e.visible = visible;
            e.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void CalculateLayout(this VisualElement e)
        {
            Assert.IsNotNull(e.panel, "Panel was null while trying to calculate layout");
            e.panel.InvokeMethod("ValidateLayout");
        }

        public static void Animate(this VisualElement e, string property, float duration = 0, float delay = 0)
        {
            e.style.transitionProperty = new List<StylePropertyName> { property };
            e.style.transitionDuration = new List<TimeValue> { duration  };
            e.style.transitionDelay = new List<TimeValue> { delay };
            e.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.Linear };
        }
        
        
        public static void OnMouseDown(this VisualElement e, Action<MouseDownEvent> action)
        {
            if (e is Button button)
            {
                var evt = new MouseDownEvent();
                button.clicked += () => action(evt);
            }
            else
            {
                e.RegisterCallback<MouseDownEvent>(evt => action(evt));
            }
            
        }

        public static bool IsFocused(this VisualElement e)
        {
            return e.focusController.focusedElement == e;
        }

        public static List<VisualElement> Siblings(this VisualElement e)
        {
            return e.parent.Children().Where(c => c != e).ToList();
        }
        
        
        public static void SetAsFirstSibling(this VisualElement e)
        {
            e.PlaceBehind(e.parent.Children().First());
        }
        
        public static void SetAsLastSibling(this VisualElement e)
        {
            e.PlaceInFront(e.parent.Children().Last());
        }
        
        public static VisualElement At<T>(this UQueryBuilder<T> query, int index) where T : VisualElement
        {
            if (index > 0)
                return query.AtIndex(0);
            var result = query.Build();
            return result.AtIndex(result.ToList().Count + index);
        }
        
        public static VisualElement Walk(this VisualElement e, params int[] indices)
        {
            VisualElement current = e;
            foreach (int i in indices)
            {
                current = current.Children().ToList()[i];
            }

            return current;
        }

        public static Vector2 MeaningOfContentSize(this VisualElement e)
        {
            return e.Size() - new Vector2(e.PaddingLeft() + e.PaddingRight(), e.PaddingTop() - e.PaddingBottom());
        }

        public static bool DidUnityLayoutCalculation(this VisualElement e)
        {
            return !float.IsNaN(e.resolvedStyle.width);
        }

        public static void ShrinkHorizontal(this VisualElement e)
        {
            e.SetWidth(e.ActualContentWidth());
        }
        
        public static Vector2 ActualContentSize(this VisualElement e)
        { 
            var elems = e.Query().Build().Where(c => c.Children().IsEmpty()).ToList();
            var maxX = elems.Max(c => c.worldTransform.GetPosition().x + c.Width() + c.EmptySpaceTo(e));
            var minX = elems.Min(c => c.worldTransform.GetPosition().x);
            var maxY = elems.Max(c => c.worldTransform.GetPosition().y + c.Height() + c.EmptySpaceTo(e));
            var minY = elems.Min(c => c.worldTransform.GetPosition().y);
            return new Vector2(maxX - minX, maxY - minY);
        }

        public static float ActualContentWidth(this VisualElement e)
        {
            var elems = e.Query().Build().Where(c => c.Children().IsEmpty()).ToList();
            var maxX = elems.Max(c => c.worldTransform.GetPosition().x + c.Width() + c.EmptySpaceXTo(e));
            var minX = elems.Min(c => c.worldTransform.GetPosition().x);
            return maxX - minX;
        }
        
        public static float ActualContentHeight(this VisualElement e)
        {
            var elems = e.Query().Build().Where(c => c.Children().IsEmpty()).ToList();
            var maxY = elems.Max(c => c.worldTransform.GetPosition().y + c.Height() + c.EmptySpaceYTo(e));
            var minY = elems.Min(c => c.worldTransform.GetPosition().y);
            return maxY - minY;
        }
        
        public static float EmptySpaceTo(this VisualElement e, VisualElement ancestor)
        {
            //Assert.IsTrue(ancestor.Query().Build().Contains(e), $"{ancestor.name} is not an ancestor of {e.name}"); //slow
            if (e == ancestor) return 0;
            return e.parent.Margin() + e.parent.Padding() + e.parent.Border() + e.parent.EmptySpaceTo(ancestor);
        }
        
        public static float EmptySpaceXTo(this VisualElement e, VisualElement ancestor)
        {
            if (e == ancestor) return 0;
            return e.parent.MarginX() + e.parent.PaddingX() + e.parent.Border() + e.parent.EmptySpaceXTo(ancestor);
        }
        
        public static float EmptySpaceYTo(this VisualElement e, VisualElement ancestor)
        {
            if (e == ancestor) return 0;
            return e.parent.MarginY() + e.parent.PaddingY() + e.parent.Border() + e.parent.EmptySpaceYTo(ancestor);
        }
        
        public static float UnscaledEmptySpaceTo(this VisualElement e, VisualElement ancestor)
        {
            if (e == ancestor) return 0;
            var style = e.parent.resolvedStyle;
            return style.marginLeft + style.paddingLeft + style.borderLeftWidth + e.parent.UnscaledEmptySpaceTo(ancestor);
        }

        public static float Bottom(this VisualElement e)
        {
            return e.WorldPosition().y + e.Height();
        }
        
        public static float BottomRelative(this VisualElement e)
        {
            return e.RelativePosition().y + e.Height();
        }
        
        public static void SetTreeToAbsolute(this VisualElement root)
        {
            SetElementToAbsolute(root);
            foreach (var child in root.Children())
            {
                SetTreeToAbsolute(child);
            }
        }

        public static void SetPosition(this VisualElement e, Vector2 pos)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            e.transform.position = pos;
            #pragma warning restore CS0618 // Type or member is obsolete
        }

        public static Vector2 GetPosition(this VisualElement e)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            return e.transform.position;
            #pragma warning restore CS0618 // Type or member is obsolete
        }
        
        private static void SetElementToAbsolute(VisualElement element)
        {
            element.style.position = UnityEngine.UIElements.Position.Absolute;
        }

        private static void SetOpacity(VisualElement element, float opacity)
        {
            element.style.opacity = opacity;
            if (element.GetComponent<IOpacity>() is { } c)
                c.opacity = opacity;
        }

    }
}