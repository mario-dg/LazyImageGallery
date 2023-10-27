using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Ookii.Dialogs.Wpf;

namespace LazyImageGallery;

/// <summary>
/// The following implementation shows how to implement an efficient and fast image gallery to display thousands
/// of images in a WPF control.
/// On selection of a directory, all image paths are loaded and stored, but only when the UI is about to show the
/// image, the actual image is loaded from disk.
/// This is much more memory efficient and prevents the program to load thousands of images into memory at once,
/// leading to a high memory consumption and long loading times.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// Collection of all images of the directory
    /// </summary>
    [ObservableProperty] private ObservableCollection<ImageViewModel> _thumbnails;

    /// <summary>
    /// Placeholder image that is displayed while the image is loaded from disk
    /// </summary>
    [ObservableProperty] private BitmapImage? _placeholder;

    /// <summary>
    /// Directory chosen by the user
    /// </summary>
    [ObservableProperty] private string _imageDirectory;

    /// <summary>
    /// Display dimension of each thumbnail, expects squared images
    /// </summary>
    [ObservableProperty] private int _imageDim;

    /// <summary>
    /// Gives information about the amount of images that were loaded
    /// </summary>
    [ObservableProperty] private string _info;

    public MainWindowViewModel()
    {
        Thumbnails = new();
        LoadPlaceholderImage();

        ImageDirectory = string.Empty;
        ImageDim = 200;
        Info = "Please choose a directory";
    }

    [RelayCommand]
    private void ChooseDirectory()
    {
        var dialog = new VistaFolderBrowserDialog();

        if (dialog.ShowDialog() == true)
        {
            ImageDirectory = dialog.SelectedPath;
            LoadImagesAndLabelsParallel();
        }
    }

    /// <summary>
    /// Search the chosen directory for all image files and create
    /// a ViewModel for each.
    /// </summary>
    private void LoadImagesAndLabelsParallel()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        var ext = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff" };
        var imageFiles = Directory.GetFiles(ImageDirectory, "*.*", SearchOption.AllDirectories).Where(f => ext.Any(f.ToLower().EndsWith));

        ConcurrentBag<ImageViewModel> imageViewModels = new();
        Parallel.ForEach(imageFiles, file => imageViewModels.Add(new(file, ImageDim)));

        Thumbnails = new(imageViewModels);
        Info = $"Loaded {Thumbnails.Count} images";
        Mouse.OverrideCursor = null;
    }

    /// <summary>
    /// Load the placeholder image, that gets displayed while the actual image is loaded
    /// </summary>
    private void LoadPlaceholderImage()
    {
        Placeholder = new BitmapImage();
        Placeholder.BeginInit();
        Placeholder.UriSource = new Uri("pack://application:,,,/Images/placeholder.png");
        Placeholder.CacheOption = BitmapCacheOption.OnLoad;
        Placeholder.CreateOptions = BitmapCreateOptions.DelayCreation;
        Placeholder.DecodePixelWidth = ImageDim;
        Placeholder.EndInit();
        Placeholder.Freeze();
    }
}