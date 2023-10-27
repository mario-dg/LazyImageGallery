# Lazy Loading Image Gallery
This repository serves as an example on how to implement a Thumbnail Image gallery in WPF to display thousand or hundreds of thousands of images.
Hereby the UI is virtualized, meaning it is only rendering the images that are in the viewports,
as well as the data part is lazily implemented, meaning the images are only loaded from disk when needed.

In addition, the program looks for a .txt file with the same name as the image in the same directory, to optionally render
shapes that are stored in the YOLOv8 format.
