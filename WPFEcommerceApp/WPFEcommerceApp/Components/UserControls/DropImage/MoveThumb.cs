using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WPFEcommerceApp
{
    public class MoveThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private ContentControl designerItem;
        private double heightCanvas;
        public double HeightCanvas
        {
            get => heightCanvas; 
            set => heightCanvas = value;
        }
        private double widthCanvas;
        public double WidthCanvas
        {
            get => widthCanvas;
            set => widthCanvas = value;
        }

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);
                if (this.rotateTransform != null)
                {
                    dragDelta = this.rotateTransform.Transform(dragDelta);
                }
                if (Canvas.GetLeft(this.designerItem) + dragDelta.X > 0)
                {
                    Canvas.SetLeft(this.designerItem, 0);
                }
                else
                {
                    if (Canvas.GetLeft(this.designerItem) + dragDelta.X + this.designerItem.Width < WidthCanvas)
                    {
                        Canvas.SetLeft(this.designerItem, -this.designerItem.Width + WidthCanvas);
                    }
                    else
                    {
                        Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + dragDelta.X);
                    }
                }
                if (Canvas.GetTop(this.designerItem) + dragDelta.Y > 0)
                {
                    Canvas.SetTop(this.designerItem, 0);
                }
                else
                {
                    if (Canvas.GetTop(this.designerItem) + dragDelta.Y + this.designerItem.Height < HeightCanvas)
                    {
                        Canvas.SetTop(this.designerItem, -this.designerItem.Height + HeightCanvas);
                    }
                    else
                    {
                        Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + dragDelta.Y);
                    }
                }
            }
        }
    }
}
