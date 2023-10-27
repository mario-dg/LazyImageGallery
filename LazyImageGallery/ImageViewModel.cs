using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using Path = System.IO.Path;

namespace LazyImageGallery
{
    public partial class ImageViewModel : ObservableObject
    {
        /// <summary>
        /// Collection of all labels that were loaded via the label file
        /// </summary>
        [ObservableProperty] private ObservableCollection<PointCollection> _labels;

        /// <summary>
        /// Indicates whether the VirtualizedWrapPanel already requested the loading of this item
        /// </summary>
        [ObservableProperty] private bool _imageLoaded;

        /// <summary>
        /// Actual thumbnail of the image
        /// </summary>
        private BitmapImage? _thumbnail;

        /// <summary>
        /// Absolute file path to the image
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Dimension of the thumbnail, also used to scale the normalized
        /// label coordinates
        /// </summary>
        private readonly int _imageSize;

        public ImageViewModel(string filePath, int imageSize)
        {
            _thumbnail = null;

            _filePath = filePath;

            _imageSize = imageSize;
            ImageLoaded = false;

            Labels = new();
            LoadLabels();
        }

        /// <summary>
        /// Allows for actual image creation, when the VirtualizedWrapPanel requests it
        /// </summary>
        public BitmapImage Thumbnail
        {
            get
            {
                _thumbnail ??= LoadImage();

                return _thumbnail;
            }
        }

        /// <summary>
        /// Load the image via the specified path,
        /// set caching and performance options for the BitmapImage
        /// </summary>
        /// <returns>Final BitmapImage containing the loaded thumbnail</returns>
        private BitmapImage LoadImage()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(_filePath);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelWidth = _imageSize;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.EndInit();
            image.Freeze();
            ImageLoaded = true;
            return image;
        }

        /// <summary>
        /// Look for a label file with the same name in the chosen directory
        /// if it exists, load it for rendering
        /// </summary>
        private void LoadLabels()
        {
            Labels.Clear();

            var labelPath = _filePath.Replace(Path.GetExtension(_filePath), ".txt");
            if (!Path.Exists(labelPath))
                return;

            foreach (var line in File.ReadAllLines(labelPath))
            {
                string[] parts = line.Split(' ');
                var points = new PointCollection();

                for (int i = 1; i < parts.Length; i += 2)
                {
                    points.Add(new Point(float.Parse(parts[i]) * _imageSize, float.Parse(parts[i + 1]) * _imageSize));
                }

                points.Freeze();
                Labels.Add(points);
            }
        }
    }
}
