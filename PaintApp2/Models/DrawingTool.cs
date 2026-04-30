using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace PaintApp2.Models
{
    // Strategy Pattern: Farklı çizim araçlarını kullanmak için strateji arayüzü
    public interface IDrawingTool
    {
        void OnMouseDown(Point point, Brush brush, Canvas canvas);
        void OnMouseMove(Point point, Brush brush, Canvas canvas);
        void OnMouseUp(Point point, Brush brush, Canvas canvas);
        DrawingCommand? GetCommand();
    }

    // Kalem aracı
    public class PenTool : IDrawingTool
    {
        private bool isDrawing = false;
        private Point? lastPoint = null;
        private PenDrawingCommand? currentCommand = null;
        private Canvas? currentCanvas = null;

        public void OnMouseDown(Point point, Brush brush, Canvas canvas)
        {
            isDrawing = true;
            lastPoint = point;
            currentCanvas = canvas;
            currentCommand = new PenDrawingCommand(canvas);
        }

        public void OnMouseMove(Point point, Brush brush, Canvas canvas)
        {
            if (isDrawing && lastPoint.HasValue)
            {
                Line line = new Line
                {
                    X1 = lastPoint.Value.X,
                    Y1 = lastPoint.Value.Y,
                    X2 = point.X,
                    Y2 = point.Y,
                    Stroke = brush,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
                currentCommand?.AddLine(line);
                lastPoint = point;
            }
        }

        public void OnMouseUp(Point point, Brush brush, Canvas canvas)
        {
            if (isDrawing && lastPoint.HasValue)
            {
                Line line = new Line
                {
                    X1 = lastPoint.Value.X,
                    Y1 = lastPoint.Value.Y,
                    X2 = point.X,
                    Y2 = point.Y,
                    Stroke = brush,
                    StrokeThickness = 2
                };
                canvas.Children.Add(line);
                currentCommand?.AddLine(line);
            }
            isDrawing = false;
            lastPoint = null;
        }

        public DrawingCommand? GetCommand()
        {
            var cmd = currentCommand;
            // Komut alındıktan sonra sıfırla (bir sonraki çizim için)
            if (currentCommand != null && currentCommand.lines.Count > 0)
            {
                currentCommand = null;
                return cmd;
            }
            return null;
        }
    }

    // Dikdörtgen aracı
    public class RectangleTool : IDrawingTool
    {
        private bool isDrawing = false;
        private Point? startPoint = null;
        private Rectangle? currentRectangle = null;
        private Canvas? currentCanvas = null;
        private Brush? currentBrush = null;

        public void OnMouseDown(Point point, Brush brush, Canvas canvas)
        {
            isDrawing = true;
            startPoint = point;
            currentCanvas = canvas;
            currentBrush = brush;
            currentRectangle = new Rectangle
            {
                Stroke = brush,
                Fill = Brushes.Transparent,
                StrokeThickness = 2
            };
            Canvas.SetLeft(currentRectangle, point.X);
            Canvas.SetTop(currentRectangle, point.Y);
            canvas.Children.Add(currentRectangle);
        }

        public void OnMouseMove(Point point, Brush brush, Canvas canvas)
        {
            if (isDrawing && startPoint.HasValue && currentRectangle != null)
            {
                double left = Math.Min(startPoint.Value.X, point.X);
                double top = Math.Min(startPoint.Value.Y, point.Y);
                double width = Math.Abs(point.X - startPoint.Value.X);
                double height = Math.Abs(point.Y - startPoint.Value.Y);

                Canvas.SetLeft(currentRectangle, left);
                Canvas.SetTop(currentRectangle, top);
                currentRectangle.Width = width;
                currentRectangle.Height = height;
            }
        }

        public void OnMouseUp(Point point, Brush brush, Canvas canvas)
        {
            isDrawing = false;
            startPoint = null;
        }

        public DrawingCommand? GetCommand()
        {
            if (currentRectangle != null)
            {
                return new AddShapeCommand(currentCanvas!, currentRectangle);
            }
            return null;
        }
    }

    // Daire aracı
    public class CircleTool : IDrawingTool
    {
        private bool isDrawing = false;
        private Point? startPoint = null;
        private Ellipse? currentEllipse = null;
        private Canvas? currentCanvas = null;
        private Brush? currentBrush = null;

        public void OnMouseDown(Point point, Brush brush, Canvas canvas)
        {
            isDrawing = true;
            startPoint = point;
            currentCanvas = canvas;
            currentBrush = brush;
            currentEllipse = new Ellipse
            {
                Stroke = brush,
                Fill = Brushes.Transparent,
                StrokeThickness = 2
            };
            Canvas.SetLeft(currentEllipse, point.X);
            Canvas.SetTop(currentEllipse, point.Y);
            canvas.Children.Add(currentEllipse);
        }

        public void OnMouseMove(Point point, Brush brush, Canvas canvas)
        {
            if (isDrawing && startPoint.HasValue && currentEllipse != null)
            {
                double left = Math.Min(startPoint.Value.X, point.X);
                double top = Math.Min(startPoint.Value.Y, point.Y);
                double width = Math.Abs(point.X - startPoint.Value.X);
                double height = Math.Abs(point.Y - startPoint.Value.Y);

                Canvas.SetLeft(currentEllipse, left);
                Canvas.SetTop(currentEllipse, top);
                currentEllipse.Width = width;
                currentEllipse.Height = height;
            }
        }

        public void OnMouseUp(Point point, Brush brush, Canvas canvas)
        {
            isDrawing = false;
            startPoint = null;
        }

        public DrawingCommand? GetCommand()
        {
            if (currentEllipse != null)
            {
                return new AddShapeCommand(currentCanvas!, currentEllipse);
            }
            return null;
        }
    }

    // Metin aracı
    public class TextTool : IDrawingTool
    {
        private TextBox? currentTextBox = null;
        private Canvas? currentCanvas = null;
        private Brush? currentBrush = null;
        private bool isTextBoxCreated = false;

        public void OnMouseDown(Point point, Brush brush, Canvas canvas)
        {
            currentCanvas = canvas;
            currentBrush = brush;
            
            currentTextBox = new TextBox
            {
                Width = 200,
                Height = 30,
                FontSize = 14,
                Foreground = brush,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gray
            };
            Canvas.SetLeft(currentTextBox, point.X);
            Canvas.SetTop(currentTextBox, point.Y);
            canvas.Children.Add(currentTextBox);
            currentTextBox.Focus();
            isTextBoxCreated = true;
        }

        public void OnMouseMove(Point point, Brush brush, Canvas canvas)
        {
            // Metin aracı için mouse move gerekli değil
        }

        public void OnMouseUp(Point point, Brush brush, Canvas canvas)
        {
            // Metin kutusu oluşturuldu, kullanıcı yazabilir
        }

        public DrawingCommand? GetCommand()
        {
            if (currentTextBox != null && isTextBoxCreated)
            {
                isTextBoxCreated = false;
                var textBox = currentTextBox;
                currentTextBox = null;
                return new AddTextCommand(currentCanvas!, textBox);
            }
            return null;
        }
    }
}
