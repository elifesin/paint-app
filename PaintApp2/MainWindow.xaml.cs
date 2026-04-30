using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PaintApp2.Models; // Models dosyasındaki pattern ve toolları kullanabilmek için 

namespace PaintApp2
{
    public partial class MainWindow : Window
    {
        private IDrawingTool? currentTool = null;
        private Brush currentBrush = Brushes.Black;
        private DrawingCommandManager commandManager = new DrawingCommandManager();
        private DrawingCommand? pendingCommand = null;

        public MainWindow()
        {
            InitializeComponent();
            UpdateButtonStates();
        }

        private void BtnPen_Click(object sender, RoutedEventArgs e)
        {
            currentTool = new PenTool();
            UpdateToolButtons();
        }

        private void BtnRectangle_Click(object sender, RoutedEventArgs e)
        {
            currentTool = new RectangleTool();
            UpdateToolButtons();
        }

        private void BtnCircle_Click(object sender, RoutedEventArgs e)
        {
            currentTool = new CircleTool();
            UpdateToolButtons();
        }

        private void BtnText_Click(object sender, RoutedEventArgs e)
        {
            currentTool = new TextTool();
            UpdateToolButtons();
        }

        private void UpdateToolButtons()
        {
            btnPen.Background = currentTool is PenTool ? Brushes.LightBlue : Brushes.White;
            btnRectangle.Background = currentTool is RectangleTool ? Brushes.LightBlue : Brushes.White;
            btnCircle.Background = currentTool is CircleTool ? Brushes.LightBlue : Brushes.White;
            btnText.Background = currentTool is TextTool ? Brushes.LightBlue : Brushes.White;
        }

        private void Color_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string colorName)
            {
                currentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorName)!);
                selectedColorBorder.Background = currentBrush;
                
                // Seçili rengi göster - tüm renk border'larını bul ve border kalınlıklarını güncelle
                var grid = (Grid)this.Content;
                var borderContainer = (Border)grid.Children[0];
                var scrollViewer = (ScrollViewer)borderContainer.Child;
                var stackPanel = (StackPanel)scrollViewer.Content;
                
                foreach (var child in stackPanel.Children)
                {
                    if (child is Border b && b.Tag != null)
                    {
                        b.BorderThickness = new Thickness(1);
                    }
                }
                border.BorderThickness = new Thickness(3);
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == null) return;

            Point point = e.GetPosition(drawingCanvas);
            currentTool.OnMouseDown(point, currentBrush, drawingCanvas);
            
            // TextTool için komutu hemen kaydet (textbox oluşturulduğunda)
            if (currentTool is TextTool)
            {
                // Biraz gecikme ile komutu kaydet (textbox'ın oluşması için)
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    var cmd = currentTool?.GetCommand();
                    if (cmd != null)
                    {
                        commandManager.ExecuteCommand(cmd);
                        UpdateButtonStates();
                    }
                };
                timer.Start();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentTool == null) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(drawingCanvas);
                currentTool.OnMouseMove(point, currentBrush, drawingCanvas);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == null) return;

            Point point = e.GetPosition(drawingCanvas);
            currentTool.OnMouseUp(point, currentBrush, drawingCanvas);
            
            // Komutu kaydet
            pendingCommand = currentTool.GetCommand();
            if (pendingCommand != null)
            {
                commandManager.ExecuteCommand(pendingCommand);
                UpdateButtonStates();
            }
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            commandManager.Undo();
            UpdateButtonStates();
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            commandManager.Redo();
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnUndo.IsEnabled = commandManager.CanUndo;
            btnRedo.IsEnabled = commandManager.CanRedo;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Bitmap Image|*.bmp|PNG Image|*.png|JPEG Image|*.jpg",
                DefaultExt = "bmp",
                FileName = "resim.bmp"
            };

            if (saveDialog.ShowDialog() == true)
            {
                SaveCanvasToFile(saveDialog.FileName);
                MessageBox.Show("Resim başarıyla kaydedildi!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveCanvasToFile(string fileName)
        {
            // Canvas'ı RenderTargetBitmap'e dönüştür
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)drawingCanvas.Width,
                (int)drawingCanvas.Height,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(drawingCanvas);

            // Bitmap encoder oluştur
            BitmapEncoder encoder;
            string extension = System.IO.Path.GetExtension(fileName).ToLower();
            
            switch (extension)
            {
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                default:
                    encoder = new BmpBitmapEncoder();
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Dosyaya kaydet
            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
