using Microsoft.JScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    public class ProductPanel: WrapPanel
    {
        public static readonly DependencyProperty ItemMinWidthProperty =
           DependencyProperty.Register(nameof(ItemMinWidth), typeof(double), typeof(ProductPanel), new PropertyMetadata(280.0));
        public double ItemMinWidth
        {
            get { return (double)GetValue(ItemMinWidthProperty); }
            set { SetValue(ItemMinWidthProperty, value); }
        }
        public static readonly DependencyProperty ItemMaxWidthProperty =
           DependencyProperty.Register(nameof(ItemMaxWidth), typeof(double), typeof(ProductPanel), new PropertyMetadata(350.0));
        public double ItemMaxWidth
        {
            get { return (double)GetValue(ItemMaxWidthProperty); }
            set { SetValue(ItemMaxWidthProperty, value); }
        }
        public static readonly DependencyProperty ItemMinHeightProperty =
           DependencyProperty.Register(nameof(ItemMinHeight), typeof(double), typeof(ProductPanel), new PropertyMetadata(400.0));
        public double ItemMinHeight
        {
            get { return (double)GetValue(ItemMinHeightProperty); }
            set { SetValue(ItemMinHeightProperty, value); }
        }
        public static readonly DependencyProperty ItemMaxHeightProperty =
           DependencyProperty.Register(nameof(ItemMaxHeight), typeof(double), typeof(ProductPanel), new PropertyMetadata(500.0));
        public double ItemMaxHeight
        {
            get { return (double)GetValue(ItemMaxHeightProperty); }
            set { SetValue(ItemMaxHeightProperty, value); }
        }
        public static DependencyProperty MaxRowProperty =
           DependencyProperty.Register(nameof(MaxRow), typeof(int?), typeof(ProductPanel), new PropertyMetadata(null));
        public int? MaxRow
        {
            get { return (int?)GetValue(MaxRowProperty); }
            set { SetValue(MaxRowProperty, value); }
        }
        private Size OldSize { get; set; }
        protected override Size MeasureOverride(Size constraint)
        { 
            double minWidth = ItemMinWidth;
            double minHeight = ItemMinHeight;
            double maxWidth = ItemMaxWidth;
            double maxHeight = ItemMaxHeight;

            //Because we all set item inside margin 5 20
            //so I set 10 in here
            //if anychange i will fix later
            int marginLeftRight = 10; 
            int slotWidth = (int)(minWidth + marginLeftRight);
            int numberOfSlot = Math.Min(InternalChildren.Count, (int)(constraint.Width / slotWidth));
            if (OldSize != null)
            {
                if (maxWidth >= minWidth && maxHeight >= minHeight)
                {
                    int maxIndex = InternalChildren.Count;
                    if (MaxRow != null)
                    {
                        maxIndex = Math.Min((MaxRow ?? 0) * numberOfSlot, maxIndex);
                    }
                    double newWidth = constraint.Width / numberOfSlot - marginLeftRight;
                    if (newWidth > maxWidth)
                    {
                        for (int i = 0; i < maxIndex; i++)
                        {
                            UIElement child = InternalChildren[i];
                            child.SetValue(WidthProperty, maxWidth);
                            child.SetValue(HeightProperty, maxHeight);
                        }
                        double temp = (maxHeight + 40) * Math.Ceiling(maxIndex * 1.0 / numberOfSlot);
                        if (!double.IsNaN(temp))
                        {
                            this.RenderSize = new Size(constraint.Width, temp);
                        }
                    }
                    else if (newWidth < minWidth)
                    {
                        for (int i = 0; i < maxIndex; i++)
                        {
                            UIElement child = InternalChildren[i];
                            child.SetValue(WidthProperty, minWidth);
                            child.SetValue(HeightProperty, minHeight);
                        }
                        double temp = (minHeight + 40) * Math.Ceiling(maxIndex * 1.0 / numberOfSlot);
                        if (!double.IsNaN(temp))
                        {
                            this.RenderSize = new Size(constraint.Width, temp);
                        }
                    }
                    else
                    {
                        double newHeight = (newWidth - minWidth) / (maxWidth - minWidth) * (maxHeight - minHeight) + minHeight;
                        for (int i = 0; i < maxIndex; i++)
                        {
                            UIElement child = InternalChildren[i];
                            child.SetValue(WidthProperty, newWidth);
                            child.SetValue(HeightProperty, newHeight);
                        } 
                        double temp = (newHeight + 40) * Math.Ceiling(maxIndex * 1.0 / numberOfSlot);
                        if(!double.IsNaN(temp))
                        {
                            this.RenderSize = new Size(constraint.Width, temp);
                        }    
                    }
                }
            }
            OldSize = constraint;
            return base.MeasureOverride(constraint);
        }
    }
}
