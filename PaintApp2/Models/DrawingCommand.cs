using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PaintApp2.Models
{
    // Command Pattern: Undo/Redo için komut arayüzü
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // Çizim komutları için base sınıf
    public abstract class DrawingCommand : ICommand
    {
        protected Canvas canvas;
        protected UIElement? element;

        public DrawingCommand(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public abstract void Execute();
        public abstract void Undo();
    }

    // Şekil ekleme komutu
    public class AddShapeCommand : DrawingCommand
    {
        public AddShapeCommand(Canvas canvas, UIElement shape) : base(canvas)
        {
            this.element = shape;
        }

        public override void Execute()
        {
            if (element != null && !canvas.Children.Contains(element))
            {
                canvas.Children.Add(element);
            }
        }

        public override void Undo()
        {
            if (element != null && canvas.Children.Contains(element))
            {
                canvas.Children.Remove(element);
            }
        }
    }

    // Metin ekleme komutu
    public class AddTextCommand : DrawingCommand
    {
        public AddTextCommand(Canvas canvas, UIElement textElement) : base(canvas)
        {
            this.element = textElement;
        }

        public override void Execute()
        {
            if (element != null && !canvas.Children.Contains(element))
            {
                canvas.Children.Add(element);
            }
        }

        public override void Undo()
        {
            if (element != null && canvas.Children.Contains(element))
            {
                canvas.Children.Remove(element);
            }
        }
    }

    // Kalem çizimi için komut (çoklu çizgi segmentleri)
    public class PenDrawingCommand : DrawingCommand
    {
        public List<Line> lines = new List<Line>();

        public PenDrawingCommand(Canvas canvas) : base(canvas) { }

        public void AddLine(Line line)
        {
            lines.Add(line);
        }

        public override void Execute()
        {
            foreach (var line in lines)
            {
                if (!canvas.Children.Contains(line))
                {
                    canvas.Children.Add(line);
                }
            }
        }

        public override void Undo()
        {
            foreach (var line in lines)
            {
                if (canvas.Children.Contains(line))
                {
                    canvas.Children.Remove(line);
                }
            }
        }
    }
}
